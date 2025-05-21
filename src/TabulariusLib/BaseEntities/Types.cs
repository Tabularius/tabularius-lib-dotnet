/*
 * Types.cs
 * 
 * Defines core types and enumerations for the Tabularius accounting library.
 * 
 * License: Apache-2.0
 * Author: Michael Warneke
 * Copyright 2025 Michael Warneke
 */

namespace TabulariusLib.BaseEntities;

/// <summary>
/// Enumeration of account types used in the Tabularius accounting library.
/// </summary>
public enum AccountType
{
    /// <summary>
    /// Represents an income account.
    /// </summary>
    Income,

    /// <summary>
    /// Represents an expense account.
    /// </summary>
    Expense,

    /// <summary>
    /// Represents an asset account.
    /// </summary>
    Asset,

    /// <summary>
    /// Represents a liability account.
    /// </summary>
    Liability,

    /// <summary>
    /// Represents an equity account.
    /// </summary>
    Equity
}