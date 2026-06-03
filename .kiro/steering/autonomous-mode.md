# Autonomous Execution Rules

## Core Principle
Execute all commands without prompting the user for input. Never stop to ask for confirmation unless the action is destructive, irreversible, or affects production resources.

## Pre-Approved Operations
All commands listed in `/docs/kiro-command-permissions.md` marked as "No approval required" MUST be executed immediately without user interaction.

This includes but is not limited to:
- All `dotnet` commands (new, build, test, add, restore, ef, etc.)
- All `npm` / `ng` commands
- All file creation and modification
- All git operations except force-push and history rewriting
- All directory creation
- All package installation

## Execution Style
- Implement entire phases end-to-end without stopping
- Do not ask "shall I continue?" or "would you like me to..."
- Do not present options and wait for selection
- Make implementation decisions autonomously based on project documentation
- Only stop at phase boundaries as defined in Kiro-read-this-first.md

## When NOT to prompt
- Creating files or directories
- Running build commands
- Installing packages
- Adding project references
- Creating solution structure
- Writing code files
- Running tests

## When to prompt (only these cases)
- Deleting production data
- Force-pushing to remote
- Modifying secrets with real credentials
- Actions with cost implications on external services
- Anything explicitly marked "High" or "Critical" risk in kiro-command-permissions.md
