# Instalment Sales

A desktop application built in Visual Basic .NET (VB.NET) to manage and track customer installment plans, payments, and sales accounts.

## Overview

**Instalment Sales** is a workflow application designed for managing deferred payment agreements, tracking outstanding balances, and handling user authentication control barriers. The architecture utilizes custom UI controls alongside a clean, form-based layout to streamline account lookups, payment processing, and administrative tracking.

## Features

- **Installment Ledger Tracking:** Comprehensive account tools to check customer payment histories, upcoming due dates, and structural ledger entries.
- **Custom UI Components:** Includes modular control fragments like a specialized secure text layout (`PasswordBox`) and transactional modules (`PayBox`).
- **Administrative Control Panel:** High-level dashboard interface (`forms/Manager`) giving administrators global management options over active sales logs.
- **Structured Global Event Pipeline:** Employs `ApplicationEvents.vb` to consistently oversee initial runtime startups, teardowns, and global unhandled exception tracking.

## Tech Stack

- **Language:** Visual Basic .NET (VB.NET)
- **Framework:** .NET / Windows Forms (WinForms)
- **IDE Development Environment:** Visual Studio

## Project Structure

```bash
instalment-sales/
├── controls/                 # Custom reusable UI controls and interface modules
│   ├── PasswordBox.vb        # Masked text input component for secure log entries
│   └── PayBox.vb             # Transaction processing module logic
├── forms/                    # Core graphical window forms and workspace layouts
│   └── Manager.vb            # Master administrative control panel interface
├── My Project/               # Visual Studio application compilation metadata and settings
├── Resources/                # Static assets, icons, dynamic localized text assets
├── .gitignore                # Active filtration directives for IDE caches and build output
├── ApplicationEvents.vb      # Lifecycle event controller for startup and error tracking
├── MyApp.sln                 # Main visual studio development solution index file
└── MyApp.vbproj              # Project-level dependency paths and build rules
```

## Setup & Execution

### Prerequisites

- Windows 10 / 11 Operating System
- **Visual Studio** (with the _.NET Desktop Development_ workload activated)

### Getting Started

- Clone the codebase structure locally:

  ```bash
  git clone https://github.com
  ```

- Launch the system project workspace by opening `MyApp.sln` inside Visual Studio.

- Verify the project has restored all configuration frameworks cleanly, then hit **F5** or select **Start Debugging** to compile and launch the runtime application layout.

## Author

H2SO4-1191 – Software Engineer
