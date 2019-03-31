using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xiropht_Connector_All.Utils;
using Xiropht_Connector_All.Wallet;
using Xiropht_Rpc_Wallet.ConsoleObject;
using Xiropht_Rpc_Wallet.Threading;
using Xiropht_Rpc_Wallet.Utility;

namespace Xiropht_Rpc_Wallet.Database
{
    public class ClassSyncDatabaseEnumeration
    {
        public const string DatabaseSyncStartLine = "[TRANSACTION]";
    }

    public class ClassSyncDatabase
    {
        private const string SyncDatabaseFile = "\\rpcsync.xirdb";
        private static StreamWriter SyncDatabaseStreamWriter;
        public static bool InSave;
        private static long TotalTransactionRead;

        /// <summary>
        /// Initialize sync database.
        /// </summary>
        /// <returns></returns>
        public static bool InitializeSyncDatabase()
        {
            try
            {
                if (!File.Exists(ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + SyncDatabaseFile)))
                {
                    File.Create(ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + SyncDatabaseFile)).Close();
                }
                else
                {
                    using (FileStream fs = File.Open(ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + SyncDatabaseFile), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (BufferedStream bs = new BufferedStream(fs))
                        {
                            using (StreamReader sr = new StreamReader(bs))
                            {
                                string line;
                                int lineRead = 0;
                                while ((line = sr.ReadLine()) != null)
                                {
                                    lineRead++;
                                    if (line.Contains(ClassSyncDatabaseEnumeration.DatabaseSyncStartLine))
                                    {
                                        string transactionLine = line.Replace(ClassSyncDatabaseEnumeration.DatabaseSyncStartLine, "");
                                        var splitTransactionLine = transactionLine.Split(new[] { "|" }, StringSplitOptions.None);
                                        string walletAddress = splitTransactionLine[0];
                                        if (ClassRpcDatabase.RpcDatabaseContent.ContainsKey(walletAddress))
                                        {
                                            string transaction = ClassAlgo.GetDecryptedResult(ClassAlgoEnumeration.Rijndael, splitTransactionLine[1], walletAddress + ClassRpcDatabase.RpcDatabaseContent[walletAddress].GetWalletPublicKey(), ClassWalletNetworkSetting.KeySize);
                                            var splitTransaction = transaction.Split(new[] { "#" }, StringSplitOptions.None);
                                            if (splitTransaction[0] == "anonymous")
                                            {
                                                ClassRpcDatabase.RpcDatabaseContent[walletAddress].InsertWalletTransactionSync(transaction, true, false);
                                            }
                                            else
                                            {
                                                ClassRpcDatabase.RpcDatabaseContent[walletAddress].InsertWalletTransactionSync(transaction, false, false);
                                            }
                                            TotalTransactionRead++;
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
            SyncDatabaseStreamWriter = new StreamWriter(ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + SyncDatabaseFile), true, Encoding.UTF8, 8192) { AutoFlush = true };
            ClassConsole.ConsoleWriteLine("Total transaction read from sync database: " + TotalTransactionRead, ClassConsoleColorEnumeration.IndexConsoleGreenLog, ClassConsoleLogLevelEnumeration.LogLevelSyncDatabase);
            return true;
        }

        /// <summary>
        /// Insert a new transaction to database.
        /// </summary>
        /// <param name="walletAddress"></param>
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
                        transaction = ClassAlgo.GetEncryptedResult(ClassAlgoEnumeration.Rijndael, transaction, walletAddress + walletPublicKey, ClassWalletNetworkSetting.KeySize);
                        string transactionLine = ClassSyncDatabaseEnumeration.DatabaseSyncStartLine + walletAddress + "|" + transaction;
                        SyncDatabaseStreamWriter.WriteLine(transactionLine);
                        success = true;
                    }
                    catch
                    {
                        SyncDatabaseStreamWriter = new StreamWriter(ClassUtility.ConvertPath(System.AppDomain.CurrentDomain.BaseDirectory + SyncDatabaseFile), true, Encoding.UTF8, 8192) { AutoFlush = true };
                    }
                }
                TotalTransactionRead++;
                ClassConsole.ConsoleWriteLine("Total transaction saved: " + TotalTransactionRead);
                InSave = false;
            }, CancellationToken.None, TaskCreationOptions.None, PriorityScheduler.Lowest);
        }
    }
}
