using System;

using tabularius.TabulariusLib.Entities;

namespace UnitTests.TabulariusLib;

public class TrialBalanceIntegrationTest
{
    [Fact]
    public void LedgerAndTrialBalance_ShouldReflectJournalEntriesForInvoiceExpenseInvestmentAndLiability()
    {
        // --- Setup Accounts ---
        var accounts = ExampleData.GetExampleAccounts();
        var cash = accounts.Find(a => a.Code == ExampleAccountCodes.Cash)!;
        var revenue = accounts.Find(a => a.Code == ExampleAccountCodes.Revenue)!;
        var expense = accounts.Find(a => a.Code == ExampleAccountCodes.Expense)!;
        var capital = accounts.Find(a => a.Code == ExampleAccountCodes.Capital)!;
        var payable = accounts.Find(a => a.Code == ExampleAccountCodes.Payable)!;

        // --- Create Journal ---
        var journal = ExampleData.GetExampleJournal();

        // --- Create Ledger from Journal ---
        var ledger = Ledger.FromJournal("Main Ledger", "Test Ledger", journal, accounts.Where(a => a.Code != ExampleAccountCodes.RetainedEarnings).ToList());

        // --- Assert Ledger Figures ---
        var cashLedger = ledger.LedgerAccounts.First(a => a.Code == cash.Code);
        Assert.Equal(6000m, cashLedger.Debit);   // 1000 (sales) + 5000 (investment)
        Assert.Equal(200m, cashLedger.Credit);   // 200 (expense paid by cash)
        Assert.Equal(5800m, cashLedger.Debit - cashLedger.Credit);

        var revenueLedger = ledger.LedgerAccounts.First(a => a.Code == revenue.Code);
        Assert.Equal(0m, revenueLedger.Debit);
        Assert.Equal(1000m, revenueLedger.Credit);

        var expenseLedger = ledger.LedgerAccounts.First(a => a.Code == expense.Code);
        Assert.Equal(500m, expenseLedger.Debit); // 200 (cash) + 300 (credit)
        Assert.Equal(0m, expenseLedger.Credit);

        var capitalLedger = ledger.LedgerAccounts.First(a => a.Code == capital.Code);
        Assert.Equal(0m, capitalLedger.Debit);
        Assert.Equal(5000m, capitalLedger.Credit);

        var payableLedger = ledger.LedgerAccounts.First(a => a.Code == payable.Code);
        Assert.Equal(0m, payableLedger.Debit);
        Assert.Equal(300m, payableLedger.Credit);

        // --- Create TrialBalance ---
        var trialBalance = TrialBalance.FromLedger("TB", "Test Trial Balance", DateTime.Today, ledger);

        // --- Assert TrialBalance Figures ---
        var tbCash = trialBalance.TrialBalanceEntries.First(e => e.AccountID == cash.Code);
        Assert.Equal(6000m, tbCash.Debit);
        Assert.Equal(200m, tbCash.Credit);

        var tbRevenue = trialBalance.TrialBalanceEntries.First(e => e.AccountID == revenue.Code);
        Assert.Equal(0m, tbRevenue.Debit);
        Assert.Equal(1000m, tbRevenue.Credit);

        var tbExpense = trialBalance.TrialBalanceEntries.First(e => e.AccountID == expense.Code);
        Assert.Equal(500m, tbExpense.Debit);
        Assert.Equal(0m, tbExpense.Credit);

        var tbCapital = trialBalance.TrialBalanceEntries.First(e => e.AccountID == capital.Code);
        Assert.Equal(0m, tbCapital.Debit);
        Assert.Equal(5000m, tbCapital.Credit);

        var tbPayable = trialBalance.TrialBalanceEntries.First(e => e.AccountID == payable.Code);
        Assert.Equal(0m, tbPayable.Debit);
        Assert.Equal(300m, tbPayable.Credit);

        // --- Assert TrialBalance is balanced ---
        Assert.Equal(trialBalance.TrialBalanceEntries.Sum(e => e.Debit), trialBalance.TrialBalanceEntries.Sum(e => e.Credit));

        // --- Assert Accounting Equation: Assets = Liabilities + Equity ---
        decimal assets = trialBalance.TrialBalanceEntries
            .Where(e => e.AccountID == cash.Code)
            .Sum(e => e.Debit - e.Credit);

        decimal liabilities = trialBalance.TrialBalanceEntries
            .Where(e => e.AccountID == payable.Code)
            .Sum(e => e.Credit - e.Debit);

        decimal equity = trialBalance.TrialBalanceEntries
            .Where(e => e.AccountID == capital.Code)
            .Sum(e => e.Credit - e.Debit);

        // Net Income = Revenue - Expense
        decimal netIncome = trialBalance.TrialBalanceEntries
            .Where(e => e.AccountID == revenue.Code)
            .Sum(e => e.Credit - e.Debit)
            -
            trialBalance.TrialBalanceEntries
            .Where(e => e.AccountID == expense.Code)
            .Sum(e => e.Debit - e.Credit);

        // Total Equity = Capital + Net Income
        decimal totalEquity = equity + netIncome;

        Assert.Equal(assets, liabilities + totalEquity);
    }

    [Fact]
    public void TrialBalance_ShouldOnlyIncludeEntriesUpToGivenDate()
    {
        // Setup accounts
        var cash = Account.Create("Cash", "Cash Account", "1000", AccountType.Asset, null, "Debit");
        var revenue = Account.Create("Revenue", "Sales Revenue", "4000", AccountType.Income, null, "Credit");

        // Create Journal
        var journal = Journal.Create(Guid.NewGuid(), "Main Journal", "Test Journal", null);

        // Entry 1: Date = 2024-01-01
        journal = journal.AddJournalEntry(
            JournalEntry.Create(
                Guid.NewGuid().ToString(),
                "Entry 1",
                new DateTime(2024, 1, 1),
                "REF-001",
                [
                    JournalLine.Create(Guid.NewGuid(), "Cash in", cash.Code, 100m, 0m),
                    JournalLine.Create(Guid.NewGuid(), "Revenue", revenue.Code, 0m, 100m)
                ]
            ));

        // Entry 2: Date = 2024-06-01
        journal = journal.AddJournalEntry(
            JournalEntry.Create(
                Guid.NewGuid().ToString(),
                "Entry 2",
                new DateTime(2024, 6, 1),
                "REF-002",
                [
                    JournalLine.Create(Guid.NewGuid(), "Cash in", cash.Code, 200m, 0m),
                    JournalLine.Create(Guid.NewGuid(), "Revenue", revenue.Code, 0m, 200m)
                ]
            ));

        // Entry 3: Date = 2025-05-17 (should be excluded)
        journal = journal.AddJournalEntry(
            JournalEntry.Create(
                Guid.NewGuid().ToString(),
                "Entry 3",
                new DateTime(2025, 5, 17),
                "REF-003",
                [
                    JournalLine.Create(Guid.NewGuid(), "Cash in", cash.Code, 400m, 0m),
                    JournalLine.Create(Guid.NewGuid(), "Revenue", revenue.Code, 0m, 400m)
                ]
            ));

        // Get trial balance up to 2024-12-31 (should only include Entry 1 and Entry 2)
        var ledger = Ledger.FromJournal("Main Ledger", "Test Ledger", journal, new[] { cash, revenue });
        var trialBalance = TrialBalance.FromLedger("TB", "Test TB", new DateTime(2024, 12, 31), ledger);

        var tbCash = trialBalance.TrialBalanceEntries.First(e => e.AccountID == cash.Code);
        var tbRevenue = trialBalance.TrialBalanceEntries.First(e => e.AccountID == revenue.Code);

        Assert.Equal(300m, tbCash.Debit); // 100 + 200
        Assert.Equal(0m, tbCash.Credit);
        Assert.Equal(0m, tbRevenue.Debit);
        Assert.Equal(300m, tbRevenue.Credit); // 100 + 200

        // Ensure Entry 3 is not included
        Assert.DoesNotContain(trialBalance.TrialBalanceEntries, e => e.Debit == 400m || e.Credit == 400m);
    }
}
