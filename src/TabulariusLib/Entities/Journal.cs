
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace tabularius.TabulariusLib.Entities;


[Table("Journals")]
public sealed record Journal
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
    private List<JournalEntry> _journalEntries { get; init; } = new();
    public IReadOnlyCollection<JournalEntry> JournalEntries => _journalEntries.AsReadOnly();

    // Method to check if all journal entries are balanced
    public bool AreAllEntriesBalanced => _journalEntries.All(entry => entry.IsBalanced);

    // Parameterless constructor for EF Core
    private Journal()
    {
        Name = string.Empty;
        Description = string.Empty;
    }

    // Private full constructor for controlled creation and validation
    private Journal(Guid id, string name, string description, IEnumerable<JournalEntry> entries)
    {
        if (id == Guid.Empty)
            throw new ArgumentException($"'{nameof(id)}' cannot be empty.", nameof(id));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException($"'{nameof(description)}' cannot be null or empty.", nameof(description));
        if (entries == null)
            throw new ArgumentNullException(nameof(entries));
        if (entries.Any(j => !j.IsBalanced))
            throw new ArgumentException($"'{nameof(entries)}' contains unbalanced journal entries.", nameof(entries));

        Id = id;
        Name = name;
        Description = description;
        _journalEntries = entries.ToList();
    }

    // Factory method for creation with validation
    public static Journal Create(Guid id, string name, string description, IEnumerable<JournalEntry>? entries)
        => new(id, name, description, entries ?? Enumerable.Empty<JournalEntry>());

    // Mutation method with validation
    public Journal AddJournalEntry(JournalEntry journalEntry)
    {
        if (journalEntry == null)
            throw new ArgumentNullException(nameof(journalEntry));
        if (!journalEntry.IsBalanced)
            throw new ArgumentException($"'{nameof(journalEntry)}' is not balanced.", nameof(journalEntry));

        return Create(Id, Name, Description, _journalEntries.Append(journalEntry));
    }
}
