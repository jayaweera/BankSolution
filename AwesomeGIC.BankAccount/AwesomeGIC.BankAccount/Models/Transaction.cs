namespace AwesomeGIC.BankAccount.Models
{
    public class Transaction
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public string Account { get; set; }
        public string Type { get; set; } 
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }

    }
}
