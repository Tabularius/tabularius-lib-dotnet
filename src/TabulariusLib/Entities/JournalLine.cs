/*
 * JournalLine.cs
 * 
 * Represents a concrete implementation of a journal line entity in the Tabularius accounting library.
 * 
 * This record provides a strongly-typed, immutable journal line entity, inheriting from JournalLineBase<JournalLine, Account>.
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
/// Represents a concrete journal line entity in the Tabularius accounting library.
/// Inherits from <see cref="JournalLineBase{JournalLine, Account}"/> and provides factory methods for creation and mutation.
/// </summary>
[Table("JournalLines")]
public sealed record JournalLine : JournalLineBase<JournalLine, Account>
{
    /// <summary>
    /// Private parameterless constructor for EF Core.
    /// </summary>
    private JournalLine() : base() { }

    /// <summary>
    /// Private full constructor for controlled creation and validation.
    /// </summary>
    /// <param name="id">The unique identifier for the journal line.</param>
    /// <param name="description">The description of the journal line.</param>
    /// <param name="accountID">The account identifier associated with this journal line.</param>
    /// <param name="debit">The debit amount.</param>
    /// <param name="credit">The credit amount.</param>
    private JournalLine(Guid id, string description, string accountID, decimal debit, decimal credit)
        : base(id, description, accountID, debit, credit)
    { }

    /// <summary>
    /// Factory method for creation with validation.
    /// </summary>
    /// <param name="id">The unique identifier for the journal line.</param>
    /// <param name="description">The description of the journal line.</param>
    /// <param name="accountID">The account identifier associated with this journal line.</param>
    /// <param name="debit">The debit amount.</param>
    /// <param name="credit">The credit amount.</param>
    /// <returns>A new <see cref="JournalLine"/> instance.</returns>
    public static JournalLine Create(Guid id, string description, string accountID, decimal debit, decimal credit)
        => new JournalLine(id, description, accountID, debit, credit);

    /// <summary>
    /// Implementation of the abstract factory method for mutation methods.
    /// </summary>
    /// <param name="id">The unique identifier for the journal line.</param>
    /// <param name="description">The description of the journal line.</param>
    /// <param name="accountID">The account identifier associated with this journal line.</param>
    /// <param name="debit">The debit amount.</param>
    /// <param name="credit">The credit amount.</param>
    /// <returns>A new <see cref="JournalLine"/> instance with the specified values.</returns>
    protected override JournalLine CreateInstance(Guid id, string description, string accountID, decimal debit, decimal credit)
        => new JournalLine(id, description, accountID, debit, credit);
}