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
        private string _walletAddress;
        private string _walletPublicKey;
        private string _walletPassword;
        private string _walletBalance;
        private string _walletPrivateKey;
        private string _walletPinCode;
        private string _walletPendingBalance;
        private string _walletUniqueId;
        private string _walletAnonymousUniqueId;
        private bool _walletOnSendingTransaction;
        private long _walletLastUpdate;
        private Dictionary<string, string> _walletListOfTransaction;
        private Dictionary<string, string> _walletListOfAnonymousTransaction;
        private string _walletContentReadLine;
        private bool _walletInUpdate;
        private Tuple<bool, string> _walletCurrentToken;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="walletAddress"></param>
        /// <param name="walletPublicKey"></param>
        /// <param name="walletPinCode"></param>
        /// <param name="walletContentReadLine">Content line of the database keep it encrypted.</param>
        /// <param name="walletPassword"></param>
        /// <param name="walletPrivateKey"></param>
        public ClassWalletObject(string walletAddress, string walletPublicKey, string walletPassword, string walletPrivateKey, string walletPinCode, string walletContentReadLine)
        {
            _walletAddress = walletAddress;
            _walletPublicKey = walletPublicKey;
            _walletPassword = walletPassword;
            _walletPrivateKey = walletPrivateKey;
            _walletPinCode = walletPinCode;
#if DEBUG
            ClassConsole.ConsoleWriteLine("ClassWalletObject - Initialize object -> Wallet Address: " + _walletAddress + " | Wallet Public Key: " + _walletPublicKey + " | Wallet Private Key: " + _walletPrivateKey + " | Wallet Password: " + walletPassword + " | Wallet Pin Code: " + walletPinCode);
#endif
            _walletBalance = "0";
            _walletLastUpdate = 0;
            _walletPendingBalance = "0";
            _walletUniqueId = "-1";
            _walletAnonymousUniqueId = "-1";
            _walletOnSendingTransaction = false;
            _walletListOfTransaction = new Dictionary<string, string>();
            _walletListOfAnonymousTransaction = new Dictionary<string, string>();
            _walletContentReadLine = walletContentReadLine;
            _walletInUpdate = false;
            _walletCurrentToken = new Tuple<bool, string>(false, string.Empty);
        }

        /// <summary>
        /// Update balance.
        /// </summary>
        /// <param name="balance"></param>
        public void SetWalletBalance(string balance)
        {
            ClassConsole.ConsoleWriteLine("Wallet " + _walletAddress + " - Balance " + _walletBalance + " " + ClassConnectorSetting.CoinNameMin + "->" + balance + " " + ClassConnectorSetting.CoinNameMin, ClassConsoleColorEnumeration.IndexConsoleBlueLog, ClassConsoleLogLevelEnumeration.LogLevelWalletObject);
            _walletBalance = balance;
        }

        /// <summary>
        /// Update pending balance.
        /// </summary>
        /// <param name="pendingBalance"></param>
        public void SetWalletPendingBalance(string pendingBalance)
        {
            ClassConsole.ConsoleWriteLine("Wallet " + _walletAddress + " - Pending Balance " + _walletPendingBalance + " " + ClassConnectorSetting.CoinNameMin + "->" + pendingBalance + " " + ClassConnectorSetting.CoinNameMin, ClassConsoleColorEnumeration.IndexConsoleBlueLog, ClassConsoleLogLevelEnumeration.LogLevelWalletObject);
            _walletPendingBalance = pendingBalance;
        }

        /// <summary>
        /// Update wallet unique id (used for synchronisation)
        /// </summary>
        /// <param name="uniqueId"></param>
        public void SetWalletUniqueId(string uniqueId)
        {
            ClassConsole.ConsoleWriteLine("Wallet " + _walletAddress + " - Unique ID " + _walletUniqueId + "->" + uniqueId, ClassConsoleColorEnumeration.IndexConsoleBlueLog, ClassConsoleLogLevelEnumeration.LogLevelWalletObject);
            _walletUniqueId = uniqueId;
        }

        /// <summary>
        /// Update wallet anonymous unique id (used for synchronisation)
        /// </summary>
        /// <param name="anonymousUniqueId"></param>
        public void SetWalletAnonymousUniqueId(string anonymousUniqueId)
        {
            ClassConsole.ConsoleWriteLine("Wallet " + _walletAddress + " - Unique Anonymous ID " + _walletAnonymousUniqueId + "->" + anonymousUniqueId, ClassConsoleColorEnumeration.IndexConsoleBlueLog, ClassConsoleLogLevelEnumeration.LogLevelWalletObject);
            _walletAnonymousUniqueId = anonymousUniqueId;
        }

        /// <summary>
        /// Update last wallet update.
        /// </summary>
        /// <param name="dateOfUpdate"></param>
        public void SetLastWalletUpdate(long dateOfUpdate)
        {
            _walletLastUpdate = dateOfUpdate;
        }

        /// <summary>
        /// Set the current status of send transaction on the wallet
        /// </summary>
        /// <param name="status"></param>
        public void SetWalletOnSendTransactionStatus(bool status)
        {
            _walletOnSendingTransaction = status;
        }

        /// <summary>
        /// Set the current wallet update status.
        /// </summary>
        /// <param name="status"></param>
        public void SetWalletOnUpdateStatus(bool status)
        {
            _walletInUpdate = status;
        }

        /// <summary>
        /// Set the current wallet address.
        /// </summary>
        /// <param name="walletAddress"></param>
        public void SetWalletAddress(string walletAddress)
        {
            _walletAddress = walletAddress;
        }

        /// <summary>
        /// Set the current wallet public key.
        /// </summary>
        /// <param name="walletPublicKey"></param>
        public void SetWalletPublicKey(string walletPublicKey)
        {
            _walletPublicKey = walletPublicKey;
        }

        /// <summary>
        /// Set the current wallet public key.
        /// </summary>
        /// <param name="walletPrivateKey"></param>
        public void SetWalletPrivateKey(string walletPrivateKey)
        {
            _walletPrivateKey = walletPrivateKey;
        }

        /// <summary>
        /// Set the current wallet password.
        /// </summary>
        /// <param name="walletPassword"></param>
        public void SetWalletPassword(string walletPassword)
        {
             _walletPassword = walletPassword;
        }

        /// <summary>
        /// Set the current wallet pin code.
        /// </summary>
        /// <param name="walletPinCode"></param>
        public void SetWalletPinCode(string walletPinCode)
        {
            _walletPinCode = walletPinCode;
        }

        public void SetWalletCurrentToken(bool status, string token)
        {
            _walletCurrentToken = new Tuple<bool, string>(status, token);
        }


        public Tuple<bool, string>  GetWalletCurrentToken()
        {
            return _walletCurrentToken;
        }


        /// <summary>
        /// Insert a transaction sync on the wallet.
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="anonymous"></param>
        /// <param name="save"></param>
        public bool InsertWalletTransactionSync(string transaction, bool anonymous, bool save = true)
        {
            if (!anonymous)
            {
                var transactionHash = transaction.Split(new[] { "#" }, StringSplitOptions.None)[2];
                if (!_walletListOfTransaction.ContainsKey(transactionHash))
                {
                    _walletListOfTransaction.Add(transactionHash, transaction);
                    if (save)
                    {
                        ClassSyncDatabase.InsertTransactionToSyncDatabaseAsync(_walletAddress, _walletPublicKey, transaction);
                    }
                    return true;
                }
            }
            else
            {
                var transactionHash = transaction.Split(new[] { "#" }, StringSplitOptions.None)[2];
                if (!_walletListOfAnonymousTransaction.ContainsKey(transactionHash))
                {
                    _walletListOfAnonymousTransaction.Add(transactionHash, transaction);
                    if (save)
                    {
                        ClassSyncDatabase.InsertTransactionToSyncDatabaseAsync(_walletAddress, _walletPublicKey, transaction);
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
            return _walletContentReadLine;
        }

        /// <summary>
        /// Return wallet unique id
        /// </summary>
        public string GetWalletUniqueId()
        {
            return _walletUniqueId;
        }

        /// <summary>
        /// Return wallet anonymous unique id
        /// </summary>
        public string GetWalletAnonymousUniqueId()
        {
            return _walletAnonymousUniqueId;
        }

        /// <summary>
        /// Return wallet balance.
        /// </summary>
        /// <returns></returns>
        public string GetWalletBalance()
        {
            return _walletBalance;
        }

        /// <summary>
        /// Return wallet pending balance.
        /// </summary>
        /// <returns></returns>
        public string GetWalletPendingBalance()
        {
            return _walletPendingBalance;
        }

        /// <summary>
        /// Return last wallet update.
        /// </summary>
        /// <returns></returns>
        public long GetLastWalletUpdate()
        {
            return _walletLastUpdate;
        }

        /// <summary>
        /// Return wallet public key.
        /// </summary>
        /// <returns></returns>
        public string GetWalletPublicKey()
        {
            return _walletPublicKey;
        }

        /// <summary>
        /// Return wallet private key.
        /// </summary>
        /// <returns></returns>
        public string GetWalletPrivateKey()
        {
            return _walletPrivateKey;
        }

        /// <summary>
        /// Return wallet address.
        /// </summary>
        /// <returns></returns>
        public string GetWalletAddress()
        {
            return _walletAddress;
        }

        /// <summary>
        /// Return Wallet password.
        /// </summary>
        /// <returns></returns>
        public string GetWalletPassword()
        {
            return _walletPassword;
        }

        /// <summary>
        /// Return wallet pin code.
        /// </summary>
        /// <returns></returns>
        public string GetWalletPinCode()
        {
            return _walletPinCode;
        }

        /// <summary>
        /// Return the current status of sending transaction on the wallet.
        /// </summary>
        /// <returns></returns>
        public bool GetWalletOnSendTransactionStatus()
        {
            return _walletOnSendingTransaction;
        }

        /// <summary>
        /// Return the total amount of transaction sync on the wallet.
        /// </summary>
        /// <returns></returns>
        public int GetWalletTotalTransactionSync()
        {
            return _walletListOfTransaction.Count;
        }

        /// <summary>
        /// Return the total amount of anonymous transaction sync on the wallet.
        /// </summary>
        /// <returns></returns>
        public int GetWalletTotalAnonymousTransactionSync()
        {
            return _walletListOfAnonymousTransaction.Count;
        }

        /// <summary>
        /// Return an transaction sync selected by index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetWalletTransactionSyncByIndex(int index)
        {
            if (_walletListOfTransaction.Count > index)
            {
                return _walletListOfTransaction.ElementAt(index).Value;
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

            if (_walletListOfAnonymousTransaction.Count > index)
            {
                return _walletListOfAnonymousTransaction.ElementAt(index).Value;
            }
            return null;
        }


        /// <summary>
        /// Return any kind of transaction synced anonymous or normal selected by his transaction hash.
        /// </summary>
        /// <param name="transactionHash"></param>
        /// <returns></returns>
        public Tuple<int, string> GetWalletAnyTransactionSyncByHash(string transactionHash)
        {
            if (_walletListOfTransaction.ContainsKey(transactionHash))
            {
                int counter = 0;
                foreach(var transaction in _walletListOfTransaction.ToArray())
                {
                    if (transaction.Key == transactionHash)
                    {
                        return new Tuple<int, string>(counter, transaction.Value);
                    }
                    counter++;
                }
            }
            else if (_walletListOfAnonymousTransaction.ContainsKey(transactionHash))
            {
                int counter = 0;
                foreach (var transaction in _walletListOfAnonymousTransaction.ToArray())
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
            return _walletInUpdate;
        }
    }
}
