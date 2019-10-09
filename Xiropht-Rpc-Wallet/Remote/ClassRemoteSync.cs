using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xiropht_Connector_All.Remote;
using Xiropht_Connector_All.Setting;
using Xiropht_Rpc_Wallet.ConsoleObject;
using Xiropht_Rpc_Wallet.Database;
using Xiropht_Rpc_Wallet.Setting;

namespace Xiropht_Rpc_Wallet.Remote
{
    public class ClassRemoteSync
    {
        private static TcpClient _tcpRemoteNodeClient;
        private static CancellationTokenSource _cancellationTokenListenRemoteNode;
        private static CancellationTokenSource _cancellationTokenCheckConnection;
        private static CancellationTokenSource _cancellationTokenAutoSync;
        private static bool _connectionStatus;
        private static bool _enableCheckConnectionStatus;
        private const int MaxTimeout = 30;

        /// <summary>
        /// Current wallet to sync.
        /// </summary>
        private static string _currentWalletAddressOnSync;

        /// <summary>
        /// Current wallet uniques id to sync.
        /// </summary>
        private static string _currentWalletIdOnSync;
        private static string _currentAnonymousWalletIdOnSync;

        /// <summary>
        /// Current total transaction to sync on the wallet.
        /// </summary>
        private static int _currentWalletTransactionToSync;
        private static int _currentWalletAnonymousTransactionToSync;

        /// <summary>
        /// Check if the current wait a transaction.
        /// </summary>
        private static bool _currentWalletOnSyncTransaction;

        /// <summary>
        /// Save last packet received date.
        /// </summary>
        private static long _lastPacketReceived;


        /// <summary>
        /// Connect RPC Wallet to a remote node selected.
        /// </summary>
        public static async Task ConnectRpcWalletToRemoteNodeSyncAsync()
        {
            while (!_connectionStatus && !Program.Exit)
            {
                try
                {
                    _tcpRemoteNodeClient?.Close();
                    _tcpRemoteNodeClient?.Dispose();
                    _tcpRemoteNodeClient = new TcpClient();
                    await _tcpRemoteNodeClient.ConnectAsync(ClassRpcSetting.RpcWalletRemoteNodeHost, ClassRpcSetting.RpcWalletRemoteNodePort);
                    _connectionStatus = true;
                    break;
                }
                catch
                {
                    ClassConsole.ConsoleWriteLine("Unable to connect to Remote Node host " + ClassRpcSetting.RpcWalletRemoteNodeHost + ":" + ClassRpcSetting.RpcWalletRemoteNodePort + " retry in 5 seconds.", ClassConsoleColorEnumeration.IndexConsoleRedLog, ClassConsoleLogLevelEnumeration.LogLevelRemoteNodeSync);
                    _connectionStatus = false;
                }
                Thread.Sleep(5000);
            }
            if (_connectionStatus)
            {
                ClassConsole.ConsoleWriteLine("Connect to Remote Node host " + ClassRpcSetting.RpcWalletRemoteNodeHost + ":" + ClassRpcSetting.RpcWalletRemoteNodePort + " successfully done, start to sync.", ClassConsoleColorEnumeration.IndexConsoleGreenLog, ClassConsoleLogLevelEnumeration.LogLevelRemoteNodeSync);
                _lastPacketReceived = DateTimeOffset.Now.ToUnixTimeSeconds();

                if (!_enableCheckConnectionStatus)
                {
                    _enableCheckConnectionStatus = true;
                    CheckRpcWalletConnectionToSync();
                }
                ListenRemoteNodeSync();
                AutoSyncWallet();
            }
        }

        /// <summary>
        /// Disconnect RPC Wallet to the remote node selected.
        /// </summary>
        public static void StopRpcWalletToSync()
        {
            CancelTaskListenRemoteNode();
            CancelTaskCheckConnection();
            CancelAutoSync();

            _connectionStatus = false;
            try
            {
                _tcpRemoteNodeClient?.Close();
                _tcpRemoteNodeClient?.Dispose();
            }
            catch
            {
                // Ignored
            }
        }

        /// <summary>
        /// Listen remote node sync packet received.
        /// </summary>
        private static void ListenRemoteNodeSync()
        {
            _cancellationTokenListenRemoteNode = new CancellationTokenSource();

            try
            {
                Task.Factory.StartNew(async delegate
                {
                    while (_connectionStatus && !Program.Exit)
                    {
                        try
                        {
                            using (var networkReader = new NetworkStream(_tcpRemoteNodeClient.Client))
                            {
                                using (BufferedStream bufferedStreamNetwork = new BufferedStream(networkReader, ClassConnectorSetting.MaxNetworkPacketSize))
                                {
                                    byte[] buffer = new byte[ClassConnectorSetting.MaxNetworkPacketSize];
                                    int received = await bufferedStreamNetwork.ReadAsync(buffer, 0, buffer.Length);
                                    if (received > 0)
                                    {
                                        _lastPacketReceived = DateTimeOffset.Now.ToUnixTimeSeconds();
                                        string packetReceived = Encoding.UTF8.GetString(buffer, 0, received);
                                        if (packetReceived.Contains("*"))
                                        {
                                            var splitPacketReceived = packetReceived.Split(new[] { "*" }, StringSplitOptions.None);
                                            if (splitPacketReceived.Length > 1)
                                            {
                                                foreach (var packetEach in splitPacketReceived)
                                                {
                                                    if (!string.IsNullOrEmpty(packetEach))
                                                    {
                                                        if (packetEach.Length > 1)
                                                        {
                                                            HandlePacketReceivedFromSync(packetEach);
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                HandlePacketReceivedFromSync(packetReceived.Replace("*", ""));
                                            }
                                        }
                                        else
                                        {
                                            HandlePacketReceivedFromSync(packetReceived);
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception error)
                        {
                            ClassConsole.ConsoleWriteLine("Exception: " + error.Message + " to listen packet received from Remote Node host " + ClassRpcSetting.RpcWalletRemoteNodeHost + ":" + ClassRpcSetting.RpcWalletRemoteNodePort + " retry to connect in a few seconds..", ClassConsoleColorEnumeration.IndexConsoleRedLog, ClassConsoleLogLevelEnumeration.LogLevelRemoteNodeSync);
                            break;
                        }
                    }
                    _connectionStatus = false;
                }, _cancellationTokenListenRemoteNode.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);
            }
            catch
            {
                // Catch the exception once the task is cancelled.
            }
        }

        /// <summary>
        /// Handle packet received from remote node sync.
        /// </summary>
        /// <param name="packet"></param>
        private static void HandlePacketReceivedFromSync(string packet)
        {
            var splitPacket = packet.Split(new[] { "|" }, StringSplitOptions.None);

            switch (splitPacket[0])
            {
                case ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration.WalletYourNumberTransaction:
                    ClassConsole.ConsoleWriteLine("Their is " + splitPacket[1] + " transaction to sync for wallet address: " + _currentWalletAddressOnSync, ClassConsoleColorEnumeration.IndexConsoleGreenLog, ClassConsoleLogLevelEnumeration.LogLevelRemoteNodeSync);
                    _currentWalletTransactionToSync = int.Parse(splitPacket[1]);
                    _currentWalletOnSyncTransaction = false;
                    break;
                case ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration.WalletYourAnonymityNumberTransaction:
                    ClassConsole.ConsoleWriteLine("Their is " + splitPacket[1] + " anonymous transaction to sync for wallet address: " + _currentWalletAddressOnSync, ClassConsoleColorEnumeration.IndexConsoleGreenLog, ClassConsoleLogLevelEnumeration.LogLevelRemoteNodeSync);
                    _currentWalletAnonymousTransactionToSync = int.Parse(splitPacket[1]);
                    _currentWalletOnSyncTransaction = false;
                    break;
                case ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration.WalletTransactionPerId:
                    ClassSortingTransaction.SaveTransactionSorted(splitPacket[1], _currentWalletAddressOnSync, ClassRpcDatabase.RpcDatabaseContent[_currentWalletAddressOnSync].GetWalletPublicKey(), false);
                    ClassConsole.ConsoleWriteLine(_currentWalletAddressOnSync + " total transaction sync " + ClassRpcDatabase.RpcDatabaseContent[_currentWalletAddressOnSync].GetWalletTotalTransactionSync() + "/" + _currentWalletTransactionToSync, ClassConsoleColorEnumeration.IndexConsoleGreenLog, ClassConsoleLogLevelEnumeration.LogLevelRemoteNodeSync);
                    _currentWalletOnSyncTransaction = false;
                    break;
                case ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration.WalletAnonymityTransactionPerId:
                    ClassSortingTransaction.SaveTransactionSorted(splitPacket[1], _currentWalletAddressOnSync, ClassRpcDatabase.RpcDatabaseContent[_currentWalletAddressOnSync].GetWalletPublicKey(), true);
                    ClassConsole.ConsoleWriteLine(_currentWalletAddressOnSync + " total anonymous transaction sync " + ClassRpcDatabase.RpcDatabaseContent[_currentWalletAddressOnSync].GetWalletTotalAnonymousTransactionSync() + "/" + _currentWalletAnonymousTransactionToSync, ClassConsoleColorEnumeration.IndexConsoleGreenLog, ClassConsoleLogLevelEnumeration.LogLevelRemoteNodeSync);
                    _currentWalletOnSyncTransaction = false;
                    break;
                default:
                    ClassConsole.ConsoleWriteLine("Unknown packet received: " + packet, ClassConsoleColorEnumeration.IndexConsoleYellowLog, ClassConsoleLogLevelEnumeration.LogLevelRemoteNodeSync);
                    break;
            }
        }

        /// <summary>
        /// Check rpc wallet connection to remote node sync.
        /// </summary>
        private static void CheckRpcWalletConnectionToSync()
        {
            _cancellationTokenCheckConnection = new CancellationTokenSource();

            try
            {
                Task.Factory.StartNew(async delegate
                {
                    while (!Program.Exit)
                    {
                        try
                        {
                            if (!_connectionStatus || _lastPacketReceived + MaxTimeout < DateTimeOffset.Now.ToUnixTimeSeconds())
                            {
                                _connectionStatus = false;
                                _lastPacketReceived = 0;
                                await Task.Delay(100);
                                CancelTaskListenRemoteNode();
                                CancelAutoSync();
                                await Task.Delay(1000);
                                ClassConsole.ConsoleWriteLine("Connection to remote node host is closed, retry to connect", ClassConsoleColorEnumeration.IndexConsoleRedLog, ClassConsoleLogLevelEnumeration.LogLevelRemoteNodeSync);
                                await ConnectRpcWalletToRemoteNodeSyncAsync();
                            }
                        }
                        catch
                        {
                            _connectionStatus = false;
                            _lastPacketReceived = 0;
                        }
                        await Task.Delay(1000);
                    }
                }, _cancellationTokenCheckConnection.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);
            }
            catch
            {
                // Catch the exception 
            }
        }

        /// <summary>
        /// Cancel the task of listen packets receive from a Remote Node.
        /// </summary>
        private static void CancelTaskListenRemoteNode()
        {
            try
            {
                if (_cancellationTokenListenRemoteNode != null)
                {
                    if (!_cancellationTokenListenRemoteNode.IsCancellationRequested)
                    {
                        _cancellationTokenListenRemoteNode.Cancel();
                    }
                }
            }
            catch
            {
                // Ignored
            }
        }

        /// <summary>
        /// Cancel the task who check the current connection status.
        /// </summary>
        private static void CancelTaskCheckConnection()
        {
            try
            {
                if (_cancellationTokenCheckConnection != null)
                {
                    if (!_cancellationTokenCheckConnection.IsCancellationRequested)
                    {
                        _cancellationTokenCheckConnection.Cancel();
                    }
                }
            }
            catch
            {
                // Ignored
            }
        }

        /// <summary>
        /// Cancel the task who auto sync transactions of wallets stored inside the Rpc Wallet Database.
        /// </summary>
        private static void CancelAutoSync()
        {
            try
            {
                if (_cancellationTokenAutoSync != null)
                {
                    if (!_cancellationTokenAutoSync.IsCancellationRequested)
                    {
                        _cancellationTokenAutoSync.Cancel();
                    }
                }
            }
            catch
            {
                // Ignored
            }
        }


        /// <summary>
        /// Send a packet to remote node.
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        private static async Task<bool> SendPacketToRemoteNode(string packet)
        {
            try
            {
                using (var networkWriter = new NetworkStream(_tcpRemoteNodeClient.Client))
                {
                    using (BufferedStream bufferedStream = new BufferedStream(networkWriter, ClassConnectorSetting.MaxNetworkPacketSize))
                    {
                        var bytePacket = Encoding.UTF8.GetBytes(packet + "*");
                        await bufferedStream.WriteAsync(bytePacket, 0, bytePacket.Length);
                        await bufferedStream.FlushAsync();
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
        /// Auto sync wallets.
        /// </summary>
        private static void AutoSyncWallet()
        {
            _cancellationTokenAutoSync = new CancellationTokenSource();
            try
            {
                Task.Factory.StartNew(async delegate
                {
                    while (_connectionStatus && !Program.Exit)
                    {
                        try
                        {
                            foreach (var walletObject in ClassRpcDatabase.RpcDatabaseContent.ToArray()) // Copy temporaly the database of wallets in the case of changes on the enumeration done by a parallal process, update sync of all of them.
                            {


                                if (ClassRpcDatabase.RpcDatabaseContent[walletObject.Key].GetWalletUniqueId() != "-1" && ClassRpcDatabase.RpcDatabaseContent[walletObject.Key].GetWalletAnonymousUniqueId() != "-1")
                                {
                                    #region Attempt to sync the current wallet on the database.


                                    _currentWalletIdOnSync = walletObject.Value.GetWalletUniqueId();
                                    _currentAnonymousWalletIdOnSync = walletObject.Value.GetWalletAnonymousUniqueId();
                                    _currentWalletAddressOnSync = walletObject.Key;
                                    _currentWalletOnSyncTransaction = true;
                                    if (await SendPacketToRemoteNode(ClassRemoteNodeCommandForWallet.RemoteNodeSendPacketEnumeration.WalletAskHisNumberTransaction + "|" + walletObject.Value.GetWalletUniqueId()))
                                    {
                                        while (_currentWalletOnSyncTransaction)
                                        {
                                            if (!_connectionStatus)
                                            {
                                                break;
                                            }
                                            await Task.Delay(50);
                                        }

                                        if (_currentWalletTransactionToSync > 0)
                                        {
                                            if (_currentWalletTransactionToSync > ClassRpcDatabase.RpcDatabaseContent[walletObject.Key].GetWalletTotalTransactionSync()) // Start to sync transaction.
                                            {
                                                for (int i = ClassRpcDatabase.RpcDatabaseContent[walletObject.Key].GetWalletTotalTransactionSync(); i < _currentWalletTransactionToSync; i++)
                                                {
                                                    _currentWalletOnSyncTransaction = true;
                                                    if (!await SendPacketToRemoteNode(ClassRemoteNodeCommandForWallet.RemoteNodeSendPacketEnumeration.WalletAskTransactionPerId + "|" + walletObject.Value.GetWalletUniqueId() + "|" + i))
                                                    {
                                                        _connectionStatus = false;
                                                        break;
                                                    }
                                                    while (_currentWalletOnSyncTransaction)
                                                    {
                                                        if (!_connectionStatus)
                                                        {
                                                            break;
                                                        }
                                                        await Task.Delay(50);
                                                    }

                                                }
                                            }
                                        }
                                        _currentWalletOnSyncTransaction = true;
                                        if (await SendPacketToRemoteNode(ClassRemoteNodeCommandForWallet.RemoteNodeSendPacketEnumeration.WalletAskHisAnonymityNumberTransaction + "|" + walletObject.Value.GetWalletAnonymousUniqueId()))
                                        {
                                            while (_currentWalletOnSyncTransaction)
                                            {
                                                if (!_connectionStatus)
                                                {
                                                    break;
                                                }
                                                await Task.Delay(50);
                                            }

                                            if (_currentWalletAnonymousTransactionToSync > 0)
                                            {
                                                if (_currentWalletAnonymousTransactionToSync > ClassRpcDatabase.RpcDatabaseContent[walletObject.Key].GetWalletTotalAnonymousTransactionSync()) // Start to sync transaction.
                                                {
                                                    for (int i = ClassRpcDatabase.RpcDatabaseContent[walletObject.Key].GetWalletTotalAnonymousTransactionSync(); i < _currentWalletAnonymousTransactionToSync; i++)
                                                    {
                                                        _currentWalletOnSyncTransaction = true;
                                                        if (!await SendPacketToRemoteNode(ClassRemoteNodeCommandForWallet.RemoteNodeSendPacketEnumeration.WalletAskAnonymityTransactionPerId + "|" + walletObject.Value.GetWalletAnonymousUniqueId() + "|" + i))
                                                        {
                                                            _connectionStatus = false;
                                                            break;
                                                        }
                                                        while (_currentWalletOnSyncTransaction)
                                                        {
                                                            if (!_connectionStatus)
                                                            {
                                                                break;
                                                            }
                                                            await Task.Delay(50);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            _connectionStatus = false;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        _connectionStatus = false;
                                        break;
                                    }

                                    #endregion
                                }
                            }
                        }
                        catch (Exception error)
                        {
                            ClassConsole.ConsoleWriteLine("Exception: " + error.Message + " to send packet on Remote Node host " + ClassRpcSetting.RpcWalletRemoteNodeHost + ":" + ClassRpcSetting.RpcWalletRemoteNodePort + " retry to connect in a few seconds..", ClassConsoleColorEnumeration.IndexConsoleRedLog, ClassConsoleLogLevelEnumeration.LogLevelRemoteNodeSync);
                            break;
                        }
                        await Task.Delay(1000);
                    }
                    _connectionStatus = false;
                }, _cancellationTokenAutoSync.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);
            }
            catch
            {
                // Catch the exception once the task is cancelled.
            }
        }
    }
}
