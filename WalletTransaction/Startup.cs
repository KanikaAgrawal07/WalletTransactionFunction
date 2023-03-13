using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WalletTransaction.DatabaseModel;
using WalletTransaction.Service;

[assembly: FunctionsStartup(typeof(WalletTransaction.StartUp))]

namespace WalletTransaction;

public class StartUp : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        string connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");
        builder.Services.AddDbContext<FunctionAppDbContext>(
          options => options.UseSqlServer(connectionString));

        builder.Services.AddTransient<IWalletService, WalletService>();
    }
}