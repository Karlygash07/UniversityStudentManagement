# UniFlow — Student Management System

Week 4 university assignment: an ASP.NET Core MVC application for maintaining student registration records with Entity Framework Core, SQL Server, and LINQ search.

## Features

- Student directory with server-side, partial-name search using LINQ `Where`.
- Create, read, edit, and confirmed delete workflows with asynchronous EF Core operations.
- SQL Server persistence, EF Core code-first migration, and a required one-to-many Student–Group relationship.
- Model validation, anti-forgery tokens, foreign-key checks, overposting protection, and safe error handling.
- Dashboard statistics queried from the database; starter academic groups are added by the initial migration.
- Responsive UniFlow dashboard interface with accessible forms and empty states.

## Technology stack

- .NET 9 / ASP.NET Core MVC / Razor Views
- Entity Framework Core 9 and Microsoft SQL Server
- HTML, CSS, and vanilla JavaScript

## Architecture

```text
Controllers/    MVC actions and database workflows
Data/           EF Core DbContext and entity configuration
Models/         Student and Group entities
ViewModels/     Input and page models
Migrations/     SQL Server schema and starter data
Views/          Razor pages and shared layout
wwwroot/        Responsive styles and small UI interactions
```

## Database structure

`Students` stores `Id`, `Name`, `Email`, and the required `GroupId` foreign key. `Groups` stores `Id` and `Name`. One group can contain multiple students; each student belongs to exactly one group. Student email and group name have unique indexes. The initial migration adds four starter groups so registration can be used immediately.

## Requirements

- .NET 9 SDK
- SQL Server LocalDB (included with Visual Studio) or a SQL Server instance
- Optional: `dotnet-ef` 9.x for creating future migrations

## Configure and run

The default connection targets the local SQL Server Express instance in `appsettings.json`:

```text
Server=.\SQLEXPRESS;Database=UniFlowStudents;Trusted_Connection=True;MultipleActiveResultSets=true;Encrypt=False;TrustServerCertificate=True
```

For LocalDB or another instance, set `ConnectionStrings:DefaultConnection` in user secrets or an ignored `appsettings.Local.json`; never commit credentials. Example PowerShell commands from the project directory:

```powershell
dotnet restore
dotnet build
dotnet ef database update
dotnet run
```

Open the HTTPS URL printed by `dotnet run`. If `dotnet-ef` is not installed, install a version matching EF Core 9 with `dotnet tool install --global dotnet-ef --version 9.0.10`, then rerun the database update command. To create later migrations:

```powershell
dotnet ef migrations add DescribeYourChange
dotnet ef database update
```

## Try the CRUD workflows

1. Open `/Students` and select **Add student**.
2. Enter a name and valid email; choose a starter academic group.
3. Search by any part of the student's name in the directory search field.
4. Use the row actions to edit details or open the delete confirmation. Deletion happens only after the confirmation form is submitted.
5. Reload the app to confirm SQL Server persistence.
6. Submit the form with an empty name or malformed email to see server-side validation.

The search trims whitespace and applies `Where(student => student.Name.Contains(searchTerm))` before SQL results are loaded. SQL Server parameterizes this expression through EF Core.

## Security and error handling

Create, edit, and delete POST actions validate anti-forgery tokens. Form ViewModels constrain bindable fields and edits update only name, email, and group ID. Group IDs are checked against the database, Razor encodes output, database queries are parameterized, and invalid record IDs return 404. Production errors show a friendly page rather than exception details. Unique email conflicts return a validation message.

## Screenshots

Add screenshots here after running the app, for example `docs/screenshots/dashboard.png` and `docs/screenshots/students.png`.

## Week 4 checklist

- [x] ASP.NET Core MVC and Razor Views
- [x] EF Core with SQL Server provider and code-first migration
- [x] Student and Group models with one-to-many foreign key
- [x] Index, create, edit, and confirmed delete
- [x] `ModelState.IsValid`, validation, and anti-forgery protection
- [x] LINQ `Where` search and eager loading with `Include`
- [x] Asynchronous EF Core reads and writes
- [x] Responsive dashboard, student directory, and forms
- [x] GitHub-ready ignore rules and setup instructions

## Test plan

Run `dotnet restore` and `dotnet build`, apply the migration, then manually exercise the CRUD, search, invalid-input, missing-ID, persistence, and mobile viewport scenarios above. SQL Server behavior requires a reachable SQL Server instance.
