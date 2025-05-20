using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tabularius.TabulariusLib.Entities;

[Table("LedgerEntries")]
public sealed record LedgerEntry
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public required Guid Id { get; init; }

    [Required]
    [MaxLength(256)]
    public required string Description { get; init; }

    [Required]
    [MaxLength(256)]
    public required string LedgerID { get; init; }

    [Required]
    [MaxLength(256)]
    public required string JournalEntryID { get; init; }

    public decimal Debit { get; init; }
    public decimal Credit { get; init; }

    [Required]
    public required DateTime Date { get; init; }

    [Required]
    [MaxLength(256)]
    public required string Reference { get; init; }

    private LedgerEntry() { }
    public static LedgerEntry Create(Guid id, string description, string ledgerID, string journalEntryID, decimal debit, decimal credit, DateTime date, string reference)
    {
        if (id == Guid.Empty)
            throw new ArgumentException($"'{nameof(id)}' cannot be empty.", nameof(id));
        if (string.IsNullOrEmpty(description))
            throw new ArgumentException($"'{nameof(description)}' cannot be null or empty.", nameof(description));
        if (string.IsNullOrEmpty(ledgerID))
            throw new ArgumentException($"'{nameof(ledgerID)}' cannot be null or empty.", nameof(ledgerID));
        if (string.IsNullOrEmpty(journalEntryID))
            throw new ArgumentException($"'{nameof(journalEntryID)}' cannot be null or empty.", nameof(journalEntryID));
        if (debit < 0)
            throw new ArgumentException($"'{nameof(debit)}' cannot be negative.", nameof(debit));
        if (credit < 0)
            throw new ArgumentException($"'{nameof(credit)}' cannot be negative.", nameof(credit));
        if (debit == 0 && credit == 0)
            throw new ArgumentException($"Either '{nameof(debit)}' or '{nameof(credit)}' must be positive.", nameof(debit));
        if (date == default)
            throw new ArgumentException($"'{nameof(date)}' cannot be empty.", nameof(date));
        if (string.IsNullOrEmpty(reference))
            throw new ArgumentException($"'{nameof(reference)}' cannot be null or empty.", nameof(reference));

        return new LedgerEntry
        {
            Id = id,
            Description = description,
            LedgerID = ledgerID,
            JournalEntryID = journalEntryID,
            Debit = debit,
            Credit = credit,
            Date = date,
            Reference = reference
        };
    }

}
