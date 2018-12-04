/* eslint 'import/no-commonjs': 'off' */
module.exports = {
  parser: 'babel-eslint',
  env: {
    es6: true,
    node: true,
    browser: true,
    jest: true
  },
  parserOptions: {
    ecmaVersion: 6,
    sourceType: 'module',
    ecmaFeatures: {
      jsx: true
    }
  },
  plugins: ['react', 'import'],
  extends: [
    'eslint:recommended',
    'plugin:react/recommended',
    'plugin:prettier/recommended'
  ],
  settings: {
    react: {
      version: '16.6.3'
    }
  },
  globals: {},
  rules: {
    'eol-last': 'off',
    'no-var': 'error',
    'no-console': 'warn',
    'space-before-function-paren': 'off',
    'generator-star-spacing': 'off',
    'import/no-commonjs': 2,
    'react/display-name': [2, { ignoreTranspilerName: true }],
    'react/forbid-prop-types': [2, { forbid: ['any'] }],
    'react/jsx-key': 2,
    'react/no-danger': 2,
    'react/no-direct-mutation-state': 2,
    'react/react-in-jsx-scope': 2,
    'react/no-multi-comp': 2,
    'react/no-unused-prop-types': 2,
    'react/no-unused-state': 2,
    'react/jsx-no-comment-textnodes': 2,
    'react/jsx-no-duplicate-props': [2, { ignoreCase: true }],
    'prettier/prettier': [
      'error',
      {
        parser: 'babylon',
        singleQuote: true,
        trailingComma: 'none',
        bracketSpacing: true,
        jsxBracketSameLine: true
      }
    ]
  }
};
