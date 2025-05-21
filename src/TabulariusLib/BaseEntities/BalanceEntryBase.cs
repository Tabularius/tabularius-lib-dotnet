/*
 * BalanceEntryBase.cs
 * 
 * Abstract base record for balance entry entities in the Tabularius accounting library.
 * 
 * This class provides a strongly-typed, immutable base for balance entry records using the Curiously Recurring Template Pattern (CRTP).
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
/// Abstract base record for balance entry entities, using CRTP for strongly-typed mutation methods.
/// </summary>
/// <typeparam name="TSelf">The concrete balance entry type inheriting from this base.</typeparam>
public abstract record BalanceEntryBase<TSelf>
    where TSelf : BalanceEntryBase<TSelf>
{
    /// <summary>
    /// The unique identifier for the account associated with this balance entry.
    /// </summary>
    [Key]
    [Required]
    [MaxLength(256)]
    public string AccountID { get; private set; }

    /// <summary>
    /// The name of the account associated with this balance entry.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public string AccountName { get; private set; }

    /// <summary>
    /// The type of the account (Asset, Liability, Equity, etc.).
    /// </summary>
    [Required]
    public AccountType Type { get; private set; }

    /// <summary>
    /// The parent account code, if any.
    /// </summary>
    [MaxLength(256)]
    public string? ParentCode { get; private set; }

    /// <summary>
    /// The balance amount for this entry.
    /// </summary>
    public decimal Balance { get; private set; }

    /// <summary>
    /// Indicates whether the account is normally Debit or Credit.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public string Normally { get; private set; }

    /// <summary>
    /// Protected parameterless constructor for EF Core.
    /// </summary>
    protected BalanceEntryBase()
    {
        AccountID = string.Empty;
        AccountName = string.Empty;
        Type = AccountType.Asset;
        ParentCode = null;
        Normally = string.Empty;
    }

    /// <summary>
    /// Protected constructor with validation for all properties.
    /// </summary>
    /// <param name="accountID">The unique identifier for the account.</param>
    /// <param name="accountName">The name of the account.</param>
    /// <param name="type">The account type.</param>
    /// <param name="parentCode">The parent account code, if any.</param>
    /// <param name="balance">The balance amount.</param>
    /// <param name="normally">Indicates whether the account is normally Debit or Credit.</param>
    /// <exception cref="ArgumentException">Thrown if any required argument is invalid.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if balance is negative.</exception>
    protected BalanceEntryBase(string accountID, string accountName, AccountType type, string? parentCode, decimal balance, string normally)
    {
        if (string.IsNullOrWhiteSpace(normally))
            throw new ArgumentException($"'{nameof(normally)}' cannot be null or empty.", nameof(normally));
        if (string.IsNullOrWhiteSpace(accountID))
            throw new ArgumentException($"'{nameof(accountID)}' cannot be null or empty.", nameof(accountID));
        if (string.IsNullOrWhiteSpace(accountName))
            throw new ArgumentException($"'{nameof(accountName)}' cannot be null or empty.", nameof(accountName));
        if (balance < 0)
            throw new ArgumentOutOfRangeException(nameof(balance), "Balance cannot be negative.");
        AccountID = accountID;
        AccountName = accountName;
        Type = type;
        ParentCode = string.IsNullOrWhiteSpace(parentCode) ? null : parentCode;
        Balance = balance;
        Normally = normally;
    }

    /// <summary>
    /// Abstract factory method for mutation methods. Must be implemented in derived types.
    /// </summary>
    /// <param name="accountID">The unique identifier for the account.</param>
    /// <param name="accountName">The name of the account.</param>
    /// <param name="type">The account type.</param>
    /// <param name="parentCode">The parent account code, if any.</param>
    /// <param name="balance">The balance amount.</param>
    /// <param name="normally">Indicates whether the account is normally Debit or Credit.</param>
    /// <returns>A new instance of the derived balance entry type.</returns>
    protected abstract TSelf CreateInstance(string accountID, string accountName, AccountType type, string? parentCode, decimal balance, string normally);

    /// <summary>
    /// Returns a new instance with the specified account ID.
    /// </summary>
    public TSelf WithAccountID(string newAccountID)
        => CreateInstance(newAccountID, AccountName, Type, ParentCode, Balance, Normally);

    /// <summary>
    /// Returns a new instance with the specified account name.
    /// </summary>
    public TSelf WithAccountName(string newAccountName)
        => CreateInstance(AccountID, newAccountName, Type, ParentCode, Balance, Normally);

    /// <summary>
    /// Returns a new instance with the specified account type.
    /// </summary>
    public TSelf WithType(AccountType newType)
        => CreateInstance(AccountID, AccountName, newType, ParentCode, Balance, Normally);

    /// <summary>
    /// Returns a new instance with the specified parent code.
    /// </summary>
    public TSelf WithParentCode(string? newParentCode)
        => CreateInstance(AccountID, AccountName, Type, newParentCode, Balance, Normally);

    /// <summary>
    /// Returns a new instance with the specified balance.
    /// </summary>
    public TSelf WithBalance(decimal newBalance)
        => CreateInstance(AccountID, AccountName, Type, ParentCode, newBalance, Normally);

    /// <summary>
    /// Returns a new instance with the specified normal balance side.
    /// </summary>
    public TSelf WithNormally(string newNormally)
        => CreateInstance(AccountID, AccountName, Type, ParentCode, Balance, newNormally);
}