using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyATMaApp.Domain.Entites
{
    public class InternalTransfer
    {
        public decimal TransferAmount { get; set; }
        public long ReceiepeintAccountNumber { get; set; }
        public string ReceiepeintAccountName { get; set; }
    }
}
