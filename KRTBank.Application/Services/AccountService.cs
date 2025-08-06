using KRTBank.Domain.Entities;
using KRTBank.Domain.Interfaces;
using KRTBank.Infrastructure.Services;
using MediatR;

namespace KRTBank.Application.Services
{
    public class AccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly FraudPreventionService _fraudPreventionService;
        private readonly CreditCardService _creditCardService;
        private readonly IMediator _mediator;

        public AccountService(
            IAccountRepository accountRepository,
            FraudPreventionService fraudPreventionService,
            CreditCardService creditCardService,
            IMediator mediator)
        {
            _accountRepository = accountRepository;
            _fraudPreventionService = fraudPreventionService;
            _creditCardService = creditCardService;
            _mediator = mediator;
        }

        public async Task<Account> CreateAccountAsync(string holderName, string CPF)
        {
            var account = new Account(holderName, CPF);
            if (!_fraudPreventionService.ValidateAccount(account))
            {
                throw new InvalidOperationException("Account validation failed.");
            }

            await _accountRepository.AddAsync(account);
            await PublishDomainEvents(account);

            if (!_creditCardService.IssueCreditCard(account))
            {
                throw new InvalidOperationException("Failed to issue credit card.");
            }

            return account;
        }

        public async Task<Account> UpdateAccountAsync(Guid id, string holderName, AccountStatus status)
        {
            var account = await _accountRepository.GetByIdAsync(id);
            if (account == null)
            {
                throw new KeyNotFoundException("Account not found.");
            }

            account.Update(holderName, status);
            await _accountRepository.UpdateAsync(account);
            await PublishDomainEvents(account);
            return account;
        }

        public async Task DeleteAccountAsync(Guid id)
        {
            var account = await _accountRepository.GetByIdAsync(id);
            if (account == null)
            {
                throw new KeyNotFoundException("Account not found.");
            }

            account.Delete();
            await _accountRepository.DeleteAsync(id);
            await PublishDomainEvents(account);
        }

        public async Task<Account> GetAccountByIdAsync(Guid id)
        {
            var account = await _accountRepository.GetByIdAsync(id);
            if (account == null)
            {
                throw new KeyNotFoundException("Account not found.");
            }
            return account;
        }

        public async Task<IEnumerable<Account>> GetAllAccountsAsync()
        {
            return await _accountRepository.GetAllAsync();
        }

        private async Task PublishDomainEvents(Account account)
        {
            foreach (var domainEvent in account.DomainEvents)
            {
                await _mediator.Publish(domainEvent);
            }
            account.ClearDomainEvents();
        }
    }
}
