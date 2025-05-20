using System;
using System.Collections.Generic;
using TabulariusLib.Entities;

namespace UnitTests.TabulariusLib;

public static class ExampleAccountCodes
{
    public const string Cash = "1000";
    public const string Revenue = "4000";
    public const string Expense = "5000";
    public const string Capital = "3000";
    public const string RetainedEarnings = "3100";
    public const string Payable = "2000";
}

public static class ExampleData
{
    public static List<Account> GetExampleAccounts()
    {
        return new List<Account>
        {
            Account.Create("Cash", "Cash Account", ExampleAccountCodes.Cash, AccountType.Asset, null, "Debit"),
            Account.Create("Revenue", "Sales Revenue", ExampleAccountCodes.Revenue, AccountType.Income, null, "Credit"),
            Account.Create("Expense", "Office Supplies", ExampleAccountCodes.Expense, AccountType.Expense, null, "Debit"),
            Account.Create("Capital", "Owner's Capital", ExampleAccountCodes.Capital, AccountType.Equity, null, "Credit"),
            Account.Create("Retained Earnings", "Retained Earnings", ExampleAccountCodes.RetainedEarnings, AccountType.Equity, null, "Credit"),
            Account.Create("Payable", "Accounts Payable", ExampleAccountCodes.Payable, AccountType.Liability, null, "Credit")
        };
    }

    public static Journal GetExampleJournal()
    {
        var accounts = GetExampleAccounts();
        var cash = accounts.Find(a => a.Code == ExampleAccountCodes.Cash);
        var revenue = accounts.Find(a => a.Code == ExampleAccountCodes.Revenue);
        var expense = accounts.Find(a => a.Code == ExampleAccountCodes.Expense);
        var capital = accounts.Find(a => a.Code == ExampleAccountCodes.Capital);
        var payable = accounts.Find(a => a.Code == ExampleAccountCodes.Payable);
        if (cash == null || revenue == null || expense == null || capital == null || payable == null)
            throw new InvalidOperationException("One or more required accounts are missing.");

        var journal = Journal.Create(Guid.NewGuid(), "Main Journal", "Test Journal", null);

        // Sales Invoice: Debit Cash, Credit Revenue
        journal = journal.AddJournalEntry(
            JournalEntry.Create(
                Guid.NewGuid().ToString(),
                "Sales Invoice",
                DateTime.Today,
                "INV-001",
                [
                    JournalLine.Create(Guid.NewGuid(), "Cash from Sales", cash.Code, 1000m, 0m),
                    JournalLine.Create(Guid.NewGuid(), "Sales Revenue", revenue.Code, 0m, 1000m)
                ]
            ));

        // Expense: Debit Expense, Credit Cash
        journal = journal.AddJournalEntry(
            JournalEntry.Create(
                Guid.NewGuid().ToString(),
                "Office Supplies Expense",
                DateTime.Today,
                "EXP-001",
                [
                    JournalLine.Create(Guid.NewGuid(), "Office Supplies", expense.Code, 200m, 0m),
                    JournalLine.Create(Guid.NewGuid(), "Paid Cash", cash.Code, 0m, 200m)
                ]
            ));

        // Owner's Investment: Debit Cash, Credit Capital
        journal = journal.AddJournalEntry(
            JournalEntry.Create(
                Guid.NewGuid().ToString(),
                "Owner's Investment",
                DateTime.Today,
                "CAP-001",
                [
                    JournalLine.Create(Guid.NewGuid(), "Cash Invested", cash.Code, 5000m, 0m),
                    JournalLine.Create(Guid.NewGuid(), "Owner's Capital", capital.Code, 0m, 5000m)
                ]
            ));

        // Liability: Purchase on Credit (Debit Expense, Credit Payable)
        journal = journal.AddJournalEntry(
            JournalEntry.Create(
                Guid.NewGuid().ToString(),
                "Office Supplies on Credit",
                DateTime.Today,
                "EXP-002",
                [
                    JournalLine.Create(Guid.NewGuid(), "Office Supplies (Credit)", expense.Code, 300m, 0m),
                    JournalLine.Create(Guid.NewGuid(), "Accounts Payable", payable.Code, 0m, 300m)
                ]
            ));

        return journal;
    }

    public static Journal GetExampleJournalWithDateRangeEntries()
    {
        var accounts = GetExampleAccounts();
        var cash = accounts.Find(a => a.Code == ExampleAccountCodes.Cash)!;
        var revenue = accounts.Find(a => a.Code == ExampleAccountCodes.Revenue)!;
        var expense = accounts.Find(a => a.Code == ExampleAccountCodes.Expense)!;

        var journal = Journal.Create(Guid.NewGuid(), "Main Journal", "Test Journal", null);

        // Entry 0: Date = 2023-12-31 (should be excluded)
        journal = journal.AddJournalEntry(
            JournalEntry.Create(
                Guid.NewGuid().ToString(),
                "Revenue Entry 0",
                new DateTime(2023, 12, 31),
                "REF-000",
                [
                    JournalLine.Create(Guid.NewGuid(), "Sales Revenue", revenue.Code, 0m, 500m),
                    JournalLine.Create(Guid.NewGuid(), "Cash", cash.Code, 500m, 0m)
                ]
            ));

        // Entry 1: Date = 2024-01-01 (should be included)
        journal = journal.AddJournalEntry(
            JournalEntry.Create(
                Guid.NewGuid().ToString(),
                "Revenue Entry 1",
                new DateTime(2024, 1, 1),
                "REF-001",
                [
                    JournalLine.Create(Guid.NewGuid(), "Sales Revenue", revenue.Code, 0m, 100m),
                    JournalLine.Create(Guid.NewGuid(), "Cash", cash.Code, 100m, 0m)
                ]
            ));

        // Entry 2: Date = 2024-06-01 (should be included)
        journal = journal.AddJournalEntry(
            JournalEntry.Create(
                Guid.NewGuid().ToString(),
                "Expense Entry 1",
                new DateTime(2024, 6, 1),
                "REF-002",
                [
                    JournalLine.Create(Guid.NewGuid(), "Office Supplies", expense.Code, 50m, 0m),
                    JournalLine.Create(Guid.NewGuid(), "Cash", cash.Code, 0m, 50m)
                ]
            ));

        // Entry 3: Date = 2025-05-17 (should be excluded)
        journal = journal.AddJournalEntry(
            JournalEntry.Create(
                Guid.NewGuid().ToString(),
                "Revenue Entry 2",
                new DateTime(2025, 5, 17),
                "REF-003",
                [
                    JournalLine.Create(Guid.NewGuid(), "Sales Revenue", revenue.Code, 0m, 200m),
                    JournalLine.Create(Guid.NewGuid(), "Cash", cash.Code, 200m, 0m)
                ]
            ));

        return journal;
    }
}
