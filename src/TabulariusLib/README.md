# TabulariusLib

A .NET library to handle basic accounting tools to create a journal and accounts. 

## Features
With the journals and accounts you can generate the ledger, trial balance and then the financial statements as of balance sheet and profit and loss statement.

This is an early version and you can contribute to it on github.
https://github.com/Tabularius/tabularius-lib-dotnet

## How to use (taken from unit tests)
### Create Account codes
```
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
```
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
### Create Journal
```
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
### Create Ledger
```
Ledger ledger = Ledger.FromJournal("Main Ledger", "Test Ledger", journal, accounts);
```
### Create Trial Balance
```
TrialBalance trialBalance = TrialBalance.FromLedger("TB", "Test TB", new DateTime(2024, 12, 31), ledger);
```

### Create Profit and Loss Statement
```
ProfitAndLossStatement plStatement = ProfitAndLossStatement.FromLedger("P&L", "Test P&L", new DateTime(2024, 1, 1), new DateTime(2024, 12, 31), ledger);
```
### Create Balance Sheet
```
TrialBalance closedTrialBalance = trialBalance.CloseAccounts(retainedEarnings); // The equity account for the P&L closing

// Create Balance from closed trial balance
Balance balance = Balance.FromTrialBalance("Balance Sheet", "Test Balance", DateTime.Today, closedTrialBalance);
```
## License
Apache-2.0