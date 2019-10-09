using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xiropht_Rpc_Wallet.ConsoleObject;
using Xiropht_Rpc_Wallet.Setting;
using Xiropht_Rpc_Wallet.Utility;

namespace Xiropht_Rpc_Wallet.Log
{
    public class ClassLogEnumeration
    {
        public const int LogIndexGeneral = 0;
        public const int LogIndexWalletUpdater = 1;
        public const int LogIndexApi = 2;
        public const int LogIndexSync = 3;
        public const int LogIndexRemoteNodeSync = 4;
    }


    public class ClassLog
    {
        /// <summary>
        /// Log file main path.
        /// </summary>
        private const string LogDirectory = "\\Log\\";

        /// <summary>
        /// Log files paths
        /// </summary>
        private const string LogGeneral = "\\Log\\rpc-general.log"; // 0
        private const string LogWalletUpdater = "\\Log\\rpc-wallet-updater.log"; // 1
        private const string LogApi = "\\Log\\rpc-api.log"; // 2
        private const string LogSync = "\\Log\\rpc-sync.log"; // 3
        private const string LogRemoteNodeSync = "\\Log\\rpc-remote-node-sync.log"; // 4

        /// <summary>
        /// Streamwriter objects
        /// </summary>
        private static StreamWriter _logGeneralStreamWriter;
        private static StreamWriter _logWalletUpdaterStreamWriter;
        private static StreamWriter _logApiStreamWriter;
        private static StreamWriter _logSyncStreamWriter;
        private static StreamWriter _logRemoteNodeSyncStreamWriter;

        /// <summary>
        /// Contains logs to write.
        /// </summary>
        private static List<Tuple<int, string>> ListOfLog = new List<Tuple<int, string>>(); // Structure Tuple => log id, content text.

        /// <summary>
        /// Write log settings.
        /// </summary>
        private const int WriteLogBufferSize = 8192;
        private static CancellationTokenSource _cancellationTokenTaskWriteLog;
        private static long _lastCleanLogDate;

        /// <summary>
        /// Log Initialization.
        /// </summary>
        /// <returns></returns>
        public static bool LogInitialization(bool fromThread = false)
        {
            try
            {
                LogInitializationFile();
                LogInitizaliationStreamWriter();
                if (!fromThread)
                {
                    AutoWriteLog();
                }
            }
            catch (Exception error)
            {
                ClassConsole.ConsoleWriteLine("Failed to initialize log system, exception error: " + error.Message, ClassConsoleColorEnumeration.IndexConsoleRedLog);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Create the log directory and log files if they not exist.
        /// </summary>
        private static bool LogInitializationFile()
        {
            if (Directory.Exists(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + LogDirectory)) == false)
            {
                Directory.CreateDirectory(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + LogDirectory));
                return false;
            }

            if (!File.Exists(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + LogGeneral)))
            {
                File.Create(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + LogGeneral)).Close();
                return false;
            }

            if (!File.Exists(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + LogWalletUpdater)))
            {
                File.Create(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + LogWalletUpdater)).Close();
                return false;
            }

            if (!File.Exists(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + LogApi)))
            {
                File.Create(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + LogApi)).Close();
                return false;
            }

            if (!File.Exists(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + LogSync)))
            {
                File.Create(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + LogSync)).Close();
                return false;
            }

            if (!File.Exists(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + LogRemoteNodeSync)))
            {
                File.Create(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + LogRemoteNodeSync)).Close();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Initialize stream writer's for push logs into log files.
        /// </summary>
        private static void LogInitizaliationStreamWriter()
        {
            _logApiStreamWriter?.Close();
            _logGeneralStreamWriter?.Close();
            _logWalletUpdaterStreamWriter?.Close();
            _logSyncStreamWriter?.Close();
            _logRemoteNodeSyncStreamWriter?.Close();


            _logGeneralStreamWriter = new StreamWriter(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + LogGeneral), true, Encoding.UTF8, WriteLogBufferSize) { AutoFlush = true };
            _logWalletUpdaterStreamWriter = new StreamWriter(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + LogWalletUpdater), true, Encoding.UTF8, WriteLogBufferSize) { AutoFlush = true };
            _logApiStreamWriter = new StreamWriter(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + LogApi), true, Encoding.UTF8, WriteLogBufferSize) { AutoFlush = true };
            _logSyncStreamWriter = new StreamWriter(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + LogSync), true, Encoding.UTF8, WriteLogBufferSize) { AutoFlush = true };
            _logRemoteNodeSyncStreamWriter = new StreamWriter(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + LogRemoteNodeSync), true, Encoding.UTF8, WriteLogBufferSize) { AutoFlush = true };

        }

        /// <summary>
        /// Clean up logs.
        /// </summary>
        private static void LogCleanUp()
        {
            _logApiStreamWriter?.Close();
            _logGeneralStreamWriter?.Close();
            _logWalletUpdaterStreamWriter?.Close();
            _logSyncStreamWriter?.Close();
            _logRemoteNodeSyncStreamWriter?.Close();
            File.Create(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + LogGeneral)).Close();
            File.Create(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + LogWalletUpdater)).Close();
            File.Create(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + LogApi)).Close();
            File.Create(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + LogSync)).Close();
            File.Create(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + LogRemoteNodeSync)).Close();
            _logGeneralStreamWriter = new StreamWriter(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + LogGeneral), true, Encoding.UTF8, WriteLogBufferSize) { AutoFlush = true };
            _logWalletUpdaterStreamWriter = new StreamWriter(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + LogWalletUpdater), true, Encoding.UTF8, WriteLogBufferSize) { AutoFlush = true };
            _logApiStreamWriter = new StreamWriter(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + LogApi), true, Encoding.UTF8, WriteLogBufferSize) { AutoFlush = true };
            _logSyncStreamWriter = new StreamWriter(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + LogSync), true, Encoding.UTF8, WriteLogBufferSize) { AutoFlush = true };
            _logRemoteNodeSyncStreamWriter = new StreamWriter(ClassUtility.ConvertPath(AppDomain.CurrentDomain.BaseDirectory + LogRemoteNodeSync), true, Encoding.UTF8, WriteLogBufferSize) { AutoFlush = true };
        }

        /// <summary>
        /// Insert logs inside the list of logs to write.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="logId"></param>
        public static void InsertLog(string text, int logId)
        {
            try
            {
                ListOfLog.Add(new Tuple<int, string>(logId, text));
            }
            catch
            {
                // Ignored
            }
        }

        /// <summary>
        /// Auto write logs
        /// </summary>
        private static void AutoWriteLog()
        {
            _cancellationTokenTaskWriteLog = new CancellationTokenSource();

            try
            {
                Task.Factory.StartNew(async delegate ()
                {
                    while (!Program.Exit)
                    {
                        try
                        {
                            if (ClassRpcSetting.WalletEnableAutoCleanLog)
                            {
                                if (_lastCleanLogDate + ClassRpcSetting.WalletAutoCleanLogInterval < DateTimeOffset.Now.ToUnixTimeSeconds())
                                {
                                    _lastCleanLogDate = DateTimeOffset.Now.ToUnixTimeSeconds();
                                    ClassConsole.ConsoleWriteLine("AutoClean Logs started..", ClassConsoleColorEnumeration.IndexConsoleYellowLog);
                                    LogCleanUp();
                                    ClassConsole.ConsoleWriteLine("AutoClean Logs done.");
                                }
                            }
                            if (ListOfLog.Count > 0)
                            {
                                if (ListOfLog.Count >= 100)
                                {
                                    if (!LogInitializationFile()) // Remake log files if one of them missing, close and open again streamwriter objects.
                                {
                                        LogInitizaliationStreamWriter();
                                    }
                                    var copyOfLog = new List<Tuple<int, string>>(ListOfLog);
                                    ListOfLog.Clear();
                                    if (copyOfLog.Count > 0)
                                    {
                                        foreach (var log in copyOfLog)
                                        {
                                            await WriteLogAsync(log.Item2, log.Item1);
                                        }
                                    }
                                    copyOfLog.Clear();
                                }
                            }
                        }
                        catch
                        {
                            try
                            {
                                ListOfLog.Clear();
                            }
                            catch
                            {
                                LogInitialization(true);
                            }
                        }
                        await Task.Delay(10 * 1000);
                    }
                }, _cancellationTokenTaskWriteLog.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);
            }
            catch
            {
                // Catch the exception once the Task is cancelled.
            }
        }

        /// <summary>
        /// Write log on the selected log file in async mode.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="idLog"></param>
        private static async Task WriteLogAsync(string text, int idLog)
        {
            switch (idLog)
            {
                case ClassLogEnumeration.LogIndexGeneral:
                    await _logGeneralStreamWriter.WriteLineAsync(text);
                    break;
                case ClassLogEnumeration.LogIndexWalletUpdater:
                    await _logWalletUpdaterStreamWriter.WriteLineAsync(text);
                    break;
                case ClassLogEnumeration.LogIndexApi:
                    await _logApiStreamWriter.WriteLineAsync(text);
                    break;
                case ClassLogEnumeration.LogIndexSync:
                    await _logApiStreamWriter.WriteLineAsync(text);
                    break;
                case ClassLogEnumeration.LogIndexRemoteNodeSync:
                    await _logRemoteNodeSyncStreamWriter.WriteLineAsync(text);
                    break;
            }
        }

        /// <summary>
        /// Stop log system.
        /// </summary>
        public static void StopLogSystem()
        {
            try
            {
                if (_cancellationTokenTaskWriteLog != null)
                {
                    if (!_cancellationTokenTaskWriteLog.IsCancellationRequested)
                    {
                        _cancellationTokenTaskWriteLog.Cancel();
                    }
                }
            }
            catch
            {
                // Ignored
            }
            try
            {
                _logApiStreamWriter?.Close();
                _logGeneralStreamWriter?.Close();
                _logWalletUpdaterStreamWriter?.Close();
                _logSyncStreamWriter?.Close();
                _logRemoteNodeSyncStreamWriter?.Close();
            }
            catch
            {
                // Ignored
            }
        }
    }
}
