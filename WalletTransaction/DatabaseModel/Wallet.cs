namespace WalletTransaction.DatabaseModel;

public partial class Wallet
{
    public string AccountId { get; set; }

    public string AccountHolderName { get; set; }

    public decimal AccountBalance { get; set; }

    public string CurrencyCode { get; set; }
}