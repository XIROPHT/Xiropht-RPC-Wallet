using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xiropht_Connector_All.Utils;
using Xiropht_Connector_All.Wallet;
using Xiropht_Rpc_Wallet.ConsoleObject;
using Xiropht_Rpc_Wallet.Database;
using Xiropht_Rpc_Wallet.Setting;
using Xiropht_Rpc_Wallet.Threading;
using Xiropht_Rpc_Wallet.Utility;
using Xiropht_Rpc_Wallet.Wallet;

namespace Xiropht_Rpc_Wallet.API
{
    public class ClassApiEnumeration
    {
        public const string GetTotalWalletIndex = "get_total_wallet_index"; // Number of total wallet created.
        public const string GetWalletAddressByIndex = "get_wallet_address_by_index"; // Get a wallet address by an index selected.
        public const string GetWalletBalanceByIndex = "get_wallet_balance_by_index"; // Get a wallet balance and pending balance by an index selected.
        public const string GetWalletBalanceByWalletAddress = "get_wallet_balance_by_wallet_address"; // Get a wallet balance and pending balance by an wallet address selected.
        public const string GetWalletTotalTransactionByIndex = "get_wallet_total_transaction_by_index"; // Get the total transaction sync from an index selected.
        public const string GetWalletTotalAnonymousTransactionByIndex = "get_total_anonymous_transaction_by_index"; // Get the total anonymous transaction sync from an index selected.
        public const string GetWalletTotalTransactionByWalletAddress = "get_wallet_total_transaction_by_wallet_address"; // Get the total transaction sync from an wallet address selected.
        public const string GetWalletTotalAnonymousTransactionByWalletAddress = "get_total_anonymous_transaction_by_wallet_address"; // Get the total anonymous transaction sync from an wallet address selected.
        public const string GetWalletTransaction = "get_wallet_transaction"; // Get a selected transaction by an index selected and a wallet address selected.
        public const string GetWalletAnonymousTransaction = "get_wallet_anonymous_transaction"; // Get a selected anonymous transaction by an index selected and a wallet address selected.
        public const string SendTransactionByWalletAddress = "send_transaction_by_wallet_address"; // Sent a transaction from a selected wallet address.
        public const string CreateWallet = "create_wallet"; // Create a new wallet, return wallet address.
        public const string CreateWalletError = "create_wallet_error"; // Return an error pending to create a wallet.
        public const string PacketNotExist = "not_exist";
        public const string WalletNotExist = "wallet_not_exist";
        public const string IndexNotExist = "index_not_exist";
    }

    public class ClassApi
    {

        private static bool ListenApiHttpConnectionStatus;
        private static Thread ThreadListenApiHttpConnection;
        private static TcpListener ListenerApiHttpConnection;

        /// <summary>
        /// Enable http/https api of the remote node, listen incoming connection throught web client.
        /// </summary>
        public static void StartApiHttpServer()
        {
            ListenApiHttpConnectionStatus = true;
            if (ClassRpcSetting.RpcWalletApiPort <= 0) // Not selected or invalid
            {
                ListenerApiHttpConnection = new TcpListener(IPAddress.Any, 8000);
            }
            else
            {
                ListenerApiHttpConnection = new TcpListener(IPAddress.Any, ClassRpcSetting.RpcWalletApiPort);
            }
            ListenerApiHttpConnection.Start();
            ThreadListenApiHttpConnection = new Thread(async delegate ()
            {
                while (ListenApiHttpConnectionStatus && !Program.Exit)
                {
                    try
                    {
                        var client = await ListenerApiHttpConnection.AcceptTcpClientAsync();
                        var ip = ((IPEndPoint)(client.Client.RemoteEndPoint)).Address.ToString();
                        await Task.Factory.StartNew(async () =>
                        {
                            using (var clientApiHttpObject = new ClassClientApiHttpObject(client, ip))
                            {
                                await clientApiHttpObject.StartHandleClientHttpAsync();
                            }
                        }, CancellationToken.None, TaskCreationOptions.None, PriorityScheduler.Lowest).ConfigureAwait(false);
                    }
                    catch
                    {

                    }
                }
            });
            ThreadListenApiHttpConnection.Start();
        }

        /// <summary>
        /// Stop Api HTTP Server
        /// </summary>
        public static void StopApiHttpServer()
        {
            ListenApiHttpConnectionStatus = false;
            if (ThreadListenApiHttpConnection != null && (ThreadListenApiHttpConnection.IsAlive || ThreadListenApiHttpConnection != null))
            {
                ThreadListenApiHttpConnection.Abort();
                GC.SuppressFinalize(ThreadListenApiHttpConnection);
            }
            try
            {
                ListenerApiHttpConnection.Stop();
            }
            catch
            {

            }
        }
    }

    public class ClassClientApiHttpObject : IDisposable
    {
        #region Disposing Part Implementation 

        private bool _disposed;

        ~ClassClientApiHttpObject()
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

        private bool _clientStatus;
        private TcpClient _client;
        private string _ip;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="client"></param>
        /// <param name="ip"></param>
        public ClassClientApiHttpObject(TcpClient client, string ip)
        {
            _clientStatus = true;
            _client = client;
            _ip = ip;
        }

        /// <summary>
        /// Start to listen incoming client.
        /// </summary>
        /// <returns></returns>
        public async Task StartHandleClientHttpAsync()
        {
            var isWhitelisted = true;

            if (ClassRpcSetting.RpcWalletApiIpWhitelist.Count > 0)
            {
                if (!ClassRpcSetting.RpcWalletApiIpWhitelist.Contains(_ip))
                {
                    ClassConsole.ConsoleWriteLine(_ip + " is not whitelisted.", ClassConsoleColorEnumeration.IndexConsoleRedLog, ClassConsoleLogLevelEnumeration.LogLevelApi);
                    isWhitelisted = false;
                }
            }

            int totalWhile = 0;
            if (isWhitelisted)
            {
                try
                {
                    while (_clientStatus)
                    {
                        try
                        {
                            byte[] buffer = new byte[8192];
                            using (NetworkStream clientHttpReader = new NetworkStream(_client.Client))
                            {

                                int received = await clientHttpReader.ReadAsync(buffer, 0, buffer.Length);
                                if (received > 0)
                                {
                                    string packet = Encoding.UTF8.GetString(buffer, 0, received);
                                    try
                                    {
                                        if (!GetAndCheckForwardedIp(packet))
                                        {
                                            break;
                                        }
                                    }
                                    catch
                                    {

                                    }

                                    packet = ClassUtility.GetStringBetween(packet, "GET /", "HTTP");
                                    packet = packet.Replace("%7C", "|"); // Translate special character | 
                                    packet = packet.Replace(" ", ""); // Remove empty,space characters
                                    ClassConsole.ConsoleWriteLine("HTTP API - packet received from IP: " + _ip + " - " + packet, ClassConsoleColorEnumeration.IndexConsoleYellowLog, ClassConsoleLogLevelEnumeration.LogLevelApi);
                                    if (ClassRpcSetting.RpcWalletApiKeyRequestEncryption != string.Empty)
                                    {
                                        packet = ClassAlgo.GetDecryptedResult(ClassAlgoEnumeration.Rijndael, packet, ClassRpcSetting.RpcWalletApiKeyRequestEncryption, ClassWalletNetworkSetting.KeySize);
                                        if (packet == ClassAlgoErrorEnumeration.AlgoError)
                                        {
                                            ClassConsole.ConsoleWriteLine("HTTP API - wrong packet received from IP: " + _ip + " - " + packet, ClassConsoleColorEnumeration.IndexConsoleRedLog, ClassConsoleLogLevelEnumeration.LogLevelApi);
                                            break;
                                        }
                                        ClassConsole.ConsoleWriteLine("HTTP API - decrypted packet received from IP: " + _ip + " - " + packet, ClassConsoleColorEnumeration.IndexConsoleYellowLog, ClassConsoleLogLevelEnumeration.LogLevelApi);
                                    }

                                    await HandlePacketHttpAsync(packet);
                                    break;
                                }
                                else
                                {
                                    totalWhile++;
                                }
                                if (totalWhile >= 8)
                                {
                                    break;
                                }
                            }
                        }
                        catch (Exception error)
                        {
                            ClassConsole.ConsoleWriteLine("HTTP API - exception error: " + error.Message, ClassConsoleColorEnumeration.IndexConsoleYellowLog, ClassConsoleLogLevelEnumeration.LogLevelApi);
                            break;
                        }
                    }
                }
                catch
                {
                }
            }
            CloseClientConnection();
        }

        /// <summary>
        /// This method permit to get back the real ip behind a proxy and check the list of banned IP.
        /// </summary>
        private bool GetAndCheckForwardedIp(string packet)
        {
            var splitPacket = packet.Split(new[] { "\n" }, StringSplitOptions.None);
            foreach (var packetEach in splitPacket)
            {
                if (packetEach != null)
                {
                    if (!string.IsNullOrEmpty(packetEach))
                    {
                        if (packetEach.ToLower().Contains("x-forwarded-for: "))
                        {
                            _ip = packetEach.ToLower().Replace("x-forwarded-for: ", "");
                            ClassConsole.ConsoleWriteLine("HTTP/HTTPS API - X-Forwarded-For ip of the client is: " + _ip, ClassConsoleColorEnumeration.IndexConsoleYellowLog, ClassConsoleLogLevelEnumeration.LogLevelApi);
                            if (ClassRpcSetting.RpcWalletApiIpWhitelist.Count > 0)
                            {
                                if (!ClassRpcSetting.RpcWalletApiIpWhitelist.Contains(_ip))
                                {
                                    return false;
                                }
                            }
                        }

                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Close connection incoming from the client.
        /// </summary>
        private void CloseClientConnection()
        {
            _client?.Close();
            _client?.Dispose();
        }

        /// <summary>
        /// Handle get request received from client.
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        private async Task HandlePacketHttpAsync(string packet)
        {
            if (packet.Contains("|"))
            {
                var splitPacket = packet.Split(new[] { "|" }, StringSplitOptions.None);

                switch (splitPacket[0])
                {
                    case ClassApiEnumeration.GetWalletAddressByIndex:
                        if (int.TryParse(splitPacket[1], out var walletIndex))
                        {
                            int counter = 0;
                            foreach (var walletObject in ClassRpcDatabase.RpcDatabaseContent)
                            {
                                counter++;
                                if (counter == walletIndex)
                                {
                                    await BuildAndSendHttpPacketAsync(walletObject.Key);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            await BuildAndSendHttpPacketAsync(ClassApiEnumeration.WalletNotExist);
                        }
                        break;
                    case ClassApiEnumeration.GetWalletBalanceByIndex:
                        if (int.TryParse(splitPacket[1], out var walletIndex2))
                        {
                            int counter = 0;
                            foreach (var walletObject in ClassRpcDatabase.RpcDatabaseContent)
                            {
                                counter++;
                                if (counter == walletIndex2)
                                {
                                    Dictionary<string, string> walletBalanceContent = new Dictionary<string, string>()
                                        {
                                            {"wallet_address", walletObject.Key },
                                            {"wallet_balance", walletObject.Value.GetWalletBalance() },
                                            {"wallet_pending_balance", walletObject.Value.GetWalletPendingBalance() }
                                        };
                                    await BuildAndSendHttpPacketAsync(null, true, walletBalanceContent);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            await BuildAndSendHttpPacketAsync(ClassApiEnumeration.WalletNotExist);
                        }
                        break;
                    case ClassApiEnumeration.GetWalletBalanceByWalletAddress:
                        if (ClassRpcDatabase.RpcDatabaseContent.ContainsKey(splitPacket[1]))
                        {
                            Dictionary<string, string> walletBalanceContent = new Dictionary<string, string>()
                                {
                                      {"wallet_address", splitPacket[1] },
                                      {"wallet_balance", ClassRpcDatabase.RpcDatabaseContent[splitPacket[1]].GetWalletBalance() },
                                      {"wallet_pending_balance", ClassRpcDatabase.RpcDatabaseContent[splitPacket[1]].GetWalletPendingBalance() }
                                };
                            await BuildAndSendHttpPacketAsync(null, true, walletBalanceContent);
                        }
                        else
                        {
                            await BuildAndSendHttpPacketAsync(ClassApiEnumeration.WalletNotExist);
                        }
                        break;
                    case ClassApiEnumeration.SendTransactionByWalletAddress:
                        if (splitPacket.Length >= 6)
                        {
                            var walletAddressSource = splitPacket[1];
                            var amount = splitPacket[2];
                            var fee = splitPacket[3];
                            var anonymousOption = splitPacket[4];
                            var walletAddressTarget = splitPacket[5];
                            if (anonymousOption == "1")
                            {
                                string result = await ClassWalletUpdater.ProceedTransactionTokenRequestAsync(walletAddressSource, amount, fee, walletAddressTarget, true);
                                await BuildAndSendHttpPacketAsync(result);
                            }
                            else
                            {
                                string result = await ClassWalletUpdater.ProceedTransactionTokenRequestAsync(walletAddressSource, amount, fee, walletAddressTarget, false);
                                await BuildAndSendHttpPacketAsync(result);
                            }
                        }
                        else
                        {
                            await BuildAndSendHttpPacketAsync(ClassApiEnumeration.PacketNotExist);
                        }
                        break;
                    case ClassApiEnumeration.CreateWallet:
                        if (long.TryParse(splitPacket[1], out var timeout))
                        {
                            var dateTimeEnd = DateTimeOffset.Now.ToUnixTimeSeconds() + timeout;

                            bool success = false;
                            while (dateTimeEnd > DateTimeOffset.Now.ToUnixTimeSeconds())
                            {
                                using (var walletCreatorObject = new ClassWalletCreator())
                                {
                                    await Task.Run(async delegate
                                    {
                                        if (!await walletCreatorObject.StartWalletConnectionAsync(ClassWalletPhase.Create, ClassUtility.MakeRandomWalletPassword()))
                                        {
                                            ClassConsole.ConsoleWriteLine("RPC Wallet cannot create a new wallet.", ClassConsoleColorEnumeration.IndexConsoleRedLog, Program.LogLevel);
                                        }
                                    }).ConfigureAwait(false);

                                    while (walletCreatorObject.WalletCreateResult == ClassWalletCreatorEnumeration.WalletCreatorPending)
                                    {
                                        await Task.Delay(100);
                                    }
                                    switch (walletCreatorObject.WalletCreateResult)
                                    {
                                        case ClassWalletCreatorEnumeration.WalletCreatorSuccess:
                                            success = true;
                                            await BuildAndSendHttpPacketAsync(walletCreatorObject.WalletAddressResult);
                                            dateTimeEnd = DateTimeOffset.Now.ToUnixTimeSeconds();
                                            break;
                                    }
                                }
                            }
                            if (!success)
                            {
                                await BuildAndSendHttpPacketAsync(ClassApiEnumeration.CreateWalletError);
                            }
                        }
                        else
                        {
                            await BuildAndSendHttpPacketAsync(ClassApiEnumeration.CreateWalletError);
                        }
                        break;
                    case ClassApiEnumeration.GetWalletTotalTransactionByIndex:
                        if (int.TryParse(splitPacket[1], out var walletIndex3))
                        {
                            int counter = 0;
                            foreach (var walletObject in ClassRpcDatabase.RpcDatabaseContent)
                            {
                                counter++;
                                if (counter == walletIndex3)
                                {
                                    Dictionary<string, string> walletTotalTransactionContent = new Dictionary<string, string>()
                                        {
                                            {"wallet_address", walletObject.Key },
                                            {"wallet_total_transaction", ""+ walletObject.Value.GetWalletTotalTransactionSync() }
                                        };
                                    await BuildAndSendHttpPacketAsync(null, true, walletTotalTransactionContent);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            await BuildAndSendHttpPacketAsync(ClassApiEnumeration.WalletNotExist);
                        }
                        break;
                    case ClassApiEnumeration.GetWalletTotalAnonymousTransactionByIndex:
                        if (int.TryParse(splitPacket[1], out var walletIndex4))
                        {
                            int counter = 0;
                            foreach (var walletObject in ClassRpcDatabase.RpcDatabaseContent)
                            {
                                counter++;
                                if (counter == walletIndex4)
                                {
                                    Dictionary<string, string> walletTotalAnonymousTransactionContent = new Dictionary<string, string>()
                                        {
                                            {"wallet_address", walletObject.Key },
                                            {"wallet_total_anonymous_transaction", ""+ walletObject.Value.GetWalletTotalAnonymousTransactionSync() }
                                        };
                                    await BuildAndSendHttpPacketAsync(null, true, walletTotalAnonymousTransactionContent);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            await BuildAndSendHttpPacketAsync(ClassApiEnumeration.WalletNotExist);
                        }
                        break;
                    case ClassApiEnumeration.GetWalletTotalTransactionByWalletAddress:
                        if (ClassRpcDatabase.RpcDatabaseContent.ContainsKey(splitPacket[1]))
                        {
                            Dictionary<string, string> walletTotalTransactionContent = new Dictionary<string, string>()
                            {
                                            {"wallet_address", splitPacket[1] },
                                            {"wallet_total_transaction", ""+ ClassRpcDatabase.RpcDatabaseContent[splitPacket[1]].GetWalletTotalTransactionSync() }
                            };
                            await BuildAndSendHttpPacketAsync(null, true, walletTotalTransactionContent);
                        }
                        else
                        {
                            await BuildAndSendHttpPacketAsync(ClassApiEnumeration.WalletNotExist);
                        }
                        break;
                    case ClassApiEnumeration.GetWalletTotalAnonymousTransactionByWalletAddress:
                        if (ClassRpcDatabase.RpcDatabaseContent.ContainsKey(splitPacket[1]))
                        {
                            Dictionary<string, string> walletTotalAnonymousTransactionContent = new Dictionary<string, string>()
                            {
                                            {"wallet_address", splitPacket[1] },
                                            {"wallet_total_anonymous_transaction", ""+ ClassRpcDatabase.RpcDatabaseContent[splitPacket[1]].GetWalletTotalAnonymousTransactionSync() }
                            };
                            await BuildAndSendHttpPacketAsync(null, true, walletTotalAnonymousTransactionContent);
                        }
                        else
                        {
                            await BuildAndSendHttpPacketAsync(ClassApiEnumeration.WalletNotExist);
                        }
                        break;
                    case ClassApiEnumeration.GetWalletTransaction:
                        if (ClassRpcDatabase.RpcDatabaseContent.ContainsKey(splitPacket[1]))
                        {
                            if (int.TryParse(splitPacket[2], out var transactionIndex))
                            {
                                if (transactionIndex == 0)
                                {
                                    transactionIndex++;
                                }
                                string transaction = ClassRpcDatabase.RpcDatabaseContent[splitPacket[1]].GetWalletTransactionSyncByIndex(transactionIndex);
                                if (transaction != null)
                                {
                                    var splitTransaction = transaction.Split(new[] { "#" }, StringSplitOptions.None);
                                    var type = splitTransaction[1];
                                    var hash = splitTransaction[2];
                                    var walletDst = splitTransaction[3];
                                    var amount = splitTransaction[4];
                                    var fee = splitTransaction[5];
                                    var timestampSend = splitTransaction[6];
                                    var timestampRecv = splitTransaction[7];
                                    var blockchainHeight = splitTransaction[8];

                                    Dictionary<string, string> walletTransactionContent = new Dictionary<string, string>()
                                    {
                                        {"index", "" + transactionIndex },
                                        {"type", splitTransaction[0] },
                                        {"mode", type },
                                        {"hash", hash },
                                        {"wallet_dst_or_src", walletDst },
                                        {"amount", amount },
                                        {"fee", fee },
                                        {"timestamp_send", timestampSend },
                                        {"timestamp_recv", timestampRecv },
                                        {"blockchain_height", blockchainHeight }

                                    };
                                    await BuildAndSendHttpPacketAsync(null, true, walletTransactionContent);
                                }
                                else
                                {
                                    await BuildAndSendHttpPacketAsync(ClassApiEnumeration.IndexNotExist);
                                }
                            }
                            else
                            {
                                await BuildAndSendHttpPacketAsync(ClassApiEnumeration.IndexNotExist);
                            }
                        }
                        else
                        {
                            await BuildAndSendHttpPacketAsync(ClassApiEnumeration.WalletNotExist);
                        }
                        break;
                    case ClassApiEnumeration.GetWalletAnonymousTransaction:
                        if (ClassRpcDatabase.RpcDatabaseContent.ContainsKey(splitPacket[1]))
                        {
                            if (int.TryParse(splitPacket[2], out var transactionIndex))
                            {
                                if (transactionIndex == 0)
                                {
                                    transactionIndex++;
                                }
                                string transaction = ClassRpcDatabase.RpcDatabaseContent[splitPacket[1]].GetWalletAnonymousTransactionSyncByIndex(transactionIndex);
                                if (transaction != null)
                                {
                                    var splitTransaction = transaction.Split(new[] { "#" }, StringSplitOptions.None);
                                    var type = splitTransaction[1];
                                    var hash = splitTransaction[2];
                                    var walletDst = splitTransaction[3];
                                    var amount = splitTransaction[4];
                                    var fee = splitTransaction[5];
                                    var timestampSend = splitTransaction[6];
                                    var timestampRecv = splitTransaction[7];
                                    var blockchainHeight = splitTransaction[8];

                                    Dictionary<string, string> walletAnonymousTransactionContent = new Dictionary<string, string>()
                                    {
                                        {"index", "" + transactionIndex },
                                        {"type", splitTransaction[0]  },
                                        {"mode", type },
                                        {"hash", hash },
                                        {"wallet_dst_or_src", walletDst },
                                        {"amount", amount },
                                        {"fee", fee },
                                        {"timestamp_send", timestampSend },
                                        {"timestamp_recv", timestampRecv },
                                        {"blockchain_height", blockchainHeight }

                                    };
                                    await BuildAndSendHttpPacketAsync(null, true, walletAnonymousTransactionContent);
                                }
                                else
                                {
                                    await BuildAndSendHttpPacketAsync(ClassApiEnumeration.IndexNotExist);
                                }
                            }
                            else
                            {
                                await BuildAndSendHttpPacketAsync(ClassApiEnumeration.IndexNotExist);
                            }
                        }
                        else
                        {
                            await BuildAndSendHttpPacketAsync(ClassApiEnumeration.WalletNotExist);
                        }
                        break;
                    default:
                        await BuildAndSendHttpPacketAsync(ClassApiEnumeration.PacketNotExist);
                        break;
                }
            }
            else
            {
                switch (packet)
                {
                    case ClassApiEnumeration.GetTotalWalletIndex:
                        await BuildAndSendHttpPacketAsync("" + ClassRpcDatabase.RpcDatabaseContent.Count);
                        break;
                    case ClassApiEnumeration.CreateWallet:
                        using (var walletCreatorObject = new ClassWalletCreator())
                        {
                            await Task.Factory.StartNew(async delegate
                            {
                                if (!await walletCreatorObject.StartWalletConnectionAsync(ClassWalletPhase.Create, ClassUtility.MakeRandomWalletPassword()))
                                {
                                    ClassConsole.ConsoleWriteLine("RPC Wallet cannot create a new wallet.", ClassConsoleColorEnumeration.IndexConsoleRedLog, Program.LogLevel);
                                }
                            }, CancellationToken.None, TaskCreationOptions.None, PriorityScheduler.Lowest).ConfigureAwait(false);

                            while (walletCreatorObject.WalletCreateResult == ClassWalletCreatorEnumeration.WalletCreatorPending)
                            {
                                await Task.Delay(100);
                            }
                            switch (walletCreatorObject.WalletCreateResult)
                            {
                                case ClassWalletCreatorEnumeration.WalletCreatorError:
                                    await BuildAndSendHttpPacketAsync(ClassApiEnumeration.CreateWalletError);
                                    break;
                                case ClassWalletCreatorEnumeration.WalletCreatorSuccess:
                                    await BuildAndSendHttpPacketAsync(walletCreatorObject.WalletAddressResult);
                                    break;
                            }
                        }
                        break;
                    default:
                        await BuildAndSendHttpPacketAsync(ClassApiEnumeration.PacketNotExist);
                        break;
                }
            }
        }

        /// <summary>
        /// build and send http packet to client.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private async Task BuildAndSendHttpPacketAsync(string content, bool multiResult = false, Dictionary<string, string> dictionaryContent = null)
        {
            string contentToSend = string.Empty;
            if (!multiResult)
            {
                contentToSend = BuildJsonString(content);
            }
            else
            {
                contentToSend = BuildFullJsonString(dictionaryContent);
            }
            if (ClassRpcSetting.RpcWalletApiKeyRequestEncryption != string.Empty)
            {
                contentToSend = ClassAlgo.GetEncryptedResult(ClassAlgoEnumeration.Rijndael, contentToSend, ClassRpcSetting.RpcWalletApiKeyRequestEncryption, ClassWalletNetworkSetting.KeySize);
            }
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(@"HTTP/1.1 200 OK");
            builder.AppendLine(@"Content-Type: text/plain");
            builder.AppendLine(@"Content-Length: " + contentToSend.Length);
            builder.AppendLine(@"Access-Control-Allow-Origin: *");
            builder.AppendLine(@"");
            builder.AppendLine(@"" + contentToSend);
            await SendPacketAsync(builder.ToString());
            builder.Clear();
            contentToSend = string.Empty;
        }

        /// <summary>
        /// Return content converted for json.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private string BuildJsonString(string content)
        {
            JObject jsonContent = new JObject
            {
                { "result", content },
                { "version", Assembly.GetExecutingAssembly().GetName().Version.ToString() },
                { "date_packet", DateTimeOffset.Now.ToUnixTimeSeconds() }
            };
            return JsonConvert.SerializeObject(jsonContent);
        }

        /// <summary>
        /// Return content converted for json.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private string BuildFullJsonString(Dictionary<string, string> dictionaryContent)
        {
            JObject jsonContent = new JObject();
            foreach (var content in dictionaryContent)
            {
                jsonContent.Add(content.Key, content.Value);
            }
            jsonContent.Add("version", Assembly.GetExecutingAssembly().GetName().Version.ToString());
            jsonContent.Add("date_packet", DateTimeOffset.Now.ToUnixTimeSeconds());
            return JsonConvert.SerializeObject(jsonContent);
        }

        /// <summary>
        /// Send packet to client.
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        private async Task SendPacketAsync(string packet)
        {
            try
            {
                var bytePacket = Encoding.UTF8.GetBytes(packet);

                using (var networkStream = new NetworkStream(_client.Client))
                {
                    await networkStream.WriteAsync(bytePacket, 0, bytePacket.Length).ConfigureAwait(false);
                    await networkStream.FlushAsync().ConfigureAwait(false);
                }
            }
            catch
            {
            }
        }
    }
}
