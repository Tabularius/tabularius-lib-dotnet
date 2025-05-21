/*
 * LedgerEntryBase.cs
 * 
 * Abstract base record for ledger entry entities in the Tabularius accounting library.
 * 
 * This class provides a strongly-typed, immutable base for ledger entry records using the Curiously Recurring Template Pattern (CRTP).
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
/// Abstract base record for ledger entry entities, using CRTP for strongly-typed mutation methods.
/// </summary>
/// <typeparam name="TSelf">The concrete ledger entry type inheriting from this base.</typeparam>
public abstract record LedgerEntryBase<TSelf>
    where TSelf : LedgerEntryBase<TSelf>
{
    /// <summary>
    /// The unique identifier for the ledger entry (primary key).
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; private set; }

    /// <summary>
    /// The description of the ledger entry.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public string Description { get; private set; }

    /// <summary>
    /// The identifier of the ledger this entry belongs to.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public string LedgerID { get; private set; }

    /// <summary>
    /// The identifier of the journal entry this ledger entry is derived from.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public string JournalEntryID { get; private set; }

    /// <summary>
    /// The debit amount for this ledger entry.
    /// </summary>
    public decimal Debit { get; private set; }

    /// <summary>
    /// The credit amount for this ledger entry.
    /// </summary>
    public decimal Credit { get; private set; }

    /// <summary>
    /// The date of the ledger entry.
    /// </summary>
    [Required]
    public DateTime Date { get; private set; }

    /// <summary>
    /// The reference string for the ledger entry.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public string Reference { get; private set; }

    /// <summary>
    /// Protected parameterless constructor for EF Core.
    /// </summary>
    protected LedgerEntryBase()
    {
        Description = string.Empty;
        LedgerID = string.Empty;
        JournalEntryID = string.Empty;
        Reference = string.Empty;
        Date = default;
    }

    /// <summary>
    /// Protected constructor with validation for all properties.
    /// </summary>
    /// <param name="id">The unique identifier for the ledger entry.</param>
    /// <param name="description">The description of the ledger entry.</param>
    /// <param name="ledgerID">The identifier of the ledger this entry belongs to.</param>
    /// <param name="journalEntryID">The identifier of the journal entry this ledger entry is derived from.</param>
    /// <param name="debit">The debit amount.</param>
    /// <param name="credit">The credit amount.</param>
    /// <param name="date">The date of the ledger entry.</param>
    /// <param name="reference">The reference string for the ledger entry.</param>
    /// <exception cref="ArgumentException">Thrown if any required argument is invalid.</exception>
    protected LedgerEntryBase(
        Guid id,
        string description,
        string ledgerID,
        string journalEntryID,
        decimal debit,
        decimal credit,
        DateTime date,
        string reference)
    {
        if (id == Guid.Empty)
            throw new ArgumentException($"'{nameof(id)}' cannot be empty.", nameof(id));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException($"'{nameof(description)}' cannot be null or empty.", nameof(description));
        if (string.IsNullOrWhiteSpace(ledgerID))
            throw new ArgumentException($"'{nameof(ledgerID)}' cannot be null or empty.", nameof(ledgerID));
        if (string.IsNullOrWhiteSpace(journalEntryID))
            throw new ArgumentException($"'{nameof(journalEntryID)}' cannot be null or empty.", nameof(journalEntryID));
        if (debit < 0)
            throw new ArgumentException($"'{nameof(debit)}' cannot be negative.", nameof(debit));
        if (credit < 0)
            throw new ArgumentException($"'{nameof(credit)}' cannot be negative.", nameof(credit));
        if (debit == 0 && credit == 0)
            throw new ArgumentException($"Either '{nameof(debit)}' or '{nameof(credit)}' must be positive.", nameof(debit));
        if (date == default)
            throw new ArgumentException($"'{nameof(date)}' cannot be empty.", nameof(date));
        if (string.IsNullOrWhiteSpace(reference))
            throw new ArgumentException($"'{nameof(reference)}' cannot be null or empty.", nameof(reference));

        Id = id;
        Description = description;
        LedgerID = ledgerID;
        JournalEntryID = journalEntryID;
        Debit = debit;
        Credit = credit;
        Date = date;
        Reference = reference;
    }

    /// <summary>
    /// Abstract factory method for mutation methods. Must be implemented in derived types.
    /// </summary>
    /// <param name="id">The unique identifier for the ledger entry.</param>
    /// <param name="description">The description of the ledger entry.</param>
    /// <param name="ledgerID">The identifier of the ledger this entry belongs to.</param>
    /// <param name="journalEntryID">The identifier of the journal entry this ledger entry is derived from.</param>
    /// <param name="debit">The debit amount.</param>
    /// <param name="credit">The credit amount.</param>
    /// <param name="date">The date of the ledger entry.</param>
    /// <param name="reference">The reference string for the ledger entry.</param>
    /// <returns>A new instance of the derived ledger entry type.</returns>
    protected abstract TSelf CreateInstance(
        Guid id,
        string description,
        string ledgerID,
        string journalEntryID,
        decimal debit,
        decimal credit,
        DateTime date,
        string reference);

    /// <summary>
    /// Returns a new instance with the specified description.
    /// </summary>
    public TSelf WithDescription(string newDescription)
        => CreateInstance(Id, newDescription, LedgerID, JournalEntryID, Debit, Credit, Date, Reference);

    /// <summary>
    /// Returns a new instance with the specified ledger ID.
    /// </summary>
    public TSelf WithLedgerID(string newLedgerID)
        => CreateInstance(Id, Description, newLedgerID, JournalEntryID, Debit, Credit, Date, Reference);

    /// <summary>
    /// Returns a new instance with the specified journal entry ID.
    /// </summary>
    public TSelf WithJournalEntryID(string newJournalEntryID)
        => CreateInstance(Id, Description, LedgerID, newJournalEntryID, Debit, Credit, Date, Reference);

    /// <summary>
    /// Returns a new instance with the specified debit amount.
    /// </summary>
    public TSelf WithDebit(decimal newDebit)
        => CreateInstance(Id, Description, LedgerID, JournalEntryID, newDebit, Credit, Date, Reference);

    /// <summary>
    /// Returns a new instance with the specified credit amount.
    /// </summary>
    public TSelf WithCredit(decimal newCredit)
        => CreateInstance(Id, Description, LedgerID, JournalEntryID, Debit, newCredit, Date, Reference);

    /// <summary>
    /// Returns a new instance with the specified date.
    /// </summary>
    public TSelf WithDate(DateTime newDate)
        => CreateInstance(Id, Description, LedgerID, JournalEntryID, Debit, Credit, newDate, Reference);

    /// <summary>
    /// Returns a new instance with the specified reference.
    /// </summary>
    public TSelf WithReference(string newReference)
        => CreateInstance(Id, Description, LedgerID, JournalEntryID, Debit, Credit, Date, newReference);
}