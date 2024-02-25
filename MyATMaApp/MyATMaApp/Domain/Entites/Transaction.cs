using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyATMaApp.Domain.Entites
{
    public class Transaction
    {
        public long TransactionID { get; set; }
        public long UserBankAccountID { get; set; }
        public DateTime TransactionDate { get; set; }  
        public TransactionType TransactionType { get; set; }
        public string Description { get; set; }
        public decimal TransactionAmount { get; set; }
        
    }
}
