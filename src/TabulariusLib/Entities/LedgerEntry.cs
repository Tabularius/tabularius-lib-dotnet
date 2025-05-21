/*
 * LedgerEntry.cs
 * 
 * Represents a concrete implementation of a ledger entry entity in the Tabularius accounting library.
 * 
 * This record provides a strongly-typed, immutable ledger entry entity, inheriting from LedgerEntryBase<LedgerEntry>.
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
/// Represents a concrete ledger entry entity in the Tabularius accounting library.
/// Inherits from <see cref="LedgerEntryBase{LedgerEntry}"/> and provides factory methods for creation and mutation.
/// </summary>
[Table("LedgerEntries")]
public sealed record LedgerEntry : LedgerEntryBase<LedgerEntry>
{
    /// <summary>
    /// Private parameterless constructor for EF Core.
    /// </summary>
    private LedgerEntry() : base() { }

    /// <summary>
    /// Private full constructor for controlled creation and validation.
    /// </summary>
    /// <param name="id">The unique identifier for the ledger entry.</param>
    /// <param name="description">The description of the ledger entry.</param>
    /// <param name="ledgerID">The identifier of the ledger this entry belongs to.</param>
    /// <param name="journalEntryID">The identifier of the journal entry this ledger entry is derived from.</param>
    /// <param name="debit">The debit amount.</param>
    /// <param name="credit">The credit amount.</param>
    /// <param name="date">The date of the ledger entry.</param>
    /// <param name="reference">The reference string for the ledger entry.</param>
    private LedgerEntry(
        Guid id,
        string description,
        string ledgerID,
        string journalEntryID,
        decimal debit,
        decimal credit,
        DateTime date,
        string reference)
        : base(id, description, ledgerID, journalEntryID, debit, credit, date, reference)
    { }

    /// <summary>
    /// Factory method for creation with validation.
    /// </summary>
    /// <param name="id">The unique identifier for the ledger entry.</param>
    /// <param name="description">The description of the ledger entry.</param>
    /// <param name="ledgerID">The identifier of the ledger this entry belongs to.</param>
    /// <param name="journalEntryID">The identifier of the journal entry this ledger entry is derived from.</param>
    /// <param name="debit">The debit amount.</param>
    /// <param name="credit">The credit amount.</param>
    /// <param name="date">The date of the ledger entry.</param>
    /// <param name="reference">The reference string for the ledger entry.</param>
    /// <returns>A new <see cref="LedgerEntry"/> instance.</returns>
    public static LedgerEntry Create(
        Guid id,
        string description,
        string ledgerID,
        string journalEntryID,
        decimal debit,
        decimal credit,
        DateTime date,
        string reference)
        => new LedgerEntry(id, description, ledgerID, journalEntryID, debit, credit, date, reference);

    /// <summary>
    /// Implementation of the abstract factory method for mutation methods.
    /// </summary>
    /// <param name="id">The unique identifier for the ledger entry.</param>
    /// <param name="description">The description of the ledger entry.</param>
    /// <param name="ledgerID">The identifier of the ledger this entry belongs to.</param>
    /// <param name="journalEntryID">The identifier of the journal entry this ledger entry is derived from.</param>
    /// <param name="debit">The debit amount.</param>
    /// <param name="credit">The credit amount.</param>
    /// <param name="date">The date of the ledger entry.</param>
    /// <param name="reference">The reference string for the ledger entry.</param>
    /// <returns>A new <see cref="LedgerEntry"/> instance with the specified values.</returns>
    protected override LedgerEntry CreateInstance(
        Guid id,
        string description,
        string ledgerID,
        string journalEntryID,
        decimal debit,
        decimal credit,
        DateTime date,
        string reference)
        => new LedgerEntry(id, description, ledgerID, journalEntryID, debit, credit, date, reference);
}