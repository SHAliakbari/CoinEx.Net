# CoinEx.Net
![Build status](https://travis-ci.com/JKorf/CoinEx.Net.svg?branch=master) ![Nuget version](https://img.shields.io/nuget/v/CoinEx.net.svg)  ![Nuget downloads](https://img.shields.io/nuget/dt/CoinEx.Net.svg)

CoinEx.Net is a wrapper around the CoinEx API as described on [CoinEx](https://github.com/coinexcom/coinex_exchange_api/wiki), including all features the API provides using clear and readable objects, both for the REST  as the websocket API's.

**If you think something is broken, something is missing or have any questions, please open an [Issue](https://github.com/JKorf/CoinEx.Net/issues)**

## CryptoExchange.Net
This library is build upon the CryptoExchange.Net library, make sure to check out the documentation on that for basic usage: [docs](https://github.com/JKorf/CryptoExchange.Net)

## Donations
I develop and maintain this package on my own for free in my spare time. Donations are greatly appreciated. If you prefer to donate any other currency please contact me.

**Btc**:  12KwZk3r2Y3JZ2uMULcjqqBvXmpDwjhhQS  
**Eth**:  0x069176ca1a4b1d6e0b7901a6bc0dbf3bb0bf5cc2  
**Nano**: xrb_1ocs3hbp561ef76eoctjwg85w5ugr8wgimkj8mfhoyqbx4s1pbc74zggw7gs  

## Discord
A Discord server is available [here](https://discord.gg/MSpeEtSY8t). Feel free to join for discussion and/or questions around the CryptoExchange.Net and implementation libraries.

## Getting started
Make sure you have installed the CoinEx.Net [Nuget](https://www.nuget.org/packages/CoinEx.Net/) package and add `using CoinEx.Net` to your usings.  You now have access to 2 clients:  
**CoinExClient**  
The client to interact with the CoinExREST API. Getting prices:
````C#
var client = new CoinExClient(new CoinExClientOptions(){
 // Specify options for the client
});
var callResult = await client.GetSymbolStatesAsync();
// Make sure to check if the call was successful
if(!callResult.Success)
{
  // Call failed, check callResult.Error for more info
}
else
{
  // Call succeeded, callResult.Data will have the resulting data
}
````

Placing an order:
````C#
var client = new CoinExClient(new CoinExClientOptions(){
 // Specify options for the client
 ApiCredentials = new ApiCredentials("Key", "Secret")
});
var callResult = await client.PlaceLimitOrderAsync("BTC-USDT", TransactionType.Buy, 50, 10);
// Make sure to check if the call was successful
if(!callResult.Success)
{
  // Call failed, check callResult.Error for more info
}
else
{
  // Call succeeded, callResult.Data will have the resulting data
}
````

**CoinExSocketClient**  
The client to interact with the CoinEx websocket API. Basic usage:
````C#
var client = new CoinExSocketClient(new CoinExSocketClientOptions()
{
  // Specify options for the client
});
var subscribeResult = client.SubscribeToSymbolStateUpdatesAsync("ETHBTC", data => {
  // Handle data when it is received
});
// Make sure to check if the subscritpion was successful
if(!subscribeResult.Success)
{
  // Subscription failed, check callResult.Error for more info
}
else
{
  // Subscription succeeded, the handler will start receiving data when it is available
}
````

## Client options
For the basic client options see also the CryptoExchange.Net [docs](https://github.com/JKorf/CryptoExchange.Net#client-options). 

## Release notes
* Version 4.0.2 - 24 Aug 2021
    * Updated CryptoExchange.Net, improving websocket and SymbolOrderBook performance

* Version 4.0.1 - 13 Aug 2021
    * Fix for OperationCancelledException being thrown when closing a socket from a .net framework project

* Version 4.0.0 - 12 Aug 2021
	* Release version with new CryptoExchange.Net version 4.0.0
		* Multiple changes regarding logging and socket connection, see [CryptoExchange.Net release notes](https://github.com/JKorf/CryptoExchange.Net#release-notes)
		
* Version 4.0.0-beta3 - 09 Aug 2021
    * Renamed GetSymbolTradesAsync to GetTradesHistoryAsync
    * Renamed GetExecutedOrderDetailsAsync to GetOrderTradesAsync
    * Renamed GetOrderStatusAsync to GetOrderAsync
    * Renamed GetTradesAsync to GetUserTradesAsync

* Version 4.0.0-beta2 - 26 Jul 2021
    * Updated CryptoExchange.Net

* Version 4.0.0-beta1 - 09 Jul 2021
    * Added Async postfix for async methods
    * Updated CryptoExchange.Net

* Version 3.3.0-beta10 - 15 Jun 2021
    * WithrawAsync fixed

* Version 3.3.0-beta9 - 14 Jun 2021
    * Fixed typo in WithdrawAsync

* Version 3.3.0-beta8 - 07 Jun 2021
    * Fixed GetWithdrawalHistory
    * Updated CryptoExchange.Net

* Version 3.3.0-beta7 - 03 Jun 2021
    * Fixed order subscription (again)

* Version 3.3.0-beta6 - 03 Jun 2021
    * Added ClientId to order update model
    * Fixed order subscription parameters

* Version 3.3.0-beta5 - 02 Jun 2021
    * Added optional PlaceLimitOrderAsync parameters
    * Fix for WithdrawAsync

* Version 3.3.0-beta4 - 02 Jun 2021
    * Added GetCurrencyRateAsync endpoint
    * Added GetAssetConfigAsync endpoint
    * Added GetDepositAddressAsync

* Version 3.3.0-beta3 - 26 May 2021
    * Removed non-async calls
    * Updated to CryptoExchange.Net changes

* Version 3.3.0-beta2 - 06 mei 2021
    * Updated CryptoExchange.Net

* Version 3.3.0-beta1 - 30 apr 2021
    * Updated to CryptoExchange.Net 4.0.0-beta1, new websocket implementation
	
* Version 3.2.6 - 04 mei 2021
    * Fix for trades subscription deserialization when extra array item is received
    * Fix parameter type in Withdraw method

* Version 3.2.5 - 28 apr 2021
    * Fix trade deserialization without order id
    * Allow symbols starting with numeric character
    * Update CryptoExchange.Net
    * Fixed check in socket balance update

* Version 3.2.4 - 19 apr 2021
    * Fixed Withdraw parameters

* Version 3.2.3 - 19 apr 2021
    * Fixed SubscribeToOrderUpdates serialization
    * Updated CryptoExchange.Net

* Version 3.2.2 - 30 mrt 2021
    * Updated CryptoExchange.Net

* Version 3.2.1 - 01 mrt 2021
    * Added Nuget SymbolPackage

* Version 3.2.0 - 01 mrt 2021
    * Added config for deterministic build
    * Updated CryptoExchange.Net

* Version 3.1.2 - 22 jan 2021
    * Updated for ICommonKline

* Version 3.1.1 - 14 jan 2021
    * Updated CryptoExchange.Net

* Version 3.1.0 - 21 dec 2020
    * Update CryptoExchange.Net
    * Updated to latest IExchangeClient

* Version 3.0.14 - 11 dec 2020
    * Updated CryptoExchange.Net
    * Implemented IExchangeClient

* Version 3.0.13 - 19 nov 2020
    * Updated CryptoExchange.Net

* Version 3.0.12 - 22 okt 2020
    * Fixed parsing of orders

* Version 3.0.11 - 28 Aug 2020
    * Updated CrytpoExchange.Net

* Version 3.0.10 - 12 Aug 2020
    * Updated CryptoExchange.Net

* Version 3.0.9 - 21 Jun 2020
    * Updated CryptoExchange

* Version 3.0.8 - 16 Jun 2020
    * Updated CryptoExchange.Net

* Version 3.0.7 - 07 Jun 2020
    * Updated CryptoExchange
	
* Version 3.0.6 - 03 Mar 2020
    * Updated CryptoExchange

* Version 3.0.5 - 03 Mar 2020
    * Updated CryptoExchange

* Version 3.0.4 - 27 Jan 2020
    * Updated CryptoExchange.Net

* Version 3.0.3 - 12 Nov 2019
    * Added DepositHistory and GetMarketInfo endpoints

* Version 3.0.2 - 23 Oct 2019
	* Fixed validation length symbols again

* Version 3.0.1 - 23 Oct 2019
	* Fixed validation length symbols

* Version 3.0.0 - 23 Oct 2019
	* See CryptoExchange.Net 3.0 release notes
	* Added input validation
	* Added CancellationToken support to all requests
	* Now using IEnumerable<> for collections
	* Renamed Market -> Symbol
	* Renamed MarketDepth -> OrderBook
	* Renamed Transaction -> Trade
	
* Version 2.0.10 - 11 Sep 2019
    * Updated CryptoExchange.Net

* Version 2.0.9 - 07 Aug 2019
    * Updated CryptoExchange.Net

* Version 2.0.8 - 05 Aug 2019
    * Added xml for code docs

* Version 2.0.7 - 09 jul 2019
	* Updated CoinExSymbolOrderBook

* Version 2.0.6 - 14 may 2019
	* Added an order book implementation for easily keeping an updated order book
	* Added additional constructor to ApiCredentials to be able to read from file

* Version 2.0.5 - 01 may 2019
	* Updated to latest CryptoExchange.Net
		* Adds response header to REST call result
		* Added rate limiter per API key
		* Unified socket client workings

* Version 2.0.4 - 07 mar 2019
	* Updated to latest CryptoExchange.Net

* Version 2.0.3 - 01 feb 2019
	* Updated to latest CryptoExchange.Net

* Version 2.0.2 - 06 dec 2018
	* Fix for user-agent error on .Net framework

* Version 2.0.1 - 06 dec 2018
	* Fixed freezes if called from the UI thread

* Version 2.0.0 - 05 dec 2018
	* Updated to CryptoExchange.Net version 2
		* Libraries now use the same standard functionalities
		* Objects returned by socket subscriptions standardized across libraries

* Version 1.0.0 - 21 sep 2018
	* Updated CryptoExchange.Net

* Version 0.0.6 - 20 aug 2018
	* Fix for default api credentials getting disposed

* Version 0.0.5 - 20 aug 2018
	* Updated CryptoExchange.Net for bug fix

* Version 0.0.4 - 17 aug 2018
	* Added handling for incosistent data in socket update
	* Added additional logging
	* Small reconnection fixes

* Version 0.0.3 - 16 aug 2018
	* Added client interfaces
	* Fixed minor Resharper warnings

* Version 0.0.2 - 13 aug 2018
	* Upped CryptoExchange.Net to fix bug

* Version 0.0.1 - 13 aug 2018
	* Initial release