﻿namespace AwesomeGIC.BankAccount.Models
{
    public class InterestRule
    {
        public DateTime Date { get; set; }
        public string RuleId { get; set; }
        public decimal Rate { get; set; }
    }
}