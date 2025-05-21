/*
 * BalanceBase.cs
 * 
 * Abstract base record for balance entities in the Tabularius accounting library.
 * 
 * This class provides a strongly-typed, immutable base for balance records using the Curiously Recurring Template Pattern (CRTP).
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
/// Abstract base record for balance entities, using CRTP for strongly-typed mutation methods.
/// </summary>
/// <typeparam name="TBalanceEntry">The concrete balance entry type inheriting from <see cref="BalanceEntryBase{TBalanceEntry}"/>.</typeparam>
public abstract record BalanceBase<TBalanceEntry>
    where TBalanceEntry : BalanceEntryBase<TBalanceEntry>
{
    /// <summary>
    /// The unique identifier for the balance (primary key).
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; private set; }

    /// <summary>
    /// The name of the balance.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public string Name { get; private set; }

    /// <summary>
    /// The description of the balance.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public string Description { get; private set; }

    /// <summary>
    /// The date of the balance.
    /// </summary>
    [Required]
    public DateTime Date { get; private set; }

    /// <summary>
    /// The list of balance entries associated with this balance.
    /// </summary>
    private List<TBalanceEntry> _balanceEntries { get; set; } = new();

    /// <summary>
    /// Gets a read-only collection of balance entries.
    /// </summary>
    public IReadOnlyCollection<TBalanceEntry> BalanceEntries => _balanceEntries.AsReadOnly();

    /// <summary>
    /// Gets the total amount of assets in the balance.
    /// </summary>
    public decimal TotalAssets => _balanceEntries.Where(e => e.Type == AccountType.Asset).Sum(e => e.Balance);

    /// <summary>
    /// Gets the total amount of liabilities in the balance.
    /// </summary>
    public decimal TotalLiabilities => _balanceEntries.Where(e => e.Type == AccountType.Liability).Sum(e => e.Balance);

    /// <summary>
    /// Gets the total amount of equity in the balance.
    /// </summary>
    public decimal TotalEquity => _balanceEntries.Where(e => e.Type == AccountType.Equity).Sum(e => e.Balance);

    /// <summary>
    /// Gets the calculated balance amount (Assets - (Liabilities + Equity)).
    /// </summary>
    public decimal BalanceAmount => TotalAssets - (TotalLiabilities + TotalEquity);

    /// <summary>
    /// Indicates whether the balance is balanced (i.e., BalanceAmount == 0).
    /// </summary>
    public bool IsBalanced => BalanceAmount == 0;

    /// <summary>
    /// Protected parameterless constructor for EF Core.
    /// </summary>
    protected BalanceBase()
    {
        Name = string.Empty;
        Description = string.Empty;
        Date = default;
    }

    /// <summary>
    /// Protected constructor with validation for all properties.
    /// </summary>
    /// <param name="id">The unique identifier for the balance.</param>
    /// <param name="name">The name of the balance.</param>
    /// <param name="description">The description of the balance.</param>
    /// <param name="date">The date of the balance.</param>
    /// <param name="entries">The collection of balance entries.</param>
    /// <exception cref="ArgumentException">Thrown if any required argument is invalid.</exception>
    protected BalanceBase(Guid id, string name, string description, DateTime date, IEnumerable<TBalanceEntry> entries)
    {
        if (id == Guid.Empty)
            throw new ArgumentException($"'{nameof(id)}' cannot be empty.", nameof(id));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException($"'{nameof(description)}' cannot be null or empty.", nameof(description));
        if (date == default)
            throw new ArgumentException($"'{nameof(date)}' cannot be default.", nameof(date));
        if (entries == null)
            throw new ArgumentNullException(nameof(entries));

        Id = id;
        Name = name;
        Description = description;
        Date = date;
        _balanceEntries = entries.ToList();
    }

    /// <summary>
    /// Abstract factory method for mutation methods. Must be implemented in derived types.
    /// </summary>
    /// <param name="id">The unique identifier for the balance.</param>
    /// <param name="name">The name of the balance.</param>
    /// <param name="description">The description of the balance.</param>
    /// <param name="date">The date of the balance.</param>
    /// <param name="entries">The collection of balance entries.</param>
    /// <returns>A new instance of the derived balance type.</returns>
    protected abstract BalanceBase<TBalanceEntry> CreateInstance(Guid id, string name, string description, DateTime date, IEnumerable<TBalanceEntry> entries);

    /// <summary>
    /// Returns a new instance with the specified name.
    /// </summary>
    public BalanceBase<TBalanceEntry> WithName(string newName)
        => CreateInstance(Id, newName, Description, Date, BalanceEntries);

    /// <summary>
    /// Returns a new instance with the specified description.
    /// </summary>
    public BalanceBase<TBalanceEntry> WithDescription(string newDescription)
        => CreateInstance(Id, Name, newDescription, Date, BalanceEntries);

    /// <summary>
    /// Returns a new instance with the specified date.
    /// </summary>
    public BalanceBase<TBalanceEntry> WithDate(DateTime newDate)
        => CreateInstance(Id, Name, Description, newDate, BalanceEntries);

    /// <summary>
    /// Returns a new instance with the specified balance entries.
    /// </summary>
    public BalanceBase<TBalanceEntry> WithBalanceEntries(IEnumerable<TBalanceEntry> newEntries)
        => CreateInstance(Id, Name, Description, Date, newEntries);

    /// <summary>
    /// Returns a new instance with the specified balance entry added.
    /// </summary>
    public BalanceBase<TBalanceEntry> AddBalanceEntry(TBalanceEntry entry)
    {
        if (entry == null)
            throw new ArgumentNullException(nameof(entry));
        return CreateInstance(Id, Name, Description, Date, BalanceEntries.Append(entry));
    }

    /// <summary>
    /// Factory method to create a Balance from a TrialBalance.
    /// </summary>
    /// <typeparam name="TSelf">The concrete balance type to create.</typeparam>
    /// <typeparam name="TTrialBalance">The trial balance type.</typeparam>
    /// <typeparam name="TTrialBalanceEntry">The trial balance entry type.</typeparam>
    /// <param name="name">The name of the balance.</param>
    /// <param name="description">The description of the balance.</param>
    /// <param name="date">The date of the balance.</param>
    /// <param name="trialBalance">The trial balance instance.</param>
    /// <param name="entryFactory">A factory function to create balance entries from trial balance entries.</param>
    /// <param name="balanceFactory">A factory function to create the balance instance.</param>
    /// <returns>A new balance instance created from the trial balance.</returns>
    /// <exception cref="ArgumentNullException">Thrown if trialBalance is null.</exception>
    /// <exception cref="ArgumentException">Thrown if trialBalance type does not have required properties.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the trial balance is not closed or not balanced.</exception>
    public static TSelf FromTrialBalance<TSelf, TTrialBalance, TTrialBalanceEntry>(
        string name,
        string description,
        DateTime date,
        TTrialBalance trialBalance,
        Func<string, string, AccountType, string?, decimal, string, TBalanceEntry> entryFactory,
        Func<Guid, string, string, DateTime, IEnumerable<TBalanceEntry>, TSelf> balanceFactory)
        where TSelf : BalanceBase<TBalanceEntry>
        where TTrialBalance : class
        where TTrialBalanceEntry : class
    {
        if (trialBalance == null)
            throw new ArgumentNullException(nameof(trialBalance));
        var isClosedProp = typeof(TTrialBalance).GetProperty("IsClosed");
        var isBalancedProp = typeof(TTrialBalance).GetProperty("IsBalanced");
        var entriesProp = typeof(TTrialBalance).GetProperty("TrialBalanceEntries");

        if (isClosedProp == null || isBalancedProp == null || entriesProp == null)
            throw new ArgumentException("TrialBalance type does not have required properties.");

        if (!(bool)isClosedProp.GetValue(trialBalance)!)
            throw new InvalidOperationException($"Trial balance is not closed at {date:yyyy-MM-dd}.");
        if (!(bool)isBalancedProp.GetValue(trialBalance)!)
            throw new InvalidOperationException($"Trial balance is not balanced at {date:yyyy-MM-dd}.");
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException($"'{nameof(description)}' cannot be null or empty.", nameof(description));
        if (date == default)
            throw new ArgumentException($"'{nameof(date)}' cannot be default.", nameof(date));

        var trialBalanceEntries = (IEnumerable<TTrialBalanceEntry>)entriesProp.GetValue(trialBalance)!;

        var balanceEntries = trialBalanceEntries
            .Where(entry =>
            {
                var type = (AccountType)typeof(TTrialBalanceEntry).GetProperty("Type")!.GetValue(entry)!;
                return type == AccountType.Asset || type == AccountType.Liability || type == AccountType.Equity;
            })
            .Select(entry =>
            {
                var accountID = (string)typeof(TTrialBalanceEntry).GetProperty("AccountID")!.GetValue(entry)!;
                var accountName = (string)typeof(TTrialBalanceEntry).GetProperty("AccountName")!.GetValue(entry)!;
                var type = (AccountType)typeof(TTrialBalanceEntry).GetProperty("Type")!.GetValue(entry)!;
                var parentCode = (string?)typeof(TTrialBalanceEntry).GetProperty("ParentCode")!.GetValue(entry);
                var debit = (decimal)typeof(TTrialBalanceEntry).GetProperty("Debit")!.GetValue(entry)!;
                var credit = (decimal)typeof(TTrialBalanceEntry).GetProperty("Credit")!.GetValue(entry)!;
                var normally = (string)typeof(TTrialBalanceEntry).GetProperty("Normally")!.GetValue(entry)!;
                var balance = normally == "Credit" ? credit - debit : debit - credit;
                return entryFactory(accountID, accountName, type, parentCode, balance, normally);
            })
            .ToList();

        return balanceFactory(Guid.NewGuid(), name, description, date, balanceEntries);
    }
}