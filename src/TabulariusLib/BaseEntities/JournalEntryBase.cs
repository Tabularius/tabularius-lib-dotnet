/*
 * JournalEntryBase.cs
 * 
 * Abstract base record for journal entry entities in the Tabularius accounting library.
 * 
 * This class provides a strongly-typed, immutable base for journal entry records using the Curiously Recurring Template Pattern (CRTP).
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
/// Abstract base record for journal entry entities, using CRTP for strongly-typed mutation methods.
/// </summary>
/// <typeparam name="TSelf">The concrete journal entry type inheriting from this base.</typeparam>
/// <typeparam name="TJournalLine">The journal line type associated with this journal entry.</typeparam>
public abstract record JournalEntryBase<TSelf, TJournalLine>
    where TSelf : JournalEntryBase<TSelf, TJournalLine>
{
    /// <summary>
    /// The unique identifier for the journal entry (primary key).
    /// </summary>
    [Key]
    [Required]
    [MaxLength(256)]
    public string JournalEntryID { get; private set; }

    /// <summary>
    /// The description of the journal entry.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public string Description { get; private set; }

    /// <summary>
    /// The date of the journal entry.
    /// </summary>
    [Required]
    public DateTime Date { get; private set; }

    /// <summary>
    /// The reference string for the journal entry.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public string Reference { get; private set; }

    /// <summary>
    /// The list of journal lines associated with this journal entry.
    /// </summary>
    private List<TJournalLine> _journalLines { get; set; } = new();

    /// <summary>
    /// Gets a read-only collection of journal lines.
    /// </summary>
    public IReadOnlyCollection<TJournalLine> JournalLines => _journalLines.AsReadOnly();

    /// <summary>
    /// Indicates whether this journal entry is balanced (total debit equals total credit).
    /// </summary>
    public bool IsBalanced => _journalLines.Sum(jl => GetDebit(jl)) == _journalLines.Sum(jl => GetCredit(jl));

    /// <summary>
    /// Protected parameterless constructor for EF Core.
    /// </summary>
    protected JournalEntryBase()
    {
        JournalEntryID = string.Empty;
        Description = string.Empty;
        Reference = string.Empty;
        Date = default;
    }

    /// <summary>
    /// Protected constructor with validation for all properties.
    /// </summary>
    /// <param name="journalEntryID">The unique identifier for the journal entry.</param>
    /// <param name="description">The description of the journal entry.</param>
    /// <param name="date">The date of the journal entry.</param>
    /// <param name="reference">The reference string for the journal entry.</param>
    /// <param name="lines">The collection of journal lines.</param>
    /// <exception cref="ArgumentException">Thrown if any required argument is invalid.</exception>
    /// <exception cref="ArgumentNullException">Thrown if lines is null.</exception>
    protected JournalEntryBase(string journalEntryID, string description, DateTime date, string reference, IEnumerable<TJournalLine> lines)
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

    /// <summary>
    /// Abstract factory method for mutation methods. Must be implemented in derived types.
    /// </summary>
    /// <param name="journalEntryID">The unique identifier for the journal entry.</param>
    /// <param name="description">The description of the journal entry.</param>
    /// <param name="date">The date of the journal entry.</param>
    /// <param name="reference">The reference string for the journal entry.</param>
    /// <param name="lines">The collection of journal lines.</param>
    /// <returns>A new instance of the derived journal entry type.</returns>
    protected abstract TSelf CreateInstance(string journalEntryID, string description, DateTime date, string reference, IEnumerable<TJournalLine> lines);

    /// <summary>
    /// Returns a new instance with the specified description.
    /// </summary>
    public TSelf WithDescription(string newDescription)
        => CreateInstance(JournalEntryID, newDescription, Date, Reference, JournalLines);

    /// <summary>
    /// Returns a new instance with the specified date.
    /// </summary>
    public TSelf WithDate(DateTime newDate)
        => CreateInstance(JournalEntryID, Description, newDate, Reference, JournalLines);

    /// <summary>
    /// Returns a new instance with the specified reference.
    /// </summary>
    public TSelf WithReference(string newReference)
        => CreateInstance(JournalEntryID, Description, Date, newReference, JournalLines);

    /// <summary>
    /// Returns a new instance with the specified journal lines.
    /// </summary>
    public TSelf WithJournalLines(IEnumerable<TJournalLine> newJournalLines)
        => CreateInstance(JournalEntryID, Description, Date, Reference, newJournalLines);

    /// <summary>
    /// Returns a new instance with the specified journal lines added.
    /// </summary>
    /// <param name="newJournalLines">The journal lines to add.</param>
    /// <returns>A new instance of the journal entry with the lines added.</returns>
    /// <exception cref="ArgumentNullException">Thrown if newJournalLines is null.</exception>
    public TSelf AddJournalLines(IEnumerable<TJournalLine> newJournalLines)
    {
        if (newJournalLines == null)
            throw new ArgumentNullException(nameof(newJournalLines));
        return CreateInstance(JournalEntryID, Description, Date, Reference, JournalLines.Concat(newJournalLines));
    }

    /// <summary>
    /// Returns a new instance with the specified journal line added.
    /// </summary>
    /// <param name="journalLine">The journal line to add.</param>
    /// <returns>A new instance of the journal entry with the line added.</returns>
    /// <exception cref="ArgumentNullException">Thrown if journalLine is null.</exception>
    public TSelf AddJournalLine(TJournalLine journalLine)
    {
        if (journalLine == null)
            throw new ArgumentNullException(nameof(journalLine));
        return CreateInstance(JournalEntryID, Description, Date, Reference, JournalLines.Append(journalLine));
    }

    /// <summary>
    /// Helper method to extract the Debit value from a journal line using reflection.
    /// </summary>
    /// <param name="line">The journal line.</param>
    /// <returns>The debit value, or 0 if not found.</returns>
    private static decimal GetDebit(TJournalLine line)
    {
        var value = typeof(TJournalLine).GetProperty("Debit")?.GetValue(line);
        return value is decimal d ? d : 0m;
    }

    /// <summary>
    /// Helper method to extract the Credit value from a journal line using reflection.
    /// </summary>
    /// <param name="line">The journal line.</param>
    /// <returns>The credit value, or 0 if not found.</returns>
    private static decimal GetCredit(TJournalLine line)
    {
        var value = typeof(TJournalLine).GetProperty("Credit")?.GetValue(line);
        return value is decimal d ? d : 0m;
    }
}