using System;
using System.Threading;
using System.Threading.Tasks;
using Xiropht_Connector_All.Wallet;
using Xiropht_Rpc_Wallet.API;
using Xiropht_Rpc_Wallet.Database;
using Xiropht_Rpc_Wallet.Log;
using Xiropht_Rpc_Wallet.Remote;
using Xiropht_Rpc_Wallet.Utility;
using Xiropht_Rpc_Wallet.Wallet;

namespace Xiropht_Rpc_Wallet.ConsoleObject
{
    public class ClassConsoleCommandLineEnumeration
    {
        public const string CommandLineCreateWallet = "createwallet";
        public const string CommandLineLogLevel = "loglevel";
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
            ThreadConsoleCommandLine = new Thread(delegate ()
            {
                while (!Program.Exit)
                {
                    string commandLine = Console.ReadLine();
                    try
                    {
                        var splitCommandLine = commandLine.Split(new char[0], StringSplitOptions.None);
                        switch (splitCommandLine[0])
                        {
                            case ClassConsoleCommandLineEnumeration.CommandLineHelp:
                                ClassConsole.ConsoleWriteLine(ClassConsoleCommandLineEnumeration.CommandLineHelp + " -> show list of command lines.", ClassConsoleColorEnumeration.IndexConsoleMagentaLog, Program.LogLevel);
                                ClassConsole.ConsoleWriteLine(ClassConsoleCommandLineEnumeration.CommandLineCreateWallet + " -> permit to create a new wallet manualy.", ClassConsoleColorEnumeration.IndexConsoleMagentaLog, Program.LogLevel);
                                ClassConsole.ConsoleWriteLine(ClassConsoleCommandLineEnumeration.CommandLineLogLevel + " -> change log level. Max log level: "+ClassConsole.MaxLogLevel, ClassConsoleColorEnumeration.IndexConsoleMagentaLog, Program.LogLevel);

                                break;
                            case ClassConsoleCommandLineEnumeration.CommandLineCreateWallet:
                                new Thread(async delegate ()
                                {
                                    using (var walletCreatorObject = new ClassWalletCreator())
                                    {

                                        if (!await walletCreatorObject.StartWalletConnectionAsync(ClassWalletPhase.Create, ClassUtility.MakeRandomWalletPassword()))
                                        {
                                            ClassConsole.ConsoleWriteLine("RPC Wallet cannot create a new wallet.", ClassConsoleColorEnumeration.IndexConsoleRedLog, Program.LogLevel);
                                        }
                                        await Task.Run(async delegate
                                        {
                                            while (walletCreatorObject.WalletCreateResult == ClassWalletCreatorEnumeration.WalletCreatorPending)
                                            {
                                                await Task.Delay(100);
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
                                        }).ConfigureAwait(false);
                                    }

                                }).Start();
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
                                ClassConsole.ConsoleWriteLine("Closing RPC Wallet..", ClassConsoleColorEnumeration.IndexConsoleRedLog, Program.LogLevel);
                                ClassApi.StopApiHttpServer();
                                ClassWalletUpdater.DisableAutoUpdateWallet();
                                ClassRemoteSync.StopRpcWalletToSync();
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
                                ClassConsole.ConsoleWriteLine("RPC Wallet is successfully stopped, press ENTER to exit.", ClassConsoleColorEnumeration.IndexConsoleBlueLog, Program.LogLevel);
                                ClassLog.StopLogSystem();
                                Console.ReadLine();
                                Program.Exit = true;
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
