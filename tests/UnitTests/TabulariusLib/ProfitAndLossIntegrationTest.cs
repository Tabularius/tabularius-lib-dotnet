using System;

using TabulariusLib.Entities;

namespace UnitTests.TabulariusLib;

public class ProfitAndLossIntegrationTest
{
    [Fact]
    public void ProfitAndLossStatement_ShouldOnlyIncludeEntriesWithinDateRange()
    {
        // Setup accounts
        var accounts = ExampleData.GetExampleAccounts();
        var cash = accounts.Find(a => a.Code == ExampleAccountCodes.Cash)!;
        var revenue = accounts.Find(a => a.Code == ExampleAccountCodes.Revenue)!;
        var expense = accounts.Find(a => a.Code == ExampleAccountCodes.Expense)!;

        // Use shared journal with date-range entries
        var journal = ExampleData.GetExampleJournalWithDateRangeEntries();

        // Get profit and loss statement from trial balance
        var ledger = Ledger.FromJournal("Main Ledger", "Test Ledger", journal, new[] { revenue, expense, cash });
        var trialBalance = TrialBalance.FromLedger("TB", "Test TB", new DateTime(2024, 12, 31), ledger);
        Assert.True(trialBalance.IsBalanced, "Trial balance must be balanced before creating the profit and loss statement.");
        var plStatement = ProfitAndLossStatement.FromLedger(
            "P&L", "Test P&L", new DateTime(2024, 1, 1), new DateTime(2024, 12, 31), ledger);

        // Assert only entries within 2024-01-01 and 2024-12-31 are included
        Assert.Equal(100m, plStatement.TotalRevenue); // Only Entry 1
        Assert.Equal(50m, plStatement.TotalExpense);  // Only Entry 2
        Assert.Equal(50m, plStatement.NetProfit);

        Assert.All(plStatement.Entries, e =>
        {
            Assert.True(e.Type == AccountType.Income || e.Type == AccountType.Expense);
        });

        // Ensure Entry 0 and Entry 3 are not included
        Assert.DoesNotContain(plStatement.Entries, e => e.Amount == 500m);
        Assert.DoesNotContain(plStatement.Entries, e => e.Amount == 200m);
    }
}

