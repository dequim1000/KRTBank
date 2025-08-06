using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRTBank.Domain.Events;

public class AccountCreatedEvent : INotification
{
    public Guid Id { get; }
    public string HolderName { get; }
    public string CPF { get; }
    public string Status { get; }

    public AccountCreatedEvent(Guid id, string holderName, string cpf, string status)
    {
        Id = id;
        HolderName = holderName;
        CPF = cpf;
        Status = status;
    }
}
