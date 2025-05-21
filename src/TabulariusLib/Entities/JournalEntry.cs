/*
 * JournalEntry.cs
 * 
 * Represents a concrete implementation of a journal entry entity in the Tabularius accounting library.
 * 
 * This record provides a strongly-typed, immutable journal entry entity, inheriting from JournalEntryBase<JournalEntry, JournalLine>.
 * It enforces validation, supports mutation methods that return new instances, and is designed for use with Entity Framework Core.
 * 
 * License: Apache-2.0
 * Author: Michael Warneke
 * Copyright 2025 Michael Warneke
 */

using System.ComponentModel.DataAnnotations.Schema;
using TabulariusLib.BaseEntities;

namespace TabulariusLib.Entities;

/// <summary>
/// Represents a concrete journal entry entity in the Tabularius accounting library.
/// Inherits from <see cref="JournalEntryBase{JournalEntry, JournalLine}"/> and provides factory methods for creation and mutation.
/// </summary>
[Table("JournalEntries")]
public sealed record JournalEntry : JournalEntryBase<JournalEntry, JournalLine>
{
    /// <summary>
    /// Private parameterless constructor for EF Core.
    /// </summary>
    private JournalEntry() : base() { }

    /// <summary>
    /// Private full constructor for controlled creation with validation.
    /// </summary>
    /// <param name="journalEntryID">The unique identifier for the journal entry.</param>
    /// <param name="description">The description of the journal entry.</param>
    /// <param name="date">The date of the journal entry.</param>
    /// <param name="reference">The reference string for the journal entry.</param>
    /// <param name="lines">The collection of journal lines.</param>
    private JournalEntry(string journalEntryID, string description, DateTime date, string reference, IEnumerable<JournalLine> lines)
        : base(journalEntryID, description, date, reference, lines)
    { }

    /// <summary>
    /// Factory method for creation with validation.
    /// </summary>
    /// <param name="journalEntryID">The unique identifier for the journal entry.</param>
    /// <param name="description">The description of the journal entry.</param>
    /// <param name="date">The date of the journal entry.</param>
    /// <param name="reference">The reference string for the journal entry.</param>
    /// <param name="lines">The collection of journal lines. If null, an empty collection is used.</param>
    /// <returns>A new <see cref="JournalEntry"/> instance.</returns>
    public static JournalEntry Create(string journalEntryID, string description, DateTime date, string reference, IEnumerable<JournalLine>? lines)
        => new(journalEntryID, description, date, reference, lines ?? Enumerable.Empty<JournalLine>());

    /// <summary>
    /// Implementation of the abstract factory method for mutation methods.
    /// </summary>
    /// <param name="journalEntryID">The unique identifier for the journal entry.</param>
    /// <param name="description">The description of the journal entry.</param>
    /// <param name="date">The date of the journal entry.</param>
    /// <param name="reference">The reference string for the journal entry.</param>
    /// <param name="lines">The collection of journal lines.</param>
    /// <returns>A new <see cref="JournalEntry"/> instance with the specified values.</returns>
    protected override JournalEntry CreateInstance(string journalEntryID, string description, DateTime date, string reference, IEnumerable<JournalLine> lines)
        => new JournalEntry(journalEntryID, description, date, reference, lines);
}