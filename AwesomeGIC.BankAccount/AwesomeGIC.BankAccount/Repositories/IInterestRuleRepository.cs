using AwesomeGIC.BankAccount.Models;

namespace AwesomeGIC.BankAccount.Repositories
{
    public interface IInterestRuleRepository
    {
        void AddRule(DateTime date, string ruleId, decimal rate);
        List<InterestRule> GetAllRules();
    }
}
