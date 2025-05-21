/*
 * Balance.cs
 * 
 * Represents a concrete implementation of a balance entity in the Tabularius accounting library.
 * 
 * This record provides a strongly-typed, immutable balance entity, inheriting from BalanceBase<BalanceEntry>.
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
/// Represents a concrete balance entity in the Tabularius accounting library.
/// Inherits from <see cref="BalanceBase{BalanceEntry}"/> and provides factory methods for creation and mutation.
/// </summary>
[Table("Balances")]
public sealed record Balance : BalanceBase<BalanceEntry>
{
    /// <summary>
    /// Private parameterless constructor for EF Core.
    /// </summary>
    private Balance() : base() { }

    /// <summary>
    /// Private full constructor for controlled creation and validation.
    /// </summary>
    /// <param name="id">The unique identifier for the balance.</param>
    /// <param name="name">The name of the balance.</param>
    /// <param name="description">The description of the balance.</param>
    /// <param name="date">The date of the balance.</param>
    /// <param name="entries">The collection of balance entries.</param>
    private Balance(Guid id, string name, string description, DateTime date, IEnumerable<BalanceEntry> entries)
        : base(id, name, description, date, entries)
    { }

    /// <summary>
    /// Factory method for creation with validation.
    /// </summary>
    /// <param name="id">The unique identifier for the balance.</param>
    /// <param name="name">The name of the balance.</param>
    /// <param name="description">The description of the balance.</param>
    /// <param name="date">The date of the balance.</param>
    /// <param name="entries">The collection of balance entries.</param>
    /// <returns>A new <see cref="Balance"/> instance.</returns>
    public static Balance Create(Guid id, string name, string description, DateTime date, IEnumerable<BalanceEntry> entries)
        => new(id, name, description, date, entries);

    /// <summary>
    /// Implementation of the abstract factory method for mutation methods.
    /// </summary>
    /// <param name="id">The unique identifier for the balance.</param>
    /// <param name="name">The name of the balance.</param>
    /// <param name="description">The description of the balance.</param>
    /// <param name="date">The date of the balance.</param>
    /// <param name="entries">The collection of balance entries.</param>
    /// <returns>A new <see cref="Balance"/> instance with the specified values.</returns>
    protected override BalanceBase<BalanceEntry> CreateInstance(Guid id, string name, string description, DateTime date, IEnumerable<BalanceEntry> entries)
        => new Balance(id, name, description, date, entries);

    /// <summary>
    /// Strongly-typed factory method to create a <see cref="Balance"/> from a <see cref="TrialBalance"/>.
    /// </summary>
    /// <param name="name">The name of the balance.</param>
    /// <param name="description">The description of the balance.</param>
    /// <param name="date">The date of the balance.</param>
    /// <param name="trialBalance">The trial balance instance to convert from.</param>
    /// <returns>A new <see cref="Balance"/> instance created from the trial balance.</returns>
    public static Balance FromTrialBalance(string name, string description, DateTime date, TrialBalance trialBalance)
        => FromTrialBalance<Balance, TrialBalance, TrialBalanceEntry>(
            name,
            description,
            date,
            trialBalance,
            // Entry factory: creates a BalanceEntry from a TrialBalanceEntry
            BalanceEntry.Create,
            // Balance factory: creates a Balance from the provided values
            (id, n, d, dt, entries) => new Balance(id, n, d, dt, entries)
        );
}