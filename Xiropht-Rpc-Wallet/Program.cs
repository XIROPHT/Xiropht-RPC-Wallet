using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using Xiropht_Connector_All.Setting;
using Xiropht_Rpc_Wallet.API;
using Xiropht_Rpc_Wallet.ConsoleObject;
using Xiropht_Rpc_Wallet.Database;
using Xiropht_Rpc_Wallet.Log;
using Xiropht_Rpc_Wallet.Remote;
using Xiropht_Rpc_Wallet.Setting;
using Xiropht_Rpc_Wallet.Utility;
using Xiropht_Rpc_Wallet.Wallet;

namespace Xiropht_Rpc_Wallet
{
    class Program
    {
        private const string UnexpectedExceptionFile = "\\error_rpc_wallet.txt";
        public static bool Exit;
        public static CultureInfo GlobalCultureInfo;
        public static int LogLevel;
        public static Thread ThreadRemoteNodeSync;

        static void Main(string[] args)
        {
            EnableCatchUnexpectedException();
            Console.CancelKeyPress += Console_CancelKeyPress;
            Thread.CurrentThread.Name = Path.GetFileName(Environment.GetCommandLineArgs()[0]);
            GlobalCultureInfo = new CultureInfo("fr-FR");
            ClassLog.LogInitialization();
            ServicePointManager.DefaultConnectionLimit = 65535;
            ClassConsole.ConsoleWriteLine(ClassConnectorSetting.CoinName + " RPC Wallet - " + Assembly.GetExecutingAssembly().GetName().Version + "R", ClassConsoleColorEnumeration.IndexConsoleBlueLog, LogLevel);
            if (ClassRpcSetting.InitializeRpcWalletSetting())
            {
                ClassConsole.ConsoleWriteLine("Please write your rpc wallet password for decrypt your databases of wallet (Input keys are hidden): ", ClassConsoleColorEnumeration.IndexConsoleYellowLog, LogLevel);
                ClassRpcDatabase.SetRpcDatabasePassword(ClassUtility.GetHiddenConsoleInput());
                ClassConsole.ConsoleWriteLine("RPC Wallet Database loading..", ClassConsoleColorEnumeration.IndexConsoleYellowLog, LogLevel);
                if (ClassRpcDatabase.LoadRpcDatabaseFile())
                {
                    ClassConsole.ConsoleWriteLine("RPC Wallet Database successfully loaded.", ClassConsoleColorEnumeration.IndexConsoleGreenLog, LogLevel);
                    ClassConsole.ConsoleWriteLine("RPC Sync Database loading..", ClassConsoleColorEnumeration.IndexConsoleYellowLog,LogLevel);
                    if (ClassSyncDatabase.InitializeSyncDatabase())
                    {

                        ClassConsole.ConsoleWriteLine("RPC Sync Database successfully loaded.", ClassConsoleColorEnumeration.IndexConsoleGreenLog, LogLevel);
                        if (ClassRpcSetting.WalletEnableAutoUpdateWallet)
                        {
                            ClassConsole.ConsoleWriteLine("Enable Auto Update Wallet System..", ClassConsoleColorEnumeration.IndexConsoleYellowLog, LogLevel);
                            ClassWalletUpdater.EnableAutoUpdateWallet();
                            ClassConsole.ConsoleWriteLine("Enable Auto Update Wallet System done.", ClassConsoleColorEnumeration.IndexConsoleGreenLog, LogLevel);
                        }
                        ClassConsole.ConsoleWriteLine("Start RPC Wallet API Server..", ClassConsoleColorEnumeration.IndexConsoleYellowLog, LogLevel);
                        ClassApi.StartApiHttpServer();
                        ClassConsole.ConsoleWriteLine("Start RPC Wallet API Server sucessfully started.", ClassConsoleColorEnumeration.IndexConsoleGreenLog, LogLevel);
                        if (ClassRpcSetting.RpcWalletEnableRemoteNodeSync && ClassRpcSetting.RpcWalletRemoteNodeHost != string.Empty && ClassRpcSetting.RpcWalletRemoteNodePort != 0)
                        {
                            ClassConsole.ConsoleWriteLine("RPC Remote Node Sync system loading..", ClassConsoleColorEnumeration.IndexConsoleYellowLog, LogLevel);
                            ThreadRemoteNodeSync = new Thread(async () => await ClassRemoteSync.ConnectRpcWalletToRemoteNodeSyncAsync());
                            ThreadRemoteNodeSync.Start();
                        }
                        ClassConsole.ConsoleWriteLine("Enable Command Line system.", ClassConsoleColorEnumeration.IndexConsoleGreenLog, LogLevel);
                        ClassConsoleCommandLine.EnableConsoleCommandLine();
                    }
                    else
                    {
                        ClassConsole.ConsoleWriteLine("Cannot read RPC Sync Database, the database is maybe corrupted.", ClassConsoleColorEnumeration.IndexConsoleRedLog, LogLevel);
                        Console.WriteLine("Press ENTER to exit.");
                        Console.ReadLine();
                        Environment.Exit(0);
                    }
                }
                else
                {
                    ClassConsole.ConsoleWriteLine("Cannot read RPC Wallet Database, the database is maybe corrupted.", ClassConsoleColorEnumeration.IndexConsoleRedLog, LogLevel);
                    Console.WriteLine("Press ENTER to exit.");
                    Console.ReadLine();
                    Environment.Exit(0);
                }
            }
            else
            {
                ClassConsole.ConsoleWriteLine("Cannot read RPC Wallet setting, the setting is maybe corrupted, you can delete your setting file to build another one.", ClassConsoleColorEnumeration.IndexConsoleRedLog, LogLevel);
                Console.WriteLine("Press ENTER to exit.");
                Console.ReadLine();
                Environment.Exit(0);
            }

        }

        /// <summary>
        /// Catch unexpected exception and them to a log file.
        /// </summary>
        private static void EnableCatchUnexpectedException()
        {
            AppDomain.CurrentDomain.UnhandledException += delegate (object sender, UnhandledExceptionEventArgs args2)
            {
                var filePath = ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + UnexpectedExceptionFile);
                var exception = (Exception)args2.ExceptionObject;
                using (var writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine("Message :" + exception.Message + "<br/>" + Environment.NewLine +
                                     "StackTrace :" +
                                     exception.StackTrace +
                                     "" + Environment.NewLine + "Date :" + DateTime.Now);
                    writer.WriteLine(Environment.NewLine +
                                     "-----------------------------------------------------------------------------" +
                                     Environment.NewLine);
                }

                Trace.TraceError(exception.StackTrace);
                Console.WriteLine("Unexpected error catched, check the error file: " + ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + UnexpectedExceptionFile));
                Environment.Exit(1);

            };
        }

        /// <summary>
        /// Event for detect Cancel Key pressed by the user for close the program.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            Exit = true;
            e.Cancel = true;
            Console.WriteLine("Close RPC Wallet tool.");
            Process.GetCurrentProcess().Kill();
        }
    }
}
