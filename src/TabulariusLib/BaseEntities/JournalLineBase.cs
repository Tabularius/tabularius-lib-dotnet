/*
 * JournalLineBase.cs
 * 
 * Abstract base record for journal line entities in the Tabularius accounting library.
 * 
 * This class provides a strongly-typed, immutable base for journal line records using the Curiously Recurring Template Pattern (CRTP).
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
/// Abstract base record for journal line entities, using CRTP for strongly-typed mutation methods.
/// </summary>
/// <typeparam name="TSelf">The concrete journal line type inheriting from this base.</typeparam>
/// <typeparam name="TAccount">The account type associated with this journal line.</typeparam>
public abstract record JournalLineBase<TSelf, TAccount>
    where TSelf : JournalLineBase<TSelf, TAccount>
    where TAccount : AccountBase<TAccount>
{
    /// <summary>
    /// The unique identifier for the journal line (primary key).
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; private set; }

    /// <summary>
    /// The description of the journal line.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public string Description { get; private set; }

    /// <summary>
    /// The account identifier associated with this journal line.
    /// </summary>
    [Required]
    public string AccountID { get; private set; }

    /// <summary>
    /// The debit amount for this journal line.
    /// </summary>
    public decimal Debit { get; private set; }

    /// <summary>
    /// The credit amount for this journal line.
    /// </summary>
    public decimal Credit { get; private set; }

    /// <summary>
    /// Protected parameterless constructor for EF Core.
    /// </summary>
    protected JournalLineBase()
    {
        Description = string.Empty;
        AccountID = string.Empty;
    }

    /// <summary>
    /// Protected constructor with validation for all properties.
    /// </summary>
    /// <param name="id">The unique identifier for the journal line.</param>
    /// <param name="description">The description of the journal line.</param>
    /// <param name="accountID">The account identifier associated with this journal line.</param>
    /// <param name="debit">The debit amount.</param>
    /// <param name="credit">The credit amount.</param>
    /// <exception cref="ArgumentException">Thrown if any required argument is invalid.</exception>
    protected JournalLineBase(Guid id, string description, string accountID, decimal debit, decimal credit)
    {
        Id = id;
        Description = description;
        AccountID = accountID;
        Debit = debit;
        Credit = credit;
        Validate();
    }

    /// <summary>
    /// Abstract factory method for mutation methods. Must be implemented in derived types.
    /// </summary>
    /// <param name="id">The unique identifier for the journal line.</param>
    /// <param name="description">The description of the journal line.</param>
    /// <param name="accountID">The account identifier associated with this journal line.</param>
    /// <param name="debit">The debit amount.</param>
    /// <param name="credit">The credit amount.</param>
    /// <returns>A new instance of the derived journal line type.</returns>
    protected abstract TSelf CreateInstance(Guid id, string description, string accountID, decimal debit, decimal credit);

    /// <summary>
    /// Validates the journal line properties.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown if any property is invalid.</exception>
    protected void Validate()
    {
        if (Id == Guid.Empty)
            throw new ArgumentException("Id cannot be empty", nameof(Id));

        if (string.IsNullOrWhiteSpace(Description))
            throw new ArgumentException("Description cannot be null/empty", nameof(Description));

        if (string.IsNullOrWhiteSpace(AccountID))
            throw new ArgumentException("AccountID cannot be null/empty", nameof(AccountID));

        if (Debit == 0 && Credit == 0)
            throw new ArgumentException("Either debit or credit must be non-zero", nameof(Debit));

        if (Debit != 0 && Credit != 0)
            throw new ArgumentException("Either debit or credit must be zero", nameof(Debit));
    }

    /// <summary>
    /// Returns a new instance with the specified description.
    /// </summary>
    public TSelf WithDescription(string newDescription)
        => CreateInstance(Id, newDescription, AccountID, Debit, Credit);

    /// <summary>
    /// Returns a new instance with the specified account ID.
    /// </summary>
    public TSelf WithAccountID(string newAccountID)
        => CreateInstance(Id, Description, newAccountID, Debit, Credit);

    /// <summary>
    /// Returns a new instance with the specified debit amount.
    /// </summary>
    public TSelf WithDebit(decimal newDebit)
        => CreateInstance(Id, Description, AccountID, newDebit, Credit);

    /// <summary>
    /// Returns a new instance with the specified credit amount.
    /// </summary>
    public TSelf WithCredit(decimal newCredit)
        => CreateInstance(Id, Description, AccountID, Debit, newCredit);

    /// <summary>
    /// Returns a new instance with the specified account.
    /// </summary>
    public TSelf WithAccount(TAccount newAccount)
        => CreateInstance(Id, Description, newAccount.Code, Debit, Credit);
}