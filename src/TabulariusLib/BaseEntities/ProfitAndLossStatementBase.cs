/*
 * ProfitAndLossStatementBase.cs
 * 
 * Abstract base record for profit and loss statement entities in the Tabularius accounting library.
 * 
 * This class provides a strongly-typed, immutable base for profit and loss statement records using the Curiously Recurring Template Pattern (CRTP).
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
/// Abstract base record for profit and loss statement entities, using CRTP for strongly-typed mutation methods.
/// </summary>
/// <typeparam name="TSelf">The concrete profit and loss statement type inheriting from this base.</typeparam>
/// <typeparam name="TEntry">The profit and loss entry type associated with this statement.</typeparam>
public abstract record ProfitAndLossStatementBase<TSelf, TEntry>
    where TSelf : ProfitAndLossStatementBase<TSelf, TEntry>
    where TEntry : ProfitAndLossEntryBase<TEntry>
{
    /// <summary>
    /// The unique identifier for the profit and loss statement (primary key).
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; private set; }

    /// <summary>
    /// The name of the profit and loss statement.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public string Name { get; private set; }

    /// <summary>
    /// The description of the profit and loss statement.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public string Description { get; private set; }

    /// <summary>
    /// The start date of the reporting period.
    /// </summary>
    [Required]
    public DateTime StartDate { get; private set; }

    /// <summary>
    /// The end date of the reporting period.
    /// </summary>
    [Required]
    public DateTime EndDate { get; private set; }

    /// <summary>
    /// The list of profit and loss entries associated with this statement.
    /// </summary>
    private List<TEntry> _entries { get; set; } = new();

    /// <summary>
    /// Gets a read-only collection of profit and loss entries.
    /// </summary>
    public IReadOnlyCollection<TEntry> Entries => _entries.AsReadOnly();

    /// <summary>
    /// Gets the total revenue (sum of all income entries).
    /// </summary>
    public decimal TotalRevenue => _entries.Where(e => e.Type == AccountType.Income).Sum(e => e.Amount);

    /// <summary>
    /// Gets the total expense (sum of all expense entries).
    /// </summary>
    public decimal TotalExpense => _entries.Where(e => e.Type == AccountType.Expense).Sum(e => e.Amount);

    /// <summary>
    /// Gets the net profit (TotalRevenue - TotalExpense).
    /// </summary>
    public decimal NetProfit => TotalRevenue - TotalExpense;

    /// <summary>
    /// Protected parameterless constructor for EF Core.
    /// </summary>
    protected ProfitAndLossStatementBase()
    {
        Name = string.Empty;
        Description = string.Empty;
        StartDate = default;
        EndDate = default;
    }

    /// <summary>
    /// Protected constructor with validation for all properties.
    /// </summary>
    /// <param name="id">The unique identifier for the statement.</param>
    /// <param name="name">The name of the statement.</param>
    /// <param name="description">The description of the statement.</param>
    /// <param name="startDate">The start date of the reporting period.</param>
    /// <param name="endDate">The end date of the reporting period.</param>
    /// <param name="entries">The collection of profit and loss entries.</param>
    /// <exception cref="ArgumentException">Thrown if any required argument is invalid.</exception>
    /// <exception cref="ArgumentNullException">Thrown if entries is null.</exception>
    protected ProfitAndLossStatementBase(Guid id, string name, string description, DateTime startDate, DateTime endDate, IEnumerable<TEntry> entries)
    {
        if (id == Guid.Empty)
            throw new ArgumentException($"'{nameof(id)}' cannot be empty.", nameof(id));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException($"'{nameof(description)}' cannot be null or empty.", nameof(description));
        if (startDate == default)
            throw new ArgumentException($"'{nameof(startDate)}' cannot be default.", nameof(startDate));
        if (endDate == default)
            throw new ArgumentException($"'{nameof(endDate)}' cannot be default.", nameof(endDate));
        if (entries == null)
            throw new ArgumentNullException(nameof(entries));
        Id = id;
        Name = name;
        Description = description;
        StartDate = startDate;
        EndDate = endDate;
        _entries = entries.ToList();
    }

    /// <summary>
    /// Abstract factory method for mutation methods. Must be implemented in derived types.
    /// </summary>
    /// <param name="id">The unique identifier for the statement.</param>
    /// <param name="name">The name of the statement.</param>
    /// <param name="description">The description of the statement.</param>
    /// <param name="startDate">The start date of the reporting period.</param>
    /// <param name="endDate">The end date of the reporting period.</param>
    /// <param name="entries">The collection of profit and loss entries.</param>
    /// <returns>A new instance of the derived profit and loss statement type.</returns>
    protected abstract TSelf CreateInstance(Guid id, string name, string description, DateTime startDate, DateTime endDate, IEnumerable<TEntry> entries);

    /// <summary>
    /// Returns a new instance with the specified name.
    /// </summary>
    public TSelf WithName(string newName)
        => CreateInstance(Id, newName, Description, StartDate, EndDate, Entries);

    /// <summary>
    /// Returns a new instance with the specified description.
    /// </summary>
    public TSelf WithDescription(string newDescription)
        => CreateInstance(Id, Name, newDescription, StartDate, EndDate, Entries);

    /// <summary>
    /// Returns a new instance with the specified start date.
    /// </summary>
    public TSelf WithStartDate(DateTime newStartDate)
        => CreateInstance(Id, Name, Description, newStartDate, EndDate, Entries);

    /// <summary>
    /// Returns a new instance with the specified end date.
    /// </summary>
    public TSelf WithEndDate(DateTime newEndDate)
        => CreateInstance(Id, Name, Description, StartDate, newEndDate, Entries);

    /// <summary>
    /// Returns a new instance with the specified entries.
    /// </summary>
    public TSelf WithEntries(IEnumerable<TEntry> newEntries)
        => CreateInstance(Id, Name, Description, StartDate, EndDate, newEntries);

    /// <summary>
    /// Returns a new instance with the specified entry added.
    /// </summary>
    /// <param name="entry">The profit and loss entry to add.</param>
    /// <returns>A new instance of the statement with the entry added.</returns>
    /// <exception cref="ArgumentNullException">Thrown if entry is null.</exception>
    public TSelf AddEntry(TEntry entry)
    {
        if (entry == null)
            throw new ArgumentNullException(nameof(entry));
        return CreateInstance(Id, Name, Description, StartDate, EndDate, Entries.Append(entry));
    }

    /// <summary>
    /// Factory method to create a profit and loss statement from a ledger.
    /// </summary>
    /// <typeparam name="TLedger">The ledger type.</typeparam>
    /// <typeparam name="TLedgerAccount">The ledger account type.</typeparam>
    /// <typeparam name="TLedgerEntry">The ledger entry type.</typeparam>
    /// <param name="name">The name of the statement.</param>
    /// <param name="description">The description of the statement.</param>
    /// <param name="startDate">The start date of the reporting period.</param>
    /// <param name="endDate">The end date of the reporting period.</param>
    /// <param name="ledger">The ledger instance.</param>
    /// <param name="entryFactory">A factory function to create profit and loss entries from ledger entries.</param>
    /// <param name="statementFactory">A factory function to create the statement instance.</param>
    /// <returns>A new profit and loss statement instance created from the ledger.</returns>
    /// <exception cref="ArgumentNullException">Thrown if ledger is null.</exception>
    /// <exception cref="ArgumentException">Thrown if name, description, startDate, or endDate is invalid.</exception>
    public static TSelf FromLedger<TLedger, TLedgerAccount, TLedgerEntry>(
        string name,
        string description,
        DateTime startDate,
        DateTime endDate,
        TLedger ledger,
        Func<string, string, AccountType, decimal, TEntry> entryFactory,
        Func<Guid, string, string, DateTime, DateTime, IEnumerable<TEntry>, TSelf> statementFactory)
        where TLedger : class
        where TLedgerAccount : class
        where TLedgerEntry : class
    {
        if (ledger == null)
            throw new ArgumentNullException(nameof(ledger));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException($"'{nameof(description)}' cannot be null or empty.", nameof(description));
        if (startDate == default)
            throw new ArgumentException($"'{nameof(startDate)}' cannot be default.", nameof(startDate));
        if (endDate == default)
            throw new ArgumentException($"'{nameof(endDate)}' cannot be default.", nameof(endDate));

        var ledgerAccountsProp = typeof(TLedger).GetProperty("LedgerAccounts");
        if (ledgerAccountsProp == null)
            throw new ArgumentException("Ledger type does not have required property 'LedgerAccounts'.");
        var ledgerAccounts = (IEnumerable<TLedgerAccount>)ledgerAccountsProp.GetValue(ledger)!;

        var typeProp = typeof(TLedgerAccount).GetProperty("Type");
        var codeProp = typeof(TLedgerAccount).GetProperty("Code");
        var nameProp = typeof(TLedgerAccount).GetProperty("Name");
        var ledgerEntriesProp = typeof(TLedgerAccount).GetProperty("LedgerEntries");
        var dateProp = typeof(TLedgerEntry).GetProperty("Date");
        var debitProp = typeof(TLedgerEntry).GetProperty("Debit");
        var creditProp = typeof(TLedgerEntry).GetProperty("Credit");

        var plEntries = ledgerAccounts
            .Where(acc => {
                var type = (AccountType)typeProp!.GetValue(acc)!;
                return type == AccountType.Income || type == AccountType.Expense;
            })
            .SelectMany(account =>
            {
                var accType = (AccountType)typeProp!.GetValue(account)!;
                var accCode = (string)codeProp!.GetValue(account)!;
                var accName = (string)nameProp!.GetValue(account)!;
                var ledgerEntries = (IEnumerable<TLedgerEntry>)ledgerEntriesProp!.GetValue(account)!;
                return ledgerEntries
                    .Where(entry =>
                    {
                        var entryDate = (DateTime)dateProp!.GetValue(entry)!;
                        return entryDate >= startDate && entryDate <= endDate;
                    })
                    .Select(entry =>
                        entryFactory(
                            accCode,
                            accName,
                            accType,
                            accType == AccountType.Income
                                ? (decimal)creditProp!.GetValue(entry)!
                                : (decimal)debitProp!.GetValue(entry)!
                        )
                    );
            })
            .ToList();

        return statementFactory(Guid.NewGuid(), name, description, startDate, endDate, plEntries);
    }
}