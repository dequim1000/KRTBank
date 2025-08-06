using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRTBank.Domain.Events;

public class AccountDeletedEvent : INotification
{
    public Guid Id { get; }

    public AccountDeletedEvent(Guid id)
    {
        Id = id;
    }
}
