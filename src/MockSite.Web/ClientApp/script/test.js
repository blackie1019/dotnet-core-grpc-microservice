const path = require('path')
const jest = require('jest')

process.env.BABEL_ENV = 'test'
process.env.NODE_ENV = 'test'

const withTestReport = !!process.env.WITH_TEST_REPORT
jest.run([
  withTestReport ? '--coverage' : '--watch',
  '--config',
  path.join(__dirname, '../config/jest/config.js')
])
