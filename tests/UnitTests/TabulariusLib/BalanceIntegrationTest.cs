using System;
using TabulariusLib.Entities;

namespace UnitTests.TabulariusLib;

public class BalanceIntegrationTest
{
    [Fact]
    public void Balance_ShouldReflectClosedTrialBalanceEntries()
    {
        // Setup accounts
        var accounts = ExampleData.GetExampleAccounts();
        var cash = accounts.Find(a => a.Code == ExampleAccountCodes.Cash)!;
        var revenue = accounts.Find(a => a.Code == ExampleAccountCodes.Revenue)!;
        var expense = accounts.Find(a => a.Code == ExampleAccountCodes.Expense)!;
        var capital = accounts.Find(a => a.Code == ExampleAccountCodes.Capital)!;
        var retainedEarnings = accounts.Find(a => a.Code == ExampleAccountCodes.RetainedEarnings)!;
        var payable = accounts.Find(a => a.Code == ExampleAccountCodes.Payable)!;

        // Create Journal
        var journal = ExampleData.GetExampleJournal();

        // Create Ledger and TrialBalance
        var ledger = Ledger.FromJournal("Main Ledger", "Test Ledger", journal, new[] { cash, revenue, expense, capital, retainedEarnings, payable });
        var trialBalance = TrialBalance.FromLedger("TB", "Test Trial Balance", DateTime.Today, ledger);
        var closedTrialBalance = trialBalance.CloseAccounts(retainedEarnings);

        // Create Balance from closed trial balance
        var balance = Balance.FromTrialBalance("Balance Sheet", "Test Balance", DateTime.Today, closedTrialBalance);

        // Assert that only non-income/expense accounts and retained earnings are present
        Assert.DoesNotContain(balance.BalanceEntries, e => e.Type == AccountType.Income || e.Type == AccountType.Expense);
        Assert.Contains(balance.BalanceEntries, e => e.AccountName == "Retained Earnings");

        // Assert that the balance is balanced
        Assert.True(balance.IsBalanced);

        // Assert that assets = liabilities + equity
        decimal assets = balance.BalanceEntries.Where(e => e.Type == AccountType.Asset).Sum(e => e.Balance);
        decimal liabilities = balance.BalanceEntries.Where(e => e.Type == AccountType.Liability).Sum(e => e.Balance);
        decimal equity = balance.BalanceEntries.Where(e => e.Type == AccountType.Equity).Sum(e => e.Balance);
        Assert.Equal(assets, liabilities + equity);
    }

    [Fact]
    public void Balance_ShouldThrowException_IfTrialBalanceNotClosed()
    {
        // Setup accounts
        var accounts = ExampleData.GetExampleAccounts();
        var cash = accounts.Find(a => a.Code == ExampleAccountCodes.Cash)!;
        var retainedEarnings = accounts.Find(a => a.Code == ExampleAccountCodes.RetainedEarnings)!;
        var journal = ExampleData.GetExampleJournal();
        var ledger = Ledger.FromJournal("Main Ledger", "Test Ledger", journal, new[] { cash, retainedEarnings });
        var trialBalance = TrialBalance.FromLedger("TB", "Test TB", DateTime.Today, ledger);
        // Try to create balance from unclosed trial balance
        Assert.Throws<InvalidOperationException>(() => Balance.FromTrialBalance("Balance Sheet", "Test Balance", DateTime.Today, trialBalance));
    }
}

