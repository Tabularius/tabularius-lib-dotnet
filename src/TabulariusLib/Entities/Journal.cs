/*
 * Journal.cs
 * 
 * Represents a concrete implementation of a journal entity in the Tabularius accounting library.
 * 
 * This record provides a strongly-typed, immutable journal entity, inheriting from JournalBase<Journal, JournalEntry, JournalLine, Account>.
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
/// Represents a concrete journal entity in the Tabularius accounting library.
/// Inherits from <see cref="JournalBase{Journal, JournalEntry, JournalLine, Account}"/> and provides factory methods for creation and mutation.
/// </summary>
[Table("Journals")]
public sealed record Journal : JournalBase<Journal, JournalEntry, JournalLine, Account>
{
    /// <summary>
    /// Private parameterless constructor for EF Core.
    /// </summary>
    private Journal() : base() { }

    /// <summary>
    /// Private full constructor for controlled creation and validation.
    /// </summary>
    /// <param name="id">The unique identifier for the journal.</param>
    /// <param name="name">The name of the journal.</param>
    /// <param name="description">The description of the journal.</param>
    /// <param name="entries">The collection of journal entries.</param>
    private Journal(Guid id, string name, string description, IEnumerable<JournalEntry> entries)
        : base(id, name, description, entries)
    { }

    /// <summary>
    /// Factory method for creation with validation.
    /// </summary>
    /// <param name="id">The unique identifier for the journal.</param>
    /// <param name="name">The name of the journal.</param>
    /// <param name="description">The description of the journal.</param>
    /// <param name="entries">The collection of journal entries. If null, an empty collection is used.</param>
    /// <returns>A new <see cref="Journal"/> instance.</returns>
    public static Journal Create(Guid id, string name, string description, IEnumerable<JournalEntry>? entries)
        => new(id, name, description, entries ?? Enumerable.Empty<JournalEntry>());

    /// <summary>
    /// Implementation of the abstract factory method for mutation methods.
    /// </summary>
    /// <param name="id">The unique identifier for the journal.</param>
    /// <param name="name">The name of the journal.</param>
    /// <param name="description">The description of the journal.</param>
    /// <param name="entries">The collection of journal entries.</param>
    /// <returns>A new <see cref="Journal"/> instance with the specified values.</returns>
    protected override Journal CreateInstance(Guid id, string name, string description, IEnumerable<JournalEntry> entries)
        => new Journal(id, name, description, entries);
}