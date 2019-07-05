## 1. List of commands of the **API** of the **RPC Wallet**

**The API of the RPC Wallet work with **GET request** in **HTTP** protocol _(or in **HTTPS** if you have setup an **Nginx Proxy** in front of this one)_.**

**This is recommended to use also the AES Encryption Key feature for encrypt your GET Request command and retrieve an encrypted result.**

Use the IP who host your **RPC Wallet** and his api port, for example: 

http://127.0.0.1:8000/command_line

Every responses sent by the **API** are return in **json syntax** data asked, for example:

```
{"result":"nJ22Xbi4dc2mBOo5XGynNkJDWzuXWX1Y00P7WIPKEhu5AuL82kbq1c78EPU","version":"0.0.2.2","date_packet":1562340803}
```


**List of commands:**


| Command  | Description |
| --- | --- |
| `/create_wallet\|max_timeout` | Permit to create a new wallet, return the new wallet address created. |
| `/get_total_wallet_index` | Return the total number of wallet(s) stored inside the RPC Wallet. |
| `/get_wallet_address_by_index\|index` | Return the wallet address from a selected index. |
| `/get_wallet_balance_by_index\|index` | Return the wallet current/pending balance from a selected index.|
| `/get_wallet_balance_by_wallet_address\|wallet_address` | Return the wallet current/pending balance from a selected wallet address.|
| `/get_wallet_total_transaction_by_index\|index` | Return the total transaction(s) sync from a selected index.|
| `/get_total_anonymous_transaction_by_index\|index` | Return the total of anonymous transaction(s) sync from a selected index.|
| `/get_wallet_total_transaction_by_wallet_address\|wallet_address` | Return the total transaction(s) sync from a selected wallet address.|
| `/get_total_anonymous_transaction_by_wallet_address\|wallet_address` | Return the total of anonymous transaction(s) sync from a selected wallet address.|
| `/get_wallet_transaction\|wallet_address\|index` | Return a transaction selected by wallet address and an index.| 
| `/get_wallet_transaction_by_hash\|wallet_address\|transaction_hash` | Return a transaction selected by a specific wallet address and by a specific transaction hash.|
| `/get_wallet_anonymous_transaction\|wallet_address\|index` | Return an anonymous transaction selected by wallet address and an index.| 
| `/get_whole_wallet_transaction_by_range\|start\|end` | Return every transactions sync from a range selected.|
| `/send_transaction_by_wallet_address\|wallet_address_source\|amount\|fee\|anonymous_option(0 or 1)\|wallet_address_target` | Send a transaction by a selected wallet address source, with a selected amount and fee, by anonymous option or not to a target wallet address.| 
| `/update_wallet_by_address\|wallet_address` | Update manually informations current balance, pending balance of the wallet address target.|
| `/update_wallet_by_index\|wallet_index` | Update manually informations current balance, pending balance of the wallet index target.|

----------------------------------------------------------------------------------------

<h3>1. create_wallet|max_timeout</h2>

This command line permit to create a wallet from the API and return his wallet address.

<h4>Ouput</h4>

| Field | Type | Description |
| --- | --- | --- |
| result | string | Return new wallet address. |
| version | string | Current version of RPC Wallet program. |
| date_packet | long | Return date of packet result. |

----------------------------------------------------------------------------------------

<h3>2. get_total_wallet_index</h2>

This command line permit to return the total amount of wallet stored inside your RPC Wallet, this command line help on the case to use command lines by index. Like `/get_wallet_address_by_index\|index`

<h4>Ouput</h4>

| Field | Type | Description |
| --- | --- | --- |
| result | uint32 | Return the total amount of wallet(s) stored. |
| version | string | Current version of RPC Wallet program. |
| date_packet | long | Return date of packet result. |

----------------------------------------------------------------------------------------

<h3>3. get_wallet_address_by_index</h3>

<h4>Ouput</h2>

| Field | Type | Description |
| --- | --- | --- |
| result | string | Return wallet address. |
| version | string | Current version of RPC Wallet program. |
| date_packet | long | Return date of packet result. |

----------------------------------------------------------------------------------------

<h3>4. /get_wallet_balance_by_index|index</h3>

<h4>Ouput</h2>

| Field | Type | Description |
| --- | --- | --- |
| wallet_address | string | Return wallet address. |
| wallet_balance | ulong | Return wallet balance. |
| wallet_pending_balance | ulong | Return wallet pending balance. |

----------------------------------------------------------------------------------------

<h3>5. /get_wallet_balance_by_wallet_address|wallet_address</h3>

<h4>Ouput</h2>

| Field | Type | Description |
| --- | --- | --- |
| wallet_address | string | Return wallet address. |
| wallet_balance | ulong | Return wallet balance. |
| wallet_pending_balance | ulong | Return wallet pending balance. |

----------------------------------------------------------------------------------------

<h3>6. /get_wallet_total_transaction_by_index|index</h3>

<h4>Ouput</h2>

| Field | Type | Description |
| --- | --- | --- |
| wallet_address | string | Return wallet address. |
| wallet_total_transaction | long | Return total transaction synced of the wallet. |

----------------------------------------------------------------------------------------

<h3>7. /get_total_anonymous_transaction_by_index|index</h3>

<h4>Ouput</h2>

| Field | Type | Description |
| --- | --- | --- |
| wallet_address | string | Return wallet address. |
| wallet_total_anonymous_transaction | long | Return total anonymous transaction synced of the wallet. |

----------------------------------------------------------------------------------------

<h3>8. /get_wallet_total_transaction_by_wallet_address|wallet_address</h3>

<h4>Ouput</h2>

| Field | Type | Description |
| --- | --- | --- |
| wallet_address | string | Return wallet address. |
| wallet_total_transaction | long | Return total transaction synced of the wallet. |

----------------------------------------------------------------------------------------

<h3>9. /get_total_anonymous_transaction_by_wallet_address|wallet_address</h3>

<h4>Ouput</h2>

| Field | Type | Description |
| --- | --- | --- |
| wallet_address | string | Return wallet address. |
| wallet_total_anonymous_transaction | long | Return total anonymous transaction synced of the wallet. |

----------------------------------------------------------------------------------------

<h3>10. /get_wallet_transaction|wallet_address|index</h3>

<h4>Ouput</h2>

| Field | Type | Description |
| --- | --- | --- |
| index | long | Return index of the transaction. |
| wallet_address | string | Return wallet address. |
| type | string | Return transaction type (RECV or SEND). |
| hash | string | Return transaction hash. |
| mode | string | Return transaction mode (normal or anonymous). |
| wallet_dst_or_src | string | Return wallet address of the target or the sender, depending the type of the transaction. |
| amount | ulong | Return transaction amount. |
| fee | ulong | Return transaction fee. |
| timestamp_send | long | Return transaction timestamp of sending. |
| timestamp_recv | long | Return transaction timestamp of receive. |
| blockchain_height | string | Return blockchain height of the transaction. |

----------------------------------------------------------------------------------------

<h3>11. /get_wallet_transaction_by_hash|wallet_address|transaction_hash</h3>

<h4>Ouput</h2>

| Field | Type | Description |
| --- | --- | --- |
| index | long | Return index of the transaction. |
| wallet_address | string | Return wallet address. |
| type | string | Return transaction type (RECV or SEND). |
| hash | string | Return transaction hash. |
| mode | string | Return transaction mode (normal or anonymous). |
| wallet_dst_or_src | string | Return wallet address of the target or the sender, depending the type of the transaction. |
| amount | ulong | Return transaction amount. |
| fee | ulong | Return transaction fee. |
| timestamp_send | long | Return transaction timestamp of sending. |
| timestamp_recv | long | Return transaction timestamp of receive. |
| blockchain_height | string | Return blockchain height of the transaction. |

----------------------------------------------------------------------------------------

<h3>12. /get_wallet_anonymous_transaction|wallet_address|index</h3>

<h4>Ouput</h2>

| Field | Type | Description |
| --- | --- | --- |
| index | long | Return index of the transaction. |
| wallet_address | string | Return wallet address. |
| type | string | Return transaction type (RECV or SEND). |
| hash | string | Return transaction hash. |
| mode | string | Return transaction mode (normal or anonymous). |
| wallet_dst_or_src | string | Return wallet address of the target or the sender, depending the type of the transaction. |
| amount | ulong | Return transaction amount. |
| fee | ulong | Return transaction fee. |
| timestamp_send | long | Return transaction timestamp of sending. |
| timestamp_recv | long | Return transaction timestamp of receive. |
| blockchain_height | string | Return blockchain height of the transaction. |


----------------------------------------------------------------------------------------

<h3>13./get_whole_wallet_transaction_by_range|start|end	</h3>

<h4>Ouput</h2>

| Type | Description |
| --- | --- |
| string[] | json transaction |

<h4>Inside the array</h2>

Each transactions lines are parsed into json:

| Field | Type | Description |
| --- | --- | --- |
| index | long | Return index of the transaction. |
| wallet_address | string | Return wallet address. |
| type | string | Return transaction type (RECV or SEND). |
| hash | string | Return transaction hash. |
| mode | string | Return transaction mode (normal or anonymous). |
| wallet_dst_or_src | string | Return wallet address of the target or the sender, depending the type of the transaction. |
| amount | ulong | Return transaction amount. |
| fee | ulong | Return transaction fee. |
| timestamp_send | long | Return transaction timestamp of sending. |
| timestamp_recv | long | Return transaction timestamp of receive. |
| blockchain_height | string | Return blockchain height of the transaction. |

----------------------------------------------------------------------------------------

<h3>13. send_transaction_by_wallet_address</h2>

Return the status of the transaction, his transaction hash and the current balance of the wallet after sending.


| Field | Type | Description |
| --- | --- | --- |
| result | string | Return status of the transaction. |
| hash | string | Return transaction hash. |
| wallet_balance | ulong | Return current wallet balance. |
| wallet_pending_balance | ulong | Return current wallet pending balance. |

**List of response received once you send a transaction:**

-> `SEND-TOKEN-TRANSACTION-CONFIRMED` the transaction sent is confirmed.

-> `SEND-TOKEN-TRANSACTION-REFUSED` the transaction sent is refused. (For example: Amount insufficient)

-> `SEND-TOKEN-TRANSACTION-BUSY` the transaction sent is refused, because the blockchain check your balance.

-> `SEND-TOKEN-TRANSACTION-INVALID-TARGET` the transaction sent is refused, because the wallet address target is invalid.


## 2. Use the encryption key option to send requests encrypted

You can use the encryption key option to encrypt those requests before to send them to the API and the same to decrypt requests received from the API. 

**AES 256** algorithm is required, please refer to the encryption Class of the **Xiropht-Connector-All**: https://github.com/XIROPHT/Xiropht-Connector-All/blob/master/Xiropht-Connector-All/Utils/ClassAlgo.cs


