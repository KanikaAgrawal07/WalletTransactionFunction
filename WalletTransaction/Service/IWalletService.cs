using System;
using System.Threading.Tasks;
using WalletTransaction.DatabaseModel;

namespace WalletTransaction.Service;

public interface IWalletService
{
    Task<Wallet> GetWalletDetailsByAccountId(string accountId);

    Task<Guid> AddTransactionDetails(WalletTransactionRecord transaction);

    Task<int> UpdateWalletAmount(string accountId, decimal accountBalance);
}