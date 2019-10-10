using System;
using Xiropht_Rpc_Wallet.Log;

namespace Xiropht_Rpc_Wallet.ConsoleObject
{

    public class ClassConsoleColorEnumeration
    {
        public const int IndexConsoleGreenLog = 0;
        public const int IndexConsoleYellowLog = 1;
        public const int IndexConsoleRedLog = 2;
        public const int IndexConsoleBlueLog = 4;
        public const int IndexConsoleMagentaLog = 5;
    }

    public class ClassConsoleLogLevelEnumeration
    {
        public const int LogLevelGeneral = 0;
        public const int LogLevelWalletObject = 1;
        public const int LogLevelApi = 2;
        public const int LogLevelSyncDatabase = 3;
        public const int LogLevelRemoteNodeSync = 4;
    }

    public class ClassConsole
    {
        public const int MaxLogLevel = 4;

        /// <summary>
        /// Log on the console.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="colorId"></param>
        /// <param name="logLevel"></param>
        public static void ConsoleWriteLine(string text, int colorId = 0, int logLevel = 0)
        {
            text = DateTime.Now + " - " + text;
            if (Program.LogLevel == logLevel)
            {
                switch (colorId)
                {
                    case ClassConsoleColorEnumeration.IndexConsoleGreenLog:
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;
                    case ClassConsoleColorEnumeration.IndexConsoleYellowLog:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    case ClassConsoleColorEnumeration.IndexConsoleRedLog:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                    case ClassConsoleColorEnumeration.IndexConsoleBlueLog:
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        break;
                    case ClassConsoleColorEnumeration.IndexConsoleMagentaLog:
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                }
                Console.WriteLine(text);
            }
            ClassLog.InsertLog(text, logLevel);
        }
    }
}
