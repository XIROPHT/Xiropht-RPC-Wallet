namespace Xiropht_Rpc_Wallet.API
{
    public class ClassApiJsonTransaction 
    {
        public long index;
        public string wallet_address;
        public string type;
        public string hash;
        public string mode;
        public string wallet_dst_or_src;
        public decimal amount;
        public decimal fee;
        public long timestamp_send;
        public long timestamp_recv;
        public string blockchain_height;
    }

    public class ClassApiJsonSendTransaction
    {
        public string result;
        public string hash;
        public decimal wallet_balance;
        public decimal wallet_pending_balance;
    }

    public class ClassApiJsonSendTransfer
    {
        public string result;
        public string hash;
        public decimal wallet_balance;
        public decimal wallet_pending_balance;
    }

    public class ClassApiJsonWalletBalance
    {
        public string wallet_address;
        public decimal wallet_balance;
        public decimal wallet_pending_balance;
    }

    public class ClassApiJsonWalletUpdate
    {
        public string wallet_address;
        public decimal wallet_balance;
        public decimal wallet_pending_balance;
        public long wallet_unique_id;
        public decimal wallet_unique_anonymous_id;
    }

    public class ClassApiJsonWalletTotalTransaction
    {
        public string wallet_address;
        public long wallet_total_transaction;
    }

    public class ClassApiJsonWalletTotalAnonymousTransaction
    {
        public string wallet_address;
        public long wallet_total_anonymous_transaction;
    }

    public class ClassApiJsonTotalWalletCount
    {
        public long result;
    }
}
