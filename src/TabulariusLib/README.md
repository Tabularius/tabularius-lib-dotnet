# TabulariusLib

A .NET library to handle basic accounting tools to create a journal and accounts.

## Features

- Immutable, strongly-typed accounting entities
- Generate ledgers, trial balances, profit and loss statements, and balance sheets
- Designed for use with Entity Framework Core
- Fully unit-tested

## Accounting

TabulariusLib is built on core accounting principles:

- **Immutability:** All main entities (Journal, Ledger, Trial Balance, Profit and Loss Statement, Balance Sheet) are immutable. Once created, they cannot be changed—only new entries can be added. This mirrors real-world accounting, where journals and ledgers are append-only and cannot be altered retroactively.
- **Entity Dependencies:**  
  - The **Trial Balance** is always derived from the current state of the **Ledger**.
  - The **Balance Sheet** is generated from the **Trial Balance** after it has been closed (i.e., after income and expense accounts are closed to equity).
  - The **Profit and Loss Statement** is generated directly from the **Ledger** for a given period.
- **Validation:** All mutation methods return new instances and enforce validation, ensuring data integrity and auditability.

## Installation

Install from NuGet:

```
dotnet add package TabulariusLib
```

## Requirements

- .NET 7.0 or later

## Usage

Add the required namespaces:

```csharp
using TabulariusLib.Entities;
using TabulariusLib.BaseEntities;
```

### Create Account Codes

```csharp
public static class ExampleAccountCodes
{
    public const string Cash = "1000";
    public const string Revenue = "4000";
    public const string Expense = "5000";
    public const string Capital = "3000";
    public const string RetainedEarnings = "3100";
    public const string Payable = "2000";
}
```

### Create Accounts

```csharp
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
```

### Create a Journal

```csharp
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
```

### Create a Ledger

```csharp
Ledger ledger = Ledger.FromJournal("Main Ledger", "Test Ledger", journal, accounts);
```

### Create a Trial Balance

```csharp
TrialBalance trialBalance = TrialBalance.FromLedger("TB", "Test TB", new DateTime(2024, 12, 31), ledger);
```

### Create a Profit and Loss Statement

```csharp
ProfitAndLossStatement plStatement = ProfitAndLossStatement.FromLedger(
    "P&L", "Test P&L", new DateTime(2024, 1, 1), new DateTime(2024, 12, 31), ledger);
```

### Create a Balance Sheet

```csharp
TrialBalance closedTrialBalance = trialBalance.CloseAccounts(
    accounts.Find(a => a.Code == ExampleAccountCodes.RetainedEarnings)!);

// Create Balance from closed trial balance
Balance balance = Balance.FromTrialBalance("Balance Sheet", "Test Balance", DateTime.Today, closedTrialBalance);
```

## License

Apache-2.0

---
