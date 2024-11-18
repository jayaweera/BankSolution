using AwesomeGIC.BankAccount.Repositories;
using AwesomeGIC.BankAccount.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AwesomeGIC.BankAccount
{
    public class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IAccountRepository, AccountRepository>() 
                .AddSingleton<IInterestRuleRepository, InterestRuleRepository>() 
                .AddSingleton<IAccountService, AccountService>() 
                .BuildServiceProvider();

            var accountkService = serviceProvider.GetService<IAccountService>();

            RunMenu(accountkService);
        }

        private static void RunMenu(IAccountService accountService)
        {
            while (true)
            {
                Console.WriteLine("Welcome to AwesomeGIC Bank! What would you like to do?");
                Console.WriteLine("[T] Input transactions");
                Console.WriteLine("[I] Define interest rules");
                Console.WriteLine("[P] Print statement");
                Console.WriteLine("[Q] Quit");
                Console.Write("> ");

                var input = Console.ReadLine()?.ToUpper();
                if (input == "Q") break;

                try
                {
                    switch (input)
                    {
                        case "T":
                            HandleTransactionInput(accountService);
                            break;
                        case "I":
                            HandleInterestRuleInput(accountService);
                            break;
                        case "P":
                            HandlePrintStatement(accountService);
                            break;
                        default:
                            Console.WriteLine("Invalid option. Try again.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            Console.WriteLine("Thank you for banking with AwesomeGIC Bank.");
            Console.WriteLine("Have a nice day!");
        }

        private static void HandleTransactionInput(IAccountService accountService)
        {
            Console.WriteLine("Please enter transaction details in <Date> <Account> <Type> <Amount> format (or press Enter to go back):");
            while (true)
            {
                Console.Write("> ");
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input)) break;

                var parts = input.Split(' ');
                if (parts.Length != 4)
                {
                    Console.WriteLine("Invalid format. Try again.");
                    continue;
                }

                if (DateTime.TryParseExact(parts[0], "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out var date) &&
                    decimal.TryParse(parts[3], out var amount))
                {
                    try
                    {
                        accountService.AddTransaction(date, parts[1], parts[2], amount);
                        Console.WriteLine("Transaction added successfully.");

                        accountService.PrintTransactions(parts[1]);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid date or amount format. Try again.");
                }
            }
        }

        private static void HandleInterestRuleInput(IAccountService accountService)
        {
            Console.WriteLine("Please enter interest rule details in <Date> <RuleId> <Rate in %> format (or press Enter to go back):");
            while (true)
            {
                Console.Write("> ");
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input)) break;

                var parts = input.Split(' ');
                if (parts.Length != 3)
                {
                    Console.WriteLine("Invalid format. Try again.");
                    continue;
                }

                if (DateTime.TryParseExact(parts[0], "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out var date) &&
                    decimal.TryParse(parts[2], out var rate))
                {
                    try
                    {
                        accountService.AddInterestRule(date, parts[1], rate);
                        Console.WriteLine("Interest rule added successfully.");

                        accountService.PrintInterestRules();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid date or rate format. Try again.");
                }
            }
        }

        private static void HandlePrintStatement(IAccountService accountService)
        {
            Console.WriteLine("Please enter account and month to generate the statement <Account> <Year><Month> (or press Enter to go back):");
            Console.Write("> ");
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input)) return;

            var parts = input.Split(' ');
            if (parts.Length != 2 || !DateTime.TryParseExact(parts[1] + "01", "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out var month))
            {
                Console.WriteLine("Invalid format. Try again.");
                return;
            }

            try
            {
                accountService.PrintStatement(parts[0], month);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
