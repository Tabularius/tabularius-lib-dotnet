using System;

using tabularius.TabulariusLib.Entities;

namespace tabularius.TabulariusLib.Interfaces
{
    internal interface IAccount
    {
        string Code { get; }
        string Name { get; }
        string Description { get; }
        AccountType Type { get; }
        string? ParentCode { get; }
        string Normally { get; }

        Account WithType(AccountType newType);
        Account WithName(string newName);
        Account WithDescription(string newDescription);
        Account WithCode(string newCode);
        Account WithParentCode(string? newParentCode);
        Account WithNormally(string newNormally);
    }
}