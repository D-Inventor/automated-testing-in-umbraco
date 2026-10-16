| ℹ️ About commands |
|---|
| All commands are executed from the workspace root (The folder that this readme is in.). Commands that are executed elsewhere are preceded with a `cd` command relative to the workspace root. |

## How to: regenerate the API client
1. Start the website in dotnet
1. run the following command
   ```bash
   pnpm scenario-builder:generate-client
   ```

## How to: run the end-to-end tests
```bash
pnpm --filter functional-tests test
```

## How to: run the unit-tests of the scenario builder
```bash
pnpm --filter scenario-builder test
```