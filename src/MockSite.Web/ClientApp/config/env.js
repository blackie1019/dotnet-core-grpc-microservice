const { execSync } = require('child_process')
const paths = require('./paths')

const ENV = process.env.NODE_ENV || 'development'
const PLUGINS = (process.env.PLUGINS || '').split(',')

const BASE_VARIABLES = {
  'process.env.NODE_ENV': ENV,
  'app.env.REVISION_NUMBER': execSync('git rev-parse HEAD')
    .toString()
    .slice(0, 7)
}

const VARIABLES = {
  production: {
    ...BASE_VARIABLES,
    'app.env': {
      ENABLE_SERVICE_WORKER: true,
      ENABLE_HOT_RELOAD: false,
      ENABLE_LOGGER: false,
      API_URL: '/api',
      SERVICE_WORKER_URL: paths.serviceWorkerUrl
    }
  },
  development: {
    ...BASE_VARIABLES,
    'app.env': {
      ENABLE_SERVICE_WORKER: false,
      ENABLE_HOT_RELOAD: true,
      ENABLE_LOGGER: true,
      API_URL: 'https://localhost:5001/api'
    }
  },
  test: {
    ...BASE_VARIABLES,
    'app.env': {
      API_URL: '/api'
    }
  }
}

module.exports = {
  isDev: ENV === 'development',
  shouldUseSourceMap: true,
  variables: VARIABLES[ENV],
  showWebpackVisualizer: PLUGINS.includes('webpack-visualizer')
}
