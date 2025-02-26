﻿using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Logging;
using Moq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoinEx.Net;
using CryptoExchange.Net.Sockets;
using Microsoft.Extensions.Logging;

namespace CryptoExchange.Net.Testing
{
    public class TestHelpers
    {
        [ExcludeFromCodeCoverage]
        public static bool PublicInstancePropertiesEqual<T>(T self, T to, params string[] ignore) where T : class
        {
            if (self != null && to != null)
            {
                var type = self.GetType();
                var ignoreList = new List<string>(ignore);
                foreach (var pi in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (ignoreList.Contains(pi.Name))
                    {
                        continue;
                    }

                    var selfValue = type.GetProperty(pi.Name).GetValue(self, null);
                    var toValue = type.GetProperty(pi.Name).GetValue(to, null);

                    if (pi.PropertyType.IsClass && !pi.PropertyType.Module.ScopeName.Equals("System.Private.CoreLib.dll"))
                    {
                        // Check of "CommonLanguageRuntimeLibrary" is needed because string is also a class
                        if (PublicInstancePropertiesEqual(selfValue, toValue, ignore))
                        {
                            continue;
                        }

                        return false;
                    }

                    if (selfValue != toValue && (selfValue == null || !selfValue.Equals(toValue)))
                    {
                        return false;
                    }
                }

                return true;
            }

            return self == to;
        }

        public static MockObjects<T> PrepareClient<T>(Func<T> construct, string responseData, HttpStatusCode code = HttpStatusCode.OK) where T : RestClient, new()
        {
            var client = construct();
            var expectedBytes = Encoding.UTF8.GetBytes(responseData);
            var responseStream = new MemoryStream();
            responseStream.Write(expectedBytes, 0, expectedBytes.Length);
            responseStream.Seek(0, SeekOrigin.Begin);

            var response = new Mock<IResponse>();
            response.Setup(c => c.IsSuccessStatusCode).Returns(code == HttpStatusCode.OK);
            response.Setup(c => c.StatusCode).Returns(code);
            response.Setup(c => c.GetResponseStreamAsync()).Returns(Task.FromResult((Stream)responseStream));

            var request = new Mock<IRequest>();
            request.Setup(c => c.Uri).Returns(new Uri("http://www.test.com"));
            request.Setup(c => c.GetResponseAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(response.Object));

            var factory = new Mock<IRequestFactory>();
            factory.Setup(c => c.Create(It.IsAny<HttpMethod>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(request.Object);
            client.RequestFactory = factory.Object;

            var log = (Log)typeof(T).GetField("log", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(client);
            log.Level = LogLevel.Debug;
            return new MockObjects<T>()
            {
                Client = client,
                Request = request,
                Response = response
            };
        }

        public static T PrepareSocketClient<T>(Func<T> construct) where T : SocketClient, new()
        {
            var factory = new Mock<IWebsocketFactory>();
            factory.Setup(s => s.CreateWebsocket(It.IsAny<Log>(), It.IsAny<string>())).Returns(CreateSocket);

            var client = construct();
            var log = (Log)typeof(T).GetField("log", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(client);
            if (log.Level == LogLevel.Information)
                log.Level = LogLevel.None;
            typeof(BaseClient).GetField("lastId", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static).SetValue(client, 0);
            typeof(T).GetProperty("SocketFactory").SetValue(client, factory.Object);
            return client;
        }

        private static event Action<Mock<IWebsocket>> OnOpen;
        private static event Action<Mock<IWebsocket>> OnClose;
        private static event Action<Mock<IWebsocket>, string> OnSend;

        private static IWebsocket CreateSocket()
        {
            bool open = false;
            bool closed = true;

            IWebsocket obj = Mock.Of<IWebsocket>();
            var socket = Mock.Get(obj);
            socket.Setup(s => s.CloseAsync()).Returns(Task.FromResult(true)).Callback(() =>
            {
                var closing = socket.Object.IsOpen;
                open = false; closed = true;
                if (closing)
                    socket.Raise(s => s.OnClose += null);
                OnClose?.Invoke(socket);
            });
            socket.Setup(s => s.IsOpen).Returns(() => open);
            socket.Setup(s => s.Send(It.IsAny<string>())).Callback(new Action<string>((data) =>
            {
                OnSend?.Invoke(socket, data);
            }));
            socket.Setup(s => s.IsClosed).Returns(() => closed);
            socket.Setup(s => s.ConnectAsync()).Returns(Task.FromResult(true)).Callback(() =>
            {
                socket.Setup(s => s.IsOpen).Returns(() => open);
                socket.Setup(s => s.IsClosed).Returns(() => closed);

                open = true; closed = false;
                socket.Raise(s => s.OnOpen += null);
                OnOpen?.Invoke(socket);
            });
            return socket.Object;
        }

        public static async Task<bool> WaitForConnect(CoinExSocketClient client, int timeout = 1000)
        {
            var evnt = new ManualResetEvent(false);
            var handler = new Action<Mock<IWebsocket>>((s) =>
            {
                evnt.Set();
            });

            OnOpen += handler;
            return await Task.Run(() =>
            {
                var result = evnt.WaitOne(timeout);
                OnOpen -= handler;
                return result;
            });
        }

        public static List<SocketConnection> GetSockets(CoinExSocketClient client)
        {            
            return ((ConcurrentDictionary<int, SocketConnection>)client.GetType().GetField("sockets", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(client)).Values.ToList();
        }

        public static async Task<bool> WaitForClose(CoinExSocketClient client, int timeout = 1000)
        {
            var evnt = new ManualResetEvent(false);
            var handler = new Action<Mock<IWebsocket>>((s) =>
            {
                var sockets = GetSockets(client);
                if (!sockets.Any())
                    return;

                var mock = Mock.Get(sockets[0].Socket);
                if (s.Equals(mock))
                    evnt.Set();
            });

            OnClose += handler;
            return await Task.Run(() =>
            {
                var result = evnt.WaitOne(timeout);
                OnClose -= handler;
                return result;
            });
        }

        public static void CloseWebsocket(CoinExSocketClient client)
        {
            var sockets = GetSockets(client);
            var mock = Mock.Get(sockets[0].Socket);
            mock.Setup(s => s.IsOpen).Returns(() => false);
            mock.Setup(s => s.IsClosed).Returns(() => true);

            mock.Raise(r => r.OnClose += null);
        }

        public static async Task<bool> WaitForSend(CoinExSocketClient client, int timeout = 3000, string expectedData = null)
        {
            var evnt = new ManualResetEvent(false);
            var handler = new Action<Mock<IWebsocket>, string>((s, data) =>
            {
                var sockets = GetSockets(client);
                if (!sockets.Any())
                    return;

                var mock = Mock.Get(sockets[0].Socket);
                if (s.Equals(mock))
                {
                    if (expectedData == null || data == expectedData)
                        evnt.Set();
                }
            });

            OnSend += handler;
            return await Task.Run(() =>
            {
                var result = evnt.WaitOne(timeout);
                OnSend -= handler;
                return result;
            });
        }

        public static void InvokeWebsocket(CoinExSocketClient client, string data)
        {
            var sockets = GetSockets(client);
            var mock = Mock.Get(sockets[0].Socket);
            mock.Raise(r => r.OnMessage += null, data);
        }
        public class MockObjects<T> where T: RestClient
        {
            public T Client { get; set; }
            public Mock<IRequest> Request { get; set; }
            public Mock<IResponse> Response { get; set; }

        }
    }
}
