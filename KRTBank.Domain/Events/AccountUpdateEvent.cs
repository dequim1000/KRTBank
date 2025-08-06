using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRTBank.Domain.Events;

public class AccountUpdatedEvent : INotification
{
    public Guid Id { get; }
    public string HolderName { get; }
    public string Status { get; }

    public AccountUpdatedEvent(Guid id, string holderName, string status)
    {
        Id = id;
        HolderName = holderName;
        Status = status;
    }
}
