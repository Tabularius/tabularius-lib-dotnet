/*
 * LedgerAccountBase.cs
 * 
 * Abstract base record for ledger account entities in the Tabularius accounting library.
 * 
 * This class provides a strongly-typed, immutable base for ledger account records using the Curiously Recurring Template Pattern (CRTP).
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
/// Abstract base record for ledger account entities, using CRTP for strongly-typed mutation methods.
/// </summary>
/// <typeparam name="TSelf">The concrete ledger account type inheriting from this base.</typeparam>
/// <typeparam name="TLedgerEntry">The ledger entry type associated with this ledger account.</typeparam>
public abstract record LedgerAccountBase<TSelf, TLedgerEntry>
    where TSelf : LedgerAccountBase<TSelf, TLedgerEntry>
    where TLedgerEntry : LedgerEntryBase<TLedgerEntry>
{
    /// <summary>
    /// The unique code for the ledger account (primary key).
    /// </summary>
    [Key]
    [Required]
    [MaxLength(256)]
    public string Code { get; private set; }

    /// <summary>
    /// The name of the ledger account.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public string Name { get; private set; }

    /// <summary>
    /// The parent account code, if any.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public string ParentCode { get; private set; }

    /// <summary>
    /// Indicates whether the account is normally Debit or Credit.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public string Normally { get; private set; }

    /// <summary>
    /// The account type (e.g., Asset, Liability, Income, Expense, Equity).
    /// </summary>
    [Required]
    public AccountType Type { get; private set; }

    /// <summary>
    /// The description of the ledger account.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public string Description { get; private set; }

    /// <summary>
    /// The list of ledger entries associated with this account.
    /// </summary>
    private List<TLedgerEntry> _ledgerEntries { get; set; } = new();

    /// <summary>
    /// Gets a read-only collection of ledger entries.
    /// </summary>
    public IReadOnlyCollection<TLedgerEntry> LedgerEntries => _ledgerEntries.AsReadOnly();

    /// <summary>
    /// Gets the total credit amount for this account.
    /// </summary>
    public decimal Credit => _ledgerEntries.Sum(entry => entry.Credit);

    /// <summary>
    /// Gets the total debit amount for this account.
    /// </summary>
    public decimal Debit => _ledgerEntries.Sum(entry => entry.Debit);

    /// <summary>
    /// Gets the calculated balance for this account (Credit - Debit).
    /// </summary>
    public decimal Balance => Credit - Debit;

    /// <summary>
    /// Protected parameterless constructor for EF Core.
    /// </summary>
    protected LedgerAccountBase()
    {
        Code = string.Empty;
        Name = string.Empty;
        ParentCode = string.Empty;
        Normally = string.Empty;
        Type = AccountType.Asset;
        Description = string.Empty;
    }

    /// <summary>
    /// Protected constructor with validation for all properties.
    /// </summary>
    /// <param name="code">The unique code for the ledger account.</param>
    /// <param name="name">The name of the ledger account.</param>
    /// <param name="type">The account type.</param>
    /// <param name="description">The description of the ledger account.</param>
    /// <param name="parentCode">The parent account code, if any.</param>
    /// <param name="normally">Indicates whether the account is normally Debit or Credit.</param>
    /// <param name="lines">The collection of ledger entries.</param>
    /// <exception cref="ArgumentException">Thrown if any required argument is invalid.</exception>
    /// <exception cref="ArgumentNullException">Thrown if lines is null.</exception>
    protected LedgerAccountBase(
        string code,
        string name,
        AccountType type,
        string description,
        string parentCode,
        string normally,
        IEnumerable<TLedgerEntry> lines)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException($"'{nameof(code)}' cannot be null or empty.", nameof(code));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException($"'{nameof(description)}' cannot be null or empty.", nameof(description));
        if (string.IsNullOrWhiteSpace(normally))
            throw new ArgumentException($"'{nameof(normally)}' cannot be null or empty.", nameof(normally));
        if (lines == null)
            throw new ArgumentNullException(nameof(lines));

        Code = code;
        Name = name;
        Type = type;
        Description = description;
        ParentCode = parentCode ?? string.Empty;
        Normally = normally;
        _ledgerEntries = lines.ToList();
    }

    /// <summary>
    /// Abstract factory method for mutation methods. Must be implemented in derived types.
    /// </summary>
    /// <param name="code">The unique code for the ledger account.</param>
    /// <param name="name">The name of the ledger account.</param>
    /// <param name="type">The account type.</param>
    /// <param name="description">The description of the ledger account.</param>
    /// <param name="parentCode">The parent account code, if any.</param>
    /// <param name="normally">Indicates whether the account is normally Debit or Credit.</param>
    /// <param name="lines">The collection of ledger entries.</param>
    /// <returns>A new instance of the derived ledger account type.</returns>
    protected abstract TSelf CreateInstance(
        string code,
        string name,
        AccountType type,
        string description,
        string parentCode,
        string normally,
        IEnumerable<TLedgerEntry> lines);

    /// <summary>
    /// Returns a new instance with the specified name.
    /// </summary>
    public TSelf WithName(string newName)
        => CreateInstance(Code, newName, Type, Description, ParentCode, Normally, LedgerEntries);

    /// <summary>
    /// Returns a new instance with the specified description.
    /// </summary>
    public TSelf WithDescription(string newDescription)
        => CreateInstance(Code, Name, Type, newDescription, ParentCode, Normally, LedgerEntries);

    /// <summary>
    /// Returns a new instance with the specified account type.
    /// </summary>
    public TSelf WithType(AccountType newType)
        => CreateInstance(Code, Name, newType, Description, ParentCode, Normally, LedgerEntries);

    /// <summary>
    /// Returns a new instance with the specified parent code.
    /// </summary>
    public TSelf WithParentCode(string newParentCode)
        => CreateInstance(Code, Name, Type, Description, newParentCode, Normally, LedgerEntries);

    /// <summary>
    /// Returns a new instance with the specified normal balance side.
    /// </summary>
    public TSelf WithNormally(string newNormally)
        => CreateInstance(Code, Name, Type, Description, ParentCode, newNormally, LedgerEntries);

    /// <summary>
    /// Returns a new instance with the specified ledger entries.
    /// </summary>
    public TSelf WithLedgerEntries(IEnumerable<TLedgerEntry> newEntries)
        => CreateInstance(Code, Name, Type, Description, ParentCode, Normally, newEntries);
}