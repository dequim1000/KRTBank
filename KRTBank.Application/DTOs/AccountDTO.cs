using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KRTBank.Application.DTOs
{
    public class AccountDTO
    {
        public Guid Id { get; set; }
        public string HolderName { get; set; }
        public string CPF { get; set; }
        public string Status { get; set; }
    }
}
