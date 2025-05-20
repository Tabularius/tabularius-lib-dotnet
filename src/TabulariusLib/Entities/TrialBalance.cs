
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace tabularius.TabulariusLib.Entities;


[Table("TrailBalances")]
public sealed record TrialBalance
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

    [Required]
    public bool IsClosed { get; init; } = false;

    // EF Core needs a settable property for navigation, but we expose only the read-only collection
    private List<TrialBalanceEntry> _trialBalanceEntries { get; init; } = new();
    public IReadOnlyCollection<TrialBalanceEntry> TrialBalanceEntries => _trialBalanceEntries.AsReadOnly();

    public decimal TotalDebit => _trialBalanceEntries.Sum(entry => entry.Debit);
    public decimal TotalCredit => _trialBalanceEntries.Sum(entry => entry.Credit);
    public decimal Balance => TotalCredit - TotalDebit;
    public bool IsBalanced => Balance == 0;

    // Parameterless constructor for EF Core
    private TrialBalance()
    {
        Name = string.Empty;
        Description = string.Empty;
        Date = default;
    }

    // Private full constructor for controlled creation and validation
    private TrialBalance(Guid id, string name, string description, DateTime date, IEnumerable<TrialBalanceEntry> trialBalanceEntries)
    {
        if (id == Guid.Empty)
            throw new ArgumentException($"'{nameof(id)}' cannot be empty.", nameof(id));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException($"'{nameof(description)}' cannot be null or empty.", nameof(description));
        if (trialBalanceEntries == null)
            throw new ArgumentNullException(nameof(trialBalanceEntries));

        Id = id;
        Name = name;
        Description = description;
        Date = date;
        _trialBalanceEntries = trialBalanceEntries.ToList();
    }

    // Factory method for creation with validation
    public static TrialBalance Create(Guid id, string name, string description, DateTime date, IEnumerable<TrialBalanceEntry> trialBalanceEntries)
        => new(id, name, description, date, trialBalanceEntries);

    // Factory method to create a TrialBalance from a Ledger
    public static TrialBalance FromLedger(
        string name,
        string description,
        DateTime upToDate,
        Ledger ledger)
    {
        if (ledger == null)
            throw new ArgumentNullException(nameof(ledger));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException($"'{nameof(description)}' cannot be null or empty.", nameof(description));
        if (ledger.LedgerAccounts == null || !ledger.LedgerAccounts.Any())
            throw new ArgumentException($"No ledger accounts found.", nameof(ledger.LedgerAccounts));
        if (ledger.LedgerAccounts.Any(account => account == null))
            throw new ArgumentException($"Some ledger accounts are null.", nameof(ledger.LedgerAccounts));
        if (ledger.LedgerAccounts.Any(account => account.LedgerEntries == null))
            throw new ArgumentException($"Some ledger accounts have null ledger entries.", nameof(ledger.LedgerAccounts));
        if (ledger.LedgerAccounts.Any(account => account.LedgerEntries.Any(entry => entry == null)))
            throw new ArgumentException($"Some ledger entries are null.", nameof(ledger.LedgerAccounts));

        var trialBalanceEntries = ledger.LedgerAccounts
            .Select(account =>
            {
                var filteredEntries = account.LedgerEntries.Where(e => e.Date <= upToDate);
                var debit = filteredEntries.Sum(e => e.Debit);
                var credit = filteredEntries.Sum(e => e.Credit);
                return TrialBalanceEntry.Create(
                    account.Code,
                    account.Name,
                    account.Type,
                    account.ParentCode,
                    debit,
                    credit,
                    account.Normally);
            })
            .ToList();

        if (!trialBalanceEntries.Any())
            throw new ArgumentException($"No account entries found.", nameof(trialBalanceEntries));

        return Create(Guid.NewGuid(), name, description, upToDate, trialBalanceEntries);
    }

    // Method to close accounts in the trial balance
    public TrialBalance CloseAccounts(Account closingEquityAccount)
    {
        if (IsClosed)
            throw new InvalidOperationException("Accounts are already closed for this trial balance.");

        // Separate entries by type
        var incomeEntries = _trialBalanceEntries.Where(e => e.Type == AccountType.Income).ToList();
        var expenseEntries = _trialBalanceEntries.Where(e => e.Type == AccountType.Expense).ToList();
        var equityEntries = _trialBalanceEntries.Where(e => e.Type == AccountType.Equity).ToList();
        var otherEntries = _trialBalanceEntries.Where(e => e.Type != AccountType.Income && e.Type != AccountType.Expense && e.Type != AccountType.Equity).ToList();

        // Calculate net income (income - expense)
        decimal totalIncome = incomeEntries.Sum(e => e.Credit - e.Debit);
        decimal totalExpense = expenseEntries.Sum(e => e.Debit - e.Credit);
        decimal netIncome = totalIncome - totalExpense;

        // Close income and expense accounts (set their balances to zero)
        var closedIncomeEntries = incomeEntries.Select(e => e with { Debit = e.Credit, Credit = e.Credit, Normally = e.Normally }).ToList();
        var closedExpenseEntries = expenseEntries.Select(e => e with { Debit = e.Debit, Credit = e.Debit, Normally = e.Normally }).ToList();

        // Add net income/loss to equity
        var closedEntries = new List<TrialBalanceEntry>();
        closedEntries.AddRange(otherEntries);
        closedEntries.AddRange(equityEntries);
        closedEntries.AddRange(closedIncomeEntries);
        closedEntries.AddRange(closedExpenseEntries);

        if (netIncome != 0)
        {
            // Find the 'Retained Earnings' equity account to post the net income/loss
            var equityAccount = equityEntries.FirstOrDefault(e => e.AccountID == closingEquityAccount.Code)
                ?? TrialBalanceEntry.Create(
                    closingEquityAccount.Code,
                    closingEquityAccount.Name,
                    closingEquityAccount.Type,
                    closingEquityAccount.ParentCode,
                    0,
                    0,
                    closingEquityAccount.Normally
                );
            if (equityAccount == null)
                throw new InvalidOperationException($"No equity account ('{closingEquityAccount.Name}') found to close net income/loss.");
            // If the account already exists in closedEntries, add to its value
            var retainedEarnings = closedEntries.FirstOrDefault(e => e.AccountID == equityAccount.AccountID);
            if (retainedEarnings != null)
            {
                var newDebit = retainedEarnings.Debit + (netIncome < 0 ? Math.Abs(netIncome) : 0);
                var newCredit = retainedEarnings.Credit + (netIncome > 0 ? netIncome : 0);
                closedEntries.Remove(retainedEarnings);
                closedEntries.Add(retainedEarnings with { Debit = newDebit, Credit = newCredit, Normally = retainedEarnings.Normally });
            }
            else
            {
                closedEntries.Add(equityAccount with
                {
                    Debit = netIncome < 0 ? Math.Abs(netIncome) : 0,
                    Credit = netIncome > 0 ? netIncome : 0,
                    Normally = equityAccount.Normally
                });
            }
        }

        return new TrialBalance(
            Id,
            Name,
            Description,
            Date,
            closedEntries
        )
        {
            IsClosed = true
        };
    }
}
