
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace TabulariusLib.Entities;


[Table("Ledgers")]
public sealed record Ledger
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

    // EF Core needs a settable property for navigation, but we expose only the read-only collection
    private List<LedgerAccount> _ledgerAccounts { get; init; } = new();
    public IReadOnlyCollection<LedgerAccount> LedgerAccounts => _ledgerAccounts.AsReadOnly();


    // Parameterless constructor for EF Core
    private Ledger()
    { 
        Name = string.Empty;
        Description = string.Empty;
    }

    // Private full constructor for controlled creation and validation
    private Ledger(Guid id, string name, string description, IEnumerable<LedgerAccount> ledgerAccounts)
    {
        if (id == Guid.Empty)
            throw new ArgumentException($"'{nameof(id)}' cannot be empty.", nameof(id));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException($"'{nameof(description)}' cannot be null or empty.", nameof(description));
        if (ledgerAccounts == null)
            throw new ArgumentNullException(nameof(ledgerAccounts));

        Id = id;
        Name = name;
        Description = description;
        _ledgerAccounts = ledgerAccounts.ToList();
    }

    // Factory method for creation with validation
    public static Ledger Create(Guid id, string name, string description, IEnumerable<LedgerAccount>? ledgerAccounts)
        => new(id, name, description, ledgerAccounts ?? Enumerable.Empty<LedgerAccount>());

    // Factory method to create a Ledger from journals and accounts
    public static Ledger FromJournal(
        string name,
        string description,
        Journal journal,
        IEnumerable<Account> accounts)
    {
        // For each account, collect all JournalLines from all JournalEntries in the journal that match the account
        var ledgerAccounts = accounts.Select(account =>
        {
            var entries = journal.JournalEntries
                .SelectMany(entry => entry.JournalLines
                    .Where(line => line.AccountID == account.Code)
                    .Select(line => LedgerEntry.Create(
                        Guid.NewGuid(),
                        entry.Description,
                        line.AccountID,
                        entry.JournalEntryID,
                        line.Debit,
                        line.Credit,
                        entry.Date,
                        entry.Reference)))
                .ToList();

            return LedgerAccount.Create(
                account.Code,
                account.Name,
                account.Type,
                account.Description,
                account.ParentCode ?? string.Empty,
                account.Normally,
                entries);
        }).Where(la => la.LedgerEntries.Any()).ToList();

        return new Ledger(Guid.NewGuid(), name, description, ledgerAccounts);
    }

}
