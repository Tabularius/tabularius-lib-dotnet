using System;

using TabulariusLib.Entities;

namespace UnitTests.TabulariusLib;

public class JournalLineTest
{
    [Fact]
    public void Constructor_ShouldThrowException_WhenIdIsEmpty()
    {
        // Arrange
        var id = Guid.Empty;
        var description = "Test Description";
        var accountID = "123";
        var debit = 100m;
        var credit = 0m;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => JournalLine.Create(id, description, accountID, debit, credit));
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenDescriptionIsNullOrEmpty()
    {
        // Arrange
        var id = Guid.NewGuid();
        var accountID = "123";
        var debit = 100m;
        var credit = 0m;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => JournalLine.Create(id, null!, accountID, debit, credit));
        Assert.Throws<ArgumentException>(() => JournalLine.Create(id, string.Empty, accountID, debit, credit));
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenAccountIDIsEmpty()
    {
        // Arrange
        var id = Guid.NewGuid();
        var description = "Test Description";
        string accountID = "";
        var debit = 100m;
        var credit = 0m;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => JournalLine.Create(id, description, accountID, debit, credit));
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenBothDebitAndCreditAreZero()
    {
        // Arrange
        var id = Guid.NewGuid();
        var description = "Test Description";
        var accountID = "123";
        var debit = 0m;
        var credit = 0m;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => JournalLine.Create(id, description, accountID, debit, credit));
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenBothDebitAndCreditAreNonZero()
    {
        // Arrange
        var id = Guid.NewGuid();
        var description = "Test Description";
        var accountID = "123";
        var debit = 100m;
        var credit = 50m;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => JournalLine.Create(id, description, accountID, debit, credit));
    }

    [Fact]
    public void Constructor_ShouldCreateInstance_WhenValidArgumentsAreProvided()
    {
        // Arrange
        var id = Guid.NewGuid();
        var description = "Test Description";
        var accountID = "123";
        var debit = 100m;
        var credit = 0m;

        // Act
        var journalLine = JournalLine.Create(id, description, accountID, debit, credit);

        // Assert
        Assert.Equal(id, journalLine.Id);
        Assert.Equal(description, journalLine.Description);
        Assert.Equal(accountID, journalLine.AccountID);
        Assert.Equal(debit, journalLine.Debit);
        Assert.Equal(credit, journalLine.Credit);
    }

    [Fact]
    public void WithDescription_ShouldReturnNewInstanceWithUpdatedDescription()
    {
        // Arrange
        var id = Guid.NewGuid();
        var description = "Test Description";
        var accountID = "123";
        var debit = 100m;
        var credit = 0m;
        var journalLine = JournalLine.Create(id, description, accountID, debit, credit);

        // Act
        var updatedJournalLine = journalLine.WithDescription("Updated Description");

        // Assert
        Assert.Equal("Updated Description", updatedJournalLine.Description);
        Assert.Equal(journalLine.Id, updatedJournalLine.Id);
        Assert.Equal(journalLine.AccountID, updatedJournalLine.AccountID);
        Assert.Equal(journalLine.Debit, updatedJournalLine.Debit);
        Assert.Equal(journalLine.Credit, updatedJournalLine.Credit);
    }

    [Fact]
    public void WithAccountID_ShouldReturnNewInstanceWithUpdatedAccountID()
    {
        // Arrange
        var id = Guid.NewGuid();
        var description = "Test Description";
        var accountID = "123";
        var debit = 100m;
        var credit = 0m;
        var journalLine = JournalLine.Create(id, description, accountID, debit, credit);
        var newAccountID = "1234";

        // Act
        var updatedJournalLine = journalLine.WithAccountID(newAccountID);

        // Assert
        Assert.Equal(newAccountID, updatedJournalLine.AccountID);
        Assert.Equal(journalLine.Id, updatedJournalLine.Id);
        Assert.Equal(journalLine.Description, updatedJournalLine.Description);
        Assert.Equal(journalLine.Debit, updatedJournalLine.Debit);
        Assert.Equal(journalLine.Credit, updatedJournalLine.Credit);
    }
}
