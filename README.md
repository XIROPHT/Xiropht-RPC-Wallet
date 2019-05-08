# Xiropht-RPC-Wallet
<h2>Xiropht RPC Wallet specialy made for exchanges, web wallet.</h2>

**RPC Wallet tool use the Token Network system described on the whitepaper: https://xiropht.com/document/Xiropht_Whitepaper_EN.pdf**

To get more explanations, please refer to our Wiki pages: https://github.com/XIROPHT/Xiropht-RPC-Wallet/wiki

Features:

- Encrypted [AES 256bit] Database by password who store wallets informations.

- Auto Update wallets balance informations. (Interval of update is set to 10 seconds, this interval can be change on the setting file).

- Log system, write logs.

- Remote Node Sync system, permit to sync transaction(s) of each wallets stored inside of the RPC Wallet.

- API HTTP System (Default port 8000), permit to link a website or a web service like an nginx proxy in front:

  -> Permit to get the total of wallet stored inside the RPC Wallet tool.
  
  -> Permit to get wallet address from an index selected.
  
  -> Permit to get the current balance and pending balance from an index or a wallet address selected.
  
  -> Permit to create a new wallet and return the wallet address created. (In case of errors after multiple retry, you can set a max keep alive argument for retry automaticaly the attempt to create a wallet until to reach it).
  
  -> Permit to send a transaction from an index or a wallet address selected with an amount,fee, anonymous option, wallet address target selected, return the status of the transaction request(refused, accepted, busy). 
  
  -> The RPC Wallet is setting up to allow only one attempt to send a transaction per wallets until to retrieve a response from the network.
  
  **-> Always return JSON string request.**
  
- API Encryption Key option system [AES 256bit], can be set to require to encrypt GET request received and response to send.

- API Whitelist, permit to accept only ip's listed, if the list is empty the API HTTP system accept every incoming connection.



- Command line system:

  -> Permit to change the log level for see what's going on every systems one by one. 
  
  -> Permit to create manualy a new wallet and store it inside the database encrypted.
 
**Newtonsoft.Json library is used since version 0.0.0.1R for the API HTTP/HTTPS system: https://github.com/JamesNK/Newtonsoft.Json**

**Developers:**

- Xiropht (Sam Segura)
