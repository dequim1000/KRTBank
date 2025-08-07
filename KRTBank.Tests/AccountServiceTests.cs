using KRTBank.Application.Services;
using KRTBank.Domain.Entities;
using KRTBank.Domain.Events;
using KRTBank.Domain.Interfaces;
using KRTBank.Infrastructure.Services;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace KRTBank.Tests;

public class AccountServiceTests
{
    private readonly Mock<IAccountRepository> _accountRepositoryMock;

    private readonly Mock<FraudPreventionService> _fraudPreventionServiceMock;
    private readonly Mock<CreditCardService> _creditCardServiceMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly AccountService _accountService;

    public AccountServiceTests()
    {
        _accountRepositoryMock = new Mock<IAccountRepository>();
        _fraudPreventionServiceMock = new Mock<FraudPreventionService>(new NullLogger<FraudPreventionService>());
        _creditCardServiceMock = new Mock<CreditCardService>(new NullLogger<CreditCardService>());
        _mediatorMock = new Mock<IMediator>();
        _accountService = new AccountService(
            _accountRepositoryMock.Object,
            _fraudPreventionServiceMock.Object,
            _creditCardServiceMock.Object,
            _mediatorMock.Object);
    }

    [Fact]
    public async Task CreateAccountAsync_ValidInput_CreatesAccount()
    {
        string holderName = "Andre Otavio";
        string cpf = "12332112322";
        _fraudPreventionServiceMock.Setup(s => s.ValidateAccount(It.IsAny<Account>())).Returns(true);
        _creditCardServiceMock.Setup(s => s.IssueCreditCard(It.IsAny<Account>())).Returns(true);

        var account = await _accountService.CreateAccountAsync(holderName, cpf);

        Assert.NotNull(account);
        Assert.Equal(holderName, account.HolderName);
        Assert.Equal(cpf, account.CPF);
        Assert.Equal(AccountStatus.Active, account.Status);
        _accountRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Account>()), Times.Once());
        _mediatorMock.Verify(m => m.Publish(It.IsAny<AccountCreatedEvent>(), default), Times.Once());
        _creditCardServiceMock.Verify(s => s.IssueCreditCard(It.IsAny<Account>()), Times.Once());
    }

    [Fact]
    public async Task CreateAccountAsync_InvalidFraudValidation_ThrowsException()
    {
        string holderName = "Andre Otavio";
        string cpf = "12332112322";
        _fraudPreventionServiceMock.Setup(s => s.ValidateAccount(It.IsAny<Account>())).Returns(false);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _accountService.CreateAccountAsync(holderName, cpf));
        _accountRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Account>()), Times.Never());
    }

    [Fact]
    public async Task CreateAccountAsync_FailedCreditCardIssue_ThrowsException()
    {
        string holderName = "Andre Otavio";
        string cpf = "12332112322";
        _fraudPreventionServiceMock.Setup(s => s.ValidateAccount(It.IsAny<Account>())).Returns(true);
        _creditCardServiceMock.Setup(s => s.IssueCreditCard(It.IsAny<Account>())).Returns(false);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _accountService.CreateAccountAsync(holderName, cpf));
        _accountRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Account>()), Times.Once());
        _mediatorMock.Verify(m => m.Publish(It.IsAny<AccountCreatedEvent>(), default), Times.Once());
    }

    [Fact]
    public async Task UpdateAccountAsync_ValidInput_UpdatesAccount()
    {
        var account = new Account("Jose Maria", "12332112322");
        _accountRepositoryMock.Setup(r => r.GetByIdAsync(account.Id)).ReturnsAsync(account);
        string newHolderName = "Jose Maria";
        string newCPF = "49878998850";
        var newStatus = AccountStatus.Inactive;

        var updatedAccount = await _accountService.UpdateAccountAsync(account.Id, newHolderName, newStatus);

        Assert.Equal(newHolderName, updatedAccount.HolderName);
        Assert.Equal(newStatus, updatedAccount.Status);
        _accountRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Account>()), Times.Once());
        _mediatorMock.Verify(m => m.Publish(It.IsAny<AccountUpdatedEvent>(), default), Times.Once());
    }

    [Fact]
    public async Task UpdateAccountAsync_AccountNotFound_ThrowsException()
    {
        _accountRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Account)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _accountService.UpdateAccountAsync(Guid.NewGuid(), "Otavio Andre", AccountStatus.Active));
    }

    [Fact]
    public async Task DeleteAccountAsync_ValidId_DeletesAccount()
    {
        var account = new Account("Andre Otavio", "12332112322");
        _accountRepositoryMock.Setup(r => r.GetByIdAsync(account.Id)).ReturnsAsync(account);

        await _accountService.DeleteAccountAsync(account.Id);

        _accountRepositoryMock.Verify(r => r.DeleteAsync(account.Id), Times.Once());
        _mediatorMock.Verify(m => m.Publish(It.IsAny<AccountDeletedEvent>(), default), Times.Once());
    }

    [Fact]
    public async Task DeleteAccountAsync_AccountNotFound_ThrowsException()
    {
        _accountRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Account)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _accountService.DeleteAccountAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetAccountByIdAsync_ValidId_ReturnsAccount()
    {
        var account = new Account("Andre Otavio", "12332112322");
        _accountRepositoryMock.Setup(r => r.GetByIdAsync(account.Id)).ReturnsAsync(account);

        var result = await _accountService.GetAccountByIdAsync(account.Id);

        Assert.NotNull(result);
        Assert.Equal(account.Id, result.Id);
    }

    [Fact]
    public async Task GetAccountByIdAsync_AccountNotFound_ThrowsException()
    {
        _accountRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Account)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _accountService.GetAccountByIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetAllAccountsAsync_ReturnsAccounts()
    {
        var accounts = new List<Account>
        {
            new Account("Andre Otavio", "12345678901"),
            new Account("Maria", "98765432109"),
            new Account("Joao", "98765432109"),
            new Account("Japones", "12332112332"),
            new Account("Kaique", "98778956722"),
        };
        _accountRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(accounts);

        var result = await _accountService.GetAllAccountsAsync();

        Assert.Equal(2, result.Count());
        Assert.Contains(result, a => a.HolderName == "Andre Otavio");
        Assert.Contains(result, a => a.HolderName == "Maria");
        Assert.Contains(result, a => a.HolderName == "Joao");
        Assert.Contains(result, a => a.HolderName == "Japones");
        Assert.Contains(result, a => a.HolderName == "Kaique");
    }
}
