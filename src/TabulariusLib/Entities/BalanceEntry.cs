/*
 * BalanceEntry.cs
 * 
 * Represents a concrete implementation of a balance entry entity in the Tabularius accounting library.
 * 
 * This record provides a strongly-typed, immutable balance entry entity, inheriting from BalanceEntryBase<BalanceEntry>.
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
/// Represents a concrete balance entry entity in the Tabularius accounting library.
/// Inherits from <see cref="BalanceEntryBase{BalanceEntry}"/> and provides factory methods for creation and mutation.
/// </summary>
[Table("BalanceEntries")]
public sealed record BalanceEntry : BalanceEntryBase<BalanceEntry>
{
    /// <summary>
    /// Private parameterless constructor for EF Core.
    /// </summary>
    private BalanceEntry() : base() { }

    /// <summary>
    /// Private full constructor for controlled creation and validation.
    /// </summary>
    /// <param name="accountID">The unique identifier for the account.</param>
    /// <param name="accountName">The name of the account.</param>
    /// <param name="type">The account type (Asset, Liability, etc.).</param>
    /// <param name="parentCode">The parent account code, if any.</param>
    /// <param name="balance">The balance amount for this entry.</param>
    /// <param name="normally">Indicates whether the account is normally Debit or Credit.</param>
    private BalanceEntry(string accountID, string accountName, AccountType type, string? parentCode, decimal balance, string normally)
        : base(accountID, accountName, type, parentCode, balance, normally)
    { }

    /// <summary>
    /// Factory method for creation with validation.
    /// </summary>
    /// <param name="accountID">The unique identifier for the account.</param>
    /// <param name="accountName">The name of the account.</param>
    /// <param name="type">The account type (Asset, Liability, etc.).</param>
    /// <param name="parentCode">The parent account code, if any.</param>
    /// <param name="balance">The balance amount for this entry.</param>
    /// <param name="normally">Indicates whether the account is normally Debit or Credit.</param>
    /// <returns>A new <see cref="BalanceEntry"/> instance.</returns>
    public static BalanceEntry Create(string accountID, string accountName, AccountType type, string? parentCode, decimal balance, string normally)
        => new BalanceEntry(accountID, accountName, type, parentCode, balance, normally);

    /// <summary>
    /// Implementation of the abstract factory method for mutation methods.
    /// </summary>
    /// <param name="accountID">The unique identifier for the account.</param>
    /// <param name="accountName">The name of the account.</param>
    /// <param name="type">The account type (Asset, Liability, etc.).</param>
    /// <param name="parentCode">The parent account code, if any.</param>
    /// <param name="balance">The balance amount for this entry.</param>
    /// <param name="normally">Indicates whether the account is normally Debit or Credit.</param>
    /// <returns>A new <see cref="BalanceEntry"/> instance with the specified values.</returns>
    protected override BalanceEntry CreateInstance(string accountID, string accountName, AccountType type, string? parentCode, decimal balance, string normally)
        => new BalanceEntry(accountID, accountName, type, parentCode, balance, normally);
}