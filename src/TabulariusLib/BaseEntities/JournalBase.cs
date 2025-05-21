/*
 * JournalBase.cs
 * 
 * Abstract base record for journal entities in the Tabularius accounting library.
 * 
 * This class provides a strongly-typed, immutable base for journal records using the Curiously Recurring Template Pattern (CRTP).
 * It enforces validation, supports mutation methods that return new instances, and is designed for use with Entity Framework Core.
 * 
 * License: Apache-2.0
 * Author: Michael Warneke
 * Copyright 2025 Michael Warneke
 */

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TabulariusLib.BaseEntities;

/// <summary>
/// Abstract base record for journal entities, using CRTP for strongly-typed mutation methods.
/// </summary>
/// <typeparam name="TSelf">The concrete journal type inheriting from this base.</typeparam>
/// <typeparam name="TJournalEntry">The journal entry type associated with this journal.</typeparam>
/// <typeparam name="TJournalLine">The journal line type associated with this journal.</typeparam>
/// <typeparam name="TAccount">The account type associated with this journal.</typeparam>
public abstract record JournalBase<TSelf, TJournalEntry, TJournalLine, TAccount>
    where TAccount : AccountBase<TAccount>
    where TJournalLine : JournalLineBase<TJournalLine, TAccount>
    where TJournalEntry : JournalEntryBase<TJournalEntry, TJournalLine>
    where TSelf : JournalBase<TSelf, TJournalEntry, TJournalLine, TAccount>
{
    /// <summary>
    /// The unique identifier for the journal (primary key).
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; private set; }

    /// <summary>
    /// The name of the journal.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public string Name { get; private set; }

    /// <summary>
    /// The description of the journal.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public string Description { get; private set; }

    /// <summary>
    /// The list of journal entries associated with this journal.
    /// </summary>
    private List<TJournalEntry> _journalEntries { get; set; } = new();

    /// <summary>
    /// Gets a read-only collection of journal entries.
    /// </summary>
    public IReadOnlyCollection<TJournalEntry> JournalEntries => _journalEntries.AsReadOnly();

    /// <summary>
    /// Indicates whether all journal entries in this journal are balanced.
    /// </summary>
    public bool AreAllEntriesBalanced => _journalEntries.All(entry => entry.IsBalanced);

    /// <summary>
    /// Protected parameterless constructor for EF Core.
    /// </summary>
    protected JournalBase()
    {
        Name = string.Empty;
        Description = string.Empty;
    }

    /// <summary>
    /// Protected constructor with validation for all properties.
    /// </summary>
    /// <param name="id">The unique identifier for the journal.</param>
    /// <param name="name">The name of the journal.</param>
    /// <param name="description">The description of the journal.</param>
    /// <param name="entries">The collection of journal entries.</param>
    /// <exception cref="ArgumentException">Thrown if any required argument is invalid or if any entry is unbalanced.</exception>
    /// <exception cref="ArgumentNullException">Thrown if entries is null.</exception>
    protected JournalBase(Guid id, string name, string description, IEnumerable<TJournalEntry> entries)
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

    /// <summary>
    /// Abstract factory method for mutation methods. Must be implemented in derived types.
    /// </summary>
    /// <param name="id">The unique identifier for the journal.</param>
    /// <param name="name">The name of the journal.</param>
    /// <param name="description">The description of the journal.</param>
    /// <param name="entries">The collection of journal entries.</param>
    /// <returns>A new instance of the derived journal type.</returns>
    protected abstract TSelf CreateInstance(Guid id, string name, string description, IEnumerable<TJournalEntry> entries);

    /// <summary>
    /// Returns a new instance with the specified name.
    /// </summary>
    public TSelf WithName(string newName)
        => CreateInstance(Id, newName ?? Name, Description, JournalEntries);

    /// <summary>
    /// Returns a new instance with the specified description.
    /// </summary>
    public TSelf WithDescription(string newDescription)
        => CreateInstance(Id, Name, newDescription ?? Description, JournalEntries);

    /// <summary>
    /// Returns a new instance with the specified journal entries.
    /// </summary>
    public TSelf WithJournalEntries(IEnumerable<TJournalEntry> newEntries)
        => CreateInstance(Id, Name, Description, newEntries);

    /// <summary>
    /// Returns a new instance with the specified journal entry added.
    /// </summary>
    /// <param name="journalEntry">The journal entry to add.</param>
    /// <returns>A new instance of the journal with the entry added.</returns>
    /// <exception cref="ArgumentNullException">Thrown if journalEntry is null.</exception>
    /// <exception cref="ArgumentException">Thrown if the journal entry is not balanced.</exception>
    public TSelf AddJournalEntry(TJournalEntry journalEntry)
    {
        if (journalEntry == null)
            throw new ArgumentNullException(nameof(journalEntry));
        if (!journalEntry.IsBalanced)
            throw new ArgumentException($"'{nameof(journalEntry)}' is not balanced.", nameof(journalEntry));

        return CreateInstance(Id, Name, Description, JournalEntries.Append(journalEntry));
    }
}