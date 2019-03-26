using Xiropht_Connector_All.Setting;
using Xiropht_Rpc_Wallet.ConsoleObject;

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

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="walletAddress"></param>
        /// <param name="walletPublicKey"></param>
        public ClassWalletObject(string walletAddress, string walletPublicKey)
        {
            WalletAddress = walletAddress;
            WalletPublicKey = walletPublicKey;
            WalletBalance = "0";
            WalletLastUpdate = 0;
            WalletPendingBalance = "0";
            WalletUniqueId = "-1";
            WalletAnonymousUniqueId = "-1";
            WalletOnSendingTransaction = false;
        }

        /// <summary>
        /// Update balance.
        /// </summary>
        /// <param name="balance"></param>
        public void SetWalletBalance(string balance)
        {
            ClassConsole.ConsoleWriteLine("Wallet " + WalletAddress + " - Balance " + WalletBalance + " " + ClassConnectorSetting.CoinNameMin + "->" + balance + " " + ClassConnectorSetting.CoinNameMin, ClassConsoleEnumeration.IndexPoolConsoleBlueLog, ClassConsoleLogLevelEnumeration.LogLevelWalletObject);
            WalletBalance = balance;
        }

        /// <summary>
        /// Update pending balance.
        /// </summary>
        /// <param name="pendingBalance"></param>
        public void SetWalletPendingBalance(string pendingBalance)
        {
            ClassConsole.ConsoleWriteLine("Wallet " + WalletAddress + " - Pending Balance " + WalletPendingBalance + " " + ClassConnectorSetting.CoinNameMin + "->" + pendingBalance + " " + ClassConnectorSetting.CoinNameMin, ClassConsoleEnumeration.IndexPoolConsoleBlueLog, ClassConsoleLogLevelEnumeration.LogLevelWalletObject);
            WalletPendingBalance = pendingBalance;
        }

        /// <summary>
        /// Update wallet unique id (used for synchronisation)
        /// </summary>
        /// <param name="uniqueId"></param>
        public void SetWalletUniqueId(string uniqueId)
        {
            ClassConsole.ConsoleWriteLine("Wallet " + WalletAddress + " - Unique ID " + WalletUniqueId + "->" + uniqueId, ClassConsoleEnumeration.IndexPoolConsoleBlueLog, ClassConsoleLogLevelEnumeration.LogLevelWalletObject);
            WalletUniqueId = uniqueId;
        }

        /// <summary>
        /// Update wallet anonymous unique id (used for synchronisation)
        /// </summary>
        /// <param name="uniqueId"></param>
        public void SetWalletAnonymousUniqueId(string anonymousUniqueId)
        {
            ClassConsole.ConsoleWriteLine("Wallet " + WalletAddress + " - Unique Anonymous ID " + WalletAnonymousUniqueId + "->" + anonymousUniqueId, ClassConsoleEnumeration.IndexPoolConsoleBlueLog, ClassConsoleLogLevelEnumeration.LogLevelWalletObject);
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
    }
}
