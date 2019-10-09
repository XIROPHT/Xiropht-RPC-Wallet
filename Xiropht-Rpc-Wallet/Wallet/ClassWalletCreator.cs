using System;
using System.Threading;
using System.Threading.Tasks;
using Xiropht_Connector_All.Seed;
using Xiropht_Connector_All.Setting;
using Xiropht_Connector_All.Utils;
using Xiropht_Connector_All.Wallet;
using Xiropht_Rpc_Wallet.ConsoleObject;
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
        private string _certificateConnection;
        private string _walletPassword;
        private string _walletPrivateKey;
        private string _walletAddress;
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
        private Thread _threadListenBlockchainNetwork;

        public ClassWalletCreator()
        {
            WalletCreateResult = ClassWalletCreatorEnumeration.WalletCreatorPending;
        }

        /// <summary>
        /// Start to connect on the blockchain wallet network.
        /// </summary>
        /// <param name="walletPhase"></param>
        /// <param name="walletPassword"></param>
        /// <param name="privatekey"></param>
        /// <param name="walletAddress"></param>
        /// <returns></returns>
        public async Task<bool> StartWalletConnectionAsync(string walletPhase, string walletPassword, string privatekey = null, string walletAddress = null)
        {

            WalletInPendingCreate = true;
            _walletPassword = walletPassword;
            WalletPhase = walletPhase;
            _walletPrivateKey = privatekey;
            _walletAddress = walletAddress;
            if (!await InitlizationWalletConnectionAsync(walletPhase, _walletPassword))
            {
                FullDisconnection();
                WalletCreateResult = ClassWalletCreatorEnumeration.WalletCreatorError;
                return false;
            }
            else
            {
                _certificateConnection = ClassUtils.GenerateCertificate();
                if (!await SendPacketBlockchainNetworkAsync(_certificateConnection, string.Empty, false))
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
                            if (!await SendPacketBlockchainNetworkWalletAsync(ClassWalletCommand.ClassWalletSendEnumeration.CreatePhase + "|" + _walletPassword))
                            {
                                FullDisconnection();
                                WalletCreateResult = ClassWalletCreatorEnumeration.WalletCreatorError;
                                return false;
                            }
                            break;
                        case ClassWalletPhase.Restore:
                            using (ClassWalletRestoreFunctions walletRestoreFunctionsObject = new ClassWalletRestoreFunctions())
                            {
                                string encryptedQrCodeRestoreRequest = walletRestoreFunctionsObject.GenerateQrCodeKeyEncryptedRepresentation(privatekey, walletPassword);

                                if (encryptedQrCodeRestoreRequest != null)
                                {
                                    await Task.Delay(1000);
                                    if (!await SendPacketBlockchainNetworkSeedNodeMode(ClassWalletCommand.ClassWalletSendEnumeration.AskPhase + "|" + encryptedQrCodeRestoreRequest))
                                    {
                                        FullDisconnection();
                                        WalletCreateResult = ClassWalletCreatorEnumeration.WalletCreatorError;
                                        return false;
                                    }
                                }
                                else
                                {
                                    FullDisconnection();
                                    WalletCreateResult = ClassWalletCreatorEnumeration.WalletCreatorError;
                                    return false;
                                }
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

            if (_threadListenBlockchainNetwork != null && (_threadListenBlockchainNetwork.IsAlive || _threadListenBlockchainNetwork != null))
            {
                _threadListenBlockchainNetwork.Abort();
                GC.SuppressFinalize(_threadListenBlockchainNetwork);
            }

            SeedNodeConnector?.DisconnectToSeed();
            SeedNodeConnector?.Dispose();

            WalletInPendingCreate = false;
            _walletAddress = string.Empty;
            _walletPassword = string.Empty;
            _walletPrivateKey = string.Empty;
        }

        /// <summary>
        /// Send packet to the blockchain network wallet.
        /// </summary>
        /// <param name="packet"></param>
        public async Task<bool> SendPacketBlockchainNetworkWalletAsync(string packet)
        {
            if (!await WalletConnector.SendPacketWallet(packet, _certificateConnection, true))
            {
                WalletCreateResult = ClassWalletCreatorEnumeration.WalletCreatorError;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Send packet to the blockchain network, respecting encryption of seed nodes.
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public async Task<bool> SendPacketBlockchainNetworkSeedNodeMode(string packet)
        {
            if (!await SeedNodeConnector.SendPacketToSeedNodeAsync(packet, _certificateConnection, false, true))
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
        /// <param name="certificate"></param>
        /// <param name="isEncrypted"></param>
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
            if (_threadListenBlockchainNetwork != null && (_threadListenBlockchainNetwork.IsAlive || _threadListenBlockchainNetwork != null))
            {
                _threadListenBlockchainNetwork.Abort();
                GC.SuppressFinalize(_threadListenBlockchainNetwork);
            }
            _threadListenBlockchainNetwork = new Thread(async delegate ()
            {
                while (SeedNodeConnector.ReturnStatus())
                {
                    string packetWallet = await WalletConnector.ListenPacketWalletAsync(_certificateConnection, true);
                    if (packetWallet.Contains("*")) // Character separator.
                    {
                        var splitPacket = packetWallet.Split(new[] { "*" }, StringSplitOptions.None);
                        foreach (var packetEach in splitPacket)
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

                                    await Task.Factory.StartNew(() => HandlePacketBlockchainNetworkWallet(packetEach.Replace("*", "")), CancellationToken.None, TaskCreationOptions.RunContinuationsAsynchronously, TaskScheduler.Current).ConfigureAwait(false);
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

                        await Task.Factory.StartNew(() => HandlePacketBlockchainNetworkWallet(packetWallet), CancellationToken.None, TaskCreationOptions.RunContinuationsAsynchronously, TaskScheduler.Current).ConfigureAwait(false);

                    }
                }
            });
            _threadListenBlockchainNetwork.Start();
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
                    if (splitPacket[1] == ClassAlgoErrorEnumeration.AlgoError)
                    {
                        WalletCreateResult = ClassWalletCreatorEnumeration.WalletCreatorError;
                        FullDisconnection();
                    }
                    else
                    {
                        var decryptWalletDataCreate = ClassAlgo.GetDecryptedResultManual(ClassAlgoEnumeration.Rijndael, splitPacket[1], _walletPassword, ClassWalletNetworkSetting.KeySize);
                        if (decryptWalletDataCreate == ClassAlgoErrorEnumeration.AlgoError)
                        {
                            WalletCreateResult = ClassWalletCreatorEnumeration.WalletCreatorError;
                            FullDisconnection();
                        }
                        else
                        {
                            string walletDataCreate = ClassUtils.DecompressData(decryptWalletDataCreate);
                            var splitDecryptWalletDataCreate = walletDataCreate.Split(new[] { "\n" }, StringSplitOptions.None);
                            var pinWallet = splitPacket[2];
                            var walletAddress = splitDecryptWalletDataCreate[0];
                            var publicKey = splitDecryptWalletDataCreate[2];
                            var privateKey = splitDecryptWalletDataCreate[3];
                            WalletAddressResult = walletAddress;
                            if (ClassRpcDatabase.RpcDatabaseContent.Count < int.MaxValue - 1)
                            {
                                ClassRpcDatabase.InsertNewWalletAsync(walletAddress, publicKey, privateKey, pinWallet, _walletPassword);
                                WalletCreateResult = ClassWalletCreatorEnumeration.WalletCreatorSuccess;
                            }
                            else
                            {
                                WalletCreateResult = ClassWalletCreatorEnumeration.WalletCreatorError;
                                ClassConsole.ConsoleWriteLine("Create wallet error, the maximum wallet of: " +(int.MaxValue-1).ToString("F0")+" has been reach.", ClassConsoleColorEnumeration.IndexConsoleRedLog);

                            }
                            FullDisconnection();
                        }
                    }
                    break;
                case ClassWalletCommand.ClassWalletReceiveEnumeration.WalletAskSuccess:
                    string walletDataCreation = splitPacket[1];

                    if (walletDataCreation == ClassAlgoErrorEnumeration.AlgoError)
                    {
                        ClassConsole.ConsoleWriteLine("Restoring wallet failed, please try again later.", ClassConsoleColorEnumeration.IndexConsoleRedLog);
                        WalletCreateResult = ClassWalletCreatorEnumeration.WalletCreatorError;
                        FullDisconnection();
                    }
                    else
                    {
                        var decryptWalletDataCreation = ClassAlgo.GetDecryptedResultManual(ClassAlgoEnumeration.Rijndael, walletDataCreation, _walletPrivateKey, ClassWalletNetworkSetting.KeySize);
                        if (decryptWalletDataCreation == ClassAlgoErrorEnumeration.AlgoError)
                        {
                            ClassConsole.ConsoleWriteLine("Restoring wallet failed, please try again later.", ClassConsoleColorEnumeration.IndexConsoleRedLog);
                            WalletCreateResult = ClassWalletCreatorEnumeration.WalletCreatorError;
                            FullDisconnection();
                        }
                        else
                        {
                            var splitWalletData = decryptWalletDataCreation.Split(new[] { "\n" }, StringSplitOptions.None);
                            var publicKey = splitWalletData[2];
                            var privateKey = splitWalletData[3];
                            var pinCode = splitWalletData[4];
                            if (ClassRpcDatabase.RpcDatabaseContent.ContainsKey(_walletAddress))
                            {
                                ClassRpcDatabase.RpcDatabaseContent[_walletAddress].SetWalletAddress(_walletAddress);
                                ClassRpcDatabase.RpcDatabaseContent[_walletAddress].SetWalletPublicKey(publicKey);
                                ClassRpcDatabase.RpcDatabaseContent[_walletAddress].SetWalletPrivateKey(privateKey);
                                ClassRpcDatabase.RpcDatabaseContent[_walletAddress].SetWalletPinCode(pinCode);
                                WalletCreateResult = ClassWalletCreatorEnumeration.WalletCreatorSuccess;
                                FullDisconnection();
                            }
                            else
                            {
                                ClassConsole.ConsoleWriteLine("Restoring wallet failed, wallet address: "+_walletAddress+" not exist inside database, please try again later.", ClassConsoleColorEnumeration.IndexConsoleRedLog);
                                WalletCreateResult = ClassWalletCreatorEnumeration.WalletCreatorError;
                                FullDisconnection();
                            }
                        }
                    }
                    break;
            }
        }
    }
}
