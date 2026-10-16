#!/usr/bin/env pwsh
<#
.SYNOPSIS
Denies dotnet test command execution before running.

.DESCRIPTION
Reads the PreToolUse JSON payload from standard input and checks
tool_input.command for 'dotnet test'. Denies matching commands and instructs
the agent to read and follow the csharp-testing skill instead.
#>

$ErrorActionPreference = 'Stop'

<#
.SYNOPSIS
Tests if a dotnet test command exists in the command nodes.

.PARAMETER CommandNodes
Array of CommandAst nodes to analyze.

.OUTPUTS
[bool] Returns $true if "dotnet test" is detected, $false otherwise.
#>
function Test-HasDotnetTest {
    param(
        [Parameter(Mandatory = $true)]
        [array]$CommandNodes
    )

    foreach ($cmdNode in $CommandNodes) {
        $commandName = $cmdNode.CommandElements[0].Value
        
        if ($commandName -eq 'dotnet' -or $commandName -like '*dotnet.exe') {
            for ($i = 1; $i -lt $cmdNode.CommandElements.Count; $i++) {
                $element = $cmdNode.CommandElements[$i]
                
                if ($element -is [System.Management.Automation.Language.CommandParameterAst]) {
                    if ($element.ParameterName -eq 'test') {
                        return $true
                    }
                } elseif ($element.Extent.Text -eq 'test') {
                    return $true
                }
            }
        }
    }

    return $false
}

<#
.SYNOPSIS
Tests if a pnpm test command exists in the command nodes.

.DESCRIPTION
Detects "pnpm test", "pnpm run test", and variations like "test:e2e" or "test:unit".

.PARAMETER CommandNodes
Array of CommandAst nodes to analyze.

.OUTPUTS
[bool] Returns $true if "pnpm test" pattern is detected, $false otherwise.
#>
function Test-HasPnpmTest {
    param(
        [Parameter(Mandatory = $true)]
        [array]$CommandNodes
    )

    foreach ($cmdNode in $CommandNodes) {
        $commandName = $cmdNode.CommandElements[0].Value
        
        if ($commandName -eq 'pnpm' -or $commandName -like '*pnpm.cmd' -or $commandName -like '*pnpm.exe') {
            # Look through the elements to find "test" or "test:*" pattern
            for ($i = 1; $i -lt $cmdNode.CommandElements.Count; $i++) {
                $element = $cmdNode.CommandElements[$i]
                $elementText = $element.Extent.Text
                
                # Match "test" or "test:something"
                if ($elementText -match '^test(:\w+)?$') {
                    return $true
                }

                if ($elementText -match '^vitest$') {
                    return $true
                }
            }
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

            # Chain detection functions - each creates its own output
            if (Test-HasDotnetTest -CommandNodes $commandNodes) {
                $output.hookSpecificOutput = @{
                    hookEventName = 'PreToolUse'
                    permissionDecision = 'deny'
                    permissionDecisionReason = 'The dotnet test console command is not permitted.'
                    additionalContext = 'Read and follow the csharp-testing skill, which explains how tests should be executed.'
                }
            }
            elseif (Test-HasPnpmTest -CommandNodes $commandNodes) {
                $output.hookSpecificOutput = @{
                    hookEventName = 'PreToolUse'
                    permissionDecision = 'deny'
                    permissionDecisionReason = 'The pnpm test console command is not permitted.'
                    additionalContext = 'Read and follow the typescript-testing skill, which explains how tests should be executed.'
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
