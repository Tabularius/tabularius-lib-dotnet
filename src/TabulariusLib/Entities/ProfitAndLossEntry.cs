/*
 * ProfitAndLossEntry.cs
 * 
 * Represents a concrete implementation of a profit and loss entry entity in the Tabularius accounting library.
 * 
 * This record provides a strongly-typed, immutable profit and loss entry entity, inheriting from ProfitAndLossEntryBase<ProfitAndLossEntry>.
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
/// Represents a concrete profit and loss entry entity in the Tabularius accounting library.
/// Inherits from <see cref="ProfitAndLossEntryBase{ProfitAndLossEntry}"/> and provides factory methods for creation and mutation.
/// </summary>
[Table("ProfitAndLossEntries")]
public sealed record ProfitAndLossEntry : ProfitAndLossEntryBase<ProfitAndLossEntry>
{
    /// <summary>
    /// Private parameterless constructor for EF Core.
    /// </summary>
    private ProfitAndLossEntry() : base() { }

    /// <summary>
    /// Private full constructor for controlled creation and validation.
    /// </summary>
    /// <param name="accountID">The unique identifier for the account.</param>
    /// <param name="accountName">The name of the account.</param>
    /// <param name="type">The account type (Income or Expense).</param>
    /// <param name="amount">The amount for this profit and loss entry.</param>
    private ProfitAndLossEntry(string accountID, string accountName, AccountType type, decimal amount)
        : base(accountID, accountName, type, amount)
    { }

    /// <summary>
    /// Factory method for creation with validation.
    /// </summary>
    /// <param name="accountID">The unique identifier for the account.</param>
    /// <param name="accountName">The name of the account.</param>
    /// <param name="type">The account type (Income or Expense).</param>
    /// <param name="amount">The amount for this profit and loss entry.</param>
    /// <returns>A new <see cref="ProfitAndLossEntry"/> instance.</returns>
    public static ProfitAndLossEntry Create(string accountID, string accountName, AccountType type, decimal amount)
        => new ProfitAndLossEntry(accountID, accountName, type, amount);

    /// <summary>
    /// Implementation of the abstract factory method for mutation methods.
    /// </summary>
    /// <param name="accountID">The unique identifier for the account.</param>
    /// <param name="accountName">The name of the account.</param>
    /// <param name="type">The account type (Income or Expense).</param>
    /// <param name="amount">The amount for this profit and loss entry.</param>
    /// <returns>A new <see cref="ProfitAndLossEntry"/> instance with the specified values.</returns>
    protected override ProfitAndLossEntry CreateInstance(string accountID, string accountName, AccountType type, decimal amount)
        => new ProfitAndLossEntry(accountID, accountName, type, amount);
}