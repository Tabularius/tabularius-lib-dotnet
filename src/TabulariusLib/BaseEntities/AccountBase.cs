/*
 * AccountBase.cs
 * 
 * Abstract base record for account entities in the Tabularius accounting library.
 * 
 * This class provides a strongly-typed, immutable base for account records using the Curiously Recurring Template Pattern (CRTP).
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
/// Abstract base record for account entities, using CRTP for strongly-typed mutation methods.
/// </summary>
/// <typeparam name="TSelf">The concrete account type inheriting from this base.</typeparam>
public abstract record AccountBase<TSelf>
    where TSelf : AccountBase<TSelf>
{
    /// <summary>
    /// The unique account code (primary key).
    /// </summary>
    [Key]
    [Required]
    [MaxLength(256)]
    public string Code { get; private set; }

    /// <summary>
    /// The account name.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public string Name { get; private set; }

    /// <summary>
    /// The account description.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public string Description { get; private set; }

    /// <summary>
    /// The account type (e.g., Asset, Liability, Income, Expense, Equity).
    /// </summary>
    [Required]
    [MaxLength(256)]
    public AccountType Type { get; private set; }

    /// <summary>
    /// The parent account code, if any.
    /// </summary>
    [MaxLength(256)]
    public string? ParentCode { get; private set; }

    /// <summary>
    /// Indicates whether the account is normally Debit or Credit.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public string Normally { get; private set; }

    /// <summary>
    /// Protected parameterless constructor for EF Core.
    /// </summary>
    protected AccountBase()
    {
        Name = string.Empty;
        Description = string.Empty;
        Code = string.Empty;
        Type = AccountType.Asset;
        ParentCode = null;
        Normally = string.Empty;
    }

    /// <summary>
    /// Protected constructor with validation for all properties.
    /// </summary>
    /// <param name="name">Account name.</param>
    /// <param name="description">Account description.</param>
    /// <param name="code">Account code.</param>
    /// <param name="type">Account type.</param>
    /// <param name="parentCode">Parent account code.</param>
    /// <param name="normally">Normal balance side ("Debit" or "Credit").</param>
    /// <exception cref="ArgumentException">Thrown if any required argument is invalid.</exception>
    protected AccountBase(
        string name,
        string description,
        string code,
        AccountType type,
        string? parentCode,
        string normally)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException($"'{nameof(description)}' cannot be null or empty for account {name}.", nameof(description));
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException($"'{nameof(code)}' cannot be null or empty.", nameof(code));
        if (!Enum.IsDefined(typeof(AccountType), type))
            throw new ArgumentException($"'{nameof(type)}' must be a valid AccountType.", nameof(type));
        if (string.IsNullOrWhiteSpace(normally))
            throw new ArgumentException($"'{nameof(normally)}' cannot be null or empty.", nameof(normally));

        Name = name;
        Description = description;
        Code = code;
        Type = type;
        ParentCode = string.IsNullOrWhiteSpace(parentCode) ? null : parentCode;
        Normally = normally;
    }

    /// <summary>
    /// Abstract factory method for mutation methods. Must be implemented in derived types.
    /// </summary>
    /// <param name="name">Account name.</param>
    /// <param name="description">Account description.</param>
    /// <param name="code">Account code.</param>
    /// <param name="type">Account type.</param>
    /// <param name="parentCode">Parent account code.</param>
    /// <param name="normally">Normal balance side.</param>
    /// <returns>A new instance of the derived account type.</returns>
    protected abstract TSelf CreateInstance(string name, string description, string code, AccountType type, string? parentCode, string normally);

    /// <summary>
    /// Returns a new instance with the specified name.
    /// </summary>
    public TSelf WithName(string newName)
        => CreateInstance(newName, Description, Code, Type, ParentCode, Normally);

    /// <summary>
    /// Returns a new instance with the specified description.
    /// </summary>
    public TSelf WithDescription(string newDescription)
        => CreateInstance(Name, newDescription, Code, Type, ParentCode, Normally);

    /// <summary>
    /// Returns a new instance with the specified code.
    /// </summary>
    public TSelf WithCode(string newCode)
        => CreateInstance(Name, Description, newCode, Type, ParentCode, Normally);

    /// <summary>
    /// Returns a new instance with the specified account type.
    /// </summary>
    public TSelf WithType(AccountType newType)
        => CreateInstance(Name, Description, Code, newType, ParentCode, Normally);

    /// <summary>
    /// Returns a new instance with the specified parent code.
    /// </summary>
    public TSelf WithParentCode(string? newParentCode)
        => CreateInstance(Name, Description, Code, Type, newParentCode, Normally);

    /// <summary>
    /// Returns a new instance with the specified normal balance side.
    /// </summary>
    public TSelf WithNormally(string newNormally)
        => CreateInstance(Name, Description, Code, Type, ParentCode, newNormally);
}