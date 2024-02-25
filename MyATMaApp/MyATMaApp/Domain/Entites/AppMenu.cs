using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyATMaApp.Domain.Entites
{
    internal enum AppMenu
    {
        CheckBalance = 1, //0
        PlaceDeposit = 2, //1
        MakeWithdrwal = 3, //2
        InternalTransfer= 4, //3
        ViewTransaction = 5, //4
        Logout = 6//5
    }
}
