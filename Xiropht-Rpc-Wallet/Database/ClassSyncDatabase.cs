using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xiropht_Connector_All.Utils;
using Xiropht_Connector_All.Wallet;
using Xiropht_Rpc_Wallet.ConsoleObject;
using Xiropht_Rpc_Wallet.Utility;

namespace Xiropht_Rpc_Wallet.Database
{
    public class ClassSyncDatabaseEnumeration
    {
        public const string DatabaseSyncStartLine = "[TRANSACTION]";
        public const string DatabaseAnonymousTransactionMode = "anonymous";
        public const string DatabaseAnonymousTransactionType = "ANONYMOUS";
    }

    public class ClassSyncDatabase
    {
        private const string SyncDatabaseFile = "\\rpcsync.xirdb";
        private static StreamWriter _syncDatabaseStreamWriter;
        public static bool InSave;
        private static long _totalTransactionRead;

        public static Dictionary<string, long> DatabaseTransactionSync = new Dictionary<string, long>();

        /// <summary>
        /// Initialize sync database.
        /// </summary>
        /// <returns></returns>
        public static bool InitializeSyncDatabase()
        {
            try
            {
                if (!File.Exists(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + SyncDatabaseFile)))
                {
                    File.Create(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + SyncDatabaseFile)).Close();
                }
                else
                {
                    using (FileStream fs = File.Open(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + SyncDatabaseFile), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (BufferedStream bs = new BufferedStream(fs))
                        {
                            using (StreamReader sr = new StreamReader(bs))
                            {
                                string line;
                                while ((line = sr.ReadLine()) != null)
                                {
                                    if (line.Contains(ClassSyncDatabaseEnumeration.DatabaseSyncStartLine))
                                    {
                                        string transactionLine = line.Replace(ClassSyncDatabaseEnumeration.DatabaseSyncStartLine, "");
                                        var splitTransactionLine = transactionLine.Split(new[] { "|" }, StringSplitOptions.None);
                                        string walletAddress = splitTransactionLine[0];
                                        if (ClassRpcDatabase.RpcDatabaseContent.ContainsKey(walletAddress))
                                        {
                                            string transaction = ClassAlgo.GetDecryptedResultManual(ClassAlgoEnumeration.Rijndael, splitTransactionLine[1], walletAddress + ClassRpcDatabase.RpcDatabaseContent[walletAddress].GetWalletPublicKey(), ClassWalletNetworkSetting.KeySize);
                                            transaction += "#" + walletAddress;

                                            var splitTransaction = transaction.Split(new[] { "#" }, StringSplitOptions.None);
                                            if (splitTransaction[0] == ClassSyncDatabaseEnumeration.DatabaseAnonymousTransactionMode)
                                            {
                                                ClassRpcDatabase.RpcDatabaseContent[walletAddress].InsertWalletTransactionSync(transaction, true, false);
                                            }
                                            else
                                            {
                                                ClassRpcDatabase.RpcDatabaseContent[walletAddress].InsertWalletTransactionSync(transaction, false, false);
                                            }
                                            if (!DatabaseTransactionSync.ContainsKey(transaction))
                                            {
                                                DatabaseTransactionSync.Add(transaction, long.Parse(splitTransaction[7]));
                                            }
                                            _totalTransactionRead++;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                return false;
            }
            _syncDatabaseStreamWriter = new StreamWriter(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + SyncDatabaseFile), true, Encoding.UTF8, 8192) { AutoFlush = true };
            ClassConsole.ConsoleWriteLine("Total transaction read from sync database: " + _totalTransactionRead, ClassConsoleColorEnumeration.IndexConsoleGreenLog, ClassConsoleLogLevelEnumeration.LogLevelSyncDatabase);
            return true;
        }

        /// <summary>
        /// Insert a new transaction to database.
        /// </summary>
        /// <param name="walletAddress"></param>
        /// <param name="walletPublicKey"></param>
        /// <param name="transaction"></param>
        public static async void InsertTransactionToSyncDatabaseAsync(string walletAddress, string walletPublicKey, string transaction)
        {
            await Task.Factory.StartNew(delegate
            {
                InSave = true;
                bool success = false;
                while (!success)
                {
                    try
                    {
                        string transactionTmp = transaction + "#" + walletAddress;
                        var splitTransaction = transactionTmp.Split(new[] { "#" }, StringSplitOptions.None);
                        if (!DatabaseTransactionSync.ContainsKey(transactionTmp))
                        {
                            DatabaseTransactionSync.Add(transactionTmp, long.Parse(splitTransaction[7]));
                        }
                        transaction = ClassAlgo.GetEncryptedResultManual(ClassAlgoEnumeration.Rijndael, transaction, walletAddress + walletPublicKey, ClassWalletNetworkSetting.KeySize);
                        string transactionLine = ClassSyncDatabaseEnumeration.DatabaseSyncStartLine + walletAddress + "|" + transaction;
                        _syncDatabaseStreamWriter.WriteLine(transactionLine);
                        success = true;
                    }
                    catch
                    {
                        _syncDatabaseStreamWriter = new StreamWriter(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + SyncDatabaseFile), true, Encoding.UTF8, 8192) { AutoFlush = true };
                    }
                }
                _totalTransactionRead++;
                ClassConsole.ConsoleWriteLine("Total transaction saved: " + DatabaseTransactionSync.Count);
                InSave = false;
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Current);
        }
    }
}
