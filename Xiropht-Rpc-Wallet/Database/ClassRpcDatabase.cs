using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xiropht_Connector_All.Utils;
using Xiropht_Connector_All.Wallet;
using Xiropht_Rpc_Wallet.ConsoleObject;
using Xiropht_Rpc_Wallet.Utility;
using Xiropht_Rpc_Wallet.Wallet;

namespace Xiropht_Rpc_Wallet.Database
{
    public class ClassRpcDatabaseEnumeration
    {
        public const string DatabaseWalletStartLine = "[WALLET]";
    }

    public class ClassRpcDatabase
    {
        private static string RpcDatabasePassword; // This password permit to decrypt each lines of the database.
        private const string RpcDatabaseFile = "\\rpcdata.xirdb"; // Content every wallet informations.
        public static Dictionary<string, ClassWalletObject> RpcDatabaseContent; // Content every wallets (wallet address and public key only)
        private static StreamWriter RpcDatabaseStreamWriter; // Permit to keep alive a stream writer for write a new wallet information created.

        /// <summary>
        /// Set rpc database password.
        /// </summary>
        /// <param name="password"></param>
        public static void SetRpcDatabasePassword(string password)
        {
            RpcDatabasePassword = ClassAlgo.GetEncryptedResult(ClassAlgoEnumeration.Rijndael, password, password, ClassWalletNetworkSetting.KeySize); // Encrypt the password with the password.
        }

        /// <summary>
        /// Load RPC Database file.
        /// </summary>
        /// <returns></returns>
        public static bool LoadRpcDatabaseFile()
        {
            RpcDatabaseContent = new Dictionary<string, ClassWalletObject>();
            try
            {
                if (File.Exists(ClassUtility.ConvertPath(Directory.GetCurrentDirectory() + RpcDatabaseFile)))
                {
                    using (FileStream fs = File.Open(Directory.GetCurrentDirectory() + RpcDatabaseFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
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
                                    if (line.StartsWith(ClassRpcDatabaseEnumeration.DatabaseWalletStartLine))
                                    {
                                        string walletData = line.Replace(ClassRpcDatabaseEnumeration.DatabaseWalletStartLine, "");
                                        walletData = ClassAlgo.GetDecryptedResult(ClassAlgoEnumeration.Rijndael, walletData, RpcDatabasePassword, ClassWalletNetworkSetting.KeySize);
                                        if (walletData != ClassAlgoErrorEnumeration.AlgoError)
                                        {
                                            var splitWalletData = walletData.Split(new[] { "|" }, StringSplitOptions.None);
                                            var walletAddress = splitWalletData[0];
                                            if (!RpcDatabaseContent.ContainsKey(walletAddress))
                                            {
                                                var walletPublicKey = splitWalletData[1];
                                                var walletObject = new ClassWalletObject(walletAddress, walletPublicKey);
                                                RpcDatabaseContent.Add(walletAddress, walletObject);
                                            }
                                        }
                                        else
                                        {
                                            ClassConsole.ConsoleWriteLine("Decryption failed at line " + lineRead);
                                            return false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    File.Create(ClassUtility.ConvertPath(Directory.GetCurrentDirectory() + RpcDatabaseFile)).Close();
                }
            }
            catch
            {
                return false;
            }
            RpcDatabaseStreamWriter = new StreamWriter(Directory.GetCurrentDirectory() + RpcDatabaseFile, true, Encoding.UTF8, 8192) { AutoFlush = true };
            return true;
        }

        /// <summary>
        /// Insert a new wallet informations to the datbases file.
        /// </summary>
        /// <param name="walletAddress"></param>
        /// <param name="walletPublicKey"></param>
        /// <param name="walletPrivateKey"></param>
        /// <param name="walletPinCode"></param>
        /// <param name="walletPassword"></param>
        public static void InsertNewWallet(string walletAddress, string walletPublicKey, string walletPrivateKey, string walletPinCode, string walletPassword)
        {
            var walletObject = new ClassWalletObject(walletAddress, walletPublicKey);
            RpcDatabaseContent.Add(walletAddress, walletObject);
            try
            {
                string encryptedWallet = ClassAlgo.GetEncryptedResult(ClassAlgoEnumeration.Rijndael, walletAddress + "|" + walletPublicKey + "|" + walletPrivateKey + "|" + walletPinCode + "|" + walletPassword, RpcDatabasePassword, ClassWalletNetworkSetting.KeySize);
                RpcDatabaseStreamWriter.WriteLine(ClassRpcDatabaseEnumeration.DatabaseWalletStartLine + encryptedWallet);
            }
            catch
            {
                RpcDatabaseStreamWriter = new StreamWriter(Directory.GetCurrentDirectory() + RpcDatabaseFile, true, Encoding.UTF8, 8192) { AutoFlush = true };

            }
        }
    }
}
