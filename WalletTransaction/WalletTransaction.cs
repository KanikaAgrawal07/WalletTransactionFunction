using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WalletTransaction.DatabaseModel;
using WalletTransaction.Model;
using WalletTransaction.Service;

namespace WalletTransaction;

public class WalletTransaction
{
    private readonly IWalletService _walletService;

    public WalletTransaction(IWalletService walletService)
    {
        _walletService = walletService;
    }

    [FunctionName("WalletTransaction")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
        ILogger log)
    {
        var responseMessage = string.Empty;
        var content = String.Empty;

        using (StreamReader streamReader = new StreamReader(req.Body))
        {
            content = await streamReader.ReadToEndAsync();
        }
        Transaction transactionRequest = JsonConvert.DeserializeObject<Transaction>(content);

        try
        {
            if (transactionRequest != null && !string.IsNullOrEmpty(transactionRequest.AccountId))
            {
                var walletAmountAfterTransaction = 0m;

                //Fetching Wallet details of an account
                var walletDetails = await _walletService.GetWalletDetailsByAccountId(transactionRequest.AccountId);

                if (walletDetails == null)
                {
                    log.LogInformation($"Account '{transactionRequest.AccountId}' not found");
                    responseMessage = $"Account '{transactionRequest.AccountId}' not found";
                    return new NotFoundObjectResult(responseMessage);
                }
                else
                {
                    log.LogInformation($"Wallet details for the account {walletDetails.AccountId}: Account Holder Name - {walletDetails.AccountHolderName}, Account Balance - {walletDetails.AccountBalance}, Currency Code - {walletDetails.CurrencyCode}");

                    if (transactionRequest.Direction.Equals(Direction.Credit.ToString(), System.StringComparison.Ordinal))
                    {
                        walletAmountAfterTransaction = walletDetails.AccountBalance + transactionRequest.TransactionAmount;
                    }

                    if (transactionRequest.Direction.Equals(Direction.Debit.ToString(), System.StringComparison.Ordinal))
                    {
                        if (walletDetails.AccountBalance > transactionRequest.TransactionAmount)
                        {
                            walletAmountAfterTransaction = walletDetails.AccountBalance - transactionRequest.TransactionAmount;
                        }
                        else
                        {
                            responseMessage = $"Not enough funds to process the transaction. Account Balance is {walletDetails.CurrencyCode + " " + walletDetails.AccountBalance}";
                            return new BadRequestObjectResult(responseMessage);
                        }
                    }

                    //Creating transaction DB object
                    var transactionDetails = new WalletTransactionRecord
                    {
                        AccountId = transactionRequest.AccountId,
                        TransactionAmount = transactionRequest.TransactionAmount,
                        Direction = transactionRequest.Direction,
                        TransactionTimestamp = DateTime.Now
                    };

                    //Adding Transaction details in DB
                    var transactionId = await _walletService.AddTransactionDetails(transactionDetails);
                    if (transactionId == Guid.Empty)
                    {
                        responseMessage = $"Transaction failed.";
                    }
                    else
                    {
                        responseMessage = $"Transaction is complete with transaction id: {transactionId}";
                        log.LogInformation($"Transaction is complete with transaction id: {transactionId}");

                        //Updating Wallet Balance
                        var rowsUpdated = await _walletService.UpdateWalletAmount(transactionRequest.AccountId, walletAmountAfterTransaction);
                        log.LogInformation($"Wallet amount has been {transactionRequest.Direction}ed by {transactionRequest.TransactionAmount}");
                    }
                }
            }

            else
            {
                responseMessage = "Please provide Account Id to process the transaction";
                return new BadRequestObjectResult(responseMessage);
            }
        }
        catch (Exception ex)
        {
            log.LogError(ex, ex.Message);
            var model = new { error = "Something went wrong. Please try after sometime" };
            return new ObjectResult(model)
            {
                StatusCode = 500
            };
        }

        return new OkObjectResult(responseMessage);
    }
}