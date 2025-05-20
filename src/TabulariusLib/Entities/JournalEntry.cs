
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace tabularius.TabulariusLib.Entities;

[Table("JournalEntries")]
public sealed record JournalEntry
{
    [Key]
    [Required]
    [MaxLength(256)]
    public string JournalEntryID { get; init; }

    [Required]
    [MaxLength(256)]
    public string Description { get; init; }

    [Required]
    public DateTime Date { get; init; }

    [Required]
    [MaxLength(256)]
    public string Reference { get; init; }

    // EF Core needs a settable property for navigation, but we expose only the read-only collection
    private readonly List<JournalLine> _journalLines /*{ get; init; }*/ = new();
    public IReadOnlyCollection<JournalLine> JournalLines => _journalLines.AsReadOnly();

    public bool IsBalanced => _journalLines.Sum(jl => jl.Debit) == _journalLines.Sum(jl => jl.Credit);

    // Parameterless constructor for EF Core
    private JournalEntry()
    { 
        JournalEntryID = string.Empty;
        Description = string.Empty;
        Reference = string.Empty;
        Date = default;
    }

    // Private full constructor for validation and controlled creation
    private JournalEntry(string journalEntryID, string description, DateTime date, string reference, IEnumerable<JournalLine> lines)
    {
        if (string.IsNullOrWhiteSpace(journalEntryID))
            throw new ArgumentException($"'{nameof(journalEntryID)}' cannot be null or empty.", nameof(journalEntryID));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException($"'{nameof(description)}' cannot be null or empty.", nameof(description));
        if (string.IsNullOrWhiteSpace(reference))
            throw new ArgumentException($"'{nameof(reference)}' cannot be null or empty.", nameof(reference));
        if (date == default)
            throw new ArgumentException($"'{nameof(date)}' cannot be empty.", nameof(date));
        if (lines == null)
            throw new ArgumentNullException(nameof(lines));

        JournalEntryID = journalEntryID;
        Description = description;
        Date = date;
        Reference = reference;
        _journalLines = lines.ToList();
    }

    // Factory method for creation with validation
    public static JournalEntry Create(string journalEntryID, string description, DateTime date, string reference, IEnumerable<JournalLine>? lines)
        => new(journalEntryID, description, date, reference, lines ?? Enumerable.Empty<JournalLine>());

    // Mutation methods with validation
    public JournalEntry WithDescription(string newDescription)
        => Create(JournalEntryID, newDescription, Date, Reference, _journalLines);

    public JournalEntry WithDate(DateTime newDate)
        => Create(JournalEntryID, Description, newDate, Reference, _journalLines);

    public JournalEntry WithReference(string newReference)
        => Create(JournalEntryID, Description, Date, newReference, _journalLines);

    public JournalEntry WithJournalLines(IEnumerable<JournalLine> newJournalLines)
        => Create(JournalEntryID, Description, Date, Reference, newJournalLines);

    public JournalEntry AddJournalLines(IEnumerable<JournalLine> newJournalLines)
    {
        if (newJournalLines == null)
            throw new ArgumentNullException(nameof(newJournalLines));
        return Create(JournalEntryID, Description, Date, Reference, _journalLines.Concat(newJournalLines));
    }

    public JournalEntry AddJournalLine(JournalLine journalLine)
    {
        if (journalLine == null)
            throw new ArgumentNullException(nameof(journalLine));
        return Create(JournalEntryID, Description, Date, Reference, _journalLines.Append(journalLine));
    }
}
