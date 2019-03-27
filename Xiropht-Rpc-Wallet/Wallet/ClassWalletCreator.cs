using System;
using System.Threading;
using System.Threading.Tasks;
using Xiropht_Connector_All.Seed;
using Xiropht_Connector_All.Setting;
using Xiropht_Connector_All.Utils;
using Xiropht_Connector_All.Wallet;
using Xiropht_Rpc_Wallet.Database;

namespace Xiropht_Rpc_Wallet.Wallet
{
    public class ClassWalletCreatorEnumeration
    {
        public const string WalletCreatorPending = "pending";
        public const string WalletCreatorError = "error";
        public const string WalletCreatorSuccess = "success";
    }
    public class ClassWalletCreator : IDisposable
    {

        #region Disposing Part Implementation 

        private bool _disposed;

        ~ClassWalletCreator()
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

        /// <summary>
        /// Objects
        /// </summary>
        public string WalletPhase;
        private string CertificateConnection;
        private string WalletPassword;
        public bool WalletInPendingCreate;
        public string WalletCreateResult;
        public string WalletAddressResult;

        /// <summary>
        /// Class objects
        /// </summary>
        public ClassSeedNodeConnector SeedNodeConnector; // Used for connect the wallet to seed nodes.
        public ClassWalletConnect WalletConnector; // Linked to the seed node connector object.

        /// <summary>
        /// Threading
        /// </summary>
        private Thread ThreadListenBlockchainNetwork;

        public ClassWalletCreator()
        {
            WalletCreateResult = ClassWalletCreatorEnumeration.WalletCreatorPending;
        }

        public async Task<bool> StartWalletConnectionAsync(string walletPhase, string walletPassword)
        {
           
            WalletInPendingCreate = true;
            WalletPassword = walletPassword;
            WalletPhase = walletPhase;
            if (!await InitlizationWalletConnectionAsync(walletPhase, WalletPassword))
            {
                FullDisconnection();
                WalletCreateResult = ClassWalletCreatorEnumeration.WalletCreatorError;
                return false;
            }
            else
            {
                CertificateConnection = ClassUtils.GenerateCertificate();
                if (!await SendPacketBlockchainNetworkAsync(CertificateConnection, string.Empty, false))
                {
                    FullDisconnection();
                    WalletCreateResult = ClassWalletCreatorEnumeration.WalletCreatorError;
                    return false;
                }
                else
                {
                    ListenBlockchainNetworkWallet();


                    switch (WalletPhase)
                    {
                        case ClassWalletPhase.Create:
                            if (!await SendPacketBlockchainNetworkWalletAsync(ClassWalletCommand.ClassWalletSendEnumeration.CreatePhase + "|" + WalletPassword))
                            {
                                FullDisconnection();
                                WalletCreateResult = ClassWalletCreatorEnumeration.WalletCreatorError;
                                return false;
                            }
                            break;
                    }

                }
            }
            return true;
        }

        /// <summary>
        /// Initialization of the wallet connection.
        /// </summary>
        /// <param name="walletPhase"></param>
        /// <param name="walletAddress"></param>
        /// <param name="walletPublicKey"></param>
        /// <param name="walletPassword"></param>
        /// <returns></returns>
        public async Task<bool> InitlizationWalletConnectionAsync(string walletPhase, string walletPassword)
        {
            if (SeedNodeConnector == null)
            {
                SeedNodeConnector = new ClassSeedNodeConnector();
            }

            if (!await SeedNodeConnector.StartConnectToSeedAsync(string.Empty, ClassConnectorSetting.SeedNodePort))
            {
                WalletCreateResult = ClassWalletCreatorEnumeration.WalletCreatorError;
                return false;
            }

            WalletConnector = new ClassWalletConnect(SeedNodeConnector)
            {
                WalletPassword = walletPassword,
                WalletPhase = walletPhase
            };
            return true;
        }

        /// <summary>
        /// Full disconnection of the wallet.
        /// </summary>
        public void FullDisconnection(bool manualDisconnection = true)
        {

            if (ThreadListenBlockchainNetwork != null && (ThreadListenBlockchainNetwork.IsAlive || ThreadListenBlockchainNetwork != null))
            {
                ThreadListenBlockchainNetwork.Abort();
                GC.SuppressFinalize(ThreadListenBlockchainNetwork);
            }

            SeedNodeConnector?.DisconnectToSeed();
            SeedNodeConnector?.Dispose();

            WalletInPendingCreate = false;
        }

        /// <summary>
        /// Send packet to the blockchain network wallet.
        /// </summary>
        /// <param name="packet"></param>
        public async Task<bool> SendPacketBlockchainNetworkWalletAsync(string packet)
        {
            if (!await WalletConnector.SendPacketWallet(packet, CertificateConnection, true))
            {
                WalletCreateResult = ClassWalletCreatorEnumeration.WalletCreatorError;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Send packet to the blockchain network.
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public async Task<bool> SendPacketBlockchainNetworkAsync(string packet, string certificate, bool isEncrypted)
        {

            if (!await WalletConnector.SendPacketWallet(packet, certificate, isEncrypted))
            {
                WalletCreateResult = ClassWalletCreatorEnumeration.WalletCreatorError;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Listen the blockchain network.
        /// </summary>
        public void ListenBlockchainNetworkWallet()
        {
            if (ThreadListenBlockchainNetwork != null && (ThreadListenBlockchainNetwork.IsAlive || ThreadListenBlockchainNetwork != null))
            {
                ThreadListenBlockchainNetwork.Abort();
                GC.SuppressFinalize(ThreadListenBlockchainNetwork);
            }
            ThreadListenBlockchainNetwork = new Thread(async delegate ()
            {
                while (SeedNodeConnector.ReturnStatus())
                {
                    string packetWallet = await WalletConnector.ListenPacketWalletAsync(CertificateConnection, true);

                    if (packetWallet.Contains("*")) // Character separator.
                    {
                        var splitPacket = packetWallet.Split(new[] { "*" }, StringSplitOptions.None);
                        foreach (var packetEach in splitPacket)
                        {
                            if (packetEach != null)
                            {
                                if (!string.IsNullOrEmpty(packetEach))
                                {
                                    if (packetEach.Length > 1)
                                    {
                                        if (packetEach == ClassAlgoErrorEnumeration.AlgoError)
                                        {
                                            WalletCreateResult = ClassWalletCreatorEnumeration.WalletCreatorError;
                                            break;
                                        }

                                        await Task.Run(() => HandlePacketBlockchainNetworkWallet(packetEach.Replace("*", ""))).ConfigureAwait(false);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (packetWallet == ClassAlgoErrorEnumeration.AlgoError)
                        {
                            WalletCreateResult = ClassWalletCreatorEnumeration.WalletCreatorError;
                            break;
                        }

                        await Task.Run(() => HandlePacketBlockchainNetworkWallet(packetWallet)).ConfigureAwait(false);

                    }
                }
            });
            ThreadListenBlockchainNetwork.Start();
        }

        /// <summary>
        /// Handle packet wallet received from the blockchain network.
        /// </summary>
        /// <param name="packet"></param>
        private void HandlePacketBlockchainNetworkWallet(string packet)
        {
            var splitPacket = packet.Split(new[] { "|" }, StringSplitOptions.None);

            switch (splitPacket[0])
            {
                case ClassWalletCommand.ClassWalletReceiveEnumeration.WaitingCreatePhase:
                    //ClassConsole.ConsoleWriteLine("Please wait a moment, your wallet pending creation..", ClassConsoleEnumeration.IndexConsoleYellowLog);
                    break;
                case ClassWalletCommand.ClassWalletReceiveEnumeration.WalletCreatePasswordNeedLetters:
                case ClassWalletCommand.ClassWalletReceiveEnumeration.WalletCreatePasswordNeedMoreCharacters:
                    WalletCreateResult = ClassWalletCreatorEnumeration.WalletCreatorError;
                    FullDisconnection();
                    break;
                case ClassWalletCommand.ClassWalletReceiveEnumeration.CreatePhase:
                    string walletDataCreate = ClassUtils.DecompressData(splitPacket[1]);
                    if (splitPacket[1] == ClassAlgoErrorEnumeration.AlgoError)
                    {
                        GC.SuppressFinalize(walletDataCreate);
                        WalletCreateResult = ClassWalletCreatorEnumeration.WalletCreatorError;
                        FullDisconnection();
                    }
                    else
                    {
                        var decryptWalletDataCreate = ClassAlgo.GetDecryptedResult(ClassAlgoEnumeration.Rijndael, walletDataCreate, WalletPassword, ClassWalletNetworkSetting.KeySize);
                        if (decryptWalletDataCreate == ClassAlgoErrorEnumeration.AlgoError)
                        {
                            WalletCreateResult = ClassWalletCreatorEnumeration.WalletCreatorError;
                            FullDisconnection();
                        }
                        else
                        {
                            var splitDecryptWalletDataCreate = decryptWalletDataCreate.Split(new[] { "\n" }, StringSplitOptions.None);
                            var pinWallet = splitPacket[2];
                            var walletAddress = splitDecryptWalletDataCreate[0];
                            var publicKey = splitDecryptWalletDataCreate[2];
                            var privateKey = splitDecryptWalletDataCreate[3];
                            WalletAddressResult = walletAddress;
                            ClassRpcDatabase.InsertNewWalletAsync(walletAddress, publicKey, privateKey, pinWallet, WalletPassword);
                            WalletCreateResult = ClassWalletCreatorEnumeration.WalletCreatorSuccess;
                            FullDisconnection();
                        }
                    }
                    break;
            }
        }
    }
}
