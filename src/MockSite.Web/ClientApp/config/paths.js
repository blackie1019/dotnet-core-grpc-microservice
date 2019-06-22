const path = require('path')

const appDir = path.join(__dirname, '..')
const reportDir = path.join(__dirname, '../report')
const coverageDir = path.join(reportDir, 'coverage')
const moduleFileExtensions = ['js', 'json', 'jsx']

module.exports = {
  appDir,
  buildDir: path.join(appDir, 'build'),
  configDir: path.join(appDir, 'config'),
  publicDir: path.join(appDir, 'public'),
  reportDir,
  scriptDir: path.join(appDir, 'script'),
  srcDir: path.join(appDir, 'src'),
  eslintReport: path.join(reportDir, 'eslint-report.json'),
  coverageDir,
  coverageReport: path.join(coverageDir, 'lcov.info'),
  allureTestReportDir: path.join(reportDir, 'allure'),
  appHtml: path.join(appDir, 'public/index.html'),
  appIndexJs: path.join(appDir, 'src/index.js'),
  iconJs: path.join(appDir, 'src/App/styles/icons'),
  appPackageJson: path.join(appDir, 'package.json'),
  nodeModulesDir: path.join(appDir, 'node_modules'),
  publicUrl: '/',
  serviceWorkerUrl: '/sw.js'
}

module.exports.moduleFileExtensions = moduleFileExtensions
