using System;

using TabulariusLib.Entities;

namespace UnitTests.TabulariusLib;

public class JournalEntryTest
    {
        [Fact]
        public void Create_ShouldThrowArgumentException_WhenDateIsDefault()
        {
            // Arrange
            var id = Guid.NewGuid();
            var journalID = "Test Journal ID";
            var description = "Test Description";
            var reference = "Test Reference";
            var defaultDate = default(DateTime);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                JournalEntry.Create(journalID, description, defaultDate, reference, null));
            Assert.Contains("'date' cannot be empty", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void Create_ShouldSetDate_WhenValidDateIsProvided()
        {
            // Arrange
            var id = Guid.NewGuid();
            var journalID = "Test Journal ID";
            var description = "Test Description";
            var reference = "Test Reference";
            var validDate = DateTime.UtcNow;

            // Act
            var journalEntry = JournalEntry.Create(journalID, description, validDate, reference, null);

            // Assert
            Assert.Equal(validDate, journalEntry.Date);
        }

        [Fact]
        public void WithDate_ShouldReturnNewInstanceWithUpdatedDate()
        {
            // Arrange
            var id = Guid.NewGuid();
            var journalID = "Test Journal ID";
            var description = "Test Description";
            var reference = "Test Reference";
            var initialDate = DateTime.UtcNow;
            var updatedDate = initialDate.AddDays(1);

            var journalEntry = JournalEntry.Create(journalID, description, initialDate, reference, null);

            // Act
            var updatedJournalEntry = journalEntry.WithDate(updatedDate);

            // Assert
            Assert.NotSame(journalEntry, updatedJournalEntry);
            Assert.Equal(updatedDate, updatedJournalEntry.Date);
            Assert.Equal(initialDate, journalEntry.Date); // Ensure immutability
        }
    }
