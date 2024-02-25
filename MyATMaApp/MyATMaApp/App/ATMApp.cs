using ConsoleTables;
using MyATMaApp.Domain;
using MyATMaApp.Domain.Entites;
using MyATMaApp.Interfaces;
using MyATMaApp.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyATMaApp
{
    internal class ATMApp:IUserLogin,IUserAccountActions,ITransaction
    {
        private List<UserAccount> userAccountList;
        private UserAccount selectedAccount;
        private List<Transaction> _listOfTransactions;
        private const decimal minimumKeptAmount = 500;
        private readonly AppScreen screen;
        
        public ATMApp()
        {
            screen = new AppScreen();
        }
        public void Run()
        {
            AppScreen.Welcome();
            CheckUserCardNumAndPassword();
            AppScreen.WelcomeCustomer(selectedAccount.FullName);
            while (true)
            {
                AppScreen.DisplayAppMenu();
                ProcessMenuOption();
            }
            
        }
        public void InitializeData()
        {
            userAccountList = new List<UserAccount>
            {
                new UserAccount{Id = 1, FullName = "BUT01", AccountNumber= 11111, CardNumber=12345, CardPin = 123456,AccountBalance = 100000.00m, IsLocked = false},
                new UserAccount{Id = 2, FullName = "BUT02", AccountNumber= 123456, CardNumber=123456, CardPin = 123456,AccountBalance = 100000.00m, IsLocked = false} , 
                new UserAccount{Id = 3, FullName = "BUT03", AccountNumber= 11113, CardNumber=123457, CardPin = 123456,AccountBalance = 100000.00m, IsLocked = false},
                new UserAccount{Id = 4, FullName = "BUT04", AccountNumber= 11114, CardNumber=123458, CardPin = 123456,AccountBalance = 100000.00m, IsLocked = true}
            };
            _listOfTransactions = new List<Transaction>();
        }
        public void CheckUserCardNumAndPassword()
        {
            bool isCorrectLogin = false;
            while (!isCorrectLogin)
            {
                UserAccount inputAccount = AppScreen.UserLoginForm();
                AppScreen.LoginProgess();
                foreach(UserAccount account in userAccountList) {
                    selectedAccount = account;
                    if (inputAccount.CardNumber.Equals(selectedAccount.CardNumber))
                    {
                        selectedAccount.TotalLogin++;
                        if(inputAccount.CardPin.Equals(selectedAccount.CardPin))
                        {
                            selectedAccount = account;
                            if(selectedAccount.IsLocked || selectedAccount.TotalLogin > 3)
                            {
                                //print a lock message
                                AppScreen.PrintLockSceen();
                            }
                            else
                            {
                                selectedAccount.TotalLogin = 0;
                                isCorrectLogin = true;
                                break;
                            }
                        }
                    }
                   
                }
                if (!isCorrectLogin)
                {
                    Utility.PrintMessage("\n Invalid cardNumber or PIN", false);
                    selectedAccount.IsLocked = selectedAccount.TotalLogin== 3;
                    if (selectedAccount.IsLocked)
                    {
                        AppScreen.PrintLockSceen();
                    }
                }
                Console.Clear();

            }
            
        }
        private void ProcessMenuOption()
        {
            switch(Validator.Convert<int>("An option: "))
            {
                case (int)AppMenu.CheckBalance:
                    CheckBalance();
                    break;
                case (int)AppMenu.PlaceDeposit:
                    PlaceDeposit();
                    break;
                case (int)AppMenu.MakeWithdrwal:
                    MakeWithdrawl();
                    break;
                case (int)AppMenu.InternalTransfer:
                    var internalTransfer = screen.InternalTransferFrom();
                    ProcessInternalTransfer(internalTransfer); 
                    break;
                case (int)AppMenu.ViewTransaction:
                    ViewTransaction();
                    break;
                case (int)AppMenu.Logout:
                    AppScreen.LogOutProgress();
                    Utility.PrintMessage("You have successfully logged out, Please collect your ATM card.");
                    Run();
                    break;
                default:
                    Utility.PrintMessage("Invalid Option.",false);
                    break;
            }
        }

        public void CheckBalance()
        {
            Utility.PrintMessage($"Your account balance is: {Utility.FormatAmount(selectedAccount.AccountBalance)}");
        }

        public void PlaceDeposit()
        {
            Console.WriteLine("\nOnly multiples of 500 and 1000 allowed.\n");
            var transaction_amt = Validator.Convert<int>($"amount {AppScreen.cur}");
            //simulate counting
            Console.WriteLine("\nChecking and Counting bank nots.");
            Utility.PrintDotAnimation(10);
            Console.WriteLine("");
            

            // some gaurd clause
            if(transaction_amt <= 0)
            {
                Utility.PrintMessage("Ammount needs to be greater than zero. Try again", false);
                return;
            }
            if(transaction_amt%500!= 0)
            {
                Utility.PrintMessage($"Enter deposit amount in multiple of 500 or 1000. Try again.");
                return;
            }
            if(PreviewBankNotesCount(transaction_amt) == false)
            {
                Utility.PrintMessage($"You have canclled your action.", false);
                return;
            }
            //bind transaction details to transaction object
            InsertTransaction(selectedAccount.Id, TransactionType.Deposit, transaction_amt, "trans");


            //Update account balance
            selectedAccount.AccountBalance += transaction_amt;
            //print success message

            Utility.PrintMessage($"Your deposit of {Utility.FormatAmount(transaction_amt)} was successful. ", true);

        }

        public void MakeWithdrawl()
        {
            var transaction_amt = 0;
            int selectedAmount = AppScreen.SelectedAmount();
            if(selectedAmount == -1)
            {
                selectedAmount = AppScreen.SelectedAmount();
            }
            else if (selectedAmount != 0) {
                transaction_amt = selectedAmount;

            }else
            {
                transaction_amt = Validator.Convert<int>($"amount {AppScreen.cur}");
            }

            // input validation
            if (transaction_amt <= 0) {
                Utility.PrintMessage("Amount needs to be greater than zero!. Try again");
                return; 
            }
            if(transaction_amt% 500!= 0)
            {
                Utility.PrintMessage("You can only withdraw amount in multiple of 500 or 1000 vnd. Try again");
                return;
            }
            //Bussiness logic validations
            if (transaction_amt > selectedAccount.AccountBalance)
            {
                Utility.PrintMessage($"Withdrawl failed. Your balance is too low to withdrawl " +
                    $"{Utility.FormatAmount(transaction_amt)}",false);
                return;
            }
            if(selectedAccount.AccountBalance - transaction_amt < minimumKeptAmount)
            {
                Utility.PrintMessage($"Withdrwal failed. Your account needs to have  minimum {Utility.FormatAmount(minimumKeptAmount)}", false);
                return;
            }
            //bind withdrwal details to transaction object
            InsertTransaction(selectedAccount.Id, TransactionType.Withdrawl,-transaction_amt,"");
            //update accout balance
            selectedAccount.AccountBalance-= transaction_amt;
            //success message
            Utility.PrintMessage($"You have successfully withdrwal {Utility.FormatAmount(transaction_amt)}",true);
        
        }
        private bool PreviewBankNotesCount(int amount)
        {
            int thousandNotesCount = amount/1000;
            int fiveHunderedNotesCount = (amount%1000) / 500;
            Console.WriteLine("\nSummary");
            Console.WriteLine("----------");
            Console.WriteLine($"{AppScreen.cur}1000 x {thousandNotesCount} = {1000 * thousandNotesCount}");
            Console.WriteLine($"{AppScreen.cur}500 x {fiveHunderedNotesCount} = {500*fiveHunderedNotesCount}");
            Console.WriteLine($"Total amount: {Utility.FormatAmount(amount)}\n\n");
            int opt = Validator.Convert<int>("1 to confirm");
                return opt.Equals(1);
        }

        
        public void ViewTransaction()
        {
            var filteredTransactionList = _listOfTransactions.Where(x => x.UserBankAccountID == selectedAccount.Id).ToList();
            if(filteredTransactionList.Count <= 0)
            {
                Utility.PrintMessage("You have no transaction yet", true);
            }
            else {
                var table = new ConsoleTable("Id","Transaction Date","Type", "Amount" + AppScreen.cur, "Descriptions");
                foreach(var tran in filteredTransactionList)
                {
                    table.AddRow(tran.TransactionID, tran.TransactionDate, tran.TransactionType, tran.TransactionAmount, tran.Description);
                }
                table.Options.EnableCount = false;
                table.Write();
                Utility.PrintMessage($"You have {filteredTransactionList.Count} transaction(s)", true);

            }
        }
        private void ProcessInternalTransfer(InternalTransfer inter)
        {
            if (inter.TransferAmount <= 0)
            {
                Utility.PrintMessage("Amount needs to nr more than zero! Try again.");
                return;
            }
            if(inter.TransferAmount > selectedAccount.AccountBalance)
            {
                Utility.PrintMessage($"Transfer failed, You do not have enough balace to transfer {Utility.FormatAmount(inter.TransferAmount)}");
                return;
            }
            //check the minimum kept amount
            if(selectedAccount.AccountBalance - minimumKeptAmount <  minimumKeptAmount)
            {
                Utility.PrintMessage($"Transfer failed. Your account needs to have minimum {Utility.FormatAmount(minimumKeptAmount)} ", false);
                return;
            }
            //check reciever 's account number is valid;
            UserAccount selectedBankAccountReciever = userAccountList.FirstOrDefault(x => x.AccountNumber == inter.ReceiepeintAccountNumber);
            if (selectedBankAccountReciever == null)
            {
                Utility.PrintMessage($"transfer failed. Receiver back account number is invalid", false);
                return;
            }
            //check receiver's name 
            if(selectedBankAccountReciever.FullName != inter.ReceiepeintAccountName)
            {
                Utility.PrintMessage("Transfer Failed. Recipient's bank account name is not match",false);
                return;
            }
            //add transaction to transaction record sender
            InsertTransaction(selectedAccount.Id,TransactionType.Transfer,inter.TransferAmount,$"Transfered to {selectedBankAccountReciever.AccountNumber} ({selectedBankAccountReciever.FullName}) " );

            //update sender account balance
            selectedAccount.AccountBalance -= inter.TransferAmount;

            //add trans to reciever 
            InsertTransaction(selectedBankAccountReciever.Id, TransactionType.Transfer, inter.TransferAmount, $"Transfered from {selectedAccount.AccountNumber}({selectedAccount.FullName} ");
            //update reciever account balance
            selectedBankAccountReciever.AccountBalance += inter.TransferAmount;
            Utility.PrintMessage($"You have successfully transfered {Utility.FormatAmount(inter.TransferAmount)} to {inter.ReceiepeintAccountNumber}", true);

        }

        public void InsertTransaction(long _userBankAccountId, TransactionType _transType, decimal _tranAmount, string _desc)
        {
            //Create a new transaction object
            var transaction = new Transaction()
            {
                TransactionID = Utility.getTransactionId(),
                UserBankAccountID = _userBankAccountId,
                TransactionDate = DateTime.Now,
                TransactionType = _transType,
                TransactionAmount = _tranAmount,
                Description= _desc
            };
            //add transaction 
            _listOfTransactions.Add(transaction);
        }
        
    }
}
