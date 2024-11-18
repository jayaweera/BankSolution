using AwesomeGIC.BankAccount.Models;
using Transaction = AwesomeGIC.BankAccount.Models.Transaction;

namespace AwesomeGIC.BankAccount.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly Dictionary<string, List<Transaction>> _accountTransactions = new();

        public void AddTransaction(DateTime date, string account, string type, decimal amount)
        {
            if (!_accountTransactions.ContainsKey(account))
            {
                _accountTransactions[account] = new List<Transaction>();
            }

            var transactions = _accountTransactions[account];
            var transactionId = GenerateTransactionId(date, account);
            var previousBalance = transactions.LastOrDefault()?.Balance ?? 0m;
            var newBalance = type.ToUpper() == "D" ? previousBalance + amount : previousBalance - amount;

            if (newBalance < 0)
                throw new InvalidOperationException("Insufficient balance for withdrawal.");

            var transaction = new Transaction
            {
                Date = date,
                Account = account,
                Type = type.ToUpper(),
                Amount = amount,
                Balance = newBalance,
                Id = transactionId
            };

            transactions.Add(transaction);
        }

        public List<Transaction> GetTransactions(string account)
        {
            if (!_accountTransactions.ContainsKey(account))
                throw new InvalidOperationException($"Account '{account}' does not exist.");

            return _accountTransactions[account].OrderBy(txn => txn.Date).ThenBy(txn => txn.Id).ToList();
        }

        public string GenerateStatement(string account, DateTime? month, List<InterestRule> interestRules)
        {
            var transactions = GetTransactions(account);
            if (month.HasValue)
            {
                transactions = transactions.Where(t => t.Date.Year == month.Value.Year && t.Date.Month == month.Value.Month).ToList();
            }

            var statement = $"Account: {account}\n| Date     | Txn Id      | Type | Amount | Balance |\n";
            foreach (var txn in transactions)
            {
                statement += $"| {txn.Date:yyyyMMdd} | {txn.Id} | {txn.Type}    | {txn.Amount:F2} | {txn.Balance:F2} |\n";
            }

            return statement;
        }

        private string GenerateTransactionId(DateTime date, string account)
        {
            var transactions = _accountTransactions.ContainsKey(account) ? _accountTransactions[account] : new List<Transaction>();
            var count = transactions.Count(txn => txn.Date == date) + 1;
            return $"{date:yyyyMMdd}-{count:00}";
        }
    }
}
