using MyATMaApp.Domain;
using MyATMaApp.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Deployment.Internal;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyATMaApp.UI
{
    public class AppScreen
    {
        internal const string cur = "N ";
        internal static void Welcome()
        {
            //clear the console screen
            Console.Clear();
            //Set title of the console
            Console.Title = "My ATM App";
            Console.ForegroundColor= ConsoleColor.White;


            //set the welcome message
            Console.WriteLine("----------------Welcome to my ATM App----------------");
            //ínsert atm card
            Console.WriteLine("Please insert ur ATM card");
            Console.WriteLine("Note: Actual atm machine will accept and validate a physical atm card, read the card number and validate it. ");
            Utility.PressEnterToContinue();
        }
        internal static UserAccount UserLoginForm()
        {
            UserAccount temp = new UserAccount();
            temp.CardNumber = Validator.Convert<long>("Your card number: ");
            temp.CardPin = Convert.ToInt32(Utility.GetSecretInput("Enter your card PIN: "));
            return temp;
        }
        internal static void LoginProgess()
        {
            Console.WriteLine("\nChecking card number and PIN ....");
            Utility.PrintDotAnimation(10);
        }
        internal static void PrintLockSceen()
        {
            Console.Clear();
            Utility.PrintMessage("Your Account is locked, please go to the nearest branch to unlock your account. Thanks", true);
            Utility.PressEnterToContinue();
            Environment.Exit(1);
        }
        internal static void WelcomeCustomer(string fullName)
        {
            Console.WriteLine($"Welcome back, {fullName}");
        }
        internal static void DisplayAppMenu()
        {
            Console.Clear();
            Console.WriteLine("-------------My ATM APP MENU------------");
            Console.WriteLine("1. Account Balance                     :");
            Console.WriteLine("2. Cash Deposit                        :");
            Console.WriteLine("3. Withdrawl                           :");
            Console.WriteLine("4. Transfer                            :");
            Console.WriteLine("5. Transactions                        :");
            Console.WriteLine("6. Logout                              :");
        }

        internal static void LogOutProgress()
        {
            Console.WriteLine("Thanks for using My ATM APP.");
            Utility.PrintDotAnimation(10);
            Console.Clear();
        }
        internal static int SelectedAmount()
        {
            Console.WriteLine("");
            Console.WriteLine(":1.{0}500     5.{0}10000 ", cur);
            Console.WriteLine(":2.{0}1000    6.{0}15000", cur);
            Console.WriteLine(":3.{0}2000    7.{0}20000", cur);
            Console.WriteLine(":4.{0}5000    8.{0}40000", cur);
            Console.WriteLine(":0. Other ", cur);

            int selectedAmount = Validator.Convert<int>("option: ");
            switch (selectedAmount)
            {
                case 1:
                    return 500;
                    break;
                case 2:
                    return 1000;
                    break;
                case 3:
                    return 2000;
                    break;
                case 4:
                    return 5000;
                    break;
                case 5:
                    return 10000;
                    break;
                case 6:
                    return 15000;
                    break;
                case 7:
                    return 20000;
                    break;
                case 8:
                    return 40000;
                    break;
                case 0:
                    return 0;
                    break;
                default:
                    Utility.PrintMessage("Invalid Input. Try Again!",false);
                    SelectedAmount();
                    return -1;
                    break;

            }
        
        
        }
        internal InternalTransfer InternalTransferFrom()
        {
            var internalTransfer = new InternalTransfer();
            internalTransfer.ReceiepeintAccountNumber = Validator.Convert<long>("Recipient's account number: ");
            internalTransfer.TransferAmount = Validator.Convert<decimal>($"{cur}");
            internalTransfer.ReceiepeintAccountName =Utility.GetUserInput("recipient's name: ");
            return internalTransfer;
        } 


    }
}
