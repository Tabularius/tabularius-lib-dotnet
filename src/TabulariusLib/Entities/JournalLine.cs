using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tabularius.TabulariusLib.Entities;



[Table("JournalLines")]
public sealed record JournalLine
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; init; }

    [Required]
    [MaxLength(256)]
    public string Description { get; init; }

    [Required]
    public string AccountID { get; init; }

    public decimal Debit { get; init; }
    public decimal Credit { get; init; }

    // Private parameterless constructor for EF Core
    private JournalLine()
    { 
        Description = string.Empty;
        AccountID = string.Empty;
    }

    // Private full constructor for controlled creation
    private JournalLine(Guid id, string description, string accountID, decimal debit, decimal credit)
    {
        if (id == Guid.Empty)
            throw new ArgumentException($"'{nameof(id)}' cannot be empty.", nameof(id));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException($"'{nameof(description)}' cannot be null or empty.", nameof(description));
        if (string.IsNullOrWhiteSpace(accountID))
            throw new ArgumentException($"'{nameof(accountID)}' cannot be null or empty.", nameof(accountID));
        if (debit == 0 && credit == 0)
            throw new ArgumentException($"Either '{nameof(debit)}' or '{nameof(credit)}' must be greater than zero.", nameof(debit));
        if (debit != 0 && credit != 0)
            throw new ArgumentException($"Either '{nameof(debit)}' or '{nameof(credit)}' must be zero, not both.", nameof(debit));

        Id = id;
        Description = description;
        AccountID = accountID;
        Debit = debit;
        Credit = credit;
    }

    // Factory method for creation with validation
    public static JournalLine Create(Guid id, string description, string accountID, decimal debit, decimal credit)
        => new JournalLine(id, description, accountID, debit, credit);

    // Mutation methods with validation
    public JournalLine WithDescription(string newDescription)
        => Create(Id, newDescription, AccountID, Debit, Credit);

    public JournalLine WithAccountID(string newAccountID)
        => Create(Id, Description, newAccountID, Debit, Credit);

    public JournalLine WithDebit(decimal newDebit)
        => Create(Id, Description, AccountID, newDebit, Credit);

    public JournalLine WithCredit(decimal newCredit)
        => Create(Id, Description, AccountID, Debit, newCredit);

    public JournalLine WithAccount(Account newAccount)
        => Create(Id, Description, newAccount.Code, Debit, Credit);
}
