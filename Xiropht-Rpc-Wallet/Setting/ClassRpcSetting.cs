using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Xiropht_Connector_All.Setting;
using Xiropht_Rpc_Wallet.ConsoleObject;
using Xiropht_Rpc_Wallet.Utility;

namespace Xiropht_Rpc_Wallet.Setting
{
    public class ClassRpcSettingEnumeration
    {
        public const string SettingApiIpBindSetting = "API-BIND-IP";
        public const string SettingApiPortSetting = "API-PORT";
        public const string SettingApiWhitelist = "API-WHITELIST";
        public const string SettingApiKeyRequestEncryption = "API-KEY-REQUEST-ENCRYPTION";
        public const string SettingApiEnableXForwardedForResolver = "API-ENABLE-X-FORWARDED-FOR-RESOLVER";
        public const string SettingEnableRemoteNodeSync = "ENABLE-REMOTE-NODE-SYNC";
        public const string SettingRemoteNodeHost = "REMOTE-NODE-HOST";
        public const string SettingRemoteNodePort = "REMOTE-NODE-PORT";
        public const string SettingWalletUpdateInterval = "WALLET-UPDATE-INTERVAL";
        public const string SettingWalletMaxKeepAliveUpdate = "WALLET-MAX-KEEP-ALIVE-UPDATE";
        public const string SettingWalletEnableAutoUpdate = "WALLET-ENABLE-AUTO-UPDATE";
        public const string SettingEnableBackupWalletSystem = "WALLET-ENABLE-BACKUP-SYSTEM";
        public const string SettingIntervalBackupWalletSystem = "WALLET-INTERVAL-BACKUP-SYSTEM";
        public const string SettingEnableBackupWalletAutoRemoveSystem = "WALLET-ENABLE-BACKUP-AUTO-REMOVE-SYSTEM";
        public const string SettingWalletBackupLapsingTimeLimit = "WALLET-BACKUP-LAPSING-TIME-LIMIT";

    }


    public class ClassRpcSettingObject
    {
        public int rpc_wallet_api_port;
        public List<string> rpc_wallet_api_ip_whitelist;
        public string rpc_wallet_api_ip_bind;
        public string rpc_wallet_key_request_encryption;
        public bool rpc_wallet_api_enable_x_forwarded_for_resolver;
        public bool rpc_wallet_enable_remote_node_sync;
        public string rpc_wallet_remote_node_host;
        public int rpc_wallet_remote_node_port;
        public bool wallet_enable_auto_update;
        public int wallet_update_interval;
        public int wallet_max_keep_alive_update;
        public bool wallet_enable_backup_system;
        public int wallet_interval_backup_system;
        public bool wallet_enable_auto_remove_backup_system;
        public int wallet_backup_lapsing_time_limit;
        public bool wallet_enable_auto_clean_log;
        public int wallet_auto_clean_log_interval;
        public bool rpc_wallet_enable_api_task_scheduler;
        public bool enable_european_exchange_rule;
    }

    public class ClassRpcSetting
    {
        private const string RpcWalletSettingFile = "\\config.json";
        private const string RpcWalletSettingOldFile = "\\config.ini";
        private const int RpcApiKeyMinSize = 8;

        public static int RpcWalletApiPort = 8000; // RPC Wallet API Default port.

        public static List<string> RpcWalletApiIpWhitelist = new List<string>(); // List of IP whitelisted on the API Server, if the list is empty everyone can try to access on the port.

        public static string RpcWalletApiIpBind = "127.0.0.1";

        public static string RpcWalletApiKeyRequestEncryption = string.Empty; // The key for encrypt request to receive/sent.

        public static bool RpcWalletApiEnableXForwardedForResolver;

        public static bool RpcWalletEnableRemoteNodeSync; // Enable remote node sync

        public static string RpcWalletRemoteNodeHost = string.Empty; // Remote Node Host address

        public static int RpcWalletRemoteNodePort = ClassConnectorSetting.RemoteNodePort; // Remote Node Port

        public static int WalletUpdateInterval = 60; // Interval of time in second(s) between whole updates of wallets informations.

        public static int WalletMaxKeepAliveUpdate = 5; // Max Keep Alive time in second(s) task of update wallet informations.

        public static bool WalletEnableAutoUpdateWallet = true; // Enable auto update of wallets informations.

        public static bool WalletEnableBackupSystem = true; // Enable auto backup system of current wallet database content.

        public static int WalletIntervalBackupSystem = 3600; // Interval of backup current wallet database content.

        public static bool WalletEnableAutoRemoveBackupSystem = false; // Enable auto remove backup system.

        public static int WalletBackupLapsingTimeLimit = 84600; // Each wallet backup file date more than this limit are deleted.

        public static bool WalletEnableAutoCleanLog = true; // Enable auto clean logs.

        public static int WalletAutoCleanLogInterval = 3600; // Interval of cleanup logs.

        public static bool RpcWalletEnableApiTaskScheduler = false;

        public static bool RpcWalletEnableEuropeanExchangeRule;

        /// <summary>
        /// Initialize setting of RPC Wallet
        /// </summary>
        /// <returns></returns>
        public static bool InitializeRpcWalletSetting()
        {
            try
            {
                if (File.Exists(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + RpcWalletSettingFile)))
                {
                    bool invalidSetting = false;
                    ClassRpcSettingObject jsonRpcWalletSettingObject = null;
                    bool needUpdate = false;
                    using (var streamReaderConfigPool = new StreamReader(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + RpcWalletSettingFile)))
                    {
                        string line = string.Empty;
                        string jsonContent = string.Empty;
                        while ((line = streamReaderConfigPool.ReadLine()) != null)
                        {
                            if (!string.IsNullOrEmpty(line))
                            {
                                if (!line.StartsWith("/"))
                                {
                                    jsonContent += line;
                                }
                            }
                        }
                        if (!jsonContent.Contains("rpc_wallet_enable_api_task_scheduler") || !jsonContent.Contains("enable_european_exchange_rule"))
                        {
                            needUpdate = true;
                        }
                        try
                        {
                            jsonRpcWalletSettingObject = JsonConvert.DeserializeObject<ClassRpcSettingObject>(jsonContent);
                        }
                        catch
                        {
                            invalidSetting = true;
                        }
                    }
                    if (invalidSetting)
                    {
                        ClassConsole.ConsoleWriteLine("Configuration file of RPC Wallet is invalid, do you want to create a new setting? [Y/N]", ClassConsoleColorEnumeration.IndexConsoleRedLog);
                        bool choose = Console.ReadLine().ToLower() == "y";
                        if (choose)
                        {
                            File.Create(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + RpcWalletSettingFile)).Close();
                            MakeRpcWalletSetting();
                        }
                        else
                        {
                            return false;
                        }
                    }

                    else
                    {
                        if (jsonRpcWalletSettingObject != null)
                        {
                            bool error = false;
                            WalletEnableBackupSystem = jsonRpcWalletSettingObject.wallet_enable_backup_system;
                            WalletIntervalBackupSystem = jsonRpcWalletSettingObject.wallet_interval_backup_system;
                            WalletEnableAutoRemoveBackupSystem = jsonRpcWalletSettingObject.wallet_enable_auto_remove_backup_system;
                            WalletBackupLapsingTimeLimit = jsonRpcWalletSettingObject.wallet_backup_lapsing_time_limit;
                            if (IPAddress.TryParse(jsonRpcWalletSettingObject.rpc_wallet_api_ip_bind, out var ipAddress))
                            {
                                RpcWalletApiIpBind = jsonRpcWalletSettingObject.rpc_wallet_api_ip_bind;
                            }
                            else
                            {
                                Console.WriteLine("Error on configuration, Exception: " + jsonRpcWalletSettingObject.rpc_wallet_api_ip_bind + " can't be parsed. It seems this is not a valid IP.");
                                error = true;
                            }
                            RpcWalletApiPort = jsonRpcWalletSettingObject.rpc_wallet_api_port;
                            RpcWalletApiIpWhitelist = jsonRpcWalletSettingObject.rpc_wallet_api_ip_whitelist;
                            if (RpcWalletApiIpWhitelist == null)
                            {
                                RpcWalletApiIpWhitelist = new List<string>();
                            }
                            RpcWalletApiKeyRequestEncryption = jsonRpcWalletSettingObject.rpc_wallet_key_request_encryption;
                            RpcWalletEnableRemoteNodeSync = jsonRpcWalletSettingObject.rpc_wallet_enable_remote_node_sync;
                            RpcWalletRemoteNodeHost = jsonRpcWalletSettingObject.rpc_wallet_remote_node_host;
                            RpcWalletRemoteNodePort = jsonRpcWalletSettingObject.rpc_wallet_remote_node_port;
                            WalletEnableAutoUpdateWallet = jsonRpcWalletSettingObject.wallet_enable_auto_update;
                            WalletUpdateInterval = jsonRpcWalletSettingObject.wallet_update_interval;
                            WalletMaxKeepAliveUpdate = jsonRpcWalletSettingObject.wallet_max_keep_alive_update;
                            RpcWalletApiEnableXForwardedForResolver = jsonRpcWalletSettingObject.rpc_wallet_api_enable_x_forwarded_for_resolver;
                            WalletEnableAutoCleanLog = jsonRpcWalletSettingObject.wallet_enable_auto_clean_log;
                            WalletAutoCleanLogInterval = jsonRpcWalletSettingObject.wallet_auto_clean_log_interval;
                            RpcWalletEnableApiTaskScheduler = jsonRpcWalletSettingObject.rpc_wallet_enable_api_task_scheduler;
                            RpcWalletEnableEuropeanExchangeRule = jsonRpcWalletSettingObject.enable_european_exchange_rule;
                            if (error)
                            {
                                ClassConsole.ConsoleWriteLine("Configuration file of RPC Wallet seems to contain some errors, do you want to continue? [Y/N]", ClassConsoleColorEnumeration.IndexConsoleRedLog);
                                bool choose = Console.ReadLine().ToLower() == "y";
                                if (!choose)
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                if (needUpdate)
                                {
                                    ClassConsole.ConsoleWriteLine("Configuration file of RPC Wallet has been updated to implement a new option.", ClassConsoleColorEnumeration.IndexConsoleYellowLog);
                                    SaveRpcWalletSetting();
                                }
                            }
                        }
                        else
                        {
                            ClassConsole.ConsoleWriteLine("Configuration file of RPC Wallet is invalid, do you want to create a new setting? [Y/N]", ClassConsoleColorEnumeration.IndexConsoleRedLog);
                            bool choose = Console.ReadLine().ToLower() == "y";
                            if (choose)
                            {
                                File.Create(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + RpcWalletSettingFile)).Close();
                                MakeRpcWalletSetting();
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }
                else
                {
                    if (File.Exists(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + RpcWalletSettingOldFile)))
                    {
                        bool containUpdate = false;

                        using (var streamReaderConfigPool = new StreamReader(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + RpcWalletSettingOldFile)))
                        {
                            int numberOfLines = 0;
                            string line = string.Empty;
                            while ((line = streamReaderConfigPool.ReadLine()) != null)
                            {
                                numberOfLines++;
                                if (!string.IsNullOrEmpty(line))
                                {
                                    if (!line.StartsWith("/"))
                                    {
                                        if (line.Contains("="))
                                        {
                                            var splitLine = line.Split(new[] { "=" }, StringSplitOptions.None);
                                            if (splitLine.Length > 1)
                                            {

                                                try
                                                {
#if DEBUG
                                                Console.WriteLine("Config line read: " + splitLine[0] + " argument read: " + splitLine[1]);
#endif
                                                    switch (splitLine[0])
                                                    {
                                                        case ClassRpcSettingEnumeration.SettingEnableBackupWalletSystem:
                                                            if (splitLine[1].ToLower() == "y" || splitLine[1].ToLower() == "true")
                                                            {
                                                                WalletEnableBackupSystem = true;
                                                            }
                                                            else
                                                            {
                                                                WalletEnableBackupSystem = false;
                                                            }
                                                            break;
                                                        case ClassRpcSettingEnumeration.SettingIntervalBackupWalletSystem:
                                                            if (int.TryParse(splitLine[1], out var intervalBackupSystem))
                                                            {
                                                                WalletIntervalBackupSystem = intervalBackupSystem;
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Error on line: " + ClassRpcSettingEnumeration.SettingIntervalBackupWalletSystem + ", use default interval: " + WalletIntervalBackupSystem);
                                                            }
                                                            break;
                                                        case ClassRpcSettingEnumeration.SettingEnableBackupWalletAutoRemoveSystem:
                                                            if (splitLine[1].ToLower() == "y" || splitLine[1].ToLower() == "true")
                                                            {
                                                                WalletEnableAutoRemoveBackupSystem = true;
                                                            }
                                                            else
                                                            {
                                                                WalletEnableAutoRemoveBackupSystem = false;
                                                            }
                                                            containUpdate = true;
                                                            break;
                                                        case ClassRpcSettingEnumeration.SettingWalletBackupLapsingTimeLimit:
                                                            if (int.TryParse(splitLine[1], out var laspingTimeLimit))
                                                            {
                                                                WalletBackupLapsingTimeLimit = laspingTimeLimit;
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Error on config line: " + splitLine[0] + " on line:" + numberOfLines + " | Exception: " + splitLine[1] + ", use default lapsing timelimit: " + WalletBackupLapsingTimeLimit);
                                                            }
                                                            containUpdate = true;
                                                            break;
                                                        case ClassRpcSettingEnumeration.SettingApiIpBindSetting:
                                                            if (IPAddress.TryParse(splitLine[1], out var ipAddress))
                                                            {
                                                                RpcWalletApiIpBind = splitLine[1];
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Error on config line: " + splitLine[0] + " on line:" + numberOfLines + " | Exception: " + splitLine[1] + " is not an IP.");
                                                            }
                                                            break;
                                                        case ClassRpcSettingEnumeration.SettingApiPortSetting:
                                                            if (splitLine.Length > 1)
                                                            {
                                                                if (int.TryParse(splitLine[1], out var rpcApiPort))
                                                                {
                                                                    if (rpcApiPort <= 0 || rpcApiPort >= 65535)
                                                                    {
                                                                        Console.WriteLine("Error on config line: " + splitLine[0] + " on line:" + numberOfLines + " | Exception: " + splitLine[1] + " is not a valid port number.");
                                                                        Console.WriteLine("Use default port 8000");
                                                                    }
                                                                    else
                                                                    {
                                                                        RpcWalletApiPort = rpcApiPort;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    Console.WriteLine("Error on config line: " + splitLine[0] + " on line:" + numberOfLines + " | Exception: " + splitLine[1] + " is not a valid port number.");
                                                                    Console.WriteLine("Use default port 8000");
                                                                }
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Error on line: " + ClassRpcSettingEnumeration.SettingApiPortSetting + ", use default port: " + RpcWalletApiPort);
                                                            }
                                                            break;
                                                        case ClassRpcSettingEnumeration.SettingApiWhitelist:
                                                            if (splitLine.Length > 1)
                                                            {
                                                                if (splitLine[1].Contains(";"))
                                                                {
                                                                    var splitLineIp = splitLine[1].Split(new[] { ";" }, StringSplitOptions.None);
                                                                    foreach (var lineIp in splitLineIp)
                                                                    {
                                                                        if (lineIp != null)
                                                                        {
                                                                            if (!string.IsNullOrEmpty(lineIp))
                                                                            {
                                                                                if (lineIp.Length > 1)
                                                                                {
                                                                                    RpcWalletApiIpWhitelist.Add(lineIp);
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (splitLine[1] != null)
                                                                    {
                                                                        if (!string.IsNullOrEmpty(splitLine[1]))
                                                                        {
                                                                            if (splitLine[1].Length > 1)
                                                                            {
                                                                                RpcWalletApiIpWhitelist.Add(splitLine[1]);
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            break;
                                                        case ClassRpcSettingEnumeration.SettingApiKeyRequestEncryption:
                                                            if (splitLine.Length > 1)
                                                            {
                                                                RpcWalletApiKeyRequestEncryption = splitLine[1];
                                                                if (RpcWalletApiKeyRequestEncryption.Length < RpcApiKeyMinSize)
                                                                {
                                                                    ClassConsole.ConsoleWriteLine("Warning the current API Key encryption length is less than " + RpcApiKeyMinSize + " characters required by the salt system of encryption !", ClassConsoleColorEnumeration.IndexConsoleRedLog, Program.LogLevel);
                                                                }
                                                            }
                                                            break;
                                                        case ClassRpcSettingEnumeration.SettingEnableRemoteNodeSync:
                                                            if (splitLine.Length > 1)
                                                            {
                                                                if (splitLine[1].ToLower() == "y" || splitLine[1].ToLower() == "true")
                                                                {
                                                                    RpcWalletEnableRemoteNodeSync = true;
                                                                }
                                                            }
                                                            break;
                                                        case ClassRpcSettingEnumeration.SettingRemoteNodeHost:
                                                            if (splitLine.Length > 1)
                                                            {
                                                                RpcWalletRemoteNodeHost = splitLine[1];
                                                            }
                                                            break;
                                                        case ClassRpcSettingEnumeration.SettingRemoteNodePort:
                                                            if (splitLine.Length > 1)
                                                            {
                                                                RpcWalletRemoteNodePort = int.Parse(splitLine[1]);
                                                            }
                                                            break;
                                                        case ClassRpcSettingEnumeration.SettingWalletUpdateInterval:
                                                            if (int.TryParse(splitLine[1], out var walletUpdateInterval))
                                                            {
                                                                WalletUpdateInterval = walletUpdateInterval;
                                                            }
                                                            break;
                                                        case ClassRpcSettingEnumeration.SettingWalletEnableAutoUpdate:
                                                            if (splitLine[1].ToLower() == "y" || splitLine[1].ToLower() == "true")
                                                            {
                                                                ClassConsole.ConsoleWriteLine("Warning auto update is enabled, be sure to select a good interval time of update for don't be detected has flooding on seed nodes.", ClassConsoleColorEnumeration.IndexConsoleRedLog);
                                                                WalletEnableAutoUpdateWallet = true;
                                                            }
                                                            else
                                                            {

                                                                ClassConsole.ConsoleWriteLine("Warning auto update system is disabled, be sure to use your API to update manually wallets informations.", ClassConsoleColorEnumeration.IndexConsoleRedLog);
                                                                WalletEnableAutoUpdateWallet = false;
                                                            }
                                                            break;
                                                        case ClassRpcSettingEnumeration.SettingWalletMaxKeepAliveUpdate:
                                                            if (int.TryParse(splitLine[1], out var walletMaxKeepAliveUpdate))
                                                            {
                                                                WalletMaxKeepAliveUpdate = walletMaxKeepAliveUpdate;
                                                            }
                                                            break;
                                                        case ClassRpcSettingEnumeration.SettingApiEnableXForwardedForResolver:
                                                            if (splitLine[1].ToLower() == "y" || splitLine[1].ToLower() == "true")
                                                            {
                                                                RpcWalletApiEnableXForwardedForResolver = true;
                                                            }
                                                            break;
                                                        default:
                                                            if (splitLine.Length > 1)
                                                            {
                                                                Console.WriteLine("Unknown config line: " + splitLine[0] + " with argument: " + splitLine[1] + " on line: " + numberOfLines);
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Unknown config line: " + splitLine[0] + " with no argument on line: " + numberOfLines);
                                                            }
                                                            break;
                                                    }
                                                }
                                                catch
                                                {
                                                    Console.WriteLine("Error on line:" + numberOfLines);
                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine("Error on config line: " + splitLine[0] + " on line:" + numberOfLines);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (!containUpdate)
                        {
                            ClassConsole.ConsoleWriteLine("Setting system of RPC Wallet has been updated, create a new setting file now: ", ClassConsoleColorEnumeration.IndexConsoleMagentaLog);
                            File.Create(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + RpcWalletSettingFile)).Close();
                            MakeRpcWalletSetting();
                        }
                        else
                        {
                            SaveRpcWalletSetting();
                            File.Delete(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + RpcWalletSettingOldFile));
                        }
                    }
                    else
                    {
                        File.Create(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + RpcWalletSettingFile)).Close();
                        MakeRpcWalletSetting();
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Make setting of RPC Wallet
        /// </summary>
        private static void MakeRpcWalletSetting()
        {
            ClassConsole.ConsoleWriteLine("Setting up Web API:", ClassConsoleColorEnumeration.IndexConsoleYellowLog);

            ClassConsole.ConsoleWriteLine("Please write the IP Address to bind (by default 127.0.0.1): ", ClassConsoleColorEnumeration.IndexConsoleBlueLog);
            RpcWalletApiIpBind = Console.ReadLine();
            while (!IPAddress.TryParse(RpcWalletApiIpBind, out var ipAddress))
            {
                ClassConsole.ConsoleWriteLine(RpcWalletApiIpBind + " is not a valid IP Address. Please write the IP Address to bind (by default 127.0.0.1): ", ClassConsoleColorEnumeration.IndexConsoleRedLog);
                RpcWalletApiIpBind = Console.ReadLine();
            }

            ClassConsole.ConsoleWriteLine("Please select a port for your API to listen (By default 8000): ", ClassConsoleColorEnumeration.IndexConsoleBlueLog);
            string choose = Console.ReadLine();
            int portTmp = 0;
            while (!int.TryParse(choose, out var port))
            {
                ClassConsole.ConsoleWriteLine(choose + " is not a valid port number, please select a port for your API ", ClassConsoleColorEnumeration.IndexConsoleRedLog);
                choose = Console.ReadLine();
            }
            RpcWalletApiPort = portTmp;
            if (RpcWalletApiPort <= 0)
            {
                RpcWalletApiPort = 8000;
            }
            ClassConsole.ConsoleWriteLine("Do you want to accept only IP whitelisted? [Y/N]", ClassConsoleColorEnumeration.IndexConsoleBlueLog);
            bool yourChoose = Console.ReadLine().ToLower() == "y";
            if (yourChoose)
            {
                bool finish = false;
                while (!finish)
                {
                    ClassConsole.ConsoleWriteLine("Write an IP to whitelist: ", ClassConsoleColorEnumeration.IndexConsoleBlueLog);
                    string ip = Console.ReadLine();
                    RpcWalletApiIpWhitelist.Add(ip);
                    ClassConsole.ConsoleWriteLine("Do you want to write another IP? [Y/N]", ClassConsoleColorEnumeration.IndexConsoleBlueLog);
                    yourChoose = Console.ReadLine().ToLower() == "y";
                    if (!yourChoose)
                    {
                        finish = true;
                    }
                }
            }
            ClassConsole.ConsoleWriteLine("Do you want to use an encryption key of request in the API [AES 256bit is used]? [Y/N]", ClassConsoleColorEnumeration.IndexConsoleBlueLog);
            yourChoose = Console.ReadLine().ToLower() == "y";
            if (yourChoose)
            {
                ClassConsole.ConsoleWriteLine("Write your API Key (" + RpcApiKeyMinSize + " characters minimum required by the salt encryption system.): ", ClassConsoleColorEnumeration.IndexConsoleBlueLog);
                RpcWalletApiKeyRequestEncryption = Console.ReadLine();
                while (RpcWalletApiKeyRequestEncryption.Length < RpcApiKeyMinSize)
                {
                    ClassConsole.ConsoleWriteLine("Your API Key characters length is less than " + RpcApiKeyMinSize + " characters (Minimum required by the salt encryption system.), please write another one: ", ClassConsoleColorEnumeration.IndexConsoleRedLog);
                    RpcWalletApiKeyRequestEncryption = Console.ReadLine();
                }
            }

            ClassConsole.ConsoleWriteLine("Do you want to enable the X-FORWARDED-FOR resolver? (This option should be used once the API is behind a Proxy setting correctly.) [Y/N]", ClassConsoleColorEnumeration.IndexConsoleBlueLog);
            yourChoose = Console.ReadLine().ToLower() == "y";
            if (yourChoose)
            {
                RpcWalletApiEnableXForwardedForResolver = true;
            }

            ClassConsole.ConsoleWriteLine("Do you want to use a remote node for sync transactions of your wallets? [Y/N]", ClassConsoleColorEnumeration.IndexConsoleBlueLog);
            RpcWalletEnableRemoteNodeSync = Console.ReadLine().ToLower() == "y";
            if (RpcWalletEnableRemoteNodeSync)
            {
                ClassConsole.ConsoleWriteLine("Write the remote node IP/Hostname address: ", ClassConsoleColorEnumeration.IndexConsoleBlueLog);
                RpcWalletRemoteNodeHost = Console.ReadLine();
                ClassConsole.ConsoleWriteLine("Write the remote node port (by default " + ClassConnectorSetting.RemoteNodePort + "): ", ClassConsoleColorEnumeration.IndexConsoleBlueLog);
                int port = ClassConnectorSetting.RemoteNodePort;
                while (!int.TryParse(Console.ReadLine(), out port))
                {
                    ClassConsole.ConsoleWriteLine("Write a valid the remote node port (by default " + ClassConnectorSetting.RemoteNodePort + "): ", ClassConsoleColorEnumeration.IndexConsoleRedLog);
                }
                RpcWalletRemoteNodePort = port;
            }

            ClassConsole.ConsoleWriteLine("Do you to enable backup system of wallet database ? [Y/N]", ClassConsoleColorEnumeration.IndexConsoleBlueLog);
            WalletEnableBackupSystem = Console.ReadLine().ToLower() == "y";
            if (WalletEnableBackupSystem)
            {
                ClassConsole.ConsoleWriteLine("Write the interval of backup (by default " + WalletIntervalBackupSystem + " seconds): ", ClassConsoleColorEnumeration.IndexConsoleBlueLog);
                int interval = WalletIntervalBackupSystem;
                while (!int.TryParse(Console.ReadLine(), out interval))
                {
                    ClassConsole.ConsoleWriteLine("Write a valid interval of backup (by default " + WalletIntervalBackupSystem + " seconds): ", ClassConsoleColorEnumeration.IndexConsoleRedLog);
                }
                WalletIntervalBackupSystem = interval;

                ClassConsole.ConsoleWriteLine("Do you want to activate the system for auto-remove outdated backup wallet database files? [Y/N]", ClassConsoleColorEnumeration.IndexConsoleBlueLog);
                WalletEnableAutoRemoveBackupSystem = Console.ReadLine().ToLower() == "y";
                if (WalletEnableAutoRemoveBackupSystem)
                {
                    ClassConsole.ConsoleWriteLine("Write the limit of time of outdated backup wallet database files (by default " + WalletBackupLapsingTimeLimit + " seconds): ", ClassConsoleColorEnumeration.IndexConsoleBlueLog);
                    interval = WalletBackupLapsingTimeLimit;
                    while (!int.TryParse(Console.ReadLine(), out interval))
                    {
                        ClassConsole.ConsoleWriteLine("Write a valid limit of time of outdated backup wallet database files (by default " + WalletBackupLapsingTimeLimit + " seconds): ", ClassConsoleColorEnumeration.IndexConsoleRedLog);
                    }
                    WalletBackupLapsingTimeLimit = interval;
                }
            }


            string host = ClassRpcSettingEnumeration.SettingApiWhitelist + "=";
            if (RpcWalletApiIpWhitelist.Count > 0)
            {
                for (int i = 0; i < RpcWalletApiIpWhitelist.Count; i++)
                {
                    if (i < RpcWalletApiIpWhitelist.Count)
                    {
                        if (i < RpcWalletApiIpWhitelist.Count - 1)
                        {
                            host += RpcWalletApiIpWhitelist[i] + ";";
                        }
                        else
                        {
                            host += RpcWalletApiIpWhitelist[i];
                        }
                    }
                }
            }



            ClassConsole.ConsoleWriteLine("Do you want to enable the auto update system? (By default this function is enabled and recommended) [Y/N]: ", ClassConsoleColorEnumeration.IndexConsoleBlueLog);
            yourChoose = Console.ReadLine().ToLower() == "y";
            if (yourChoose)
            {
                WalletEnableAutoUpdateWallet = true;
                ClassConsole.ConsoleWriteLine("Write the interval of time in second(s) to update wallets informations. (By default " + WalletUpdateInterval + "): ", ClassConsoleColorEnumeration.IndexConsoleBlueLog);
                int interval = WalletUpdateInterval;
                while (!int.TryParse(Console.ReadLine(), out interval))
                {
                    ClassConsole.ConsoleWriteLine("Write a valid interval of time in second(s) to update wallets informations. (By default " + WalletUpdateInterval + "): ", ClassConsoleColorEnumeration.IndexConsoleRedLog);
                }
                WalletUpdateInterval = interval;

                ClassConsole.ConsoleWriteLine("Write the max keep alive update wallet of time in second(s). (By default " + WalletMaxKeepAliveUpdate + "): ", ClassConsoleColorEnumeration.IndexConsoleBlueLog);
                interval = WalletMaxKeepAliveUpdate;
                while (!int.TryParse(Console.ReadLine(), out interval))
                {
                    ClassConsole.ConsoleWriteLine("Write a max keep alive update wallet of time in second(s). (By default " + WalletMaxKeepAliveUpdate + "): ", ClassConsoleColorEnumeration.IndexConsoleRedLog);
                }
                WalletMaxKeepAliveUpdate = interval;
            }
            else
            {
                WalletEnableAutoUpdateWallet = false;
            }
            SaveRpcWalletSetting();
        }

        /// <summary>
        /// Save setting of RPC Wallet
        /// </summary>
        private static void SaveRpcWalletSetting()
        {
            var jsonRpcWalletSettingObject = new ClassRpcSettingObject
            {
                wallet_enable_backup_system = WalletEnableBackupSystem,
                wallet_interval_backup_system = WalletIntervalBackupSystem,
                wallet_enable_auto_remove_backup_system = WalletEnableAutoRemoveBackupSystem,
                wallet_backup_lapsing_time_limit = WalletBackupLapsingTimeLimit,
                rpc_wallet_api_port = RpcWalletApiPort,
                rpc_wallet_api_ip_whitelist = RpcWalletApiIpWhitelist,
                rpc_wallet_key_request_encryption = RpcWalletApiKeyRequestEncryption,
                rpc_wallet_enable_remote_node_sync = RpcWalletEnableRemoteNodeSync,
                rpc_wallet_remote_node_host = RpcWalletRemoteNodeHost,
                rpc_wallet_remote_node_port = RpcWalletRemoteNodePort,
                wallet_enable_auto_update = WalletEnableAutoUpdateWallet,
                wallet_update_interval = WalletUpdateInterval,
                wallet_max_keep_alive_update = WalletMaxKeepAliveUpdate,
                rpc_wallet_api_enable_x_forwarded_for_resolver = RpcWalletApiEnableXForwardedForResolver,
                wallet_enable_auto_clean_log = WalletEnableAutoCleanLog,
                wallet_auto_clean_log_interval = WalletAutoCleanLogInterval,
                rpc_wallet_enable_api_task_scheduler = RpcWalletEnableApiTaskScheduler
            };
            if (IPAddress.TryParse(RpcWalletApiIpBind, out var ipAddress))
            {
                jsonRpcWalletSettingObject.rpc_wallet_api_ip_bind = RpcWalletApiIpBind;
            }
            else
            {
                Console.WriteLine("Error on configuration, Exception: " + RpcWalletApiIpBind + " can't be parsed. It seems this is not a valid IP.");
                jsonRpcWalletSettingObject.rpc_wallet_api_ip_bind = string.Empty;
            }
            if (RpcWalletApiIpWhitelist == null)
            {
                RpcWalletApiIpWhitelist = new List<string>();
            }

            using (var writer = new StreamWriter(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + RpcWalletSettingFile)){ AutoFlush = true })
            {
                writer.Write(JsonConvert.SerializeObject(jsonRpcWalletSettingObject, Formatting.Indented));
            }
        }
    }
}
