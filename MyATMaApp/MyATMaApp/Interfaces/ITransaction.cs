using MyATMaApp.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyATMaApp.Interfaces
{
    public interface ITransaction
    {
        void InsertTransaction(long _userBankAccountId, TransactionType _transType,decimal _tranAmount, string _desc );
        void ViewTransaction();
    }
}
