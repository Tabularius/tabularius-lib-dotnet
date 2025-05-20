
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace TabulariusLib.Entities
{
    [Table("Balances")]
    public sealed record Balance
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; init; }

        [Required]
        [MaxLength(256)]
        public string Name { get; init; }

        [Required]
        [MaxLength(256)]
        public string Description { get; init; }

        [Required]
        public DateTime Date { get; init; }

        private List<BalanceEntry> _balanceEntries { get; init; } = new();
        public IReadOnlyCollection<BalanceEntry> BalanceEntries => _balanceEntries.AsReadOnly();

        public decimal TotalAssets => _balanceEntries
            .Where(entry => entry.Type == AccountType.Asset)
            .Sum(entry => entry.Balance);
        public decimal TotalLiabilities => _balanceEntries
            .Where(entry => entry.Type == AccountType.Liability)
            .Sum(entry => entry.Balance);
        public decimal TotalEquity => _balanceEntries
            .Where(entry => entry.Type == AccountType.Equity)
            .Sum(entry => entry.Balance);
        public decimal BalanceAmount => TotalAssets - (TotalLiabilities + TotalEquity);
        public bool IsBalanced => BalanceAmount == 0;

        private Balance()
        { 
            Name = string.Empty;
            Description = string.Empty;
            Date = default;
        }

        private Balance(Guid id, string name, string description, DateTime date, IEnumerable<BalanceEntry> entries)
        {
            if (id == Guid.Empty)
                throw new ArgumentException($"'{nameof(id)}' cannot be empty.", nameof(id));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException($"'{nameof(description)}' cannot be null or empty.", nameof(description));
            if (date == default)
                throw new ArgumentException($"'{nameof(date)}' cannot be default.", nameof(date));
            if (entries == null)
                throw new ArgumentNullException(nameof(entries));

            Id = id;
            Name = name;
            Description = description;
            Date = date;
            _balanceEntries = entries.ToList();
        }

        public static Balance Create(Guid id, string name, string description, DateTime date, IEnumerable<BalanceEntry> entries)
            => new(id, name, description, date, entries);

        // Factory method to create a Balance from a TrialBalance
        public static Balance FromTrialBalance(string name, string description, DateTime date, TrialBalance trialBalance)
        {
            if (trialBalance == null)
                throw new ArgumentNullException(nameof(trialBalance));
            if (!trialBalance.IsClosed)
                throw new InvalidOperationException($"Trial balance is not closed at {date:yyyy-MM-dd}.");
            if (!trialBalance.IsBalanced)
                    throw new InvalidOperationException($"Trial balance is not balanced at {date:yyyy-MM-dd}.");
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException($"'{nameof(description)}' cannot be null or empty.", nameof(description));
            if (date == default)
                throw new ArgumentException($"'{nameof(date)}' cannot be default.", nameof(date));

            var balanceEntries = trialBalance.TrialBalanceEntries
                .Where(entry => entry.Type == AccountType.Asset 
                             || entry.Type == AccountType.Liability 
                             || entry.Type == AccountType.Equity)
                .Select(entry => 
                    BalanceEntry.Create(
                        entry.AccountID,
                        entry.AccountName,
                        entry.Type,
                        entry.ParentCode,
                        entry.Normally == "Credit"
                            ? entry.Credit - entry.Debit
                            : entry.Debit - entry.Credit,
                        entry.Normally))
                .ToList();

            return new Balance(Guid.NewGuid(), name, description, date, balanceEntries);
        }
    }

    [Table("BalanceEntries")]
    public sealed record BalanceEntry
    {
        [Key]
        [MaxLength(256)]
        public string AccountID { get; init; }

        [Required]
        [MaxLength(256)]
        public string AccountName { get; init; }

        [Required]
        public AccountType Type { get; init; }

        [MaxLength(256)]
        public string? ParentCode { get; init; }

        public decimal Balance { get; init; }

        [Required]
        [MaxLength(256)]
        public string Normally { get; init; }

        private BalanceEntry()
        {
            AccountID = string.Empty;
            AccountName = string.Empty;
            Type = AccountType.Asset;
            ParentCode = null; 
            Normally = string.Empty;
        }

        private BalanceEntry(string accountID, string accountName, AccountType type, string? parentCode, decimal balance, string normally)
        {
            if (string.IsNullOrWhiteSpace(normally))
                throw new ArgumentException($"'{nameof(normally)}' cannot be null or empty.", nameof(normally));
            if (string.IsNullOrWhiteSpace(accountID))
                throw new ArgumentException($"'{nameof(accountID)}' cannot be null or empty.", nameof(accountID));
            if (string.IsNullOrWhiteSpace(accountName))
                throw new ArgumentException($"'{nameof(accountName)}' cannot be null or empty.", nameof(accountName));
            if (balance < 0)
                throw new ArgumentOutOfRangeException(nameof(balance), "Balance cannot be negative.");
            AccountID = accountID;
            AccountName = accountName;
            Type = type;
            ParentCode = string.IsNullOrWhiteSpace(parentCode) ? null : parentCode;
            Balance = balance;
            Normally = normally;
        }

        public static BalanceEntry Create(string accountID, string accountName, AccountType type, string? parentCode, decimal balance, string normally)
            => new(accountID, accountName, type, parentCode, balance, normally);
    }
}
