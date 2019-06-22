/* eslint-disable no-console */

process.env.BABEL_ENV = 'development'
process.env.NODE_ENV = 'development'

const chalk = require('chalk')
const { prepareUrls } = require('react-dev-utils/WebpackDevServerUtils')
const webpack = require('webpack')
const WebpackDevServer = require('webpack-dev-server')
const config = require('../config/webpack/config')

const compiler = webpack(config)
const serverConfig = config.devServer
const { host, port } = serverConfig

new WebpackDevServer(compiler, serverConfig).listen(port, host, err => {
  if (err) return console.log(err)
  const urls = prepareUrls('http', host, port)
  const url = urls.lanUrlForTerminal
  console.log(chalk.cyan(`Starting the development server at ${url}\n`))
})
