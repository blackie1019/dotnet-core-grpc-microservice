const path = require('path')
const env = require('../env')
const paths = require('../paths')
const expandEnvVariables = require('./expandEnvVariables')

const withTestReport = !!process.env.WITH_TEST_REPORT
const reporters = ['default']

if (withTestReport) {
  reporters.push([
    path.join(__dirname, './allureReporter.js'),
    {
      category: 'Mocksite-UnitTest',
      srcDir: paths.srcDir,
      outputDir: paths.allureTestReportDir
    }
  ])
}

module.exports = {
  verbose: true,
  rootDir: '../../',
  testMatch: ['<rootDir>/src/**/*.spec.js'],
  transform: {
    '^.+\\.(js|jsx)$': 'babel-jest',
    '^.+\\.css$': '<rootDir>/config/jest/cssTransform.js',
    '^(?!.*\\.(js|jsx|css|json)$)': '<rootDir>/config/jest/fileTransform.js'
  },
  globals: { app: expandEnvVariables(env.variables).app },
  clearMocks: true,
  collectCoverageFrom: ['<rootDir>/src/**/*.{js,jsx}', '!**/node_modules/**'],
  coverageDirectory: paths.coverageDir,
  coverageReporters: ['lcov'],
  reporters,
  testFailureExitCode: 0
}
