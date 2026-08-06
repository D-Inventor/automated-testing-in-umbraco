import eslint from '@eslint/js';
import tseslint from 'typescript-eslint';
import eslintConfigPrettier from 'eslint-config-prettier';
import { globalIgnores } from 'eslint/config';

export default tseslint.config(
  eslint.configs.recommended,
  tseslint.configs.recommended,
  {
    languageOptions: {
      parserOptions: {
        projectService: {
          allowDefaultProject: ['*.js', 'packages/*/*.ts'],
        },
        tsconfigRootDir: import.meta.dirname,
      },
    },
  },
  // must be last so Prettier formatting rules win over ESLint formatting rules
  eslintConfigPrettier,
  globalIgnores([
    'packages/scenario-builder/src/client/client',
    'packages/scenario-builder/src/client/core',
  ]),
);
