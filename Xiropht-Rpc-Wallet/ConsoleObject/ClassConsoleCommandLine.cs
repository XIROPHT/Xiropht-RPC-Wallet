using System;
using System.Threading;
using System.Threading.Tasks;
using Xiropht_Connector_All.Wallet;
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
                                ClassConsole.ConsoleWriteLine(ClassConsoleCommandLineEnumeration.CommandLineHelp + " -> show list of command lines.", ClassConsoleEnumeration.IndexPoolConsoleMagentaLog, Program.LogLevel);
                                ClassConsole.ConsoleWriteLine(ClassConsoleCommandLineEnumeration.CommandLineCreateWallet + " -> permit to create a new wallet manualy.", ClassConsoleEnumeration.IndexPoolConsoleMagentaLog, Program.LogLevel);
                                ClassConsole.ConsoleWriteLine(ClassConsoleCommandLineEnumeration.CommandLineLogLevel + " -> change log level.", ClassConsoleEnumeration.IndexPoolConsoleMagentaLog, Program.LogLevel);

                                break;
                            case ClassConsoleCommandLineEnumeration.CommandLineCreateWallet:
                                new Thread(async delegate ()
                                {
                                    using (var walletCreatorObject = new ClassWalletCreator())
                                    {

                                        if (!await walletCreatorObject.StartWalletConnectionAsync(ClassWalletPhase.Create, ClassUtility.MakeRandomWalletPassword()))
                                        {
                                            ClassConsole.ConsoleWriteLine("RPC Wallet cannot create a new wallet.", ClassConsoleEnumeration.IndexPoolConsoleRedLog, Program.LogLevel);
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
                                                    ClassConsole.ConsoleWriteLine("RPC Wallet cannot create a new wallet.", ClassConsoleEnumeration.IndexPoolConsoleRedLog, Program.LogLevel);
                                                    break;
                                                case ClassWalletCreatorEnumeration.WalletCreatorSuccess:
                                                    ClassConsole.ConsoleWriteLine("RPC Wallet successfully create a new wallet.", ClassConsoleEnumeration.IndexPoolConsoleGreenLog, Program.LogLevel);
                                                    ClassConsole.ConsoleWriteLine("New wallet address generated: " + walletCreatorObject.WalletAddressResult, ClassConsoleEnumeration.IndexPoolConsoleBlueLog, Program.LogLevel);
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
                                        ClassConsole.ConsoleWriteLine("New log level " + Program.LogLevel + " -> " + logLevel, ClassConsoleEnumeration.IndexPoolConsoleMagentaLog, Program.LogLevel);
                                        Program.LogLevel = logLevel;
                                    }
                                }
                                else
                                {
                                    ClassConsole.ConsoleWriteLine("Please select a log level.", ClassConsoleEnumeration.IndexPoolConsoleRedLog, Program.LogLevel);
                                }
                                break;
                        }
                    }
                    catch (Exception error)
                    {
                        ClassConsole.ConsoleWriteLine("Error command line exception: " + error.Message, ClassConsoleEnumeration.IndexPoolConsoleRedLog, Program.LogLevel);
                        ClassConsole.ConsoleWriteLine("For get help use command line " + ClassConsoleCommandLineEnumeration.CommandLineHelp, ClassConsoleEnumeration.IndexPoolConsoleRedLog, Program.LogLevel);
                    }
                }
            });
            ThreadConsoleCommandLine.Start();
        }
    }
}
