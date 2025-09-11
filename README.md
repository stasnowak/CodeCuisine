[![🔬 • CodeQL](https://github.com/stasnowak/CodeCuisine/actions/workflows/codeql.yml/badge.svg)](https://github.com/stasnowak/CodeCuisine/actions/workflows/codeql.yml)
[![🛰️ • .NET CI](https://github.com/stasnowak/CodeCuisine/actions/workflows/build.yml/badge.svg)](https://github.com/stasnowak/CodeCuisine/actions/workflows/build.yml)
 
# CodeCuisine

A tiny CLI to bootstrap a new .NET solution with sensible defaults.

NuGet: https://www.nuget.org/packages/CodeCuisine/

CodeCuisine helps you:
- Generate a Directory.Build.props with common settings (TargetFramework, Nullable, ImplicitUsings)
- Centralize NuGet package versions into Directory.Packages.props (and remove inline versions from .csproj files)
- Add a default .gitignore
- Add a default .editorconfig
- Add a default global.json

Commands use kebab-case and are designed to be script-friendly.

## Prerequisites
- .NET SDK 9.0 or newer

## Install

Global tool:
- Install: `dotnet tool install -g CodeCuisine`
- Update: `dotnet tool update -g CodeCuisine`

Local (per-repo) tool:
- Create manifest: `dotnet new tool-manifest`
- Install: `dotnet tool install CodeCuisine`
- Run locally: `dotnet tool run cc ...` (or use `cc ...` if your shell resolves local tools on PATH)

## Usage

- Show help
  - `cc --help`
  - `cc <command> --help`

- Generate/refresh Directory.Build.props
  - `cc build`
  - Options: `-f|--force` overwrite if exists, `-d|--dry-run` preview without writing

- Centralize package versions into Directory.Packages.props
  - `cc packages`
  - Options: `-f|--force`, `-d|--dry-run`

- Add a default .gitignore
  - `cc gitignore`
  - Options: `-f|--force`, `-d|--dry-run`

- Add a default .editorconfig
  - `cc editorconfig`
  - Options: `-f|--force`, `-d|--dry-run`

- Add a default global.json
  - `cc global`
  - Options: `-f|--force`, `-d|--dry-run`

## Notes
- Commands search upward from the current directory to find a `.sln` file when relevant.
- Use `-d|--dry-run` to preview changes; nothing is written to disk.
- Use `-f|--force` to overwrite existing files where applicable.
