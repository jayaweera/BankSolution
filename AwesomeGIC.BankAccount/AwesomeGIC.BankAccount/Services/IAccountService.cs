namespace AwesomeGIC.BankAccount.Services
{
    public interface IAccountService
    {
        void AddTransaction(DateTime date, string account, string type, decimal amount);
        void AddInterestRule(DateTime date, string ruleId, decimal rate);
        void PrintTransactions(string account);
        void PrintStatement(string account, DateTime month);
        void PrintInterestRules();
    }
}
