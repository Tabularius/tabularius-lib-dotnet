/*
 * LedgerBase.cs
 * 
 * Abstract base record for ledger entities in the Tabularius accounting library.
 * 
 * This class provides a strongly-typed, immutable base for ledger records using the Curiously Recurring Template Pattern (CRTP).
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
/// Abstract base record for ledger entities, using CRTP for strongly-typed mutation methods.
/// </summary>
/// <typeparam name="TSelf">The concrete ledger type inheriting from this base.</typeparam>
/// <typeparam name="TLedgerAccount">The ledger account type associated with this ledger.</typeparam>
public abstract record LedgerBase<TSelf, TLedgerAccount>
    where TSelf : LedgerBase<TSelf, TLedgerAccount>
{
    /// <summary>
    /// The unique identifier for the ledger (primary key).
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; private set; }

    /// <summary>
    /// The name of the ledger.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public string Name { get; private set; }

    /// <summary>
    /// The description of the ledger.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public string Description { get; private set; }

    /// <summary>
    /// The list of ledger accounts associated with this ledger.
    /// </summary>
    private List<TLedgerAccount> _ledgerAccounts { get; set; } = new();

    /// <summary>
    /// Gets a read-only collection of ledger accounts.
    /// </summary>
    public IReadOnlyCollection<TLedgerAccount> LedgerAccounts => _ledgerAccounts.AsReadOnly();

    /// <summary>
    /// Protected parameterless constructor for EF Core.
    /// </summary>
    protected LedgerBase()
    {
        Name = string.Empty;
        Description = string.Empty;
    }

    /// <summary>
    /// Protected constructor with validation for all properties.
    /// </summary>
    /// <param name="id">The unique identifier for the ledger.</param>
    /// <param name="name">The name of the ledger.</param>
    /// <param name="description">The description of the ledger.</param>
    /// <param name="ledgerAccounts">The collection of ledger accounts.</param>
    /// <exception cref="ArgumentException">Thrown if any required argument is invalid.</exception>
    /// <exception cref="ArgumentNullException">Thrown if ledgerAccounts is null.</exception>
    protected LedgerBase(Guid id, string name, string description, IEnumerable<TLedgerAccount> ledgerAccounts)
    {
        if (id == Guid.Empty)
            throw new ArgumentException($"'{nameof(id)}' cannot be empty.", nameof(id));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException($"'{nameof(description)}' cannot be null or empty.", nameof(description));
        if (ledgerAccounts == null)
            throw new ArgumentNullException(nameof(ledgerAccounts));

        Id = id;
        Name = name;
        Description = description;
        _ledgerAccounts = ledgerAccounts.ToList();
    }

    /// <summary>
    /// Abstract factory method for mutation methods. Must be implemented in derived types.
    /// </summary>
    /// <param name="id">The unique identifier for the ledger.</param>
    /// <param name="name">The name of the ledger.</param>
    /// <param name="description">The description of the ledger.</param>
    /// <param name="ledgerAccounts">The collection of ledger accounts.</param>
    /// <returns>A new instance of the derived ledger type.</returns>
    protected abstract TSelf CreateInstance(Guid id, string name, string description, IEnumerable<TLedgerAccount> ledgerAccounts);

    /// <summary>
    /// Returns a new instance with the specified name.
    /// </summary>
    public TSelf WithName(string newName)
        => CreateInstance(Id, newName, Description, LedgerAccounts);

    /// <summary>
    /// Returns a new instance with the specified description.
    /// </summary>
    public TSelf WithDescription(string newDescription)
        => CreateInstance(Id, Name, newDescription, LedgerAccounts);

    /// <summary>
    /// Returns a new instance with the specified ledger accounts.
    /// </summary>
    public TSelf WithLedgerAccounts(IEnumerable<TLedgerAccount> newLedgerAccounts)
        => CreateInstance(Id, Name, Description, newLedgerAccounts);

    /// <summary>
    /// Returns a new instance with the specified ledger account added.
    /// </summary>
    /// <param name="ledgerAccount">The ledger account to add.</param>
    /// <returns>A new instance of the ledger with the account added.</returns>
    /// <exception cref="ArgumentNullException">Thrown if ledgerAccount is null.</exception>
    public TSelf AddLedgerAccount(TLedgerAccount ledgerAccount)
    {
        if (ledgerAccount == null)
            throw new ArgumentNullException(nameof(ledgerAccount));
        return CreateInstance(Id, Name, Description, LedgerAccounts.Append(ledgerAccount));
    }

    /// <summary>
    /// Factory method to create a Ledger from journals and accounts.
    /// </summary>
    /// <typeparam name="TJournal">The journal type.</typeparam>
    /// <typeparam name="TJournalEntry">The journal entry type.</typeparam>
    /// <typeparam name="TJournalLine">The journal line type.</typeparam>
    /// <typeparam name="TAccount">The account type.</typeparam>
    /// <typeparam name="TLedgerEntry">The ledger entry type.</typeparam>
    /// <param name="name">The name of the ledger.</param>
    /// <param name="description">The description of the ledger.</param>
    /// <param name="journal">The journal instance.</param>
    /// <param name="accounts">The collection of accounts.</param>
    /// <param name="ledgerEntryFactory">A factory function to create ledger entries from journal lines.</param>
    /// <param name="ledgerAccountFactory">A factory function to create ledger accounts from accounts and ledger entries.</param>
    /// <param name="ledgerFactory">A factory function to create the ledger instance.</param>
    /// <returns>A new ledger instance created from the journal and accounts.</returns>
    /// <exception cref="ArgumentNullException">Thrown if journal or accounts is null.</exception>
    /// <exception cref="ArgumentException">Thrown if name or description is null or empty.</exception>
    public static TSelf FromJournal<TJournal, TJournalEntry, TJournalLine, TAccount, TLedgerEntry>(
        string name,
        string description,
        TJournal journal,
        IEnumerable<TAccount> accounts,
        Func<Guid, string, string, string, decimal, decimal, DateTime, string, TLedgerEntry> ledgerEntryFactory,
        Func<string, string, AccountType, string, string, string, IEnumerable<TLedgerEntry>, TLedgerAccount> ledgerAccountFactory,
        Func<Guid, string, string, IEnumerable<TLedgerAccount>, TSelf> ledgerFactory)
        where TJournal : class
        where TJournalEntry : class
        where TJournalLine : class
        where TAccount : class
        where TLedgerEntry : class
    {
        if (journal == null)
            throw new ArgumentNullException(nameof(journal));
        if (accounts == null)
            throw new ArgumentNullException(nameof(accounts));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException($"'{nameof(description)}' cannot be null or empty.", nameof(description));

        var journalEntriesProp = typeof(TJournal).GetProperty("JournalEntries");
        var journalLinesProp = typeof(TJournalEntry).GetProperty("JournalLines");
        var accountCodeProp = typeof(TAccount).GetProperty("Code");
        var accountNameProp = typeof(TAccount).GetProperty("Name");
        var accountTypeProp = typeof(TAccount).GetProperty("Type");
        var accountDescriptionProp = typeof(TAccount).GetProperty("Description");
        var accountParentCodeProp = typeof(TAccount).GetProperty("ParentCode");
        var accountNormallyProp = typeof(TAccount).GetProperty("Normally");

        var entryDescriptionProp = typeof(TJournalEntry).GetProperty("Description");
        var entryJournalEntryIDProp = typeof(TJournalEntry).GetProperty("JournalEntryID");
        var entryDateProp = typeof(TJournalEntry).GetProperty("Date");
        var entryReferenceProp = typeof(TJournalEntry).GetProperty("Reference");

        var lineAccountIDProp = typeof(TJournalLine).GetProperty("AccountID");
        var lineDebitProp = typeof(TJournalLine).GetProperty("Debit");
        var lineCreditProp = typeof(TJournalLine).GetProperty("Credit");

        var ledgerAccounts = accounts.Select(account =>
        {
            var accountCode = (string)accountCodeProp!.GetValue(account)!;
            var accountName = (string)accountNameProp!.GetValue(account)!;
            var accountType = (AccountType)accountTypeProp!.GetValue(account)!;
            var accountDescription = (string)accountDescriptionProp!.GetValue(account)!;
            var accountParentCode = (string?)accountParentCodeProp!.GetValue(account) ?? string.Empty;
            var accountNormally = (string)accountNormallyProp!.GetValue(account)!;

            var journalEntries = (IEnumerable<TJournalEntry>)journalEntriesProp!.GetValue(journal)!;
            var entries = journalEntries
                .SelectMany(entry =>
                {
                    var entryDescription = (string)entryDescriptionProp!.GetValue(entry)!;
                    var entryJournalEntryID = (string)entryJournalEntryIDProp!.GetValue(entry)!;
                    var entryDate = (DateTime)entryDateProp!.GetValue(entry)!;
                    var entryReference = (string)entryReferenceProp!.GetValue(entry)!;
                    var journalLines = (IEnumerable<TJournalLine>)journalLinesProp!.GetValue(entry)!;
                    return journalLines
                        .Where(line => (string)lineAccountIDProp!.GetValue(line)! == accountCode)
                        .Select(line => ledgerEntryFactory(
                            Guid.NewGuid(),
                            entryDescription,
                            (string)lineAccountIDProp!.GetValue(line)!,
                            entryJournalEntryID,
                            (decimal)lineDebitProp!.GetValue(line)!,
                            (decimal)lineCreditProp!.GetValue(line)!,
                            entryDate,
                            entryReference
                        ));
                })
                .ToList();

            return ledgerAccountFactory(
                accountCode,
                accountName,
                accountType,
                accountDescription,
                accountParentCode,
                accountNormally,
                entries
            );
        }).Where(la =>
        {
            var ledgerEntriesProp = typeof(TLedgerAccount).GetProperty("LedgerEntries");
            var ledgerEntries = (IEnumerable<TLedgerEntry>)ledgerEntriesProp!.GetValue(la)!;
            return ledgerEntries.Any();
        }).ToList();

        return ledgerFactory(Guid.NewGuid(), name, description, ledgerAccounts);
    }
}