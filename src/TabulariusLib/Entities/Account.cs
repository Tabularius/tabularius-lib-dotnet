/*
 * Account.cs
 * 
 * Represents a concrete implementation of an account entity in the Tabularius accounting library.
 * 
 * This record provides a strongly-typed, immutable account entity, inheriting from AccountBase<Account>.
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
/// Represents a concrete account entity in the Tabularius accounting library.
/// Inherits from <see cref="AccountBase{Account}"/> and provides factory methods for creation and mutation.
/// </summary>
[Table("Accounts")]
public sealed record Account : AccountBase<Account>
{
    /// <summary>
    /// Private parameterless constructor for EF Core.
    /// </summary>
    private Account() : base() { }

    /// <summary>
    /// Private full constructor for controlled creation and validation.
    /// </summary>
    /// <param name="name">The name of the account.</param>
    /// <param name="description">The description of the account.</param>
    /// <param name="code">The unique code for the account.</param>
    /// <param name="type">The account type (Asset, Liability, etc.).</param>
    /// <param name="parentCode">The parent account code, if any.</param>
    /// <param name="normally">Indicates whether the account is normally Debit or Credit.</param>
    private Account(
        string name,
        string description,
        string code,
        AccountType type,
        string? parentCode,
        string normally)
        : base(name, description, code, type, parentCode, normally)
    { }

    /// <summary>
    /// Factory method for creation with validation.
    /// </summary>
    /// <param name="name">The name of the account.</param>
    /// <param name="description">The description of the account.</param>
    /// <param name="code">The unique code for the account.</param>
    /// <param name="type">The account type (Asset, Liability, etc.).</param>
    /// <param name="parentCode">The parent account code, if any.</param>
    /// <param name="normally">Indicates whether the account is normally Debit or Credit.</param>
    /// <returns>A new <see cref="Account"/> instance.</returns>
    public static Account Create(
        string name,
        string description,
        string code,
        AccountType type,
        string? parentCode,
        string normally)
        => new Account(name, description, code, type, parentCode, normally);

    /// <summary>
    /// Implementation of the abstract factory method for mutation methods.
    /// </summary>
    /// <param name="name">The name of the account.</param>
    /// <param name="description">The description of the account.</param>
    /// <param name="code">The unique code for the account.</param>
    /// <param name="type">The account type (Asset, Liability, etc.).</param>
    /// <param name="parentCode">The parent account code, if any.</param>
    /// <param name="normally">Indicates whether the account is normally Debit or Credit.</param>
    /// <returns>A new <see cref="Account"/> instance with the specified values.</returns>
    protected override Account CreateInstance(string name, string description, string code, AccountType type, string? parentCode, string normally)
        => new Account(name, description, code, type, parentCode, normally);
}