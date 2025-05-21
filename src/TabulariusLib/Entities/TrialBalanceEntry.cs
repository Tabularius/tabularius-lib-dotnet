/*
 * TrialBalanceEntry.cs
 * 
 * Represents a concrete implementation of a trial balance entry entity in the Tabularius accounting library.
 * 
 * This record provides a strongly-typed, immutable trial balance entry entity, inheriting from TrialBalanceEntryBase<TrialBalanceEntry>.
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
/// Represents a concrete trial balance entry entity in the Tabularius accounting library.
/// Inherits from <see cref="TrialBalanceEntryBase{TrialBalanceEntry}"/> and provides factory methods for creation and mutation.
/// </summary>
[Table("TrialBalanceEntries")]
public sealed record TrialBalanceEntry : TrialBalanceEntryBase<TrialBalanceEntry>
{
    /// <summary>
    /// Private parameterless constructor for EF Core.
    /// </summary>
    private TrialBalanceEntry() : base() { }

    /// <summary>
    /// Private full constructor for controlled creation and validation.
    /// </summary>
    /// <param name="accountID">The unique identifier for the account.</param>
    /// <param name="accountName">The name of the account.</param>
    /// <param name="type">The account type (Asset, Liability, Equity, Income, Expense, etc.).</param>
    /// <param name="parentCode">The parent account code, if any.</param>
    /// <param name="debit">The debit amount for this trial balance entry.</param>
    /// <param name="credit">The credit amount for this trial balance entry.</param>
    /// <param name="normally">Indicates whether the account is normally Debit or Credit.</param>
    private TrialBalanceEntry(string accountID, string accountName, AccountType type, string? parentCode, decimal debit, decimal credit, string normally)
        : base(accountID, accountName, type, parentCode, debit, credit, normally)
    { }

    /// <summary>
    /// Factory method for creation with validation.
    /// </summary>
    /// <param name="accountID">The unique identifier for the account.</param>
    /// <param name="accountName">The name of the account.</param>
    /// <param name="type">The account type (Asset, Liability, Equity, Income, Expense, etc.).</param>
    /// <param name="parentCode">The parent account code, if any.</param>
    /// <param name="debit">The debit amount for this trial balance entry.</param>
    /// <param name="credit">The credit amount for this trial balance entry.</param>
    /// <param name="normally">Indicates whether the account is normally Debit or Credit.</param>
    /// <returns>A new <see cref="TrialBalanceEntry"/> instance.</returns>
    public static TrialBalanceEntry Create(string accountID, string accountName, AccountType type, string? parentCode, decimal debit, decimal credit, string normally)
        => new TrialBalanceEntry(accountID, accountName, type, parentCode, debit, credit, normally);

    /// <summary>
    /// Implementation of the abstract factory method for mutation methods.
    /// </summary>
    /// <param name="accountID">The unique identifier for the account.</param>
    /// <param name="accountName">The name of the account.</param>
    /// <param name="type">The account type (Asset, Liability, Equity, Income, Expense, etc.).</param>
    /// <param name="parentCode">The parent account code, if any.</param>
    /// <param name="debit">The debit amount for this trial balance entry.</param>
    /// <param name="credit">The credit amount for this trial balance entry.</param>
    /// <param name="normally">Indicates whether the account is normally Debit or Credit.</param>
    /// <returns>A new <see cref="TrialBalanceEntry"/> instance with the specified values.</returns>
    protected override TrialBalanceEntry CreateInstance(string accountID, string accountName, AccountType type, string? parentCode, decimal debit, decimal credit, string normally)
        => new TrialBalanceEntry(accountID, accountName, type, parentCode, debit, credit, normally);
}