using KRTBank.Domain.Events;

namespace KRTBank.Domain.Entities
{
    public enum AccountStatus
    {
        Active,
        Inactive
    }

    public class Account
    {
        private readonly List<object> _domainEvents = new List<object>();

        public Guid Id { get; private set; }
        public string HolderName { get; private set; }
        public string CPF { get; private set; }
        public AccountStatus Status { get; private set; }
        public IReadOnlyCollection<object> DomainEvents => _domainEvents.AsReadOnly();

        public Account(string holderName, string CPF, AccountStatus status = AccountStatus.Active)
        {
            Id = Guid.NewGuid();
            HolderName = holderName ?? throw new ArgumentNullException(nameof(holderName));
            this.CPF = ValidateCPF(CPF) ? CPF : throw new ArgumentException("CPF Invalido", nameof(CPF));
            Status = status;
            _domainEvents.Add(new AccountCreatedEvent(Id, HolderName, this.CPF, Status.ToString()));
        }

        public void Update(string holderName, AccountStatus status)
        {
            HolderName = holderName ?? throw new ArgumentNullException(nameof(holderName));
            Status = status;
            _domainEvents.Add(new AccountUpdatedEvent(Id, HolderName, Status.ToString()));
        }

        public void Deactivate()
        {
            Status = AccountStatus.Inactive;
            _domainEvents.Add(new AccountUpdatedEvent(Id, HolderName, Status.ToString()));
        }

        public void Delete()
        {
            _domainEvents.Add(new AccountDeletedEvent(Id));
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        private static bool ValidateCPF(string cpf)
        {
            //Valida apenas o tamanho de 11 digitos.
            return !string.IsNullOrWhiteSpace(cpf) && cpf.Length == 11 && cpf.All(char.IsDigit);
        }
    }

}

