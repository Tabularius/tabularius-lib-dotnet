using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace TabulariusLib.Entities
{
    [Table("ProfitAndLossStatements")]
    public sealed record ProfitAndLossStatement
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
        public DateTime StartDate { get; init; }

        [Required]
        public DateTime EndDate { get; init; }

        private List<ProfitAndLossEntry> _entries { get; init; } = new();
        public IReadOnlyCollection<ProfitAndLossEntry> Entries => _entries.AsReadOnly();

        public decimal TotalRevenue => _entries.Where(e => e.Type == AccountType.Income).Sum(e => e.Amount);
        public decimal TotalExpense => _entries.Where(e => e.Type == AccountType.Expense).Sum(e => e.Amount);
        public decimal NetProfit => TotalRevenue - TotalExpense;

        private ProfitAndLossStatement() { Name = string.Empty; Description = string.Empty; StartDate = default; EndDate = default; }

        private ProfitAndLossStatement(Guid id, string name, string description, DateTime startDate, DateTime endDate, IEnumerable<ProfitAndLossEntry> entries)
        {
            if (id == Guid.Empty)
                throw new ArgumentException($"'{nameof(id)}' cannot be empty.", nameof(id));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException($"'{nameof(description)}' cannot be null or empty.", nameof(description));
            if (startDate == default)
                throw new ArgumentException($"'{nameof(startDate)}' cannot be default.", nameof(startDate));
            if (endDate == default)
                throw new ArgumentException($"'{nameof(endDate)}' cannot be default.", nameof(endDate));
            if (entries == null)
                throw new ArgumentNullException(nameof(entries));
            Id = id;
            Name = name;
            Description = description;
            StartDate = startDate;
            EndDate = endDate;
            _entries = entries.ToList();
        }

        public static ProfitAndLossStatement Create(Guid id, string name, string description, DateTime startDate, DateTime endDate, IEnumerable<ProfitAndLossEntry> entries)
            => new(id, name, description, startDate, endDate, entries);

        // Factory method to create a ProfitAndLossStatement from a Ledger
        public static ProfitAndLossStatement FromLedger(
            string name,
            string description,
            DateTime startDate,
            DateTime endDate,
            Ledger ledger)
        {
            if (ledger == null)
                throw new ArgumentNullException(nameof(ledger));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException($"'{nameof(description)}' cannot be null or empty.", nameof(description));
            if (startDate == default)
                throw new ArgumentException($"'{nameof(startDate)}' cannot be default.", nameof(startDate));
            if (endDate == default)
                throw new ArgumentException($"'{nameof(endDate)}' cannot be default.", nameof(endDate));

            var plEntries = ledger.LedgerAccounts
                .Where(acc => acc.Type == AccountType.Income || acc.Type == AccountType.Expense)
                .SelectMany(account => account.LedgerEntries
                    .Where(entry => entry.Date >= startDate && entry.Date <= endDate)
                    .Select(entry => ProfitAndLossEntry.Create(
                        account.Code,
                        account.Name,
                        account.Type,
                        account.Type == AccountType.Income ? entry.Credit : entry.Debit)))
                .ToList();

            return new ProfitAndLossStatement(Guid.NewGuid(), name, description, startDate, endDate, plEntries);
        }
    }

    [Table("ProfitAndLossEntries")]
    public sealed record ProfitAndLossEntry
    {
        [Key]
        [MaxLength(256)]
        public string AccountID { get; init; }

        [Required]
        [MaxLength(256)]
        public string AccountName { get; init; }

        [Required]
        public AccountType Type { get; init; }

        public decimal Amount { get; init; }

        private ProfitAndLossEntry() { AccountID = string.Empty; AccountName = string.Empty; }

        private ProfitAndLossEntry(string accountID, string accountName, AccountType type, decimal amount)
        {
            if (string.IsNullOrWhiteSpace(accountID))
                throw new ArgumentException($"'{nameof(accountID)}' cannot be null or empty.", nameof(accountID));
            if (string.IsNullOrWhiteSpace(accountName))
                throw new ArgumentException($"'{nameof(accountName)}' cannot be null or empty.", nameof(accountName));
            if (amount < 0)
                throw new ArgumentException($"'{nameof(amount)}' cannot be negative.", nameof(amount));
            AccountID = accountID;
            AccountName = accountName;
            Type = type;
            Amount = amount;
        }

        public static ProfitAndLossEntry Create(string accountID, string accountName, AccountType type, decimal amount)
            => new(accountID, accountName, type, amount);
    }
}
