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

try {
    $hookInput = [Console]::In.ReadToEnd() | ConvertFrom-Json
    $command = $hookInput.tool_input.command
    $output = @{}

    if ($command -is [string] -and $command -match '(?<![\w.-])dotnet(?:\.exe)?["'']?\s+test(?=\s|$|[;&|)])') {
        $output.hookSpecificOutput = @{
            hookEventName = 'PreToolUse'
            permissionDecision = 'deny'
            permissionDecisionReason = 'The dotnet test console command is not permitted.'
            additionalContext = 'Read and follow the csharp-testing skill, which explains how tests should be executed.'
        }
    }

    $output | ConvertTo-Json -Depth 3 -Compress
    exit 0
}
catch {
    [Console]::Error.WriteLine("Hook validation failed: $_")
    exit 1
}
