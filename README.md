[![CodeQL](https://github.com/stasnowak/CodeCuisine/actions/workflows/codeql.yml/badge.svg)](https://github.com/stasnowak/CodeCuisine/actions/workflows/codeql.yml)
[![🚀 • .NET CI](https://github.com/stasnowak/CodeCuisine/actions/workflows/build.yml/badge.svg)](https://github.com/stasnowak/CodeCuisine/actions/workflows/build.yml)
 
 # CodeCuisine (Not Yet Released)

# To Do
- Make Override Optional Force Option

A tiny CLI to bootstrap a new .NET solution with sensible defaults.

CodeCuisine helps you:
- Generate a Directory.Build.props with common settings (TargetFramework, Nullable, ImplicitUsings)
- Centralize NuGet package versions into Directory.Packages.props (and remove inline versions from .csproj files)
- Add a default .gitignore (via `dotnet new gitignore`)
- Run all of the above in one go

The tool uses kebab-case command names and is powered by CommandDotNet.

## Prerequisites

- .NET SDK 9.0 or newer

## Quick start (run locally without installing)

From the repository root:

- Run all setup steps
  - `dotnet run --project CodeCuisine -- all`

- Generate/refresh Directory.Build.props
  - `dotnet run --project CodeCuisine -- build`

- Centralize package versions into Directory.Packages.props
  - `dotnet run --project CodeCuisine -- packages`

- Add a default .gitignore (use `--force` to overwrite if it exists)
  - `dotnet run --project CodeCuisine -- gitignore --force`

Notes:
- Commands search upward from the current directory to find a `.sln` file.
- The `packages` command:
  - Reads all `.csproj` files in the solution
  - Collects `PackageReference` versions
  - Writes/merges them into `Directory.Packages.props`
  - Removes `Version` attributes from the `.csproj` files (central package management)

## Global tool (optional)

This project is configured to be packed as a .NET tool with the command name `cc`.
Once published to a NuGet feed, you could install it globally like:
