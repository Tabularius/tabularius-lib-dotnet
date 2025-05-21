/*
 * ProfitAndLossEntryBase.cs
 * 
 * Abstract base record for profit and loss entry entities in the Tabularius accounting library.
 * 
 * This class provides a strongly-typed, immutable base for profit and loss entry records using the Curiously Recurring Template Pattern (CRTP).
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
/// Abstract base record for profit and loss entry entities, using CRTP for strongly-typed mutation methods.
/// </summary>
/// <typeparam name="TSelf">The concrete profit and loss entry type inheriting from this base.</typeparam>
public abstract record ProfitAndLossEntryBase<TSelf>
    where TSelf : ProfitAndLossEntryBase<TSelf>
{
    /// <summary>
    /// The unique identifier for the account associated with this profit and loss entry.
    /// </summary>
    [Key]
    [MaxLength(256)]
    public string AccountID { get; private set; }

    /// <summary>
    /// The name of the account associated with this profit and loss entry.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public string AccountName { get; private set; }

    /// <summary>
    /// The type of the account (Income or Expense).
    /// </summary>
    [Required]
    public AccountType Type { get; private set; }

    /// <summary>
    /// The amount for this profit and loss entry.
    /// </summary>
    public decimal Amount { get; private set; }

    /// <summary>
    /// Protected parameterless constructor for EF Core.
    /// </summary>
    protected ProfitAndLossEntryBase()
    {
        AccountID = string.Empty;
        AccountName = string.Empty;
        Type = AccountType.Income;
        Amount = 0m;
    }

    /// <summary>
    /// Protected constructor with validation for all properties.
    /// </summary>
    /// <param name="accountID">The unique identifier for the account.</param>
    /// <param name="accountName">The name of the account.</param>
    /// <param name="type">The account type (Income or Expense).</param>
    /// <param name="amount">The amount for this entry.</param>
    /// <exception cref="ArgumentException">Thrown if any required argument is invalid.</exception>
    protected ProfitAndLossEntryBase(string accountID, string accountName, AccountType type, decimal amount)
    {
        if (string.IsNullOrWhiteSpace(accountID))
            throw new ArgumentException($"'{nameof(accountID)}' cannot be null or empty.", nameof(accountID));
        if (string.IsNullOrWhiteSpace(accountName))
            throw new ArgumentException($"'{nameof(accountName)}' cannot be null or empty.", nameof(accountName));
        if (!Enum.IsDefined(typeof(AccountType), type))
            throw new ArgumentException($"'{nameof(type)}' must be a valid AccountType.", nameof(type));
        if (amount < 0)
            throw new ArgumentException($"'{nameof(amount)}' cannot be negative.", nameof(amount));
        AccountID = accountID;
        AccountName = accountName;
        Type = type;
        Amount = amount;
    }

    /// <summary>
    /// Abstract factory method for mutation methods. Must be implemented in derived types.
    /// </summary>
    /// <param name="accountID">The unique identifier for the account.</param>
    /// <param name="accountName">The name of the account.</param>
    /// <param name="type">The account type.</param>
    /// <param name="amount">The amount for this entry.</param>
    /// <returns>A new instance of the derived profit and loss entry type.</returns>
    protected abstract TSelf CreateInstance(string accountID, string accountName, AccountType type, decimal amount);

    /// <summary>
    /// Returns a new instance with the specified account ID.
    /// </summary>
    public TSelf WithAccountID(string newAccountID)
        => CreateInstance(newAccountID, AccountName, Type, Amount);

    /// <summary>
    /// Returns a new instance with the specified account name.
    /// </summary>
    public TSelf WithAccountName(string newAccountName)
        => CreateInstance(AccountID, newAccountName, Type, Amount);

    /// <summary>
    /// Returns a new instance with the specified account type.
    /// </summary>
    public TSelf WithType(AccountType newType)
        => CreateInstance(AccountID, AccountName, newType, Amount);

    /// <summary>
    /// Returns a new instance with the specified amount.
    /// </summary>
    public TSelf WithAmount(decimal newAmount)
        => CreateInstance(AccountID, AccountName, Type, newAmount);
}