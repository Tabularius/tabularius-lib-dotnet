/*
 * TrialBalance.cs
 * 
 * Represents a concrete implementation of a trial balance entity in the Tabularius accounting library.
 * 
 * This record provides a strongly-typed, immutable trial balance entity, inheriting from TrialBalanceBase<TrialBalance, TrialBalanceEntry>.
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
/// Represents a concrete trial balance entity in the Tabularius accounting library.
/// Inherits from <see cref="TrialBalanceBase{TrialBalance, TrialBalanceEntry}"/> and provides factory methods for creation and mutation.
/// </summary>
[Table("TrialBalances")]
public sealed record TrialBalance : TrialBalanceBase<TrialBalance, TrialBalanceEntry>
{
    /// <summary>
    /// Private parameterless constructor for EF Core.
    /// </summary>
    private TrialBalance() : base() { }

    /// <summary>
    /// Private full constructor for controlled creation and validation.
    /// </summary>
    /// <param name="id">The unique identifier for the trial balance.</param>
    /// <param name="name">The name of the trial balance.</param>
    /// <param name="description">The description of the trial balance.</param>
    /// <param name="date">The date of the trial balance.</param>
    /// <param name="entries">The collection of trial balance entries.</param>
    /// <param name="isClosed">Indicates whether the trial balance is closed.</param>
    private TrialBalance(Guid id, string name, string description, DateTime date, IEnumerable<TrialBalanceEntry> entries, bool isClosed = false)
        : base(id, name, description, date, entries, isClosed)
    { }

    /// <summary>
    /// Factory method for creation with validation.
    /// </summary>
    /// <param name="id">The unique identifier for the trial balance.</param>
    /// <param name="name">The name of the trial balance.</param>
    /// <param name="description">The description of the trial balance.</param>
    /// <param name="date">The date of the trial balance.</param>
    /// <param name="entries">The collection of trial balance entries.</param>
    /// <param name="isClosed">Indicates whether the trial balance is closed.</param>
    /// <returns>A new <see cref="TrialBalance"/> instance.</returns>
    public static TrialBalance Create(Guid id, string name, string description, DateTime date, IEnumerable<TrialBalanceEntry> entries, bool isClosed = false)
        => new(id, name, description, date, entries, isClosed);

    /// <summary>
    /// Implementation of the abstract factory method for mutation methods.
    /// </summary>
    /// <param name="id">The unique identifier for the trial balance.</param>
    /// <param name="name">The name of the trial balance.</param>
    /// <param name="description">The description of the trial balance.</param>
    /// <param name="date">The date of the trial balance.</param>
    /// <param name="entries">The collection of trial balance entries.</param>
    /// <param name="isClosed">Indicates whether the trial balance is closed.</param>
    /// <returns>A new <see cref="TrialBalance"/> instance with the specified values.</returns>
    protected override TrialBalance CreateInstance(Guid id, string name, string description, DateTime date, IEnumerable<TrialBalanceEntry> entries, bool isClosed)
        => new TrialBalance(id, name, description, date, entries, isClosed);

    /// <summary>
    /// Strongly-typed factory method to create a <see cref="TrialBalance"/> from a <see cref="Ledger"/>.
    /// </summary>
    /// <param name="name">The name of the trial balance.</param>
    /// <param name="description">The description of the trial balance.</param>
    /// <param name="upToDate">The date up to which entries are considered.</param>
    /// <param name="ledger">The ledger instance to convert from.</param>
    /// <returns>A new <see cref="TrialBalance"/> instance created from the ledger.</returns>
    public static TrialBalance FromLedger(string name, string description, DateTime upToDate, Ledger ledger)
        => FromLedger<Ledger, LedgerAccount, LedgerEntry>(
            name,
            description,
            upToDate,
            ledger,
            TrialBalanceEntry.Create,
            (id, n, d, dt, entries) => new TrialBalance(id, n, d, dt, entries)
        );

    /// <summary>
    /// Closes the income and expense accounts in the trial balance and transfers the net income/loss to the specified equity account.
    /// </summary>
    /// <param name="closingEquityAccount">The equity account to which net income/loss will be transferred (e.g., Retained Earnings).</param>
    /// <returns>A new <see cref="TrialBalance"/> instance with closed accounts and net income/loss transferred.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the trial balance is already closed or if the equity account is not found.</exception>
    public TrialBalance CloseAccounts(Account closingEquityAccount)
    {
        if (IsClosed)
            throw new InvalidOperationException("Accounts are already closed for this trial balance.");

        // Separate entries by type
        var incomeEntries = _trialBalanceEntries.Where(e => e.Type == AccountType.Income).ToList();
        var expenseEntries = _trialBalanceEntries.Where(e => e.Type == AccountType.Expense).ToList();
        var equityEntries = _trialBalanceEntries.Where(e => e.Type == AccountType.Equity).ToList();
        var otherEntries = _trialBalanceEntries.Where(e => e.Type != AccountType.Income && e.Type != AccountType.Expense && e.Type != AccountType.Equity).ToList();

        // Calculate net income (income - expense)
        decimal totalIncome = incomeEntries.Sum(e => e.Credit - e.Debit);
        decimal totalExpense = expenseEntries.Sum(e => e.Debit - e.Credit);
        decimal netIncome = totalIncome - totalExpense;

        // Close income accounts: set their balances to zero by moving their credit to debit
        var closedIncomeEntries = incomeEntries.Select(e =>
            TrialBalanceEntry.Create(
                e.AccountID,
                e.AccountName,
                e.Type,
                e.ParentCode,
                e.Credit, // Debit set to Credit
                e.Credit, // Credit unchanged
                e.Normally
            )
        ).ToList();

        // Close expense accounts: set their balances to zero by moving their debit to credit
        var closedExpenseEntries = expenseEntries.Select(e =>
            TrialBalanceEntry.Create(
                e.AccountID,
                e.AccountName,
                e.Type,
                e.ParentCode,
                e.Debit, // Debit unchanged
                e.Debit, // Credit set to Debit
                e.Normally
            )
        ).ToList();

        // Add net income/loss to equity
        var closedEntries = new List<TrialBalanceEntry>();
        closedEntries.AddRange(otherEntries);
        closedEntries.AddRange(equityEntries);
        closedEntries.AddRange(closedIncomeEntries);
        closedEntries.AddRange(closedExpenseEntries);

        if (netIncome != 0)
        {
            // Find the 'Retained Earnings' equity account to post the net income/loss
            var equityAccount = equityEntries.FirstOrDefault(e => e.AccountID == closingEquityAccount.Code)
                ?? TrialBalanceEntry.Create(
                    closingEquityAccount.Code,
                    closingEquityAccount.Name,
                    closingEquityAccount.Type,
                    closingEquityAccount.ParentCode,
                    0,
                    0,
                    closingEquityAccount.Normally
                );
            if (equityAccount == null)
                throw new InvalidOperationException($"No equity account ('{closingEquityAccount.Name}') found to close net income/loss.");

            // If the account already exists in closedEntries, add to its value
            var retainedEarnings = closedEntries.FirstOrDefault(e => e.AccountID == equityAccount.AccountID);
            if (retainedEarnings != null)
            {
                var newDebit = retainedEarnings.Debit + (netIncome < 0 ? Math.Abs(netIncome) : 0);
                var newCredit = retainedEarnings.Credit + (netIncome > 0 ? netIncome : 0);
                closedEntries.Remove(retainedEarnings);
                closedEntries.Add(
                    TrialBalanceEntry.Create(
                        retainedEarnings.AccountID,
                        retainedEarnings.AccountName,
                        retainedEarnings.Type,
                        retainedEarnings.ParentCode,
                        newDebit,
                        newCredit,
                        retainedEarnings.Normally
                    )
                );
            }
            else
            {
                closedEntries.Add(
                    TrialBalanceEntry.Create(
                        equityAccount.AccountID,
                        equityAccount.AccountName,
                        equityAccount.Type,
                        equityAccount.ParentCode,
                        netIncome < 0 ? Math.Abs(netIncome) : 0,
                        netIncome > 0 ? netIncome : 0,
                        equityAccount.Normally
                    )
                );
            }
        }

        return new TrialBalance(
            Id,
            Name,
            Description,
            Date,
            closedEntries
        )
        {
            IsClosed = true
        };
    }
}