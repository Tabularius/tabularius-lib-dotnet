using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace TabulariusLib.Entities;

[Table("LedgerAccounts")]
public sealed record LedgerAccount
{

    [Key]
    [Required]
    [MaxLength(256)]
    public required string Code { get; init; }

    [Required]
    [MaxLength(256)]
    public required string Name { get; init; }

    [Required]
    [MaxLength(256)]
    public required string ParentCode { get; init; }

    [Required]
    [MaxLength(256)]
    public required string Normally { get; init; }

    [Required]
    public required AccountType Type { get; init; }

    [Required]
    [MaxLength(256)]
    public required string Description { get; init; }

    internal List<LedgerEntry> _ledgerEntries = new();
    public IReadOnlyCollection<LedgerEntry> LedgerEntries => _ledgerEntries.AsReadOnly();

    public decimal Credit => _ledgerEntries.Sum(entry => entry.Credit);
    public decimal Debit => _ledgerEntries.Sum(entry => entry.Debit);
    public decimal Balance => Credit - Debit;
    private LedgerAccount() { }
    public static LedgerAccount Create(string code, string name, AccountType type, string description, string parentCode, string normally, IEnumerable<LedgerEntry>? lines)
    {
        if (string.IsNullOrEmpty(code))
            throw new ArgumentException($"'{nameof(code)}' cannot be null or empty.", nameof(code));
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
        if (string.IsNullOrEmpty(description))
            throw new ArgumentException($"'{nameof(description)}' cannot be null or empty.", nameof(description));
        if (string.IsNullOrEmpty(parentCode))
            parentCode = string.Empty;
        if (string.IsNullOrEmpty(normally))
            throw new ArgumentException($"'{nameof(normally)}' cannot be null or empty.", nameof(normally));
        if (lines == null)
            lines = Enumerable.Empty<LedgerEntry>();

        return new LedgerAccount
        {
            Code = code,
            Name = name,
            Type = type,
            Description = description,
            ParentCode = parentCode,
            Normally = normally,
            _ledgerEntries = lines.ToList()
        };
    }
}
