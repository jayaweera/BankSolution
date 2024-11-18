using AwesomeGIC.BankAccount.Models;

namespace AwesomeGIC.BankAccount.Repositories
{
    public class InterestRuleRepository : IInterestRuleRepository
    {
        private readonly List<InterestRule> _interestRules = new();

        public void AddRule(DateTime date, string ruleId, decimal rate)
        {
            if (rate <= 0 || rate >= 100)
                throw new ArgumentException("Interest rate must be greater than 0 and less than 100.");

            _interestRules.RemoveAll(rule => rule.Date == date);
            _interestRules.Add(new InterestRule { Date = date, RuleId = ruleId, Rate = rate });
        }

        public List<InterestRule> GetAllRules()
        {
            return _interestRules.OrderBy(rule => rule.Date).ToList();
        }

    }
}
