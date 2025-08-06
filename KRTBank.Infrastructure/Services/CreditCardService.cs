using KRTBank.Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRTBank.Infrastructure.Services;

public class CreditCardService
{
    private readonly ILogger<CreditCardService> _logger;

    public CreditCardService(ILogger<CreditCardService> logger)
    {
        _logger = logger;
    }

    public bool IssueCreditCard(Account account)
    {
        _logger.LogInformation($"Credit Card Service: Issuing credit card - ID: {account.Id}, Holder: {account.HolderName}, CPF: {account.CPF}, Status: {account.Status}");
        // Simulação de emissão de cartão
        return true;
    }
}
