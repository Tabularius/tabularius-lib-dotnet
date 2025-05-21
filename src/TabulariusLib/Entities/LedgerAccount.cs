/*
 * LedgerAccount.cs
 * 
 * Represents a concrete implementation of a ledger account entity in the Tabularius accounting library.
 * 
 * This record provides a strongly-typed, immutable ledger account entity, inheriting from LedgerAccountBase<LedgerAccount, LedgerEntry>.
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
/// Represents a concrete ledger account entity in the Tabularius accounting library.
/// Inherits from <see cref="LedgerAccountBase{LedgerAccount, LedgerEntry}"/> and provides factory methods for creation and mutation.
/// </summary>
[Table("LedgerAccounts")]
public sealed record LedgerAccount : LedgerAccountBase<LedgerAccount, LedgerEntry>
{
    /// <summary>
    /// Private parameterless constructor for EF Core.
    /// </summary>
    private LedgerAccount() : base() { }

    /// <summary>
    /// Private full constructor for controlled creation and validation.
    /// </summary>
    /// <param name="code">The unique code for the ledger account.</param>
    /// <param name="name">The name of the ledger account.</param>
    /// <param name="type">The account type (Asset, Liability, etc.).</param>
    /// <param name="description">The description of the ledger account.</param>
    /// <param name="parentCode">The parent account code, if any.</param>
    /// <param name="normally">Indicates whether the account is normally Debit or Credit.</param>
    /// <param name="lines">The collection of ledger entries.</param>
    private LedgerAccount(
        string code,
        string name,
        AccountType type,
        string description,
        string parentCode,
        string normally,
        IEnumerable<LedgerEntry> lines)
        : base(code, name, type, description, parentCode, normally, lines)
    { }

    /// <summary>
    /// Factory method for creation with validation.
    /// </summary>
    /// <param name="code">The unique code for the ledger account.</param>
    /// <param name="name">The name of the ledger account.</param>
    /// <param name="type">The account type (Asset, Liability, etc.).</param>
    /// <param name="description">The description of the ledger account.</param>
    /// <param name="parentCode">The parent account code, if any.</param>
    /// <param name="normally">Indicates whether the account is normally Debit or Credit.</param>
    /// <param name="lines">The collection of ledger entries. If null, an empty collection is used.</param>
    /// <returns>A new <see cref="LedgerAccount"/> instance.</returns>
    public static LedgerAccount Create(
        string code,
        string name,
        AccountType type,
        string description,
        string parentCode,
        string normally,
        IEnumerable<LedgerEntry>? lines)
        => new LedgerAccount(code, name, type, description, parentCode, normally, lines ?? Enumerable.Empty<LedgerEntry>());

    /// <summary>
    /// Implementation of the abstract factory method for mutation methods.
    /// </summary>
    /// <param name="code">The unique code for the ledger account.</param>
    /// <param name="name">The name of the ledger account.</param>
    /// <param name="type">The account type (Asset, Liability, etc.).</param>
    /// <param name="description">The description of the ledger account.</param>
    /// <param name="parentCode">The parent account code, if any.</param>
    /// <param name="normally">Indicates whether the account is normally Debit or Credit.</param>
    /// <param name="lines">The collection of ledger entries.</param>
    /// <returns>A new <see cref="LedgerAccount"/> instance with the specified values.</returns>
    protected override LedgerAccount CreateInstance(
        string code,
        string name,
        AccountType type,
        string description,
        string parentCode,
        string normally,
        IEnumerable<LedgerEntry> lines)
        => new LedgerAccount(code, name, type, description, parentCode, normally, lines);
}