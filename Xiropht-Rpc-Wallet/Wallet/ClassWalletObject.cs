using System;
using System.Collections.Generic;
using System.Linq;
using Xiropht_Connector_All.Setting;
using Xiropht_Rpc_Wallet.ConsoleObject;
using Xiropht_Rpc_Wallet.Database;

namespace Xiropht_Rpc_Wallet.Wallet
{
    public class ClassWalletObject
    {
        private string WalletAddress;
        private string WalletPublicKey;
        private string WalletPassword;
        private string WalletBalance;
        private string WalletPrivateKey;
        private string WalletPinCode;
        private string WalletPendingBalance;
        private string WalletUniqueId;
        private string WalletAnonymousUniqueId;
        private bool WalletOnSendingTransaction;
        private long WalletLastUpdate;
        private Dictionary<string, string> WalletListOfTransaction;
        private Dictionary<string, string> WalletListOfAnonymousTransaction;
        private string WalletContentReadLine;
        private bool WalletInUpdate;
        private Tuple<bool, string> WalletCurrentToken;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="walletAddress"></param>
        /// <param name="walletPublicKey"></param>
        /// <param name="walletContentReadLine">Content line of the database keep it encrypted.</param>
        public ClassWalletObject(string walletAddress, string walletPublicKey, string walletPassword, string walletPrivateKey, string walletPinCode, string walletContentReadLine)
        {
            WalletAddress = walletAddress;
            WalletPublicKey = walletPublicKey;
            WalletPassword = walletPassword;
            WalletPrivateKey = walletPrivateKey;
            WalletPinCode = walletPinCode;
#if DEBUG
            ClassConsole.ConsoleWriteLine("ClassWalletObject - Initialize object -> Wallet Address: " + WalletAddress + " | Wallet Public Key: " + WalletPublicKey + " | Wallet Private Key: " + WalletPrivateKey + " | Wallet Password: " + walletPassword + " | Wallet Pin Code: " + walletPinCode);
#endif
            WalletBalance = "0";
            WalletLastUpdate = 0;
            WalletPendingBalance = "0";
            WalletUniqueId = "-1";
            WalletAnonymousUniqueId = "-1";
            WalletOnSendingTransaction = false;
            WalletListOfTransaction = new Dictionary<string, string>();
            WalletListOfAnonymousTransaction = new Dictionary<string, string>();
            WalletContentReadLine = walletContentReadLine;
            WalletInUpdate = false;
            WalletCurrentToken = new Tuple<bool, string>(false, string.Empty);
        }

        /// <summary>
        /// Update balance.
        /// </summary>
        /// <param name="balance"></param>
        public void SetWalletBalance(string balance)
        {
            ClassConsole.ConsoleWriteLine("Wallet " + WalletAddress + " - Balance " + WalletBalance + " " + ClassConnectorSetting.CoinNameMin + "->" + balance + " " + ClassConnectorSetting.CoinNameMin, ClassConsoleColorEnumeration.IndexConsoleBlueLog, ClassConsoleLogLevelEnumeration.LogLevelWalletObject);
            WalletBalance = balance;
        }

        /// <summary>
        /// Update pending balance.
        /// </summary>
        /// <param name="pendingBalance"></param>
        public void SetWalletPendingBalance(string pendingBalance)
        {
            ClassConsole.ConsoleWriteLine("Wallet " + WalletAddress + " - Pending Balance " + WalletPendingBalance + " " + ClassConnectorSetting.CoinNameMin + "->" + pendingBalance + " " + ClassConnectorSetting.CoinNameMin, ClassConsoleColorEnumeration.IndexConsoleBlueLog, ClassConsoleLogLevelEnumeration.LogLevelWalletObject);
            WalletPendingBalance = pendingBalance;
        }

        /// <summary>
        /// Update wallet unique id (used for synchronisation)
        /// </summary>
        /// <param name="uniqueId"></param>
        public void SetWalletUniqueId(string uniqueId)
        {
            ClassConsole.ConsoleWriteLine("Wallet " + WalletAddress + " - Unique ID " + WalletUniqueId + "->" + uniqueId, ClassConsoleColorEnumeration.IndexConsoleBlueLog, ClassConsoleLogLevelEnumeration.LogLevelWalletObject);
            WalletUniqueId = uniqueId;
        }

        /// <summary>
        /// Update wallet anonymous unique id (used for synchronisation)
        /// </summary>
        /// <param name="uniqueId"></param>
        public void SetWalletAnonymousUniqueId(string anonymousUniqueId)
        {
            ClassConsole.ConsoleWriteLine("Wallet " + WalletAddress + " - Unique Anonymous ID " + WalletAnonymousUniqueId + "->" + anonymousUniqueId, ClassConsoleColorEnumeration.IndexConsoleBlueLog, ClassConsoleLogLevelEnumeration.LogLevelWalletObject);
            WalletAnonymousUniqueId = anonymousUniqueId;
        }

        /// <summary>
        /// Update last wallet update.
        /// </summary>
        /// <param name="dateOfUpdate"></param>
        public void SetLastWalletUpdate(long dateOfUpdate)
        {
            WalletLastUpdate = dateOfUpdate;
        }

        /// <summary>
        /// Set the current status of send transaction on the wallet
        /// </summary>
        /// <param name="status"></param>
        public void SetWalletOnSendTransactionStatus(bool status)
        {
            WalletOnSendingTransaction = status;
        }

        /// <summary>
        /// Set the current wallet update status.
        /// </summary>
        /// <param name="status"></param>
        public void SetWalletOnUpdateStatus(bool status)
        {
            WalletInUpdate = status;
        }

        /// <summary>
        /// Set the current wallet address.
        /// </summary>
        /// <param name="walletAddress"></param>
        public void SetWalletAddress(string walletAddress)
        {
            WalletAddress = walletAddress;
        }

        /// <summary>
        /// Set the current wallet public key.
        /// </summary>
        /// <param name="walletPublicKey"></param>
        public void SetWalletPublicKey(string walletPublicKey)
        {
            WalletPublicKey = walletPublicKey;
        }

        /// <summary>
        /// Set the current wallet public key.
        /// </summary>
        /// <param name="walletPrivateKey"></param>
        public void SetWalletPrivateKey(string walletPrivateKey)
        {
            WalletPrivateKey = walletPrivateKey;
        }

        /// <summary>
        /// Set the current wallet password.
        /// </summary>
        /// <param name="walletPassword"></param>
        public void SetWalletPassword(string walletPassword)
        {
             WalletPassword = walletPassword;
        }

        /// <summary>
        /// Set the current wallet pin code.
        /// </summary>
        /// <param name="walletPinCode"></param>
        public void SetWalletPinCode(string walletPinCode)
        {
            WalletPinCode = walletPinCode;
        }

        public void SetWalletCurrentToken(bool status, string token)
        {
            WalletCurrentToken = new Tuple<bool, string>(status, token);
        }


        public Tuple<bool, string>  GetWalletCurrentToken()
        {
            return WalletCurrentToken;
        }


        /// <summary>
        /// Insert a transaction sync on the wallet.
        /// </summary>
        /// <param name="transaction"></param>
        public bool InsertWalletTransactionSync(string transaction, bool anonymous, bool save = true)
        {
            if (!anonymous)
            {
                var transactionHash = transaction.Split(new[] { "#" }, StringSplitOptions.None)[2];
                if (!WalletListOfTransaction.ContainsKey(transactionHash))
                {
                    WalletListOfTransaction.Add(transactionHash, transaction);
                    if (save)
                    {
                        ClassSyncDatabase.InsertTransactionToSyncDatabaseAsync(WalletAddress, WalletPublicKey, transaction);
                    }
                    return true;
                }
            }
            else
            {
                var transactionHash = transaction.Split(new[] { "#" }, StringSplitOptions.None)[2];
                if (!WalletListOfAnonymousTransaction.ContainsKey(transactionHash))
                {
                    WalletListOfAnonymousTransaction.Add(transactionHash, transaction);
                    if (save)
                    {
                        ClassSyncDatabase.InsertTransactionToSyncDatabaseAsync(WalletAddress, WalletPublicKey, transaction);
                    }
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Get the read line of the wallet. (stay encrypted)
        /// </summary>
        /// <returns></returns>
        public string GetWalletReadLine()
        {
            return WalletContentReadLine;
        }

        /// <summary>
        /// Return wallet unique id
        /// </summary>
        public string GetWalletUniqueId()
        {
            return WalletUniqueId;
        }

        /// <summary>
        /// Return wallet anonymous unique id
        /// </summary>
        public string GetWalletAnonymousUniqueId()
        {
            return WalletAnonymousUniqueId;
        }

        /// <summary>
        /// Return wallet balance.
        /// </summary>
        /// <returns></returns>
        public string GetWalletBalance()
        {
            return WalletBalance;
        }

        /// <summary>
        /// Return wallet pending balance.
        /// </summary>
        /// <returns></returns>
        public string GetWalletPendingBalance()
        {
            return WalletPendingBalance;
        }

        /// <summary>
        /// Return last wallet update.
        /// </summary>
        /// <returns></returns>
        public long GetLastWalletUpdate()
        {
            return WalletLastUpdate;
        }

        /// <summary>
        /// Return wallet public key.
        /// </summary>
        /// <returns></returns>
        public string GetWalletPublicKey()
        {
            return WalletPublicKey;
        }

        /// <summary>
        /// Return wallet private key.
        /// </summary>
        /// <returns></returns>
        public string GetWalletPrivateKey()
        {
            return WalletPrivateKey;
        }

        /// <summary>
        /// Return wallet address.
        /// </summary>
        /// <returns></returns>
        public string GetWalletAddress()
        {
            return WalletAddress;
        }

        /// <summary>
        /// Return Wallet password.
        /// </summary>
        /// <returns></returns>
        public string GetWalletPassword()
        {
            return WalletPassword;
        }

        /// <summary>
        /// Return wallet pin code.
        /// </summary>
        /// <returns></returns>
        public string GetWalletPinCode()
        {
            return WalletPinCode;
        }

        /// <summary>
        /// Return the current status of sending transaction on the wallet.
        /// </summary>
        /// <returns></returns>
        public bool GetWalletOnSendTransactionStatus()
        {
            return WalletOnSendingTransaction;
        }

        /// <summary>
        /// Return the total amount of transaction sync on the wallet.
        /// </summary>
        /// <returns></returns>
        public int GetWalletTotalTransactionSync()
        {
            return WalletListOfTransaction.Count;
        }

        /// <summary>
        /// Return the total amount of anonymous transaction sync on the wallet.
        /// </summary>
        /// <returns></returns>
        public int GetWalletTotalAnonymousTransactionSync()
        {
            return WalletListOfAnonymousTransaction.Count;
        }

        /// <summary>
        /// Return an transaction sync selected by index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetWalletTransactionSyncByIndex(int index)
        {
            if (WalletListOfTransaction.Count > index)
            {
                return WalletListOfTransaction.ElementAt(index).Value;
            }
            return null;
        }



        /// <summary>
        /// Return an anonymous transaction sync selected by index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetWalletAnonymousTransactionSyncByIndex(int index)
        {

            if (WalletListOfAnonymousTransaction.Count > index)
            {
                return WalletListOfAnonymousTransaction.ElementAt(index).Value;
            }
            return null;
        }


        /// <summary>
        /// Return any kind of transaction synced anonymous or normal selected by his transaction hash.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Tuple<int, string> GetWalletAnyTransactionSyncByHash(string transactionHash)
        {
            if (WalletListOfTransaction.ContainsKey(transactionHash))
            {
                int counter = 0;
                foreach(var transaction in WalletListOfTransaction.ToArray())
                {
                    if (transaction.Key == transactionHash)
                    {
                        return new Tuple<int, string>(counter, transaction.Value);
                    }
                    counter++;
                }
            }
            else if (WalletListOfAnonymousTransaction.ContainsKey(transactionHash))
            {
                int counter = 0;
                foreach (var transaction in WalletListOfAnonymousTransaction.ToArray())
                {
                    if (transaction.Key == transactionHash)
                    {
                        return new Tuple<int, string>(counter, transaction.Value);
                    }
                    counter++;
                }
            }
            return null;
        }

        /// <summary>
        /// Return the current wallet status.
        /// </summary>
        /// <returns></returns>
        public bool GetWalletUpdateStatus()
        {
            return WalletInUpdate;
        }
    }
}
