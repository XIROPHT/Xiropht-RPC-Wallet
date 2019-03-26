using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using Xiropht_Connector_All.Setting;
using Xiropht_Rpc_Wallet.API;
using Xiropht_Rpc_Wallet.ConsoleObject;
using Xiropht_Rpc_Wallet.Database;
using Xiropht_Rpc_Wallet.Log;
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

        static void Main(string[] args)
        {
            EnableCatchUnexpectedException();
            Console.CancelKeyPress += Console_CancelKeyPress;
            Thread.CurrentThread.Name = Path.GetFileName(Environment.GetCommandLineArgs()[0]);
            GlobalCultureInfo = new CultureInfo("fr-FR");
            ClassLog.LogInitialization();
            ClassConsole.ConsoleWriteLine(ClassConnectorSetting.CoinName + " RPC Wallet - " + Assembly.GetExecutingAssembly().GetName().Version + "R", ClassConsoleEnumeration.IndexPoolConsoleBlueLog);
            ClassConsole.ConsoleWriteLine("Please write your rpc wallet password for decrypt your databases of wallet (Input keys are hidden): ", ClassConsoleEnumeration.IndexPoolConsoleYellowLog);
            ClassRpcDatabase.SetRpcDatabasePassword(ClassUtility.GetHiddenConsoleInput());
            if (ClassRpcDatabase.LoadRpcDatabaseFile())
            {
                ClassConsole.ConsoleWriteLine("RPC Wallet Database successfully loaded.", ClassConsoleEnumeration.IndexPoolConsoleGreenLog);
                ClassConsole.ConsoleWriteLine("Enable Auto Update Wallet System..", ClassConsoleEnumeration.IndexPoolConsoleYellowLog);
                ClassWalletUpdater.EnableAutoUpdateWallet();
                ClassConsole.ConsoleWriteLine("Enable Auto Update Wallet System done.", ClassConsoleEnumeration.IndexPoolConsoleGreenLog);
                ClassConsole.ConsoleWriteLine("Start RPC Wallet API Server..", ClassConsoleEnumeration.IndexPoolConsoleYellowLog);
                ClassApi.StartApiHttpServer();
                ClassConsole.ConsoleWriteLine("Start RPC Wallet API Server sucessfully started.", ClassConsoleEnumeration.IndexPoolConsoleGreenLog);
                ClassConsole.ConsoleWriteLine("Enable Command Line system.", ClassConsoleEnumeration.IndexPoolConsoleGreenLog);
                ClassConsoleCommandLine.EnableConsoleCommandLine();

            }
            else
            {
                ClassConsole.ConsoleWriteLine("Cannot read RPC Wallet Database, the database is maybe corrupted.", ClassConsoleEnumeration.IndexPoolConsoleRedLog);
                Console.WriteLine("Press ENTER to exit.");
                Console.ReadLine();
            }

        }

        /// <summary>
        /// Catch unexpected exception and them to a log file.
        /// </summary>
        private static void EnableCatchUnexpectedException()
        {
            AppDomain.CurrentDomain.UnhandledException += delegate (object sender, UnhandledExceptionEventArgs args2)
            {
                var filePath = ClassUtility.ConvertPath(Directory.GetCurrentDirectory() + UnexpectedExceptionFile);
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
                Console.WriteLine("Unexpected error catched, check the error file: " + ClassUtility.ConvertPath(Directory.GetCurrentDirectory() + UnexpectedExceptionFile));
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
