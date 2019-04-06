using System.Collections.Generic;
using Xiropht_Connector_All.Setting;
using Xiropht_Rpc_Wallet.ConsoleObject;
using Xiropht_Rpc_Wallet.Database;

namespace Xiropht_Rpc_Wallet.Wallet
{
    public class ClassWalletObject
    {
        private string WalletAddress;
        private string WalletPublicKey;
        private string WalletBalance;
        private string WalletPendingBalance;
        private string WalletUniqueId;
        private string WalletAnonymousUniqueId;
        private bool WalletOnSendingTransaction;
        private long WalletLastUpdate;
        private List<string> WalletListOfTransaction;
        private List<string> WalletListOfAnonymousTransaction;
        private string WalletReadLine;
        private bool WalletInUpdate;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="walletAddress"></param>
        /// <param name="walletPublicKey"></param>
        /// <param name="line">Content line of the database keep it encrypted.</param>
        public ClassWalletObject(string walletAddress, string walletPublicKey, string line)
        {
            WalletAddress = walletAddress;
            WalletPublicKey = walletPublicKey;
            WalletBalance = "0";
            WalletLastUpdate = 0;
            WalletPendingBalance = "0";
            WalletUniqueId = "-1";
            WalletAnonymousUniqueId = "-1";
            WalletOnSendingTransaction = false;
            WalletListOfTransaction = new List<string>();
            WalletListOfAnonymousTransaction = new List<string>();
            WalletReadLine = line;
            WalletInUpdate = false;
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
        /// Insert a transaction sync on the wallet.
        /// </summary>
        /// <param name="transaction"></param>
        public bool InsertWalletTransactionSync(string transaction, bool anonymous, bool save = true)
        {
            if (!anonymous)
            {
                if (!WalletListOfTransaction.Contains(transaction))
                {
                    WalletListOfTransaction.Add(transaction);
                    if (save)
                    {
                        ClassSyncDatabase.InsertTransactionToSyncDatabaseAsync(WalletAddress, WalletPublicKey, transaction);
                    }
                    return true;
                }
            }
            else
            {
                if (!WalletListOfAnonymousTransaction.Contains(transaction))
                {
                    WalletListOfAnonymousTransaction.Add(transaction);
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
            return WalletReadLine;
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
        /// Return wallet address.
        /// </summary>
        /// <returns></returns>
        public string GetWalletAddress()
        {
            return WalletAddress;
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
            if (index > 0)
            {
                index--;
            }
            if (WalletListOfTransaction.Count > index)
            {
                return WalletListOfTransaction[index];
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
            if (index > 0)
            {
                index--;
            }
            if (WalletListOfAnonymousTransaction.Count > index)
            {
                return WalletListOfAnonymousTransaction[index];
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
