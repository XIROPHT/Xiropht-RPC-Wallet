using System;
using Xiropht_Connector_All.Utils;
using Xiropht_Connector_All.Wallet;
using Xiropht_Rpc_Wallet.Database;

namespace Xiropht_Rpc_Wallet.Remote
{
    public class ClassSortingTransaction
    {
        /// <summary>
        /// Decrypt and sorting a transaction received on sync, finaly save it.
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="walletAddress"></param>
        /// <param name="walletPublicKey"></param>
        /// <param name="anonymous"></param>
        /// <returns></returns>
        public static void SaveTransactionSorted(string transaction, string walletAddress, string walletPublicKey, bool anonymous)
        {
            var splitTransaction = transaction.Split(new[] { "#" }, StringSplitOptions.None);
            string type = splitTransaction[0];
            string timestamp = splitTransaction[3];
            string hashTransaction = splitTransaction[4];
            string timestampReceived = splitTransaction[5];
            string blockchainHeight = splitTransaction[6];
            string realTransactionInformationSenderSide = splitTransaction[7];
            string realTransactionInformationReceiverSide = splitTransaction[8];

            string realTransactionInformationDecrypted = "NULL";
            if (type == "SEND")
            {
                realTransactionInformationDecrypted = ClassAlgo.GetDecryptedResultManual(ClassAlgoEnumeration.Rijndael, realTransactionInformationSenderSide, walletAddress + walletPublicKey, ClassWalletNetworkSetting.KeySize);
            }
            else if (type == "RECV")
            {
                realTransactionInformationDecrypted = ClassAlgo.GetDecryptedResultManual(ClassAlgoEnumeration.Rijndael, realTransactionInformationReceiverSide, walletAddress + walletPublicKey, ClassWalletNetworkSetting.KeySize);
            }
            if (realTransactionInformationDecrypted != "NULL" && realTransactionInformationDecrypted != ClassAlgoErrorEnumeration.AlgoError)
            {
                var splitDecryptedTransactionInformation = realTransactionInformationDecrypted.Split(new[] { "-" }, StringSplitOptions.None);
                string amountTransaction = splitDecryptedTransactionInformation[0];
                string feeTransaction = splitDecryptedTransactionInformation[1];
                string walletAddressDstOrSrc = splitDecryptedTransactionInformation[2];
                string finalTransaction;
                if (anonymous)
                {
                    finalTransaction = "anonymous#" + type + "#" + hashTransaction + "#" + walletAddressDstOrSrc + "#" + amountTransaction + "#" + feeTransaction + "#" + timestamp + "#" + timestampReceived + "#" + blockchainHeight;
                    ClassRpcDatabase.RpcDatabaseContent[walletAddress].InsertWalletTransactionSync(finalTransaction, true, true);
                }
                else
                {
                    finalTransaction = "normal#" + type + "#" + hashTransaction + "#" + walletAddressDstOrSrc + "#" + amountTransaction + "#" + feeTransaction + "#" + timestamp + "#" + timestampReceived + "#" + blockchainHeight;
                    ClassRpcDatabase.RpcDatabaseContent[walletAddress].InsertWalletTransactionSync(finalTransaction, false, true);
                }
            }
        }
    }
}
