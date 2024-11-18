using AwesomeGIC.BankAccount.Models;

namespace AwesomeGIC.BankAccount.Repositories
{
    public interface IAccountRepository
    {
        void AddTransaction(DateTime date, string account, string type, decimal amount);
        List<Transaction> GetTransactions(string account);
        string GenerateStatement(string account, DateTime? month, List<InterestRule> interestRules);
    }
}
