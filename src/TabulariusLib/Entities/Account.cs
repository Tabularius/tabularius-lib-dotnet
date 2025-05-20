
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using TabulariusLib.Interfaces;

namespace TabulariusLib.Entities;

[Table("Accounts")]
public sealed record Account : IAccount
{
    [Key]
    [Required]
    [MaxLength(256)]
    public string Code { get; init; }

    [Required]
    [MaxLength(256)]
    public string Name { get; init; }
    
    [Required]
    [MaxLength(256)]
    public string Description { get; init; }
        
    [Required]
    [MaxLength(256)]
    public AccountType Type { get; init; }
    
    [MaxLength(256)]
    public string? ParentCode { get; init; }
    
    [Required]
    [MaxLength(256)]
    public string Normally { get; init; }

    // Parameterless constructor for EF Core
    private Account()
    {
        Name = string.Empty;
        Description = string.Empty;
        Code = string.Empty;
        Type = AccountType.Asset;
        ParentCode = null;
        Normally = string.Empty;
    }

    // Private full constructor for validation
    private Account(
        string name,
        string description,
        string code,
        AccountType type,
        string? parentCode,
        string normally)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException($"'{nameof(description)}' cannot be null or empty for account {name}.", nameof(description));
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException($"'{nameof(code)}' cannot be null or empty.", nameof(code));
        if (!Enum.IsDefined(typeof(AccountType), type))
            throw new ArgumentException($"'{nameof(type)}' must be a valid AccountType.", nameof(type));
        // ParentCode can be null or whitespace, so no validation here
        if (string.IsNullOrWhiteSpace(normally))
            throw new ArgumentException($"'{nameof(normally)}' cannot be null or empty.", nameof(normally));

        Name = name;
        Description = description;
        Code = code;
        Type = type;
        ParentCode = string.IsNullOrWhiteSpace(parentCode) ? null : parentCode;
        Normally = normally;
    }

    // Factory method for creation with validation
    public static Account Create(
        string name,
        string description,
        string code,
        AccountType type,
        string? parentCode,
        string normally)
        => new(name, description, code, type, parentCode, normally);

    // Change methods (immutably create new validated Account)
    public Account WithType(AccountType newType) =>
        Create(Name, Description, Code, newType, ParentCode, Normally);

    public Account WithName(string newName) =>
        Create(newName ?? Name, Description, Code, Type, ParentCode, Normally);

    public Account WithDescription(string newDescription) =>
        Create(Name, newDescription ?? Description, Code, Type, ParentCode, Normally);

    public Account WithCode(string newCode) =>
        Create(Name, Description, newCode ?? Code, Type, ParentCode, Normally);

    public Account WithParentCode(string? newParentCode) =>
        Create(Name, Description, Code, Type, newParentCode, Normally);

    public Account WithNormally(string newNormally) =>
        Create(Name, Description, Code, Type, ParentCode, newNormally ?? Normally);
}
