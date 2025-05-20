using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tabularius.TabulariusLib.Entities;

[Table("TrialBalanceEntries")]
public sealed record TrialBalanceEntry
{
    [Key]
    [Required]
    [MaxLength(256)]
    public string AccountID { get; init; }

    [Required]
    [MaxLength(256)]
    public string AccountName { get; init; }

    [Required]
    public AccountType Type { get; init; }

    [MaxLength(256)]
    public string? ParentCode { get; init; }

    public decimal Debit { get; init; }
    public decimal Credit { get; init; }

    [Required]
    [MaxLength(256)]
    public string Normally { get; init; }

    // Parameterless constructor for EF Core
    private TrialBalanceEntry()
    {
        AccountID = string.Empty;
        AccountName = string.Empty;
        Type = AccountType.Asset;
        ParentCode = null;
        Normally = string.Empty;
    }

    // Private full constructor for validation and controlled creation
    private TrialBalanceEntry(string accountID, string accountName, AccountType type, string? parentCode, decimal debit, decimal credit, string normally)
    {
        if (string.IsNullOrWhiteSpace(normally))
            throw new ArgumentException($"'{nameof(normally)}' cannot be null or empty.", nameof(normally));
        if (string.IsNullOrWhiteSpace(accountID))
            throw new ArgumentException($"'{nameof(accountID)}' cannot be null or empty.", nameof(accountID));
        if (string.IsNullOrWhiteSpace(accountName))
            throw new ArgumentException($"'{nameof(accountName)}' cannot be null or empty.", nameof(accountName));
        if (debit < 0)
            throw new ArgumentOutOfRangeException(nameof(debit), "Debit cannot be negative.");
        if (credit < 0)
            throw new ArgumentOutOfRangeException(nameof(credit), "Credit cannot be negative.");

        AccountID = accountID;
        AccountName = accountName;
        Type = type;
        ParentCode = string.IsNullOrWhiteSpace(parentCode) ? null : parentCode;
        Debit = debit;
        Credit = credit;
        Normally = normally;
    }

    // Factory method for creation with validation
    public static TrialBalanceEntry Create(string accountID, string accountName, AccountType type, string? parentCode, decimal debit, decimal credit, string normally)
        => new(accountID, accountName, type, parentCode, debit, credit, normally);
}
