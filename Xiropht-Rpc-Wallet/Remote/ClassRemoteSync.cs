using System;
using System.Collections.Generic;
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
using Xiropht_Rpc_Wallet.Threading;

namespace Xiropht_Rpc_Wallet.Remote
{
    public class ClassRemoteSync
    {
        private static TcpClient TcpRemoteNodeClient;
        private static Thread ThreadRemoteNodeListen;
        private static Thread ThreadRemoteNodeCheckConnection;
        private static Thread ThreadAutoSync;
        private static bool ConnectionStatus;
        private static bool EnableCheckConnectionStatus;

        /// <summary>
        /// Current wallet to sync.
        /// </summary>
        private static string CurrentWalletAddressOnSync;

        /// <summary>
        /// Current wallet uniques id to sync.
        /// </summary>
        private static string CurrentWalletIdOnSync;
        private static string CurrentAnonymousWalletIdOnSync;

        /// <summary>
        /// Current total transaction to sync on the wallet.
        /// </summary>
        private static int CurrentWalletTransactionToSync;
        private static int CurrentWalletAnonymousTransactionToSync;

        /// <summary>
        /// Check if the current wait a transaction.
        /// </summary>
        private static bool CurrentWalletOnSyncTransaction;


        /// <summary>
        /// Connect RPC Wallet to a remote node selected.
        /// </summary>
        public static async Task ConnectRpcWalletToSyncAsync()
        {
            while(!ConnectionStatus)
            {
                try
                {
                    TcpRemoteNodeClient?.Close();
                    TcpRemoteNodeClient?.Dispose();
                    TcpRemoteNodeClient = new TcpClient();
                    await TcpRemoteNodeClient.ConnectAsync(ClassRpcSetting.RpcWalletRemoteNodeHost, ClassRpcSetting.RpcWalletRemoteNodePort);
                    ConnectionStatus = true;
                    break;
                }
                catch
                {
                    ClassConsole.ConsoleWriteLine("Unable to connect to Remote Node host " + ClassRpcSetting.RpcWalletRemoteNodeHost + ":" + ClassRpcSetting.RpcWalletRemoteNodePort + " retry in 5 seconds.", ClassConsoleColorEnumeration.IndexConsoleRedLog, ClassConsoleLogLevelEnumeration.LogLevelRemoteNodeSync);
                    ConnectionStatus = false;
                }
                Thread.Sleep(5000);
            }
            if (ConnectionStatus)
            {
                ClassConsole.ConsoleWriteLine("Connect to Remote Node host " + ClassRpcSetting.RpcWalletRemoteNodeHost + ":" + ClassRpcSetting.RpcWalletRemoteNodePort + " successfully done, start to sync.", ClassConsoleColorEnumeration.IndexConsoleGreenLog, ClassConsoleLogLevelEnumeration.LogLevelRemoteNodeSync);

                if (!EnableCheckConnectionStatus)
                {
                    EnableCheckConnectionStatus = true;
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
            if (ThreadRemoteNodeListen != null && (ThreadRemoteNodeListen.IsAlive || ThreadRemoteNodeListen != null))
            {
                ThreadRemoteNodeListen.Abort();
                GC.SuppressFinalize(ThreadRemoteNodeListen);
            }
            if (ThreadRemoteNodeCheckConnection != null && (ThreadRemoteNodeCheckConnection.IsAlive || ThreadRemoteNodeCheckConnection != null))
            {
                ThreadRemoteNodeCheckConnection.Abort();
                GC.SuppressFinalize(ThreadRemoteNodeCheckConnection);
            }
            if (ThreadAutoSync != null && (ThreadAutoSync.IsAlive || ThreadAutoSync != null))
            {
                ThreadAutoSync.Abort();
                GC.SuppressFinalize(ThreadAutoSync);
            }
            ConnectionStatus = false;
            try
            {
                TcpRemoteNodeClient?.Close();
                TcpRemoteNodeClient?.Dispose();
            }
            catch
            {

            }
        }

        /// <summary>
        /// Listen remote node sync packet received.
        /// </summary>
        private static void ListenRemoteNodeSync()
        {
            if (ThreadRemoteNodeListen != null && (ThreadRemoteNodeListen.IsAlive || ThreadRemoteNodeListen != null))
            {
                ThreadRemoteNodeListen.Abort();
                GC.SuppressFinalize(ThreadRemoteNodeListen);
            }
            ThreadRemoteNodeListen = new Thread(async delegate ()
            {
                while(ConnectionStatus)
                {
                    try
                    {
                        using (var networkReader = new NetworkStream(TcpRemoteNodeClient.Client))
                        {
                            byte[] buffer = new byte[ClassConnectorSetting.MaxNetworkPacketSize];
                            int received = await networkReader.ReadAsync(buffer, 0, buffer.Length);
                            if (received > 0)
                            {
                                string packetReceived = Encoding.UTF8.GetString(buffer, 0, received);
                                if (packetReceived.Contains("*"))
                                {
                                    var splitPacketReceived = packetReceived.Split(new[] { "*" }, StringSplitOptions.None);
                                    if (splitPacketReceived.Length > 1)
                                    {
                                        foreach(var packetEach in splitPacketReceived)
                                        {
                                            if (packetEach != null)
                                            {
                                                if (!string.IsNullOrEmpty(packetEach))
                                                {
                                                    if (packetEach.Length > 1)
                                                    {
                                                        await Task.Factory.StartNew(() => HandlePacketReceivedFromSync(packetEach), CancellationToken.None, TaskCreationOptions.None, PriorityScheduler.Lowest).ConfigureAwait(false);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        await Task.Factory.StartNew(() => HandlePacketReceivedFromSync(packetReceived.Replace("*", "")), CancellationToken.None, TaskCreationOptions.None, PriorityScheduler.Lowest).ConfigureAwait(false);
                                    }
                                }
                                else
                                {
                                    await Task.Factory.StartNew(() => HandlePacketReceivedFromSync(packetReceived), CancellationToken.None, TaskCreationOptions.None, PriorityScheduler.Lowest).ConfigureAwait(false);
                                }
                            }
                        }
                    }
                    catch(Exception error)
                    {
                        ClassConsole.ConsoleWriteLine("Exception: "+error.Message+" to listen packet received from Remote Node host " + ClassRpcSetting.RpcWalletRemoteNodeHost + ":" + ClassRpcSetting.RpcWalletRemoteNodePort + " retry to connect in a few seconds..", ClassConsoleColorEnumeration.IndexConsoleRedLog, ClassConsoleLogLevelEnumeration.LogLevelRemoteNodeSync);
                        break;
                    }
                }
                ConnectionStatus = false;
            });
            ThreadRemoteNodeListen.Start();
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
                    ClassConsole.ConsoleWriteLine("Their is " + splitPacket[1] + " transaction to sync for wallet address: " + CurrentWalletAddressOnSync, ClassConsoleColorEnumeration.IndexConsoleGreenLog, ClassConsoleLogLevelEnumeration.LogLevelRemoteNodeSync);
                    CurrentWalletTransactionToSync = int.Parse(splitPacket[1]);
                    CurrentWalletOnSyncTransaction = false;
                    break;
                case ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration.WalletYourAnonymityNumberTransaction:
                    ClassConsole.ConsoleWriteLine("Their is " + splitPacket[1] + " anonymous transaction to sync for wallet address: " + CurrentWalletAddressOnSync, ClassConsoleColorEnumeration.IndexConsoleGreenLog, ClassConsoleLogLevelEnumeration.LogLevelRemoteNodeSync);
                    CurrentWalletAnonymousTransactionToSync = int.Parse(splitPacket[1]);
                    CurrentWalletOnSyncTransaction = false;
                    break;
                case ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration.WalletTransactionPerId:
                    ClassSortingTransaction.SaveTransactionSorted(splitPacket[1], CurrentWalletAddressOnSync, ClassRpcDatabase.RpcDatabaseContent[CurrentWalletAddressOnSync].GetWalletPublicKey(), false);
                    ClassConsole.ConsoleWriteLine(CurrentWalletAddressOnSync + " total transaction sync " + ClassRpcDatabase.RpcDatabaseContent[CurrentWalletAddressOnSync].GetWalletTotalTransactionSync() + "/" + CurrentWalletTransactionToSync, ClassConsoleColorEnumeration.IndexConsoleGreenLog, ClassConsoleLogLevelEnumeration.LogLevelRemoteNodeSync);
                    CurrentWalletOnSyncTransaction = false;
                    break;
                case ClassRemoteNodeCommandForWallet.RemoteNodeRecvPacketEnumeration.WalletAnonymityTransactionPerId:
                    ClassSortingTransaction.SaveTransactionSorted(splitPacket[1], CurrentWalletAddressOnSync, ClassRpcDatabase.RpcDatabaseContent[CurrentWalletAddressOnSync].GetWalletPublicKey(), true);
                    ClassConsole.ConsoleWriteLine(CurrentWalletAddressOnSync + " total anonymous transaction sync " + ClassRpcDatabase.RpcDatabaseContent[CurrentWalletAddressOnSync].GetWalletTotalAnonymousTransactionSync() + "/" + CurrentWalletAnonymousTransactionToSync, ClassConsoleColorEnumeration.IndexConsoleGreenLog, ClassConsoleLogLevelEnumeration.LogLevelRemoteNodeSync);
                    CurrentWalletOnSyncTransaction = false;
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
            if (ThreadRemoteNodeCheckConnection != null && (ThreadRemoteNodeCheckConnection.IsAlive || ThreadRemoteNodeCheckConnection!=null))
            {
                ThreadRemoteNodeCheckConnection.Abort();
                GC.SuppressFinalize(ThreadRemoteNodeCheckConnection);
            }
            ThreadRemoteNodeCheckConnection = new Thread(async delegate ()
            {
                while(true)
                {
                    if (!ConnectionStatus)
                    {
                        if (ThreadRemoteNodeListen != null && (ThreadRemoteNodeListen.IsAlive || ThreadRemoteNodeListen != null))
                        {
                            ThreadRemoteNodeListen.Abort();
                            GC.SuppressFinalize(ThreadRemoteNodeListen);
                        }
                        if (ThreadAutoSync != null && (ThreadAutoSync.IsAlive || ThreadAutoSync != null))
                        {
                            ThreadAutoSync.Abort();
                            GC.SuppressFinalize(ThreadAutoSync);
                        }
                        Thread.Sleep(1000);
                        ClassConsole.ConsoleWriteLine("Connection to remote node host is closed, retry to connect", ClassConsoleColorEnumeration.IndexConsoleRedLog, ClassConsoleLogLevelEnumeration.LogLevelRemoteNodeSync);
                        await ConnectRpcWalletToSyncAsync();
                    }
                    Thread.Sleep(1000);
                }
            });
            ThreadRemoteNodeCheckConnection.Start();
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
                using (var networkWriter = new NetworkStream(TcpRemoteNodeClient.Client))
                {
                    var bytePacket = Encoding.UTF8.GetBytes(packet+"*");
                    await networkWriter.WriteAsync(bytePacket, 0, bytePacket.Length);
                    await networkWriter.FlushAsync();
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
            if (ThreadAutoSync != null && (ThreadAutoSync.IsAlive || ThreadAutoSync != null))
            {
                ThreadAutoSync.Abort();
                GC.SuppressFinalize(ThreadAutoSync);
            }
            ThreadAutoSync = new Thread(async delegate ()
            {
                while (ConnectionStatus)
                {
                    try
                    {
                        foreach (var walletObject in ClassRpcDatabase.RpcDatabaseContent)
                        {


                            if (walletObject.Value.GetWalletUniqueId() != "-1" && walletObject.Value.GetWalletAnonymousUniqueId() != "-1")
                            {
                                #region Attempt to sync the current wallet on the database.


                                CurrentWalletIdOnSync = walletObject.Value.GetWalletUniqueId();
                                CurrentAnonymousWalletIdOnSync = walletObject.Value.GetWalletAnonymousUniqueId();
                                CurrentWalletAddressOnSync = walletObject.Key;
                                CurrentWalletOnSyncTransaction = true;
                                if (await SendPacketToRemoteNode(ClassRemoteNodeCommandForWallet.RemoteNodeSendPacketEnumeration.WalletAskHisNumberTransaction + "|" + walletObject.Value.GetWalletUniqueId()))
                                {
                                    while (CurrentWalletOnSyncTransaction)
                                    {
                                        if (!ConnectionStatus)
                                        {
                                            break;
                                        }
                                        Thread.Sleep(50);
                                    }
                                    
                                    if (CurrentWalletTransactionToSync > 0)
                                    {
                                        if (CurrentWalletTransactionToSync > walletObject.Value.GetWalletTotalTransactionSync()) // Start to sync transaction.
                                        {
                                            for (int i = walletObject.Value.GetWalletTotalTransactionSync(); i < CurrentWalletTransactionToSync; i++)
                                            {
                                                CurrentWalletOnSyncTransaction = true;
                                                if (!await SendPacketToRemoteNode(ClassRemoteNodeCommandForWallet.RemoteNodeSendPacketEnumeration.WalletAskTransactionPerId + "|" + walletObject.Value.GetWalletUniqueId() + "|" + i))
                                                {
                                                    ConnectionStatus = false;
                                                    break;
                                                }
                                                while (CurrentWalletOnSyncTransaction)
                                                {
                                                    if (!ConnectionStatus)
                                                    {
                                                        break;
                                                    }
                                                    Thread.Sleep(50);
                                                }
                                                
                                            }
                                        }
                                    }
                                    CurrentWalletOnSyncTransaction = true;
                                    if (await SendPacketToRemoteNode(ClassRemoteNodeCommandForWallet.RemoteNodeSendPacketEnumeration.WalletAskHisAnonymityNumberTransaction + "|" + walletObject.Value.GetWalletAnonymousUniqueId()))
                                    {
                                        while (CurrentWalletOnSyncTransaction)
                                        {
                                            if (!ConnectionStatus)
                                            {
                                                break;
                                            }
                                            Thread.Sleep(50);
                                        }
                                       
                                        if (CurrentWalletAnonymousTransactionToSync > 0)
                                        {
                                            if (CurrentWalletAnonymousTransactionToSync > walletObject.Value.GetWalletTotalAnonymousTransactionSync()) // Start to sync transaction.
                                            {
                                                for (int i = walletObject.Value.GetWalletTotalAnonymousTransactionSync(); i < CurrentWalletAnonymousTransactionToSync; i++)
                                                {
                                                    CurrentWalletOnSyncTransaction = true;
                                                    if (!await SendPacketToRemoteNode(ClassRemoteNodeCommandForWallet.RemoteNodeSendPacketEnumeration.WalletAskAnonymityTransactionPerId + "|" + walletObject.Value.GetWalletAnonymousUniqueId() + "|" + i))
                                                    {
                                                        ConnectionStatus = false;
                                                        break;
                                                    }
                                                    while (CurrentWalletOnSyncTransaction)
                                                    {
                                                        if (!ConnectionStatus)
                                                        {
                                                            break;
                                                        }
                                                        Thread.Sleep(50);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ConnectionStatus = false;
                                        break;
                                    }
                                }
                                else
                                {
                                    ConnectionStatus = false;
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
                    Thread.Sleep(1000);
                }
                ConnectionStatus = false;
            });
            ThreadAutoSync.Start();
        }
    }
}
