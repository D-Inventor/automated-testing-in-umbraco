#!/usr/bin/env pwsh
<#
.SYNOPSIS
Denies npm command execution before running.

.DESCRIPTION
Reads the PreToolUse JSON payload from standard input and checks
tool_input.command for 'npm' commands. Denies matching commands and instructs
the agent to use 'pnpm' instead.
#>

$ErrorActionPreference = 'Stop'

<#
.SYNOPSIS
Tests if an npm command exists in the command nodes.

.PARAMETER CommandNodes
Array of CommandAst nodes to analyze.

.OUTPUTS
[bool] Returns $true if "npm" is detected, $false otherwise.
#>
function Test-HasNpm {
    param(
        [Parameter(Mandatory = $true)]
        [array]$CommandNodes
    )

    foreach ($cmdNode in $CommandNodes) {
        $commandName = $cmdNode.CommandElements[0].Value
        
        if ($commandName -eq 'npm' -or $commandName -like '*npm.cmd' -or $commandName -like '*npm.exe') {
            return $true
        }
    }

    return $false
}

try {
    $hookInput = [Console]::In.ReadToEnd() | ConvertFrom-Json
    $command = $hookInput.tool_input.command
    $output = @{}

    if ($command -is [string]) {
        try {
            # Parse the command using AST to extract all commands
            $ast = [System.Management.Automation.Language.Parser]::ParseInput(
                $command, 
                [ref]$null, 
                [ref]$null
            )

            # Find all CommandAst nodes (individual commands)
            $commandNodes = $ast.FindAll(
                { $args[0] -is [System.Management.Automation.Language.CommandAst] }, 
                $true
            )

            # Detect npm commands and deny
            if (Test-HasNpm -CommandNodes $commandNodes) {
                $output.hookSpecificOutput = @{
                    hookEventName = 'PreToolUse'
                    permissionDecision = 'deny'
                    permissionDecisionReason = 'This project uses pnpm'
                }
            }
        }
        catch {
            # If AST parsing fails, log the error but don't attempt detection
            [Console]::Error.WriteLine("Failed to parse command with AST: $_")
        }
    }

    $output | ConvertTo-Json -Depth 3 -Compress
    exit 0
}
catch {
    [Console]::Error.WriteLine("Hook validation failed: $_")
    exit 1
}
