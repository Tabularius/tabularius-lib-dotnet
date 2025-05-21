/*
 * TrialBalanceBase.cs
 * 
 * Abstract base record for trial balance entities in the Tabularius accounting library.
 * 
 * This class provides a strongly-typed, immutable base for trial balance records using the Curiously Recurring Template Pattern (CRTP).
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
/// Abstract base record for trial balance entities, using CRTP for strongly-typed mutation methods.
/// </summary>
/// <typeparam name="TSelf">The concrete trial balance type inheriting from this base.</typeparam>
/// <typeparam name="TTrialBalanceEntry">The trial balance entry type associated with this trial balance.</typeparam>
public abstract record TrialBalanceBase<TSelf, TTrialBalanceEntry>
    where TSelf : TrialBalanceBase<TSelf, TTrialBalanceEntry>
    where TTrialBalanceEntry : TrialBalanceEntryBase<TTrialBalanceEntry>
{
    /// <summary>
    /// The unique identifier for the trial balance (primary key).
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; private set; }

    /// <summary>
    /// The name of the trial balance.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public string Name { get; private set; }

    /// <summary>
    /// The description of the trial balance.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public string Description { get; private set; }

    /// <summary>
    /// The date of the trial balance.
    /// </summary>
    [Required]
    public DateTime Date { get; private set; }

    /// <summary>
    /// Indicates whether the trial balance is closed.
    /// </summary>
    [Required]
    public bool IsClosed { get; protected set; } = false;

    /// <summary>
    /// The list of trial balance entries associated with this trial balance.
    /// </summary>
    protected List<TTrialBalanceEntry> _trialBalanceEntries { get; set; } = new();

    /// <summary>
    /// Gets a read-only collection of trial balance entries.
    /// </summary>
    public IReadOnlyCollection<TTrialBalanceEntry> TrialBalanceEntries => _trialBalanceEntries.AsReadOnly();

    /// <summary>
    /// Gets the total debit amount for this trial balance.
    /// </summary>
    public decimal TotalDebit => _trialBalanceEntries.Sum(entry => entry.Debit);

    /// <summary>
    /// Gets the total credit amount for this trial balance.
    /// </summary>
    public decimal TotalCredit => _trialBalanceEntries.Sum(entry => entry.Credit);

    /// <summary>
    /// Gets the calculated balance (TotalCredit - TotalDebit).
    /// </summary>
    public decimal Balance => TotalCredit - TotalDebit;

    /// <summary>
    /// Indicates whether the trial balance is balanced (i.e., Balance == 0).
    /// </summary>
    public bool IsBalanced => Balance == 0;

    /// <summary>
    /// Protected parameterless constructor for EF Core.
    /// </summary>
    protected TrialBalanceBase()
    {
        Name = string.Empty;
        Description = string.Empty;
        Date = default;
    }

    /// <summary>
    /// Protected constructor with validation for all properties.
    /// </summary>
    /// <param name="id">The unique identifier for the trial balance.</param>
    /// <param name="name">The name of the trial balance.</param>
    /// <param name="description">The description of the trial balance.</param>
    /// <param name="date">The date of the trial balance.</param>
    /// <param name="trialBalanceEntries">The collection of trial balance entries.</param>
    /// <param name="isClosed">Indicates whether the trial balance is closed.</param>
    /// <exception cref="ArgumentException">Thrown if any required argument is invalid.</exception>
    /// <exception cref="ArgumentNullException">Thrown if trialBalanceEntries is null.</exception>
    protected TrialBalanceBase(Guid id, string name, string description, DateTime date, IEnumerable<TTrialBalanceEntry> trialBalanceEntries, bool isClosed = false)
    {
        if (id == Guid.Empty)
            throw new ArgumentException($"'{nameof(id)}' cannot be empty.", nameof(id));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException($"'{nameof(description)}' cannot be null or empty.", nameof(description));
        if (trialBalanceEntries == null)
            throw new ArgumentNullException(nameof(trialBalanceEntries));

        Id = id;
        Name = name;
        Description = description;
        Date = date;
        _trialBalanceEntries = trialBalanceEntries.ToList();
        IsClosed = isClosed;
    }

    /// <summary>
    /// Abstract factory method for mutation methods. Must be implemented in derived types.
    /// </summary>
    /// <param name="id">The unique identifier for the trial balance.</param>
    /// <param name="name">The name of the trial balance.</param>
    /// <param name="description">The description of the trial balance.</param>
    /// <param name="date">The date of the trial balance.</param>
    /// <param name="trialBalanceEntries">The collection of trial balance entries.</param>
    /// <param name="isClosed">Indicates whether the trial balance is closed.</param>
    /// <returns>A new instance of the derived trial balance type.</returns>
    protected abstract TSelf CreateInstance(Guid id, string name, string description, DateTime date, IEnumerable<TTrialBalanceEntry> trialBalanceEntries, bool isClosed);

    /// <summary>
    /// Returns a new instance with the specified name.
    /// </summary>
    public TSelf WithName(string newName)
        => CreateInstance(Id, newName, Description, Date, TrialBalanceEntries, IsClosed);

    /// <summary>
    /// Returns a new instance with the specified description.
    /// </summary>
    public TSelf WithDescription(string newDescription)
        => CreateInstance(Id, Name, newDescription, Date, TrialBalanceEntries, IsClosed);

    /// <summary>
    /// Returns a new instance with the specified date.
    /// </summary>
    public TSelf WithDate(DateTime newDate)
        => CreateInstance(Id, Name, Description, newDate, TrialBalanceEntries, IsClosed);

    /// <summary>
    /// Returns a new instance with the specified trial balance entries.
    /// </summary>
    public TSelf WithTrialBalanceEntries(IEnumerable<TTrialBalanceEntry> newEntries)
        => CreateInstance(Id, Name, Description, Date, newEntries, IsClosed);

    /// <summary>
    /// Returns a new instance with the specified closed state.
    /// </summary>
    public TSelf WithIsClosed(bool isClosed)
        => CreateInstance(Id, Name, Description, Date, TrialBalanceEntries, isClosed);

    /// <summary>
    /// Returns a new instance with the specified trial balance entry added.
    /// </summary>
    /// <param name="entry">The trial balance entry to add.</param>
    /// <returns>A new instance of the trial balance with the entry added.</returns>
    /// <exception cref="ArgumentNullException">Thrown if entry is null.</exception>
    public TSelf AddTrialBalanceEntry(TTrialBalanceEntry entry)
    {
        if (entry == null)
            throw new ArgumentNullException(nameof(entry));
        return CreateInstance(Id, Name, Description, Date, TrialBalanceEntries.Append(entry), IsClosed);
    }

    /// <summary>
    /// Factory method to create a TrialBalance from a Ledger.
    /// </summary>
    /// <typeparam name="TLedger">The ledger type.</typeparam>
    /// <typeparam name="TLedgerAccount">The ledger account type.</typeparam>
    /// <typeparam name="TLedgerEntry">The ledger entry type.</typeparam>
    /// <param name="name">The name of the trial balance.</param>
    /// <param name="description">The description of the trial balance.</param>
    /// <param name="upToDate">The date up to which entries are considered.</param>
    /// <param name="ledger">The ledger instance.</param>
    /// <param name="entryFactory">A factory function to create trial balance entries from ledger entries.</param>
    /// <param name="trialBalanceFactory">A factory function to create the trial balance instance.</param>
    /// <returns>A new trial balance instance created from the ledger.</returns>
    /// <exception cref="ArgumentNullException">Thrown if ledger is null.</exception>
    /// <exception cref="ArgumentException">Thrown if name, description, or ledger accounts are invalid.</exception>
    public static TSelf FromLedger<TLedger, TLedgerAccount, TLedgerEntry>(
        string name,
        string description,
        DateTime upToDate,
        TLedger ledger,
        Func<string, string, AccountType, string?, decimal, decimal, string, TTrialBalanceEntry> entryFactory,
        Func<Guid, string, string, DateTime, IEnumerable<TTrialBalanceEntry>, TSelf> trialBalanceFactory)
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

        var ledgerAccountsProp = typeof(TLedger).GetProperty("LedgerAccounts");
        if (ledgerAccountsProp == null)
            throw new ArgumentException("Ledger type does not have required property 'LedgerAccounts'.");
        var ledgerAccounts = (IEnumerable<TLedgerAccount>)ledgerAccountsProp.GetValue(ledger)!;

        if (ledgerAccounts == null || !ledgerAccounts.Any())
            throw new ArgumentException($"No ledger accounts found.", nameof(ledgerAccounts));
        if (ledgerAccounts.Any(account => account == null))
            throw new ArgumentException($"Some ledger accounts are null.", nameof(ledgerAccounts));

        var ledgerEntriesProp = typeof(TLedgerAccount).GetProperty("LedgerEntries");
        if (ledgerEntriesProp == null)
            throw new ArgumentException("LedgerAccount type does not have required property 'LedgerEntries'.");

        var trialBalanceEntries = ledgerAccounts
            .Select(account =>
            {
                var code = (string)typeof(TLedgerAccount).GetProperty("Code")!.GetValue(account)!;
                var nameProp = (string)typeof(TLedgerAccount).GetProperty("Name")!.GetValue(account)!;
                var type = (AccountType)typeof(TLedgerAccount).GetProperty("Type")!.GetValue(account)!;
                var parentCode = (string?)typeof(TLedgerAccount).GetProperty("ParentCode")!.GetValue(account);
                var normally = (string)typeof(TLedgerAccount).GetProperty("Normally")!.GetValue(account)!;
                var ledgerEntries = (IEnumerable<TLedgerEntry>)ledgerEntriesProp.GetValue(account)!;
                if (ledgerEntries == null)
                    throw new ArgumentException($"Some ledger accounts have null ledger entries.", nameof(ledgerAccounts));
                if (ledgerEntries.Any(entry => entry == null))
                    throw new ArgumentException($"Some ledger entries are null.", nameof(ledgerAccounts));
                var filteredEntries = ledgerEntries.Where(e =>
                    (DateTime)typeof(TLedgerEntry).GetProperty("Date")!.GetValue(e)! <= upToDate);
                var debit = filteredEntries.Sum(e => (decimal)typeof(TLedgerEntry).GetProperty("Debit")!.GetValue(e)!);
                var credit = filteredEntries.Sum(e => (decimal)typeof(TLedgerEntry).GetProperty("Credit")!.GetValue(e)!);
                return entryFactory(code, nameProp, type, parentCode, debit, credit, normally);
            })
            .ToList();

        if (!trialBalanceEntries.Any())
            throw new ArgumentException($"No account entries found.", nameof(trialBalanceEntries));

        return trialBalanceFactory(Guid.NewGuid(), name, description, upToDate, trialBalanceEntries);
    }
}