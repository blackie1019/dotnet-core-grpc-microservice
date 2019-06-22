/* eslint-disable no-console */

process.env.BABEL_ENV = 'production'
process.env.NODE_ENV = 'production'

const chalk = require('chalk')
const fs = require('fs-extra')
const formatWebpackMessages = require('react-dev-utils/formatWebpackMessages')
const FileSizeReporter = require('react-dev-utils/FileSizeReporter')
const printBuildError = require('react-dev-utils/printBuildError')
const webpack = require('webpack')
const paths = require('../config/paths')
const config = require('../config/webpack/config')

const {
  measureFileSizesBeforeBuild,
  printFileSizesAfterBuild
} = FileSizeReporter

const WARN_AFTER_BUNDLE_GZIP_SIZE = 512 * 1024
const WARN_AFTER_CHUNK_GZIP_SIZE = 1024 * 1024

function cleanBuildFolder() {
  fs.emptyDirSync(paths.buildDir)
}

function copyPublicFolder() {
  fs.copySync(paths.publicDir, paths.buildDir, {
    dereference: true,
    filter: file => file !== paths.appHtml
  })
}

function buildSourceCode() {
  console.log(chalk.cyan('\nCreating an optimized production build...\n'))
  const compiler = webpack(config)
  return new Promise((resolve, reject) => {
    compiler.run((err, stats) => {
      let messages
      if (err) {
        if (!err.message) return reject(err)
        messages = formatWebpackMessages({
          errors: [err.message],
          warnings: []
        })
      } else {
        const json = stats.toJson({ all: false, warnings: true, errors: true })
        messages = formatWebpackMessages(json)
      }
      if (messages.errors.length) {
        if (messages.errors.length > 1) messages.errors.length = 1
        return reject(new Error(messages.errors.join('\n\n')))
      }
      return resolve({ stats, messages })
    })
  })
}

function printBuildResult(messages) {
  if (messages.warnings.length) {
    console.log(chalk.yellow('\nCompiled with warnings.\n'))
    console.log(messages.warnings.join('\n\n'))
  } else {
    console.log(chalk.green('\nCompiled successfully.\n'))
  }
}

function printBuildSize({ stats, prevFileSizes }) {
  console.log(chalk.green('\nFile sizes after gzip:\n'))
  printFileSizesAfterBuild(
    stats,
    prevFileSizes,
    paths.buildDir,
    WARN_AFTER_BUNDLE_GZIP_SIZE,
    WARN_AFTER_CHUNK_GZIP_SIZE
  )
}

;(async () => {
  try {
    const prevFileSizes = await measureFileSizesBeforeBuild(paths.buildDir)
    cleanBuildFolder()
    copyPublicFolder()
    const { stats, messages } = await buildSourceCode()
    printBuildResult(messages)
    printBuildSize({ stats, prevFileSizes })
  } catch (err) {
    console.log(chalk.red('\nFailed to compile.\n'))
    printBuildError(err)
    process.exit(1)
  }
})()
