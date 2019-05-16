using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xiropht_Connector_All.Wallet;
using Xiropht_Rpc_Wallet.API;
using Xiropht_Rpc_Wallet.Database;
using Xiropht_Rpc_Wallet.Log;
using Xiropht_Rpc_Wallet.Remote;
using Xiropht_Rpc_Wallet.Setting;
using Xiropht_Rpc_Wallet.Utility;
using Xiropht_Rpc_Wallet.Wallet;

namespace Xiropht_Rpc_Wallet.ConsoleObject
{
    public class ClassConsoleCommandLineEnumeration
    {
        public const string CommandLineCreateWallet = "createwallet";
        public const string CommandLineRestoreWallet = "restorewallet";
        public const string CommandLineLogLevel = "loglevel";
        public const string CommandLineSaveWallet = "savewallet";
        public const string CommandLineHelp = "help";
        public const string CommandLineExit = "exit";
    }

    public class ClassConsoleCommandLine
    {
        private static Thread ThreadConsoleCommandLine;

        /// <summary>
        /// Enable console command line.
        /// </summary>
        public static void EnableConsoleCommandLine()
        {
            ThreadConsoleCommandLine = new Thread(async delegate ()
            {
                while (!Program.Exit)
                {
                    string commandLine = Console.ReadLine();
                    if (Program.Exit)
                    {
                        break;
                    }
                    try
                    {
                        var splitCommandLine = commandLine.Split(new char[0], StringSplitOptions.None);
                        switch (splitCommandLine[0])
                        {
                            case ClassConsoleCommandLineEnumeration.CommandLineHelp:
                                ClassConsole.ConsoleWriteLine(ClassConsoleCommandLineEnumeration.CommandLineHelp + " -> show list of command lines.", ClassConsoleColorEnumeration.IndexConsoleMagentaLog, Program.LogLevel);
                                ClassConsole.ConsoleWriteLine(ClassConsoleCommandLineEnumeration.CommandLineCreateWallet + " -> permit to create a new wallet manualy.", ClassConsoleColorEnumeration.IndexConsoleMagentaLog, Program.LogLevel);
                                ClassConsole.ConsoleWriteLine(ClassConsoleCommandLineEnumeration.CommandLineRestoreWallet + " -> permit to restore a wallet manualy. Syntax: " + ClassConsoleCommandLineEnumeration.CommandLineRestoreWallet + " wallet_address", ClassConsoleColorEnumeration.IndexConsoleMagentaLog, Program.LogLevel);
                                ClassConsole.ConsoleWriteLine(ClassConsoleCommandLineEnumeration.CommandLineSaveWallet + " -> permit to save manually the database of wallets.", ClassConsoleColorEnumeration.IndexConsoleMagentaLog, Program.LogLevel);
                                ClassConsole.ConsoleWriteLine(ClassConsoleCommandLineEnumeration.CommandLineLogLevel + " -> change log level. Max log level: " + ClassConsole.MaxLogLevel, ClassConsoleColorEnumeration.IndexConsoleMagentaLog, Program.LogLevel);
                                break;
                            case ClassConsoleCommandLineEnumeration.CommandLineCreateWallet:

                                using (var walletCreatorObject = new ClassWalletCreator())
                                {

                                    new Thread(async delegate ()
                                    {
                                        if (!await walletCreatorObject.StartWalletConnectionAsync(ClassWalletPhase.Create, ClassUtility.MakeRandomWalletPassword()))
                                        {
                                            ClassConsole.ConsoleWriteLine("RPC Wallet cannot create a new wallet.", ClassConsoleColorEnumeration.IndexConsoleRedLog, Program.LogLevel);
                                        }
                                    }).Start();


                                    while (walletCreatorObject.WalletCreateResult == ClassWalletCreatorEnumeration.WalletCreatorPending)
                                    {
                                        Thread.Sleep(100);
                                    }
                                    switch (walletCreatorObject.WalletCreateResult)
                                    {
                                        case ClassWalletCreatorEnumeration.WalletCreatorError:
                                            ClassConsole.ConsoleWriteLine("RPC Wallet cannot create a new wallet.", ClassConsoleColorEnumeration.IndexConsoleRedLog, Program.LogLevel);
                                            break;
                                        case ClassWalletCreatorEnumeration.WalletCreatorSuccess:
                                            ClassConsole.ConsoleWriteLine("RPC Wallet successfully create a new wallet.", ClassConsoleColorEnumeration.IndexConsoleGreenLog, Program.LogLevel);
                                            ClassConsole.ConsoleWriteLine("New wallet address generated: " + walletCreatorObject.WalletAddressResult, ClassConsoleColorEnumeration.IndexConsoleBlueLog, Program.LogLevel);
                                            break;
                                    }
                                }

                                break;
                            case ClassConsoleCommandLineEnumeration.CommandLineRestoreWallet:

                                if (splitCommandLine.Length < 2)
                                {
                                    ClassConsole.ConsoleWriteLine("Please, put the wallet address to restore.", ClassConsoleColorEnumeration.IndexConsoleRedLog, Program.LogLevel);
                                }
                                else
                                {
                                    if (ClassRpcDatabase.RpcDatabaseContent.ContainsKey(splitCommandLine[1]))
                                    {
                                        using (var walletCreatorObject = new ClassWalletCreator())
                                        {

                                            new Thread(async delegate ()
                                            {
                                                if (!await walletCreatorObject.StartWalletConnectionAsync(ClassWalletPhase.Restore, ClassUtility.MakeRandomWalletPassword(), ClassRpcDatabase.RpcDatabaseContent[splitCommandLine[1]].GetWalletPrivateKey(), splitCommandLine[1]))
                                                {
                                                    ClassConsole.ConsoleWriteLine("RPC Wallet cannot restore your wallet: " + splitCommandLine[1], ClassConsoleColorEnumeration.IndexConsoleRedLog, Program.LogLevel);
                                                }
                                            }).Start();


                                            while (walletCreatorObject.WalletCreateResult == ClassWalletCreatorEnumeration.WalletCreatorPending)
                                            {
                                                Thread.Sleep(100);
                                            }
                                            switch (walletCreatorObject.WalletCreateResult)
                                            {
                                                case ClassWalletCreatorEnumeration.WalletCreatorError:
                                                    ClassConsole.ConsoleWriteLine("RPC Wallet cannot restore a wallet: " + splitCommandLine[1], ClassConsoleColorEnumeration.IndexConsoleRedLog, Program.LogLevel);
                                                    break;
                                                case ClassWalletCreatorEnumeration.WalletCreatorSuccess:
                                                    ClassConsole.ConsoleWriteLine("RPC Wallet successfully restore wallet: " + splitCommandLine[1], ClassConsoleColorEnumeration.IndexConsoleGreenLog, Program.LogLevel);
                                                    ClassConsole.ConsoleWriteLine("RPC Wallet execute save the database of wallets..", ClassConsoleColorEnumeration.IndexConsoleYellowLog, Program.LogLevel);
                                                    if (await ClassRpcDatabase.SaveWholeRpcWalletDatabaseFile())
                                                    {
                                                        ClassConsole.ConsoleWriteLine("RPC Wallet save of the database of wallets done successfully.", ClassConsoleColorEnumeration.IndexConsoleGreenLog, Program.LogLevel);
                                                    }
                                                    else
                                                    {
                                                        ClassConsole.ConsoleWriteLine("RPC Wallet save of the database of wallets failed, please retry by command line: " + ClassConsoleCommandLineEnumeration.CommandLineSaveWallet, ClassConsoleColorEnumeration.IndexConsoleGreenLog, Program.LogLevel);
                                                    }
                                                    break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ClassConsole.ConsoleWriteLine("Please, put a valid wallet address stored on the database of your rpc wallet to restore. " + splitCommandLine[1] + " not exist.", ClassConsoleColorEnumeration.IndexConsoleRedLog, Program.LogLevel);
                                    }
                                }
                                break;
                            case ClassConsoleCommandLineEnumeration.CommandLineSaveWallet:
                                if (await ClassRpcDatabase.SaveWholeRpcWalletDatabaseFile())
                                {
                                    ClassConsole.ConsoleWriteLine("RPC Wallet save of the database of wallets done successfully.", ClassConsoleColorEnumeration.IndexConsoleGreenLog, Program.LogLevel);
                                }
                                else
                                {
                                    ClassConsole.ConsoleWriteLine("RPC Wallet save of the database of wallets failed, please retry by command line: " + ClassConsoleCommandLineEnumeration.CommandLineSaveWallet, ClassConsoleColorEnumeration.IndexConsoleGreenLog, Program.LogLevel);
                                }
                                break;
                            case ClassConsoleCommandLineEnumeration.CommandLineLogLevel:
                                if (splitCommandLine.Length > 1)
                                {
                                    if (int.TryParse(splitCommandLine[1], out var logLevel))
                                    {
                                        if (logLevel < 0)
                                        {
                                            logLevel = 0;
                                        }
                                        else
                                        {
                                            if (logLevel > ClassConsole.MaxLogLevel)
                                            {
                                                logLevel = ClassConsole.MaxLogLevel;
                                            }
                                        }
                                        ClassConsole.ConsoleWriteLine("New log level " + Program.LogLevel + " -> " + logLevel, ClassConsoleColorEnumeration.IndexConsoleMagentaLog, Program.LogLevel);
                                        Program.LogLevel = logLevel;
                                    }
                                }
                                else
                                {
                                    ClassConsole.ConsoleWriteLine("Please select a log level.", ClassConsoleColorEnumeration.IndexConsoleRedLog, Program.LogLevel);
                                }
                                break;
                            case ClassConsoleCommandLineEnumeration.CommandLineExit:
                                Program.Exit = true;
                                ClassConsole.ConsoleWriteLine("Closing RPC Wallet..", ClassConsoleColorEnumeration.IndexConsoleRedLog, Program.LogLevel);
                                ClassApi.StopApiHttpServer();
                                if (ClassRpcSetting.RpcWalletEnableRemoteNodeSync)
                                {
                                    ClassWalletUpdater.DisableAutoUpdateWallet();
                                }
                                ClassRemoteSync.StopRpcWalletToSync();
                                if (Program.ThreadRemoteNodeSync != null && (Program.ThreadRemoteNodeSync.IsAlive || Program.ThreadRemoteNodeSync != null))
                                {
                                    Program.ThreadRemoteNodeSync.Abort();
                                    GC.SuppressFinalize(Program.ThreadRemoteNodeSync);
                                }
                                ClassConsole.ConsoleWriteLine("Waiting end of save RPC Wallet Database..", ClassConsoleColorEnumeration.IndexConsoleYellowLog, Program.LogLevel);
                                while (ClassRpcDatabase.InSave)
                                {
                                    Thread.Sleep(100);
                                }
                                ClassConsole.ConsoleWriteLine("Waiting end of save RPC Wallet Sync Database..", ClassConsoleColorEnumeration.IndexConsoleYellowLog, Program.LogLevel);
                                while (ClassSyncDatabase.InSave)
                                {
                                    Thread.Sleep(100);
                                }
                                ClassLog.StopLogSystem();
                                ClassConsole.ConsoleWriteLine("RPC Wallet is successfully stopped, press ENTER to exit.", ClassConsoleColorEnumeration.IndexConsoleBlueLog, Program.LogLevel);
                                Console.ReadLine();
                                Process.GetCurrentProcess().Kill();
                                break;
                        }
                        if (Program.Exit)
                        {
                            break;
                        }
                    }
                    catch (Exception error)
                    {
                        ClassConsole.ConsoleWriteLine("Error command line exception: " + error.Message, ClassConsoleColorEnumeration.IndexConsoleRedLog, Program.LogLevel);
                        ClassConsole.ConsoleWriteLine("For get help use command line " + ClassConsoleCommandLineEnumeration.CommandLineHelp, ClassConsoleColorEnumeration.IndexConsoleRedLog, Program.LogLevel);
                    }
                }
            });
            ThreadConsoleCommandLine.Start();
        }
    }
}
