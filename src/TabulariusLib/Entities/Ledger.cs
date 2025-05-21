/*
 * Ledger.cs
 * 
 * Represents a concrete implementation of a ledger entity in the Tabularius accounting library.
 * 
 * This record provides a strongly-typed, immutable ledger entity, inheriting from LedgerBase<Ledger, LedgerAccount>.
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
/// Represents a concrete ledger entity in the Tabularius accounting library.
/// Inherits from <see cref="LedgerBase{Ledger, LedgerAccount}"/> and provides factory methods for creation and mutation.
/// </summary>
[Table("Ledgers")]
public sealed record Ledger : LedgerBase<Ledger, LedgerAccount>
{
    /// <summary>
    /// Private parameterless constructor for EF Core.
    /// </summary>
    private Ledger() : base() { }

    /// <summary>
    /// Private full constructor for controlled creation and validation.
    /// </summary>
    /// <param name="id">The unique identifier for the ledger.</param>
    /// <param name="name">The name of the ledger.</param>
    /// <param name="description">The description of the ledger.</param>
    /// <param name="ledgerAccounts">The collection of ledger accounts.</param>
    private Ledger(Guid id, string name, string description, IEnumerable<LedgerAccount> ledgerAccounts)
        : base(id, name, description, ledgerAccounts)
    { }

    /// <summary>
    /// Factory method for creation with validation.
    /// </summary>
    /// <param name="id">The unique identifier for the ledger.</param>
    /// <param name="name">The name of the ledger.</param>
    /// <param name="description">The description of the ledger.</param>
    /// <param name="ledgerAccounts">The collection of ledger accounts. If null, an empty collection is used.</param>
    /// <returns>A new <see cref="Ledger"/> instance.</returns>
    public static Ledger Create(Guid id, string name, string description, IEnumerable<LedgerAccount>? ledgerAccounts)
        => new(id, name, description, ledgerAccounts ?? Enumerable.Empty<LedgerAccount>());

    /// <summary>
    /// Implementation of the abstract factory method for mutation methods.
    /// </summary>
    /// <param name="id">The unique identifier for the ledger.</param>
    /// <param name="name">The name of the ledger.</param>
    /// <param name="description">The description of the ledger.</param>
    /// <param name="ledgerAccounts">The collection of ledger accounts.</param>
    /// <returns>A new <see cref="Ledger"/> instance with the specified values.</returns>
    protected override Ledger CreateInstance(Guid id, string name, string description, IEnumerable<LedgerAccount> ledgerAccounts)
        => new Ledger(id, name, description, ledgerAccounts);

    /// <summary>
    /// Strongly-typed factory method to create a <see cref="Ledger"/> from a <see cref="Journal"/> and a collection of <see cref="Account"/>.
    /// </summary>
    /// <param name="name">The name of the ledger.</param>
    /// <param name="description">The description of the ledger.</param>
    /// <param name="journal">The journal instance to convert from.</param>
    /// <param name="accounts">The collection of accounts to include in the ledger.</param>
    /// <returns>A new <see cref="Ledger"/> instance created from the journal and accounts.</returns>
    public static Ledger FromJournal(string name, string description, Journal journal, IEnumerable<Account> accounts)
        => FromJournal<Journal, JournalEntry, JournalLine, Account, LedgerEntry>(
            name,
            description,
            journal,
            accounts,
            LedgerEntry.Create,
            LedgerAccount.Create,
            (id, n, d, ledgerAccounts) => new Ledger(id, n, d, ledgerAccounts)
        );
}