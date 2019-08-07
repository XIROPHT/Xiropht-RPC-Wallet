using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xiropht_Connector_All.RPC;
using Xiropht_Connector_All.Utils;
using Xiropht_Rpc_Wallet.ConsoleObject;
using Xiropht_Rpc_Wallet.Wallet;

namespace Xiropht_Rpc_Wallet.API
{
    public enum ClassApiTaskStatus
    {
        API_TASK_STATUS_PENDING = 0,
        API_TASK_STATUS_COMPLETE = 1,
        API_TASK_STATUS_FAILED = 2
    }

    public enum ClassApiTaskType
    {
        API_TASK_TYPE_TRANSACTION = 0,
        API_TASK_TYPE_TRANSFER = 1
    }

    public class ClassApiTask
    {
        public long TaskDate;
        public ClassApiTaskStatus TaskStatus;
        public ClassApiTaskType TaskType;
        public string TaskWalletSrc;
        public string TaskWalletAmount;
        public string TaskWalletFee;
        public string TaskWalletAnonymity;
        public string TaskWalletDst;
        public string TaskResult = string.Empty;
    }

    public class ClassApiTaskScheduler
    {
        private static Dictionary<string, ClassApiTask> DictionaryApiTaskScheduled = new Dictionary<string, ClassApiTask>();
        private static CancellationTokenSource TaskApiSchedulerCancellationSource;

        private const int MaxInsertTaskScheduledTimeout = 10; // Attempt pending maximum 10 seconds to generate a unique hash of idenficiation of the task.

        /// <summary>
        /// Start api task scheduler process.
        /// </summary>
        public static void StartApiTaskScheduler()
        {
            StopApiTaskScheduler();
            TaskApiSchedulerCancellationSource = new CancellationTokenSource();
            try
            {
                Task.Factory.StartNew(async () =>
                {
                    ClassConsole.ConsoleWriteLine("API Task Scheduler - Processing task(s) scheduled start successfully.", ClassConsoleColorEnumeration.IndexConsoleGreenLog, Program.LogLevel);

                    while (!Program.Exit)
                    {
                        try
                        {
                            if (DictionaryApiTaskScheduled.Count > 0)
                            {
                                foreach (var taskScheduled in DictionaryApiTaskScheduled.ToArray())
                                {
                                    try
                                    {
                                        if (taskScheduled.Value != null)
                                        {
                                            if (taskScheduled.Value.TaskStatus == ClassApiTaskStatus.API_TASK_STATUS_PENDING)
                                            {
                                                if (taskScheduled.Value.TaskDate <= DateTimeOffset.Now.ToUnixTimeSeconds())
                                                {
                                                    switch (taskScheduled.Value.TaskType)
                                                    {
                                                        case ClassApiTaskType.API_TASK_TYPE_TRANSACTION:
                                                            bool anonymous = taskScheduled.Value.TaskWalletAnonymity == "1";
                                                            string result = await ClassWalletUpdater.ProceedTransactionTokenRequestAsync(taskScheduled.Value.TaskWalletSrc, taskScheduled.Value.TaskWalletAmount, taskScheduled.Value.TaskWalletFee, taskScheduled.Value.TaskWalletDst, anonymous);
                                                            var splitResult = result.Split(new[] { "|" }, StringSplitOptions.None);
                                                            if (splitResult[0] == ClassRpcWalletCommand.SendTokenTransactionConfirmed)
                                                            {
                                                                ClassConsole.ConsoleWriteLine("API Task Scheduler - Task Hash ID: " + taskScheduled.Key + " sucessfully proceed. | Result: " + splitResult[0], ClassConsoleColorEnumeration.IndexConsoleGreenLog, ClassConsoleLogLevelEnumeration.LogLevelApi);
                                                                DictionaryApiTaskScheduled[taskScheduled.Key].TaskStatus = ClassApiTaskStatus.API_TASK_STATUS_COMPLETE;
                                                                DictionaryApiTaskScheduled[taskScheduled.Key].TaskResult = result;
                                                            }
                                                            else
                                                            {
                                                                ClassConsole.ConsoleWriteLine("API Task Scheduler - Task Hash ID: " + taskScheduled.Key + " failed. | Result: " + splitResult[0], ClassConsoleColorEnumeration.IndexConsoleYellowLog, ClassConsoleLogLevelEnumeration.LogLevelApi);
                                                                DictionaryApiTaskScheduled[taskScheduled.Key].TaskStatus = ClassApiTaskStatus.API_TASK_STATUS_FAILED;
                                                                DictionaryApiTaskScheduled[taskScheduled.Key].TaskResult = result;
                                                            }
                                                            break;
                                                        case ClassApiTaskType.API_TASK_TYPE_TRANSFER:
                                                            result = await ClassWalletUpdater.ProceedTransferTokenRequestAsync(taskScheduled.Value.TaskWalletSrc, taskScheduled.Value.TaskWalletAmount, taskScheduled.Value.TaskWalletDst);
                                                            splitResult = result.Split(new[] { "|" }, StringSplitOptions.None);
                                                            if (splitResult[0] == ClassRpcWalletCommand.SendTokenTransferConfirmed)
                                                            {
                                                                ClassConsole.ConsoleWriteLine("API Task Scheduler - Task Hash ID: " + taskScheduled.Key + " sucessfully proceed. | Result: " + splitResult[0], ClassConsoleColorEnumeration.IndexConsoleGreenLog, ClassConsoleLogLevelEnumeration.LogLevelApi);
                                                                DictionaryApiTaskScheduled[taskScheduled.Key].TaskStatus = ClassApiTaskStatus.API_TASK_STATUS_COMPLETE;
                                                                DictionaryApiTaskScheduled[taskScheduled.Key].TaskResult = result;
                                                            }
                                                            else
                                                            {
                                                                ClassConsole.ConsoleWriteLine("API Task Scheduler - Task Hash ID: " + taskScheduled.Key + " failed. | Result: " + splitResult[0], ClassConsoleColorEnumeration.IndexConsoleYellowLog, ClassConsoleLogLevelEnumeration.LogLevelApi);
                                                                DictionaryApiTaskScheduled[taskScheduled.Key].TaskStatus = ClassApiTaskStatus.API_TASK_STATUS_FAILED;
                                                                DictionaryApiTaskScheduled[taskScheduled.Key].TaskResult = result;
                                                            }
                                                            break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception errorTask)
                                    {
                                        ClassConsole.ConsoleWriteLine("API Task Scheduler - error from Task Hash ID: " + taskScheduled.Key + " | Exception: " + errorTask.Message, ClassConsoleColorEnumeration.IndexConsoleRedLog, ClassConsoleLogLevelEnumeration.LogLevelApi);
                                    }
                                }
                            }
                        }
                        catch (Exception errorTask)
                        {
                            ClassConsole.ConsoleWriteLine("API Task Scheduler - Can't travel and proceed task(s) scheduled stored. | Exception: " + errorTask.Message, ClassConsoleColorEnumeration.IndexConsoleRedLog, ClassConsoleLogLevelEnumeration.LogLevelApi);

                        }
                        await Task.Delay(1000);
                    }
                }, TaskApiSchedulerCancellationSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current).ConfigureAwait(false);
            }
            catch(Exception error)
            {
                ClassConsole.ConsoleWriteLine("API Task Scheduler - exception: " + error.Message, ClassConsoleColorEnumeration.IndexConsoleRedLog, ClassConsoleLogLevelEnumeration.LogLevelApi);
            }
        }

        /// <summary>
        /// Stop api task scheduler process.
        /// </summary>
        public static void StopApiTaskScheduler()
        {
            bool errorStop = true;
            while (errorStop)
            {
                try
                {
                    if (TaskApiSchedulerCancellationSource != null)
                    {
                        if (!TaskApiSchedulerCancellationSource.IsCancellationRequested)
                        {
                            TaskApiSchedulerCancellationSource.Cancel();
                            TaskApiSchedulerCancellationSource.Dispose();
                        }
                    }
                    errorStop = false;
                }
                catch
                {
                    errorStop = true;
                }
            }
        }

        /// <summary>
        /// Insert a new task scheduled.
        /// </summary>
        /// <param name="apiTaskType"></param>
        /// <param name="walletSrc"></param>
        /// <param name="amount"></param>
        /// <param name="fee"></param>
        /// <param name="anonymous"></param>
        /// <param name="walletDst"></param>
        /// <param name="dateScheduled"></param>
        /// <returns></returns>
        public static Tuple<bool, string> InsertTaskScheduled(ClassApiTaskType apiTaskType, string walletSrc, string amount, string fee, string anonymous, string walletDst, long dateScheduled)
        {
            try
            {
                string randomIdentificationHash = string.Empty;
                long dateInsertEnd = DateTimeOffset.Now.ToUnixTimeSeconds() + MaxInsertTaskScheduledTimeout;

                bool exist = true;
                while (exist && dateInsertEnd < DateTimeOffset.Now.ToUnixTimeSeconds())
                {
                    if (DictionaryApiTaskScheduled.Count >= int.MaxValue - 1)
                    {
                        ClassConsole.ConsoleWriteLine("API Task Scheduler - can't insert a new task, the number of task stored have reach the maximum limit of int32 objects: "+int.MaxValue.ToString("F0"), ClassConsoleColorEnumeration.IndexConsoleYellowLog, ClassConsoleLogLevelEnumeration.LogLevelApi);
                        return new Tuple<bool, string>(false, null);
                    }
                    randomIdentificationHash = GenerateTaskIdentificationHash(walletSrc, walletDst);
                    if (!DictionaryApiTaskScheduled.ContainsKey(randomIdentificationHash))
                    {
                        DictionaryApiTaskScheduled.Add(randomIdentificationHash, new ClassApiTask() { TaskDate = dateScheduled, TaskType = apiTaskType, TaskStatus = ClassApiTaskStatus.API_TASK_STATUS_PENDING, TaskWalletAmount = amount, TaskWalletFee = fee, TaskWalletAnonymity = anonymous, TaskWalletSrc = walletSrc, TaskWalletDst = walletDst, TaskResult = string.Empty });
                        long startTaskScheduled = dateScheduled - DateTimeOffset.Now.ToUnixTimeSeconds();
                        if (startTaskScheduled < 0)
                        {
                            startTaskScheduled = 0;
                        }
                        ClassConsole.ConsoleWriteLine("API Task Scheduler - insert a new task scheduled | Task Hash ID: " + randomIdentificationHash + " , Task Wallet Src: " + walletSrc + " , Task amount: " + amount + " , Task fee: " + fee + " , Task Anonymous: " + anonymous + " Task wallet dst: " + walletDst + " executed in " + startTaskScheduled + " second(s).", ClassConsoleColorEnumeration.IndexConsoleYellowLog, ClassConsoleLogLevelEnumeration.LogLevelApi);
                        exist = false;
                        return new Tuple<bool, string>(true, randomIdentificationHash);
                    }
                }
            }
            catch(Exception error)
            {
                ClassConsole.ConsoleWriteLine("API Task Scheduler - Can't insert task scheduled. | Exception: " + error.Message, ClassConsoleColorEnumeration.IndexConsoleRedLog, ClassConsoleLogLevelEnumeration.LogLevelApi);
            }
            return new Tuple<bool, string>(false, null);
        }

        /// <summary>
        /// Clear every task scheduled in complete status or in failure status.
        /// </summary>
        /// <returns></returns>
        public static Tuple<bool, long> ClearTaskScheduledComplete()
        {
            try
            {
                List<string> listOfTaskComplete = new List<string>();
                foreach(var task in DictionaryApiTaskScheduled.ToArray())
                {
                    if (task.Value.TaskStatus == ClassApiTaskStatus.API_TASK_STATUS_COMPLETE || task.Value.TaskStatus == ClassApiTaskStatus.API_TASK_STATUS_FAILED)
                    {
                        listOfTaskComplete.Add(task.Key);
                    }
                }
                ClassConsole.ConsoleWriteLine("API Task Scheduler - Attempt to clear " + listOfTaskComplete.Count.ToString("F0") + " task(s) complete.", ClassConsoleColorEnumeration.IndexConsoleYellowLog, ClassConsoleLogLevelEnumeration.LogLevelApi);
                ClassConsole.ConsoleWriteLine("API Task Scheduler - stop task(s) scheduler process pending cleaning.", ClassConsoleColorEnumeration.IndexConsoleYellowLog, ClassConsoleLogLevelEnumeration.LogLevelApi);
                StopApiTaskScheduler();
                ClassConsole.ConsoleWriteLine("API Task Scheduler - stop task(s) scheduler process successfully done.", ClassConsoleColorEnumeration.IndexConsoleYellowLog, ClassConsoleLogLevelEnumeration.LogLevelApi);

                long totalTaskCleared = 0;
                foreach (var taskHashIdComplete in listOfTaskComplete)
                {
                    if (DictionaryApiTaskScheduled.ContainsKey(taskHashIdComplete))
                    {
                        DictionaryApiTaskScheduled.Remove(taskHashIdComplete);
                        totalTaskCleared++;
                    }
                }

                ClassConsole.ConsoleWriteLine("API Task Scheduler - " + totalTaskCleared.ToString("F0") + " cleared successfully. Total task in pending: "+DictionaryApiTaskScheduled.Count.ToString("F0"), ClassConsoleColorEnumeration.IndexConsoleGreenLog, ClassConsoleLogLevelEnumeration.LogLevelApi);
                StartApiTaskScheduler();
                listOfTaskComplete.Clear();
                return new Tuple<bool, long>(true, totalTaskCleared);
            }
            catch(Exception error)
            {
                ClassConsole.ConsoleWriteLine("API Task Scheduler - Can't clear task scheduled complete/failed. | Exception: "+error.Message, ClassConsoleColorEnumeration.IndexConsoleRedLog, ClassConsoleLogLevelEnumeration.LogLevelApi);

            }
            return new Tuple<bool, long>(false, 0);

        }

        /// <summary>
        /// Generate a task of identification
        /// </summary>
        /// <returns></returns>
        private static string GenerateTaskIdentificationHash(string walletSrc, string walletDst)
        {
            double randomIdentificationNumber = DateTimeOffset.Now.ToUnixTimeMilliseconds() + ClassUtils.GetRandomBetweenLong(0, long.MaxValue - 1);
            string randomIdentificationMerged = randomIdentificationNumber.ToString("F0") + walletSrc + walletDst;
            return ClassUtils.ConvertStringtoSHA512(randomIdentificationMerged);
        }
    }
}
