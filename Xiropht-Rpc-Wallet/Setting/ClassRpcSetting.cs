using System.Collections.Generic;

namespace Xiropht_Rpc_Wallet.Setting
{
    public class ClassRpcSetting
    {
        public static int RpcWalletApiPort = 8000; // RPC Wallet API Default port.

        public static List<string> RpcWalletApiIpWhitelist = new List<string>(); // List of IP whitelisted on the API Server, if the list is empty everyone can try to access on the port.

        public static string RpcWalletApiKeyRequestEncryption = string.Empty; // The key for encrypt request to receive/sent.

    }
}
