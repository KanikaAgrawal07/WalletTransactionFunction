using System;
using Microsoft.EntityFrameworkCore;

namespace WalletTransaction.DatabaseModel;

public partial class FunctionAppDbContext : DbContext
{
    public FunctionAppDbContext()
    {
    }

    public FunctionAppDbContext(DbContextOptions<FunctionAppDbContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("SqlConnectionString"));
        }
    }

    public virtual DbSet<Wallet> Wallets { get; set; }

    public virtual DbSet<WalletTransactionRecord> WalletTransactionRecords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Wallet>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__Wallet__349DA5861843ED44");

            entity.ToTable("Wallet");

            entity.HasIndex(e => e.AccountId, "wallet_accountId");

            entity.Property(e => e.AccountId)
                .HasMaxLength(50)
                .HasColumnName("AccountID");
            entity.Property(e => e.AccountBalance)
            .IsRequired()
            .HasColumnType("decimal(9, 2)");
            entity.Property(e => e.AccountHolderName)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.CurrencyCode)
                .IsRequired()
                .HasMaxLength(20);
        });

        modelBuilder.Entity<WalletTransactionRecord>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__WalletTr__55433A4B81F6E8C3");

            entity.Property(e => e.TransactionId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("TransactionID");
            entity.Property(e => e.AccountId)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("AccountID");
            entity.Property(e => e.Direction)
                .IsRequired()
                .HasMaxLength(20);
            entity.Property(e => e.TransactionTimestamp).HasColumnType("datetime");
            entity.Property(e => e.TransactionAmount).HasColumnType("decimal(9, 2)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}