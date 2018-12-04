/* eslint 'import/no-commonjs': 'off' */
const config = require('./jest.config.watch');
module.exports = {
  ...config,
  collectCoverageFrom: ['<rootDir>/src/**/*.{js,jsx}', '!**/node_modules/**'],
  testResultsProcessor: '<rootDir>/node_modules/jest-html-reporter'
};
