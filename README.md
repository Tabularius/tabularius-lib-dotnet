# TabulariusLib (.NET)

TabulariusLib is a modern .NET library for core accounting operations, designed to provide immutable, strongly-typed entities for journals, ledgers, trial balances, profit and loss statements, and balance sheets. The library is built for extensibility, correctness, and auditability, and is suitable for integration with Entity Framework Core and other .NET data platforms.

## Project Goals

- **Immutability:** All main entities (Journal, Ledger, Trial Balance, Profit and Loss Statement, Balance Sheet) are immutable. Once created, they cannot be changed—only new entries can be added. This mirrors real-world accounting, where journals and ledgers are append-only and cannot be altered retroactively.
- **Strong Typing:** All accounting entities are implemented as strongly-typed records, ensuring compile-time safety and clarity.
- **Entity Relationships:**  
  - The **Trial Balance** is always derived from the current state of the **Ledger**.
  - The **Balance Sheet** is generated from the **Trial Balance** after it has been closed (i.e., after income and expense accounts are closed to equity).
  - The **Profit and Loss Statement** is generated directly from the **Ledger** for a given period.
- **Validation:** All mutation methods return new instances and enforce validation, ensuring data integrity and auditability.
- **Designed for EF Core:** Entities are ready for use with Entity Framework Core, supporting modern .NET data workflows.

## Accounting Principles

TabulariusLib is built on core accounting principles:

- **Immutability:** Journals and ledgers are append-only. Once an entry is recorded, it cannot be changed or deleted.
- **Auditability:** All changes result in new instances, preserving the full history of accounting data.
- **Clear Entity Dependencies:**  
  - **Journal** → **Ledger** → **Trial Balance** → **Balance Sheet**
  - **Journal** → **Ledger** → **Profit and Loss Statement**
- **Closing Process:** The trial balance must be closed (income and expense accounts transferred to equity) before generating a balance sheet.

## Features

- Immutable, strongly-typed accounting entities
- Generate ledgers, trial balances, profit and loss statements, and balance sheets
- Designed for use with Entity Framework Core
- Fully unit-tested
- Extensible for custom accounting workflows

## Getting Started

### Clone the repo

```sh
git clone https://github.com/Tabularius/tabularius-lib-dotnet.git
```

- Open with VS Code and start the devcontainer
- Open the folder in VS Code
- Use the "Reopen in Container" command to start the devcontainer
- Make your changes and commit a pull request

## Contributing
Contributions are welcome! Please fork the repository, create a feature branch, and submit a pull request. For major changes, please open an issue first to discuss your ideas.

## License
Apache-2.0

## For more details, see the NuGet package documentation and the unit tests in the test/UnitTests folder. 
