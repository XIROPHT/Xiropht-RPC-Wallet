using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Linq;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xiropht_Connector_All.RPC;
using Xiropht_Connector_All.Setting;
using Xiropht_Connector_All.Utils;
using Xiropht_Connector_All.Wallet;
using Xiropht_Rpc_Wallet.ConsoleObject;
using Xiropht_Rpc_Wallet.Database;
using Xiropht_Rpc_Wallet.Setting;
using System.Net.Sockets;
using System.Text;
using Xiropht_Rpc_Wallet.Utility;
using System.Collections.Generic;

namespace Xiropht_Rpc_Wallet.Wallet
{
    public class ClassWalletUpdater
    {
        private static Thread ThreadAutoUpdateWallet;
        private const string RpcTokenNetworkNotExist = "not_exist";
        private const string RpcTokenNetworkWalletAddressNotExist = "wallet_address_not_exist";
        private const string RpcTokenNetworkWalletBusyOnUpdate = "WALLET-BUSY-ON-UPDATE";

        /// <summary>
        /// Enable auto update wallet system.
        /// </summary>
        public static void EnableAutoUpdateWallet()
        {

            if (ThreadAutoUpdateWallet != null && (ThreadAutoUpdateWallet.IsAlive || ThreadAutoUpdateWallet != null))
            {
                ThreadAutoUpdateWallet.Abort();
                GC.SuppressFinalize(ThreadAutoUpdateWallet);
            }

            ThreadAutoUpdateWallet = new Thread(delegate ()
            {
                while (!Program.Exit)
                {
                    if (ClassRpcDatabase.RpcDatabaseContent.Count > 0)
                    {
                        string getSeedNodeRandom = string.Empty;
                        bool seedNodeSelected = false;
                        if (ClassConnectorSetting.SeedNodeIp.Count > 1)
                        {
                            foreach (var seedNode in GetSeedNodeSpeedList().ToArray())
                            {
                                getSeedNodeRandom = seedNode.Key;
                                Task taskCheckSeedNode = Task.Run(async () => seedNodeSelected = await CheckTcp.CheckTcpClientAsync(seedNode.Key, ClassConnectorSetting.SeedNodeTokenPort));
                                taskCheckSeedNode.Wait(ClassConnectorSetting.MaxTimeoutConnect);
                                if (seedNodeSelected)
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            getSeedNodeRandom = ClassConnectorSetting.SeedNodeIp.ToArray().ElementAt(0).Key;
                            seedNodeSelected = true;
                        }
                        if (seedNodeSelected)
                        {
                            foreach (var walletObject in ClassRpcDatabase.RpcDatabaseContent.ToArray()) // Copy temporaly the database of wallets in the case of changes on the enumeration done by a parallal process, update all of them.
                            {
                                try
                                {
                                    if (Program.Exit)
                                    {
                                        break;
                                    }


                                    if (!walletObject.Value.GetWalletUpdateStatus() && walletObject.Value.GetLastWalletUpdate() <= DateTimeOffset.Now.ToUnixTimeSeconds())
                                    {
                                        ClassRpcDatabase.RpcDatabaseContent[walletObject.Key].SetLastWalletUpdate(DateTimeOffset.Now.ToUnixTimeSeconds() + ClassRpcSetting.WalletUpdateInterval);
                                        ClassRpcDatabase.RpcDatabaseContent[walletObject.Key].SetWalletOnUpdateStatus(true);
                                        UpdateWalletTarget(getSeedNodeRandom, walletObject.Key);
                                    }

                                }
                                catch (Exception error)
                                {
#if DEBUG
                                    Console.WriteLine("Error on update wallet: " + walletObject.Key + " | Exception: " + error.Message);
#endif
                                }
                            }
                        }


                    }
                    Thread.Sleep(ClassRpcSetting.WalletUpdateInterval * 1000);
                }
            });
            ThreadAutoUpdateWallet.Start();
        }

        /// <summary>
        /// Update wallet target
        /// </summary>
        /// <param name="getSeedNodeRandom"></param>
        /// <param name="walletAddress"></param>
        private static void UpdateWalletTarget(string getSeedNodeRandom, string walletAddress)
        {
            ThreadPool.QueueUserWorkItem(async delegate
            {

#if DEBUG
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
#endif
                try
                {
                    if (!await GetWalletBalanceTokenAsync(getSeedNodeRandom, walletAddress))
                    {
                        ClassRpcDatabase.RpcDatabaseContent[walletAddress].SetLastWalletUpdate(0);
#if DEBUG
                        Console.WriteLine("Wallet: " + walletAddress + " update failed. Node: " + getSeedNodeRandom);
#endif
                    }
                    else
                    {
                        ClassRpcDatabase.RpcDatabaseContent[walletAddress].SetLastWalletUpdate(DateTimeOffset.Now.ToUnixTimeSeconds() + ClassRpcSetting.WalletUpdateInterval);
#if DEBUG
                        Console.WriteLine("Wallet: " + walletAddress + " updated successfully. Node: " + getSeedNodeRandom);
#endif
                    }
                }
                catch (Exception error)
                {
#if DEBUG
                    Console.WriteLine("Error on update wallet: " + walletAddress + " exception: " + error.Message);
#endif
                }
#if DEBUG
                stopwatch.Stop();
                Console.WriteLine("Wallet: " + walletAddress + " updated in: " + stopwatch.ElapsedMilliseconds + " ms. Node: " + getSeedNodeRandom);
#endif
                ClassRpcDatabase.RpcDatabaseContent[walletAddress].SetWalletOnUpdateStatus(false);

            });
        }

        /// <summary>
        /// Disable auto update wallet system.
        /// </summary>
        public static void DisableAutoUpdateWallet()
        {
            if (ThreadAutoUpdateWallet != null && (ThreadAutoUpdateWallet.IsAlive || ThreadAutoUpdateWallet != null))
            {
                ThreadAutoUpdateWallet.Abort();
                GC.SuppressFinalize(ThreadAutoUpdateWallet);
            }
        }

        /// <summary>
        /// Update Wallet target
        /// </summary>
        /// <param name="walletAddress"></param>
        /// <returns></returns>
        public static async Task UpdateWallet(string walletAddress)
        {
            string getSeedNodeRandom = string.Empty;
            bool seedNodeSelected = false;
            foreach (var seedNode in GetSeedNodeSpeedList().ToArray())
            {
                getSeedNodeRandom = seedNode.Key;
                Task taskCheckSeedNode = Task.Run(async () => seedNodeSelected = await CheckTcp.CheckTcpClientAsync(seedNode.Key, ClassConnectorSetting.SeedNodeTokenPort));
                taskCheckSeedNode.Wait(ClassConnectorSetting.MaxTimeoutConnect);
                if (seedNodeSelected)
                {
                    break;
                }
            }
            if (seedNodeSelected)
            {
                ClassRpcDatabase.RpcDatabaseContent[walletAddress].SetWalletOnUpdateStatus(true);
                await GetWalletBalanceTokenAsync(getSeedNodeRandom, walletAddress);
                ClassRpcDatabase.RpcDatabaseContent[walletAddress].SetWalletOnUpdateStatus(false);
            }
        }

        private static Dictionary<string, int> ListOfSeedNodesSpeed = new Dictionary<string, int>();

        /// <summary>
        /// Get Seed Node list sorted by the faster to the slowest one.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, int> GetSeedNodeSpeedList()
        {
            if (ListOfSeedNodesSpeed.Count == 0)
            {
                foreach (var seedNode in ClassConnectorSetting.SeedNodeIp.ToArray())
                {

                    try
                    {
                        int seedNodeResponseTime = -1;
                        Task taskCheckSeedNode = Task.Run(() => seedNodeResponseTime = CheckPing.CheckPingHost(seedNode.Key, true));
                        taskCheckSeedNode.Wait(ClassConnectorSetting.MaxPingDelay);
                        if (seedNodeResponseTime == -1)
                        {
                            seedNodeResponseTime = ClassConnectorSetting.MaxSeedNodeTimeoutConnect;
                        }
#if DEBUG
                        Console.WriteLine(seedNode.Key + " response time: " + seedNodeResponseTime + " ms.");
#endif
                        ListOfSeedNodesSpeed.Add(seedNode.Key, seedNodeResponseTime);

                    }
                    catch
                    {
                        ListOfSeedNodesSpeed.Add(seedNode.Key, ClassConnectorSetting.MaxSeedNodeTimeoutConnect); // Max delay.
                    }

                }
            }
            else if (ListOfSeedNodesSpeed.Count != ClassConnectorSetting.SeedNodeIp.Count)
            {
                ClassConsole.ConsoleWriteLine("New seed node(s) listed, update the list of seed nodes sorted by their ping time.", ClassConsoleColorEnumeration.IndexConsoleYellowLog, ClassConsoleLogLevelEnumeration.LogLevelGeneral);
                var tmpListOfSeedNodesSpeed = new Dictionary<string, int>();
                foreach (var seedNode in ClassConnectorSetting.SeedNodeIp.ToArray())
                {

                    try
                    {
                        int seedNodeResponseTime = -1;
                        Task taskCheckSeedNode = Task.Run(() => seedNodeResponseTime = CheckPing.CheckPingHost(seedNode.Key, true));
                        taskCheckSeedNode.Wait(ClassConnectorSetting.MaxPingDelay);
                        if (seedNodeResponseTime == -1)
                        {
                            seedNodeResponseTime = ClassConnectorSetting.MaxSeedNodeTimeoutConnect;
                        }
#if DEBUG
                        Console.WriteLine(seedNode.Key + " response time: " + seedNodeResponseTime + " ms.");
#endif
                        tmpListOfSeedNodesSpeed.Add(seedNode.Key, seedNodeResponseTime);

                    }
                    catch
                    {
                        tmpListOfSeedNodesSpeed.Add(seedNode.Key, ClassConnectorSetting.MaxSeedNodeTimeoutConnect); // Max delay.
                    }

                }
                ListOfSeedNodesSpeed = tmpListOfSeedNodesSpeed;
                ClassConsole.ConsoleWriteLine("List of seed nodes sorted by their ping time done.", ClassConsoleColorEnumeration.IndexConsoleGreenLog, ClassConsoleLogLevelEnumeration.LogLevelGeneral);

            }
            return ListOfSeedNodesSpeed.ToArray().OrderBy(u => u.Value).ToDictionary(z => z.Key, y => y.Value);
        }

        /// <summary>
        /// Get wallet token from token system.
        /// </summary>
        /// <param name="getSeedNodeRandom"></param>
        /// <param name="walletAddress"></param>
        /// <returns></returns>
        private static async Task<string> GetWalletTokenAsync(string getSeedNodeRandom, string walletAddress)
        {
            string encryptedRequest = ClassRpcWalletCommand.TokenAsk + "|empty-token|" + (DateTimeOffset.Now.ToUnixTimeSeconds() + 1).ToString("F0");
            encryptedRequest = ClassAlgo.GetEncryptedResultManual(ClassAlgoEnumeration.Rijndael, encryptedRequest, walletAddress + ClassRpcDatabase.RpcDatabaseContent[walletAddress].GetWalletPublicKey() + ClassRpcDatabase.RpcDatabaseContent[walletAddress].GetWalletPassword(), ClassWalletNetworkSetting.KeySize);
            string responseWallet = await ProceedTokenRequestHttpAsync("http://" + getSeedNodeRandom + ":" + ClassConnectorSetting.SeedNodeTokenPort + "/" + ClassConnectorSettingEnumeration.WalletTokenType + "|" + walletAddress + "|" + encryptedRequest);
            //string responseWallet = await ProceedTokenRequestTcpAsync(getSeedNodeRandom, ClassConnectorSetting.SeedNodeTokenPort, ClassConnectorSettingEnumeration.WalletTokenType + "|" + walletAddress + "|" + encryptedRequest);

            try
            {
                var responseWalletJson = JObject.Parse(responseWallet);
                responseWallet = responseWalletJson["result"].ToString();
                if (responseWallet != RpcTokenNetworkNotExist)
                {
                    responseWallet = ClassAlgo.GetDecryptedResultManual(ClassAlgoEnumeration.Rijndael, responseWallet, walletAddress + ClassRpcDatabase.RpcDatabaseContent[walletAddress].GetWalletPublicKey() + ClassRpcDatabase.RpcDatabaseContent[walletAddress].GetWalletPassword(), ClassWalletNetworkSetting.KeySize);
                    var splitResponseWallet = responseWallet.Split(new[] { "|" }, StringSplitOptions.None);
                    if ((long.Parse(splitResponseWallet[splitResponseWallet.Length - 1]) + 10) - DateTimeOffset.Now.ToUnixTimeSeconds() < 60)
                    {
                        if (long.Parse(splitResponseWallet[splitResponseWallet.Length - 1]) + 60 >= DateTimeOffset.Now.ToUnixTimeSeconds())
                        {
                            ClassRpcDatabase.RpcDatabaseContent[walletAddress].SetWalletCurrentToken(true, splitResponseWallet[1]);
                            return splitResponseWallet[1];
                        }
                        else
                        {
                            return RpcTokenNetworkNotExist;
                        }
                    }
                    else
                    {
                        return RpcTokenNetworkNotExist;
                    }
                }
                else
                {
                    return RpcTokenNetworkNotExist;
                }
            }
            catch(Exception error)
            {
#if DEBUG
                Debug.WriteLine("Exception GetWalletTokenAsync: " + error.Message);
#endif
                return RpcTokenNetworkNotExist;
            }
        }

        /// <summary>
        /// Update wallet balance from token system.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="getSeedNodeRandom"></param>
        /// <param name="walletAddress"></param>
        public static async Task<bool> GetWalletBalanceTokenAsync(string getSeedNodeRandom, string walletAddress)
        {
            string token = await GetWalletTokenAsync(getSeedNodeRandom, walletAddress);
            if (token != RpcTokenNetworkNotExist)
            {
                if (ClassRpcDatabase.RpcDatabaseContent[walletAddress].GetWalletCurrentToken().Item1)
                {
                    token = ClassRpcDatabase.RpcDatabaseContent[walletAddress].GetWalletCurrentToken().Item2;
                    ClassRpcDatabase.RpcDatabaseContent[walletAddress].SetWalletCurrentToken(false, string.Empty);
                    string encryptedRequest = ClassRpcWalletCommand.TokenAskBalance + "|" + token + "|" + (DateTimeOffset.Now.ToUnixTimeSeconds() + 1);
                    encryptedRequest = ClassAlgo.GetEncryptedResultManual(ClassAlgoEnumeration.Rijndael, encryptedRequest, walletAddress + ClassRpcDatabase.RpcDatabaseContent[walletAddress].GetWalletPublicKey() + ClassRpcDatabase.RpcDatabaseContent[walletAddress].GetWalletPassword(), ClassWalletNetworkSetting.KeySize);
                    string responseWallet = await ProceedTokenRequestHttpAsync("http://" + getSeedNodeRandom + ":" + ClassConnectorSetting.SeedNodeTokenPort + "/" + ClassConnectorSettingEnumeration.WalletTokenType + "|" + walletAddress + "|" + encryptedRequest);

                    //string responseWallet = await ProceedTokenRequestTcpAsync(getSeedNodeRandom, ClassConnectorSetting.SeedNodeTokenPort, ClassConnectorSettingEnumeration.WalletTokenType + "|" + walletAddress + "|" + encryptedRequest);

                    try
                    {
                        var responseWalletJson = JObject.Parse(responseWallet);
                        responseWallet = responseWalletJson["result"].ToString();
                        if (responseWallet != RpcTokenNetworkNotExist)
                        {
                            responseWallet = ClassAlgo.GetDecryptedResultManual(ClassAlgoEnumeration.Rijndael, responseWallet, walletAddress + ClassRpcDatabase.RpcDatabaseContent[walletAddress].GetWalletPublicKey() + ClassRpcDatabase.RpcDatabaseContent[walletAddress].GetWalletPassword() + token, ClassWalletNetworkSetting.KeySize);
                            if (responseWallet != ClassAlgoErrorEnumeration.AlgoError)
                            {
                                string walletBalance = responseWallet;
                                var splitWalletBalance = walletBalance.Split(new[] { "|" }, StringSplitOptions.None);
                                if ((long.Parse(splitWalletBalance[splitWalletBalance.Length - 1]) + 10) - DateTimeOffset.Now.ToUnixTimeSeconds() < 60)
                                {
                                    if (long.Parse(splitWalletBalance[splitWalletBalance.Length - 1]) + 10 >= DateTimeOffset.Now.ToUnixTimeSeconds())
                                    {
                                        ClassRpcDatabase.RpcDatabaseContent[walletAddress].SetWalletBalance(splitWalletBalance[1]);
                                        ClassRpcDatabase.RpcDatabaseContent[walletAddress].SetWalletPendingBalance(splitWalletBalance[2]);
                                        ClassRpcDatabase.RpcDatabaseContent[walletAddress].SetWalletUniqueId(splitWalletBalance[3]);
                                        ClassRpcDatabase.RpcDatabaseContent[walletAddress].SetWalletAnonymousUniqueId(splitWalletBalance[4]);
                                        return true;
                                    }
                                    else
                                    {
                                        ClassConsole.ConsoleWriteLine("Wallet packet time balance token request: " + walletBalance + " is expired.", ClassConsoleColorEnumeration.IndexConsoleRedLog, ClassConsoleLogLevelEnumeration.LogLevelWalletObject);
                                    }
                                }
                                else
                                {
                                    ClassConsole.ConsoleWriteLine("Wallet packet time balance token request: " + walletBalance + " is too huge", ClassConsoleColorEnumeration.IndexConsoleRedLog, ClassConsoleLogLevelEnumeration.LogLevelWalletObject);
                                }
                            }
                        }
                    }
                    catch (Exception error)
                    {
                        Debug.WriteLine("Exception GetWalletBalanceTokenAsync: " + error.Message);
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            return false;
        }


        /// <summary>
        /// Send a transaction from a selected wallet address stored to a specific wallet address target.
        /// </summary>
        /// <param name="getSeedNodeRandom"></param>
        /// <param name="walletAddress"></param>
        /// <param name="walletAddressTarget"></param>
        /// <param name="amount"></param>
        /// <param name="fee"></param>
        /// <param name="anonymous"></param>
        /// <returns></returns>
        private static async Task<string> SendWalletTransactionTokenAsync(string getSeedNodeRandom, string walletAddress, string walletAddressTarget, string amount, string fee, bool anonymous)
        {

            string tokenWallet = await GetWalletTokenAsync(getSeedNodeRandom, walletAddress);
            if (tokenWallet != RpcTokenNetworkNotExist)
            {
                if (ClassRpcDatabase.RpcDatabaseContent[walletAddress].GetWalletCurrentToken().Item1)
                {
                    tokenWallet = ClassRpcDatabase.RpcDatabaseContent[walletAddress].GetWalletCurrentToken().Item2;
                    ClassRpcDatabase.RpcDatabaseContent[walletAddress].SetWalletCurrentToken(false, string.Empty);

                    string encryptedRequest = string.Empty;
                    if (anonymous)
                    {
                        encryptedRequest = ClassRpcWalletCommand.TokenAskWalletSendTransaction + "|" + tokenWallet + "|" + walletAddressTarget + "|" + amount + "|" + fee + "|1|" + (DateTimeOffset.Now.ToUnixTimeSeconds() + 1).ToString("F0");
                    }
                    else
                    {
                        encryptedRequest = ClassRpcWalletCommand.TokenAskWalletSendTransaction + "|" + tokenWallet + "|" + walletAddressTarget + "|" + amount + "|" + fee + "|0|" + (DateTimeOffset.Now.ToUnixTimeSeconds() + 1).ToString("F0");
                    }
                    encryptedRequest = ClassAlgo.GetEncryptedResultManual(ClassAlgoEnumeration.Rijndael, encryptedRequest, walletAddress + ClassRpcDatabase.RpcDatabaseContent[walletAddress].GetWalletPublicKey() + ClassRpcDatabase.RpcDatabaseContent[walletAddress].GetWalletPassword(), ClassWalletNetworkSetting.KeySize);
                    string responseWallet = await ProceedTokenRequestHttpAsync("http://" + getSeedNodeRandom + ":" + ClassConnectorSetting.SeedNodeTokenPort + "/" + ClassConnectorSettingEnumeration.WalletTokenType + "|" + walletAddress + "|" + encryptedRequest);
                    //string responseWallet = await ProceedTokenRequestTcpAsync(getSeedNodeRandom, ClassConnectorSetting.SeedNodeTokenPort, ClassConnectorSettingEnumeration.WalletTokenType + "|" + walletAddress + "|" + encryptedRequest);

                    ClassRpcDatabase.RpcDatabaseContent[walletAddress].SetWalletOnUpdateStatus(false);
                    try
                    {
                        var responseWalletJson = JObject.Parse(responseWallet);
                        responseWallet = responseWalletJson["result"].ToString();
                        if (responseWallet != RpcTokenNetworkNotExist)
                        {
                            responseWallet = ClassAlgo.GetDecryptedResultManual(ClassAlgoEnumeration.Rijndael, responseWallet, walletAddress + ClassRpcDatabase.RpcDatabaseContent[walletAddress].GetWalletPublicKey() + ClassRpcDatabase.RpcDatabaseContent[walletAddress].GetWalletPassword() + tokenWallet, ClassWalletNetworkSetting.KeySize);
                            if (responseWallet != ClassAlgoErrorEnumeration.AlgoError)
                            {
                                string walletTransaction = responseWallet;
                                if (responseWallet != RpcTokenNetworkNotExist)
                                {
                                    var splitWalletTransaction = walletTransaction.Split(new[] { "|" }, StringSplitOptions.None);
                                    if ((long.Parse(splitWalletTransaction[splitWalletTransaction.Length - 1]) + 10) - DateTimeOffset.Now.ToUnixTimeSeconds() < 60)
                                    {
                                        if (long.Parse(splitWalletTransaction[splitWalletTransaction.Length - 1]) + 10 >= DateTimeOffset.Now.ToUnixTimeSeconds())
                                        {
                                            ClassRpcDatabase.RpcDatabaseContent[walletAddress].SetWalletBalance(splitWalletTransaction[1]);
                                            ClassRpcDatabase.RpcDatabaseContent[walletAddress].SetWalletPendingBalance(splitWalletTransaction[2]);
                                            ClassConsole.ConsoleWriteLine("Send transaction response " + splitWalletTransaction[0] + " from wallet address " + walletAddress + " of amount " + amount + " " + ClassConnectorSetting.CoinNameMin + " fee " + fee + " " + ClassConnectorSetting.CoinNameMin + " transaction hash: " + splitWalletTransaction[3].ToLower() + " to target -> " + walletAddressTarget, ClassConsoleColorEnumeration.IndexConsoleYellowLog, ClassConsoleLogLevelEnumeration.LogLevelWalletObject);
                                            ClassRpcDatabase.RpcDatabaseContent[walletAddress].SetWalletOnSendTransactionStatus(false);
                                            return splitWalletTransaction[0] + "|" + splitWalletTransaction[3];
                                        }
                                        return splitWalletTransaction[0] + "|expired_packet";
                                    }
                                }
                                else
                                {
                                    ClassRpcDatabase.RpcDatabaseContent[walletAddress].SetWalletOnSendTransactionStatus(false);
                                    return ClassRpcWalletCommand.SendTokenTransactionBusy + "|None";
                                }
                            }
                            else
                            {
                                ClassRpcDatabase.RpcDatabaseContent[walletAddress].SetWalletOnSendTransactionStatus(false);
                                return ClassRpcWalletCommand.SendTokenTransactionBusy + "|None";
                            }
                        }
                        else
                        {
                            ClassRpcDatabase.RpcDatabaseContent[walletAddress].SetWalletOnSendTransactionStatus(false);
                            return ClassRpcWalletCommand.SendTokenTransactionRefused + "|None";
                        }
                    }
                    catch (Exception error)
                    {
#if DEBUG
                    Debug.WriteLine("Exception SendWalletTransactionTokenAsync: " + error.Message);
#endif
                    }
                }
            }

            ClassConsole.ConsoleWriteLine("Send transaction refused from wallet address " + walletAddress + " of amount " + amount + " " + ClassConnectorSetting.CoinNameMin + " fee " + fee + " " + ClassConnectorSetting.CoinNameMin + " to target -> " + walletAddressTarget, ClassConsoleColorEnumeration.IndexConsoleYellowLog, ClassConsoleLogLevelEnumeration.LogLevelWalletObject);
            ClassRpcDatabase.RpcDatabaseContent[walletAddress].SetWalletOnSendTransactionStatus(false);
            return ClassRpcWalletCommand.SendTokenTransactionRefused + "|None";
        }


        /// <summary>
        /// Send a transaction from a selected wallet address stored to a specific wallet address target.
        /// </summary>
        /// <param name="getSeedNodeRandom"></param>
        /// <param name="walletAddress"></param>
        /// <param name="walletAddressTarget"></param>
        /// <param name="amount"></param>
        /// <param name="fee"></param>
        /// <param name="anonymous"></param>
        /// <returns></returns>
        private static async Task<string> SendWalletTransferTokenAsync(string getSeedNodeRandom, string walletAddress, string walletAddressTarget, string amount)
        {
            if (ClassRpcDatabase.RpcDatabaseContent.ContainsKey(walletAddressTarget))
            {
                string tokenWallet = await GetWalletTokenAsync(getSeedNodeRandom, walletAddress);
                if (tokenWallet != RpcTokenNetworkNotExist)
                {
                    if (ClassRpcDatabase.RpcDatabaseContent[walletAddress].GetWalletCurrentToken().Item1)
                    {
                        tokenWallet = ClassRpcDatabase.RpcDatabaseContent[walletAddress].GetWalletCurrentToken().Item2;
                        ClassRpcDatabase.RpcDatabaseContent[walletAddress].SetWalletCurrentToken(false, string.Empty);

                        string privateKeyTarget = ClassRpcDatabase.RpcDatabaseContent[walletAddressTarget].GetWalletPrivateKey(); 
                        if (privateKeyTarget.Contains("$"))
                        {
                            privateKeyTarget = privateKeyTarget.Split(new[] { "$" }, StringSplitOptions.None)[0];
                        }
                        string keyTargetRequest = walletAddressTarget + ClassRpcDatabase.RpcDatabaseContent[walletAddressTarget].GetWalletPublicKey() + ClassRpcDatabase.RpcDatabaseContent[walletAddressTarget].GetWalletPassword() + privateKeyTarget;
                        string encryptedTargetRequest = ClassAlgo.GetEncryptedResultManual(ClassAlgoEnumeration.Rijndael, amount, keyTargetRequest, ClassWalletNetworkSetting.KeySize);


                        string encryptedRequest = ClassRpcWalletCommand.TokenAskWalletTransfer + "|" + tokenWallet + "|" + walletAddressTarget + "|" + encryptedTargetRequest + "|" + (DateTimeOffset.Now.ToUnixTimeSeconds() + 1).ToString("F0");
                        encryptedRequest = ClassAlgo.GetEncryptedResultManual(ClassAlgoEnumeration.Rijndael, encryptedRequest, walletAddress + ClassRpcDatabase.RpcDatabaseContent[walletAddress].GetWalletPublicKey() + ClassRpcDatabase.RpcDatabaseContent[walletAddress].GetWalletPassword(), ClassWalletNetworkSetting.KeySize);

                        string responseWallet = await ProceedTokenRequestHttpAsync("http://" + getSeedNodeRandom + ":" + ClassConnectorSetting.SeedNodeTokenPort + "/" + ClassConnectorSettingEnumeration.WalletTokenType + "|" + walletAddress + "|" + encryptedRequest);

                        ClassRpcDatabase.RpcDatabaseContent[walletAddress].SetWalletOnUpdateStatus(false);
                        var responseWalletJson = JObject.Parse(responseWallet);
                        responseWallet = responseWalletJson["result"].ToString();
                        if (responseWallet != RpcTokenNetworkNotExist)
                        {
                            responseWallet = ClassAlgo.GetDecryptedResultManual(ClassAlgoEnumeration.Rijndael, responseWallet, walletAddress + ClassRpcDatabase.RpcDatabaseContent[walletAddress].GetWalletPublicKey() + ClassRpcDatabase.RpcDatabaseContent[walletAddress].GetWalletPassword() + tokenWallet, ClassWalletNetworkSetting.KeySize);
                            if (responseWallet != ClassAlgoErrorEnumeration.AlgoError)
                            {
                                string walletTransaction = responseWallet;
                                if (responseWallet != RpcTokenNetworkNotExist)
                                {
                                    var splitWalletTransaction = walletTransaction.Split(new[] { "|" }, StringSplitOptions.None);
                                    if ((long.Parse(splitWalletTransaction[splitWalletTransaction.Length - 1]) + 10) - DateTimeOffset.Now.ToUnixTimeSeconds() < 60)
                                    {
                                        if (long.Parse(splitWalletTransaction[splitWalletTransaction.Length - 1]) + 10 >= DateTimeOffset.Now.ToUnixTimeSeconds())
                                        {
                                            ClassRpcDatabase.RpcDatabaseContent[walletAddress].SetWalletBalance(splitWalletTransaction[1]);
                                            ClassRpcDatabase.RpcDatabaseContent[walletAddress].SetWalletPendingBalance(splitWalletTransaction[2]);
                                            ClassConsole.ConsoleWriteLine("Send transfer response " + splitWalletTransaction[0] + " from wallet address " + walletAddress + " of amount " + amount + " " + ClassConnectorSetting.CoinNameMin + " transaction hash: " + splitWalletTransaction[3].ToLower() + " to target -> " + walletAddressTarget, ClassConsoleColorEnumeration.IndexConsoleYellowLog, ClassConsoleLogLevelEnumeration.LogLevelWalletObject);
                                            ClassRpcDatabase.RpcDatabaseContent[walletAddress].SetWalletOnSendTransactionStatus(false);
                                            return splitWalletTransaction[0] + "|" + splitWalletTransaction[3];
                                        }
                                        return splitWalletTransaction[0] + "|expired_packet";
                                    }
                                }
                                else
                                {
                                    ClassRpcDatabase.RpcDatabaseContent[walletAddress].SetWalletOnSendTransactionStatus(false);
                                    return ClassRpcWalletCommand.SendTokenTransferBusy + "|None";
                                }
                            }
                            else
                            {
                                ClassRpcDatabase.RpcDatabaseContent[walletAddress].SetWalletOnSendTransactionStatus(false);
                                return ClassRpcWalletCommand.SendTokenTransferBusy + "|None";
                            }
                        }
                        else
                        {
                            ClassRpcDatabase.RpcDatabaseContent[walletAddress].SetWalletOnSendTransactionStatus(false);
                            return ClassRpcWalletCommand.SendTokenTransferRefused + "|None";
                        }
                    }
                }
                else
                {
                    ClassConsole.ConsoleWriteLine("Send transfer refused from wallet address " + walletAddress + " of amount " + amount + " to target -> " + walletAddressTarget, ClassConsoleColorEnumeration.IndexConsoleYellowLog, ClassConsoleLogLevelEnumeration.LogLevelWalletObject);
                    ClassRpcDatabase.RpcDatabaseContent[walletAddress].SetWalletOnSendTransactionStatus(false);
                    return ClassRpcWalletCommand.SendTokenTransferRefused + "|None";
                }
            }
            else
            {
                ClassConsole.ConsoleWriteLine("Send transfer refused from wallet address " + walletAddress + " of amount " + amount + " to target -> " + walletAddressTarget + " | RPC Wallet don't contain wallet informations of: " + walletAddressTarget, ClassConsoleColorEnumeration.IndexConsoleYellowLog, ClassConsoleLogLevelEnumeration.LogLevelWalletObject);
                ClassRpcDatabase.RpcDatabaseContent[walletAddress].SetWalletOnSendTransactionStatus(false);

                return ClassRpcWalletCommand.SendTokenTransactionInvalidTarget + "|None";
            }
            ClassRpcDatabase.RpcDatabaseContent[walletAddress].SetWalletOnSendTransactionStatus(false);
            return ClassRpcWalletCommand.SendTokenTransferRefused + "|None";

        }

        /// <summary>
        /// Proceed token request throught http protocol.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static async Task<string> ProceedTokenRequestHttpAsync(string url)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;
            request.ServicePoint.Expect100Continue = false;
            request.ServicePoint.ConnectionLimit = 65535;
            request.KeepAlive = false;
            request.Timeout = ClassRpcSetting.WalletMaxKeepAliveUpdate * 1000;
            request.UserAgent = ClassConnectorSetting.CoinName + " RPC Wallet - " + Assembly.GetExecutingAssembly().GetName().Version + "R";
            string responseContent = string.Empty;
            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();

            }
        }

        /// <summary>
        /// Proceed token request in full TCP Mode
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="packet"></param>
        /// <returns></returns>
        private static async Task<string> ProceedTokenRequestTcpAsync(string host, int port, string packet)
        {
            string httpTokenPacket = "GET /" + packet + " HTTP/1.1\r\n";

            using (var tokenPacketObject = new SendTcpTokenPacketObject(ClassRpcSetting.WalletMaxKeepAliveUpdate))
            {
                return await tokenPacketObject.ProceedTokenPacketByTcp(host, port, httpTokenPacket);
            }
        }

        /// <summary>
        /// Send a transaction with token system with a selected wallet address, amount and fee.
        /// </summary>
        /// <param name="walletAddress"></param>
        /// <returns></returns>
        public static async Task<string> ProceedTransactionTokenRequestAsync(string walletAddress, string amount, string fee, string walletAddressTarget, bool anonymous)
        {
            if (anonymous)
            {
                ClassConsole.ConsoleWriteLine("Attempt to send an anonymous transaction from wallet address " + walletAddress + " of amount " + amount + " " + ClassConnectorSetting.CoinNameMin + " fee " + fee + " " + ClassConnectorSetting.CoinNameMin + " and anonymous fee option of: " + ClassConnectorSetting.MinimumWalletTransactionAnonymousFee + " " + ClassConnectorSetting.CoinNameMin + " to target -> " + walletAddressTarget, ClassConsoleColorEnumeration.IndexConsoleYellowLog, ClassConsoleLogLevelEnumeration.LogLevelWalletObject);
            }
            else
            {
                ClassConsole.ConsoleWriteLine("Attempt to send transaction from wallet address " + walletAddress + " of amount " + amount + " " + ClassConnectorSetting.CoinNameMin + " fee " + fee + " " + ClassConnectorSetting.CoinNameMin + " to target -> " + walletAddressTarget, ClassConsoleColorEnumeration.IndexConsoleYellowLog, ClassConsoleLogLevelEnumeration.LogLevelWalletObject);
            }
            if (ClassRpcDatabase.RpcDatabaseContent.ContainsKey(walletAddress))
            {
                if (!ClassRpcDatabase.RpcDatabaseContent[walletAddress].GetWalletUpdateStatus() && !ClassRpcDatabase.RpcDatabaseContent[walletAddress].GetWalletOnSendTransactionStatus())
                {
                    ClassRpcDatabase.RpcDatabaseContent[walletAddress].SetWalletOnSendTransactionStatus(true);
                    decimal balanceFromDatabase = decimal.Parse(ClassRpcDatabase.RpcDatabaseContent[walletAddress].GetWalletBalance().Replace(".", ","), NumberStyles.Currency, Program.GlobalCultureInfo);
                    decimal balanceFromRequest = decimal.Parse(amount.Replace(".", ","), NumberStyles.Currency, Program.GlobalCultureInfo);
                    decimal feeFromRequest = decimal.Parse(fee.Replace(".", ","), NumberStyles.Currency, Program.GlobalCultureInfo);

                    ClassRpcDatabase.RpcDatabaseContent[walletAddress].SetLastWalletUpdate(DateTimeOffset.Now.ToUnixTimeSeconds());
                    string getSeedNodeRandom = string.Empty;
                    bool seedNodeSelected = false;
                    foreach (var seedNode in GetSeedNodeSpeedList().ToArray())
                    {
                        getSeedNodeRandom = seedNode.Key;
                        Task taskCheckSeedNode = Task.Run(async () => seedNodeSelected = await CheckTcp.CheckTcpClientAsync(seedNode.Key, ClassConnectorSetting.SeedNodeTokenPort));
                        taskCheckSeedNode.Wait(ClassConnectorSetting.MaxTimeoutConnect);
                        if (seedNodeSelected)
                        {
                            break;
                        }
                    }
                    if (seedNodeSelected)
                    {
                        try
                        {
                            return await SendWalletTransactionTokenAsync(getSeedNodeRandom, walletAddress, walletAddressTarget, amount, fee, anonymous);
                        }
                        catch (Exception error)
                        {
                            ClassConsole.ConsoleWriteLine("Error on send transaction from wallet: " + ClassRpcDatabase.RpcDatabaseContent[walletAddress] + " exception: " + error.Message, ClassConsoleColorEnumeration.IndexConsoleRedLog, ClassConsoleLogLevelEnumeration.LogLevelWalletObject);
#if DEBUG
                            Console.WriteLine("Error on send transaction from wallet: " + ClassRpcDatabase.RpcDatabaseContent[walletAddress] + " exception: " + error.Message);
#endif
                            ClassRpcDatabase.RpcDatabaseContent[walletAddress].SetWalletOnSendTransactionStatus(false);
                            return ClassRpcWalletCommand.SendTokenTransactionRefused + "|None";
                        }
                    }
                    else
                    {
                        ClassConsole.ConsoleWriteLine("Error on send transaction from wallet: " + ClassRpcDatabase.RpcDatabaseContent[walletAddress] + " exception: can't connect on each seed nodes checked.", ClassConsoleColorEnumeration.IndexConsoleRedLog, ClassConsoleLogLevelEnumeration.LogLevelWalletObject);
                        ClassRpcDatabase.RpcDatabaseContent[walletAddress].SetWalletOnSendTransactionStatus(false);
                        return ClassRpcWalletCommand.SendTokenTransactionRefused + "|None";
                    }
                }
                else
                {
                    if (ClassRpcDatabase.RpcDatabaseContent[walletAddress].GetWalletUpdateStatus())
                    {
                        return RpcTokenNetworkWalletBusyOnUpdate + "|None";
                    }
                    else
                    {
                        return ClassRpcWalletCommand.SendTokenTransactionBusy + "|None";
                    }
                }
            }

            return RpcTokenNetworkWalletAddressNotExist + "|None";

        }

        /// <summary>
        /// Send a transaction with token system with a selected wallet address, amount and fee.
        /// </summary>
        /// <param name="walletAddress"></param>
        /// <returns></returns>
        public static async Task<string> ProceedTransferTokenRequestAsync(string walletAddress, string amount, string walletAddressTarget)
        {

            ClassConsole.ConsoleWriteLine("Attempt to send transfer from wallet address " + walletAddress + " of amount " + amount + " to target -> " + walletAddressTarget, ClassConsoleColorEnumeration.IndexConsoleYellowLog, ClassConsoleLogLevelEnumeration.LogLevelWalletObject);

            if (ClassRpcDatabase.RpcDatabaseContent.ContainsKey(walletAddress))
            {
                if (!ClassRpcDatabase.RpcDatabaseContent[walletAddress].GetWalletUpdateStatus() && !ClassRpcDatabase.RpcDatabaseContent[walletAddress].GetWalletOnSendTransactionStatus())
                {
                    ClassRpcDatabase.RpcDatabaseContent[walletAddress].SetWalletOnSendTransactionStatus(true);
                    decimal balanceFromDatabase = decimal.Parse(ClassRpcDatabase.RpcDatabaseContent[walletAddress].GetWalletBalance().Replace(".", ","), NumberStyles.Currency, Program.GlobalCultureInfo);
                    decimal balanceFromRequest = decimal.Parse(amount.Replace(".", ","), NumberStyles.Currency, Program.GlobalCultureInfo);

                    ClassRpcDatabase.RpcDatabaseContent[walletAddress].SetLastWalletUpdate(DateTimeOffset.Now.ToUnixTimeSeconds());
                    string getSeedNodeRandom = string.Empty;
                    bool seedNodeSelected = false;
                    foreach (var seedNode in GetSeedNodeSpeedList().ToArray())
                    {
                        getSeedNodeRandom = seedNode.Key;
                        Task taskCheckSeedNode = Task.Run(async () => seedNodeSelected = await CheckTcp.CheckTcpClientAsync(getSeedNodeRandom, ClassConnectorSetting.SeedNodeTokenPort));
                        taskCheckSeedNode.Wait(ClassConnectorSetting.MaxTimeoutConnect);
                        if (seedNodeSelected)
                        {
                            break;
                        }
                    }
                    if (seedNodeSelected)
                    {
                        try
                        {
                            return await SendWalletTransferTokenAsync(getSeedNodeRandom, walletAddress, walletAddressTarget, amount);
                        }
                        catch (Exception error)
                        {
                            ClassConsole.ConsoleWriteLine("Error on send transfer from wallet: " + ClassRpcDatabase.RpcDatabaseContent[walletAddress] + " exception: " + error.Message, ClassConsoleColorEnumeration.IndexConsoleRedLog, ClassConsoleLogLevelEnumeration.LogLevelWalletObject);
#if DEBUG
                            Console.WriteLine("Error on send transaction from wallet: " + ClassRpcDatabase.RpcDatabaseContent[walletAddress] + " exception: " + error.Message);
#endif
                            ClassRpcDatabase.RpcDatabaseContent[walletAddress].SetWalletOnSendTransactionStatus(false);
                            return ClassRpcWalletCommand.SendTokenTransferRefused + "|None";
                        }
                    }
                    else
                    {
                        ClassConsole.ConsoleWriteLine("Error on send transfer from wallet: " + ClassRpcDatabase.RpcDatabaseContent[walletAddress] + " exception: can't connect on each seed nodes checked.", ClassConsoleColorEnumeration.IndexConsoleRedLog, ClassConsoleLogLevelEnumeration.LogLevelWalletObject);
                        ClassRpcDatabase.RpcDatabaseContent[walletAddress].SetWalletOnSendTransactionStatus(false);
                        return ClassRpcWalletCommand.SendTokenTransferRefused + "|None";
                    }
                }
                else
                {
                    if (ClassRpcDatabase.RpcDatabaseContent[walletAddress].GetWalletUpdateStatus())
                    {
                        return RpcTokenNetworkWalletBusyOnUpdate + "|None";
                    }
                    else
                    {
                        return ClassRpcWalletCommand.SendTokenTransferBusy + "|None";
                    }
                }
            }
            ClassRpcDatabase.RpcDatabaseContent[walletAddress].SetWalletOnSendTransactionStatus(false);
            return RpcTokenNetworkWalletAddressNotExist + "|None";

        }
    }

    public class SendTcpTokenPacketObject : IDisposable
    {

        #region Disposing Part Implementation 

        private bool _disposed;

        ~SendTcpTokenPacketObject()
        {
            Dispose(false);
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                }
            }

            _disposed = true;
        }

#endregion

        private int MaxKeepAlive;
        private string result;
        private TcpClient client;
        private long startDate;
        private bool connnectionStatus;

        public SendTcpTokenPacketObject(int maxKeepAlive)
        {
            MaxKeepAlive = maxKeepAlive;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        public async Task<string> ProceedTokenPacketByTcp(string host, int port, string packet)
        {
            await Task.Delay(100);

            result = string.Empty;
            startDate = DateTimeOffset.Now.ToUnixTimeSeconds() + MaxKeepAlive;
            client = new TcpClient();
            try
            {
                await client.ConnectAsync(host, port);
            }
            catch
            {
                return string.Empty;
            }
            connnectionStatus = true;
            await Task.Factory.StartNew(ListenConnection, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Current).ConfigureAwait(false);

            if (await SendPacketToTokenNetwork(packet))
            {
                while (startDate >= DateTimeOffset.Now.ToUnixTimeSeconds() && connnectionStatus && string.IsNullOrEmpty(result))
                {
                    await Task.Delay(1000);
#if DEBUG
                    Debug.WriteLine("Packet Received: " + result);
#endif
                    result = "{" + ClassUtility.RemoveHTTPHeader(result) + "}";
#if DEBUG
                    Debug.WriteLine("Packet formatted: "+ result);
#endif
                    return result;
                }
            }
            return result;
        }

        private async Task ListenConnection()
        {
            while (connnectionStatus && startDate >= DateTimeOffset.Now.ToUnixTimeSeconds() && string.IsNullOrEmpty(result))
            {
                try
                {
                    using (var readerNetwork = new NetworkStream(client.Client))
                    {
                        int received = 0;
                        byte[] buffer = new byte[ClassConnectorSetting.MaxNetworkPacketSize];
                        while ((received = await readerNetwork.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            result = Encoding.UTF8.GetString(buffer, 0, received);
                            connnectionStatus = false;
                            break;
                        }
                    }
                }
                catch
                {
                    break;
                }
            }
            connnectionStatus = false;
        }

        /// <summary>
        /// Send packet to the network of blockchain.
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="encrypted"></param>
        private async Task<bool> SendPacketToTokenNetwork(string packet, bool encrypted = false)
        {
            try
            {
                using (var networkStream = new NetworkStream(client.Client))
                {
                    byte[] bytePacket = Encoding.UTF8.GetBytes(packet);
                    await networkStream.WriteAsync(bytePacket, 0, bytePacket.Length).ConfigureAwait(false);
                    await networkStream.FlushAsync().ConfigureAwait(false);
                }
            }
            catch
            {
                connnectionStatus = false;
                return false;
            }
            return true;
        }

    }

}
