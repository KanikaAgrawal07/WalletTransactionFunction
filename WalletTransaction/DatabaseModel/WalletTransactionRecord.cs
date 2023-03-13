using System;

namespace WalletTransaction.DatabaseModel;

public partial class WalletTransactionRecord
{
    public Guid TransactionId { get; set; }

    public string AccountId { get; set; }

    public decimal TransactionAmount { get; set; }

    public string Direction { get; set; }

    public DateTime TransactionTimestamp { get; set; }
}