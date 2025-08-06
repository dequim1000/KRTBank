using KRTBank.Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRTBank.Infrastructure.Services;

public class FraudPreventionService
{
    private readonly ILogger<FraudPreventionService> _logger;

    public FraudPreventionService(ILogger<FraudPreventionService> logger)
    {
        _logger = logger;
    }

    public bool ValidateAccount(Account account)
    {
        _logger.LogInformation($"Fraud Prevention Service: Validating account - ID: {account.Id}, Holder: {account.HolderName}, CPF: {account.CPF}, Status: {account.Status}");
        // Simulação de validação de fraude
        return true;
    }
}
