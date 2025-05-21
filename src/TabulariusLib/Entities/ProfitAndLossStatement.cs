/*
 * ProfitAndLossStatement.cs
 * 
 * Represents a concrete implementation of a profit and loss statement entity in the Tabularius accounting library.
 * 
 * This record provides a strongly-typed, immutable profit and loss statement entity, inheriting from ProfitAndLossStatementBase<ProfitAndLossStatement, ProfitAndLossEntry>.
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
/// Represents a concrete profit and loss statement entity in the Tabularius accounting library.
/// Inherits from <see cref="ProfitAndLossStatementBase{ProfitAndLossStatement, ProfitAndLossEntry}"/> and provides factory methods for creation and mutation.
/// </summary>
[Table("ProfitAndLossStatements")]
public sealed record ProfitAndLossStatement : ProfitAndLossStatementBase<ProfitAndLossStatement, ProfitAndLossEntry>
{
    /// <summary>
    /// Private parameterless constructor for EF Core.
    /// </summary>
    private ProfitAndLossStatement() : base() { }

    /// <summary>
    /// Private full constructor for controlled creation and validation.
    /// </summary>
    /// <param name="id">The unique identifier for the profit and loss statement.</param>
    /// <param name="name">The name of the statement.</param>
    /// <param name="description">The description of the statement.</param>
    /// <param name="startDate">The start date of the reporting period.</param>
    /// <param name="endDate">The end date of the reporting period.</param>
    /// <param name="entries">The collection of profit and loss entries.</param>
    private ProfitAndLossStatement(Guid id, string name, string description, DateTime startDate, DateTime endDate, IEnumerable<ProfitAndLossEntry> entries)
        : base(id, name, description, startDate, endDate, entries)
    { }

    /// <summary>
    /// Factory method for creation with validation.
    /// </summary>
    /// <param name="id">The unique identifier for the profit and loss statement.</param>
    /// <param name="name">The name of the statement.</param>
    /// <param name="description">The description of the statement.</param>
    /// <param name="startDate">The start date of the reporting period.</param>
    /// <param name="endDate">The end date of the reporting period.</param>
    /// <param name="entries">The collection of profit and loss entries. If null, an empty collection is used.</param>
    /// <returns>A new <see cref="ProfitAndLossStatement"/> instance.</returns>
    public static ProfitAndLossStatement Create(Guid id, string name, string description, DateTime startDate, DateTime endDate, IEnumerable<ProfitAndLossEntry>? entries)
        => new ProfitAndLossStatement(id, name, description, startDate, endDate, entries ?? Enumerable.Empty<ProfitAndLossEntry>());

    /// <summary>
    /// Implementation of the abstract factory method for mutation methods.
    /// </summary>
    /// <param name="id">The unique identifier for the profit and loss statement.</param>
    /// <param name="name">The name of the statement.</param>
    /// <param name="description">The description of the statement.</param>
    /// <param name="startDate">The start date of the reporting period.</param>
    /// <param name="endDate">The end date of the reporting period.</param>
    /// <param name="entries">The collection of profit and loss entries.</param>
    /// <returns>A new <see cref="ProfitAndLossStatement"/> instance with the specified values.</returns>
    protected override ProfitAndLossStatement CreateInstance(Guid id, string name, string description, DateTime startDate, DateTime endDate, IEnumerable<ProfitAndLossEntry> entries)
        => new ProfitAndLossStatement(id, name, description, startDate, endDate, entries);

    /// <summary>
    /// Strongly-typed factory method to create a <see cref="ProfitAndLossStatement"/> from a <see cref="Ledger"/>.
    /// </summary>
    /// <param name="name">The name of the statement.</param>
    /// <param name="description">The description of the statement.</param>
    /// <param name="startDate">The start date of the reporting period.</param>
    /// <param name="endDate">The end date of the reporting period.</param>
    /// <param name="ledger">The ledger instance to convert from.</param>
    /// <returns>A new <see cref="ProfitAndLossStatement"/> instance created from the ledger.</returns>
    public static ProfitAndLossStatement FromLedger(string name, string description, DateTime startDate, DateTime endDate, Ledger ledger)
        => FromLedger<Ledger, LedgerAccount, LedgerEntry>(
            name,
            description,
            startDate,
            endDate,
            ledger,
            ProfitAndLossEntry.Create,
            (id, n, d, sd, ed, entries) => new ProfitAndLossStatement(id, n, d, sd, ed, entries)
        );
}