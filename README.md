# Xiropht-RPC-Wallet
<h2>Xiropht RPC Wallet specifically made for coin exchanges, web wallet./h2>

**RPC Wallet tool, use the Token Network system described in the whitepaper: https://xiropht.com/document/Xiropht_Whitepaper_EN.pdf**

Further information can be found on the Wiki pages: https://github.com/XIROPHT/Xiropht-RPC-Wallet/wiki

Features:

- Encrypted [AES 256bit] Database of passwords where stored wallet information is kept.

- Auto Update of the wallets balance information. (Interval of update is set to 10 seconds, this interval can be change in the settings file).

- Log system, write logs.

- Remote Node Sync system, permit to sync transaction(s) of each wallet stored inside of the RPC Wallet.

- API HTTP System (Default port 8000), permit to link a website or a web service like an nginx proxy in front:

  -> Permit to get the total of wallets stored inside the RPC Wallet tool.
  
  -> Permit to get wallet address from an index selected.
  
  -> Permit to get the current balance and pending balance from an index or a wallet address selected.
  
  -> Permit to create a new wallet and return the wallet address created. (In case of errors after multiple retry, you can set a max keep alive argument for automatic retry attempts to create a wallet until achieved).
  
  ->  Permit to send a transaction from an index or a wallet address selected with an amount,fee, anonymous option, wallet address target selected, return the status of the transaction request(refused, accepted, busy).
  
  -> The RPC Wallet is set up to allow only one attempt to send a transaction per wallet until a response is retrieved from the network.
  
  **->  Always return JSON string request.**
  
- API Encryption Key option system [AES 256bit], can be set to require encrypt GET request received and response to send.

- API Whitelist, permit to accept only ip's listed, if the list is empty the API HTTP system will accept every incoming connection.



- Command line in program side: https://github.com/XIROPHT/Xiropht-RPC-Wallet/wiki/List-of-command-lines-in-RPC-Wallet-program-side
 
- How to manually set the configuration file: https://github.com/XIROPHT/Xiropht-RPC-Wallet/wiki/How-to-setting-Xiropht-RPC-Wallet-configuration-file

**Newtonsoft.Json library is used since version 0.0.0.1R for the API HTTP/HTTPS system: https://github.com/JamesNK/Newtonsoft.Json**

**External library used: ZXing.Net, a QR Code generator used since version 0.0.1.6R: https://github.com/micjahn/ZXing.Net/**

**Developers:**

- Xiropht (Sam Segura)
