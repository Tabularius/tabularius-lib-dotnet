using System;
using TabulariusLib.Entities;

using Xunit;

namespace UnitTests.TabulariusLib;

public class AccountTest
{
    [Fact]
    public void Constructor_ShouldThrowException_WhenNameIsNullOrEmpty()
    {
        Assert.Throws<ArgumentException>(() => Account.Create(null!, "Description", "Code", AccountType.Asset, null, "Normally"));
        Assert.Throws<ArgumentException>(() => Account.Create("", "Description", "Code", AccountType.Asset, null, "Normally"));
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenDescriptionIsNullOrEmpty()
    {
        Assert.Throws<ArgumentException>(() => Account.Create("Name", null!, "Code", AccountType.Asset, null, "Normally"));
        Assert.Throws<ArgumentException>(() => Account.Create("Name", "", "Code", AccountType.Asset, null, "Normally"));
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenCodeIsNullOrEmpty()
    {
        Assert.Throws<ArgumentException>(() => Account.Create("Name", "Description", null!, AccountType.Asset, null, "Normally"));
        Assert.Throws<ArgumentException>(() => Account.Create("Name", "Description", "", AccountType.Asset, null, "Normally"));
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenNormallyIsNullOrEmpty()
    {
        Assert.Throws<ArgumentException>(() => Account.Create("Name", "Description", "Code", AccountType.Asset, null, null!));
        Assert.Throws<ArgumentException>(() => Account.Create("Name", "Description", "Code", AccountType.Asset, null, ""));
    }

    [Fact]
    public void Constructor_ShouldInitializeProperties_WhenValidArgumentsAreProvided()
    {
        var name = "Name";
        var description = "Description";
        var code = "Code";
        var type = AccountType.Asset;
        string? parentCode = null;
        var normally = "Normally";
        var account = Account.Create(name, description, code, type, parentCode, normally);
        Assert.Equal(name, account.Name);
        Assert.Equal(description, account.Description);
        Assert.Equal(code, account.Code);
        Assert.Equal(type, account.Type);
        Assert.Equal(parentCode, account.ParentCode);
        Assert.Equal(normally, account.Normally);
    }

    [Fact]
    public void WithName_ShouldReturnNewAccount_WithUpdatedName()
    {
        var account = Account.Create("Name", "Description", "Code", AccountType.Asset, null, "Normally");
        var newName = "NewName";
        var updatedAccount = account.WithName(newName);
        Assert.Equal(newName, updatedAccount.Name);
        Assert.Equal(account.Description, updatedAccount.Description);
    }

    [Fact]
    public void WithDescription_ShouldReturnNewAccount_WithUpdatedDescription()
    {
        var account = Account.Create("Name", "Description", "Code", AccountType.Asset, null, "Normally");
        var newDescription = "NewDescription";
        var updatedAccount = account.WithDescription(newDescription);
        Assert.Equal(newDescription, updatedAccount.Description);
        Assert.Equal(account.Name, updatedAccount.Name);
    }

    [Fact]
    public void WithCode_ShouldReturnNewAccount_WithUpdatedCode()
    {
        var account = Account.Create("Name", "Description", "Code", AccountType.Asset, null, "Normally");
        var newCode = "NewCode";
        var updatedAccount = account.WithCode(newCode);
        Assert.Equal(newCode, updatedAccount.Code);
        Assert.Equal(account.Name, updatedAccount.Name);
    }

    [Fact]
    public void WithType_ShouldReturnNewAccount_WithUpdatedType()
    {
        var account = Account.Create("Name", "Description", "Code", AccountType.Asset, null, "Normally");
        var newType = AccountType.Expense;
        var updatedAccount = account.WithType(newType);
        Assert.Equal(newType, updatedAccount.Type);
        Assert.Equal(account.Name, updatedAccount.Name);
    }

    [Fact]
    public void WithParentCode_ShouldReturnNewAccount_WithUpdatedParentCode()
    {
        var account = Account.Create("Name", "Description", "Code", AccountType.Asset, null, "Normally");
        var newParentCode = "Parent001";
        var updatedAccount = account.WithParentCode(newParentCode);
        Assert.Equal(newParentCode, updatedAccount.ParentCode);
        Assert.Equal(account.Name, updatedAccount.Name);
    }

    [Fact]
    public void WithNormally_ShouldReturnNewAccount_WithUpdatedNormally()
    {
        var account = Account.Create("Name", "Description", "Code", AccountType.Asset, null, "Normally");
        var newNormally = "NewNormally";
        var updatedAccount = account.WithNormally(newNormally);
        Assert.Equal(newNormally, updatedAccount.Normally);
        Assert.Equal(account.Name, updatedAccount.Name);
    }
}