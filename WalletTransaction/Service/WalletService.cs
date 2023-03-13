using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WalletTransaction.DatabaseModel;

namespace WalletTransaction.Service;

public class WalletService : IWalletService
{
    private readonly FunctionAppDbContext _dbContext;

    public WalletService(FunctionAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Wallet> GetWalletDetailsByAccountId(string accountId)
    {
        Wallet walletDetailsById = await _dbContext.Wallets.FromSqlInterpolated($"SELECT * FROM dbo.Wallet WHERE AccountId = {accountId}").AsNoTracking().FirstOrDefaultAsync();
        return walletDetailsById;
    }

    public async Task<Guid> AddTransactionDetails(WalletTransactionRecord transaction)
    {
        await _dbContext.WalletTransactionRecords.AddAsync(transaction);
        await _dbContext.SaveChangesAsync();

        return transaction.TransactionId;
    }

    public async Task<int> UpdateWalletAmount(string accountId, decimal accountBalance)
    {
        var rowsModified = await _dbContext.Database.ExecuteSqlInterpolatedAsync($"UPDATE dbo.Wallet SET AccountBalance = {accountBalance} where AccountID = {accountId}");
        return rowsModified;
    }
}

