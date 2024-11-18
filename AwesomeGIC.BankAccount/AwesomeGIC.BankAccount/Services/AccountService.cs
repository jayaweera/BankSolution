using AwesomeGIC.BankAccount.Repositories;

namespace AwesomeGIC.BankAccount.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IInterestRuleRepository _interestRuleRepository;

        public AccountService(IAccountRepository accountRepository, IInterestRuleRepository interestRuleRepository)
        {
            _accountRepository = accountRepository;
            _interestRuleRepository = interestRuleRepository;
        }

        public void AddTransaction(DateTime date, string account, string type, decimal amount)
        {
            _accountRepository.AddTransaction(date, account, type, amount);
        }

        public void AddInterestRule(DateTime date, string ruleId, decimal rate)
        {
            _interestRuleRepository.AddRule(date, ruleId, rate);
        }

        public void PrintInterestRules()
        {
            var interestRules = _interestRuleRepository.GetAllRules();
            if (interestRules.Any())
            {
                Console.WriteLine("\nInterest Rules:");
                Console.WriteLine("| Date     | RuleId | Rate (%) |");
                foreach (var rule in interestRules.OrderBy(r => r.Date))
                {
                    Console.WriteLine($"| {rule.Date:yyyyMMdd} | {rule.RuleId} | {rule.Rate:F2} |");
                }
            }
            else
            {
                Console.WriteLine("No interest rules defined yet.");
            }
        }

        public void PrintTransactions(string account)
        {
            var transactions = _accountRepository.GetTransactions(account)
            .OrderBy(t => t.Date)
            .ToList();

            Console.WriteLine($"Account: {account}");
            Console.WriteLine("| Date     | Txn Id      | Type | Amount | Balance |");

            foreach (var txn in transactions)
            {
                Console.WriteLine($"| {txn.Date:yyyyMMdd} | {txn.Id} | {txn.Type}    | {txn.Amount:F2} | {txn.Balance:F2} |");
            }
        }

        public void PrintStatement(string account, DateTime month)
        {
            var transactions = _accountRepository.GetTransactions(account)
            .Where(t => t.Date.Year == month.Year && t.Date.Month == month.Month)
            .OrderBy(t => t.Date)
            .ToList();

            Console.WriteLine($"Account: {account}");
            Console.WriteLine("| Date     | Txn Id      | Type | Amount | Balance |");

            foreach (var txn in transactions)
            {
                Console.WriteLine($"| {txn.Date:yyyyMMdd} | {txn.Id} | {txn.Type}    | {txn.Amount:F2} | {txn.Balance:F2} |");
            }

            var interest = CalculateInterest(account, month);
            if (interest > 0)
            {
                Console.WriteLine($"| {month:yyyyMMdd} |             | I    | {interest:F2} | {transactions.Last().Balance + interest:F2} |");
            }
        }

        public decimal CalculateInterest(string accountId, DateTime month)
        {
            var interestRules = _interestRuleRepository.GetAllRules();

            var transactions = _accountRepository.GetTransactions(accountId)
                .Where(t => t.Date.Year == month.Year && t.Date.Month == month.Month)
                .OrderBy(t => t.Date)
                .ToList();

            var eodBalances = new List<(DateTime Date, decimal Balance)>();
            foreach (var txn in transactions)
            {
                eodBalances.Add((txn.Date, txn.Balance));
            }

            decimal totalInterest = 0m;
            for (int i = 0; i < interestRules.Count; i++)
            {
                var rule = interestRules[i];
                var nextRuleDate = (i + 1 < interestRules.Count) ? interestRules[i + 1].Date : DateTime.MaxValue;
                var applicableBalances = eodBalances.Where(b => b.Date >= rule.Date && b.Date < nextRuleDate).ToList();

                if (applicableBalances.Any())
                {
                    var periodStart = applicableBalances.First().Date;
                    var periodEnd = applicableBalances.Last().Date;
                    var numDays = (periodEnd - periodStart).Days + 1;
                    var eodBalance = applicableBalances.First().Balance;
                    var interest = (eodBalance * rule.Rate / 100) * numDays;

                    totalInterest += interest;
                }
            }

            return Math.Round(totalInterest / 365, 2);
        }
    }
}
