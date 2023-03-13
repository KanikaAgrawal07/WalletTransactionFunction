namespace WalletTransaction.Model;

public class Transaction
{
    public string AccountId { get; set; }
    public decimal TransactionAmount { get; set; }
    public string Direction { get; set; }
}