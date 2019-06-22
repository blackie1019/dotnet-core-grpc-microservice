const CaseSensitivePathsPlugin = require('case-sensitive-paths-webpack-plugin')
const HtmlPlugin = require('html-webpack-plugin')
const MiniCssExtractPlugin = require('mini-css-extract-plugin')
const errorOverlayMiddleware = require('react-dev-utils/errorOverlayMiddleware')
const evalSourceMapMiddleware = require('react-dev-utils/evalSourceMapMiddleware')
const InterpolateHtmlPlugin = require('react-dev-utils/InterpolateHtmlPlugin')
const WatchMissingNodeModulesPlugin = require('react-dev-utils/WatchMissingNodeModulesPlugin')
const ModuleNotFoundPlugin = require('react-dev-utils/ModuleNotFoundPlugin')
const OptimizeCssAssetsPlugin = require('optimize-css-assets-webpack-plugin')
const TerserPlugin = require('terser-webpack-plugin')
const webpack = require('webpack')
const { BundleAnalyzerPlugin } = require('webpack-bundle-analyzer')
const ManifestPlugin = require('webpack-manifest-plugin')
const WorkboxPlugin = require('workbox-webpack-plugin')
const packageJson = require('../../package.json')
const env = require('../env')
const paths = require('../paths')
const getHtmlMinifierOption = require('./getHtmlMinifierOption')
const getOptimizeCssAssetsPluginOption = require('./getOptimizeCssAssetsPluginOption')
const getStyleLoaders = require('./getStyleLoaders')
const getTerserPluginOption = require('./getTerserPluginOption')
const stringify = require('./stringify')

const stats = {
  colors: true,
  modules: false
}

module.exports = {
  mode: env.isDev ? 'development' : 'production',
  stats,
  bail: !env.isDev,
  devtool: (() => {
    if (env.isDev) return 'cheap-module-eval-source-map'
    if (env.shouldUseSourceMap) return 'source-map'
    return false
  })(),
  entry: [
    env.isDev && 'react-dev-utils/webpackHotDevClient',
    paths.appIndexJs
  ].filter(Boolean),
  output: (() => {
    if (env.isDev) {
      return {
        filename: 'static/js/bundle.js',
        chunkFilename: 'static/js/[name].chunk.js',
        publicPath: paths.publicUrl
      }
    } else {
      return {
        path: paths.buildDir,
        filename: 'static/js/[name].[chunkhash:8].js',
        chunkFilename: 'static/js/[name].[chunkhash:8].chunk.js',
        publicPath: paths.publicUrl
      }
    }
  })(),
  optimization: (() => {
    if (env.isDev) {
      return {
        splitChunks: { chunks: 'all' },
        runtimeChunk: true
      }
    } else {
      return {
        splitChunks: {
          chunks: 'all',
          name: false,
          cacheGroups: {
            app: {
              test: /[\\/]src[\\/]app[\\/]/,
              enforce: true
            },
            antd: {
              test: /[\\/]node_modules[\\/](antd|rc-[a-z-]+|@ant-design)[\\/]/,
              enforce: true,
              priority: 1
            },
            react: {
              test: /[\\/]node_modules[\\/](react(-dom|-redux)?|styled-components|redux|@?redux-saga)[\\/]/,
              enforce: true,
              priority: 1
            },
            utils: {
              test: /[\\/]node_modules[\\/]/,
              enforce: true
            }
          }
        },
        runtimeChunk: true,
        minimizer: [
          new TerserPlugin(getTerserPluginOption(env)),
          new OptimizeCssAssetsPlugin(getOptimizeCssAssetsPluginOption(env))
        ]
      }
    }
  })(),
  resolve: {
    alias: { '@ant-design/icons/lib/dist': paths.iconJs },
    extensions: paths.moduleFileExtensions.map(ext => `.${ext}`)
  },
  module: {
    strictExportPresence: true,
    rules: [
      {
        test: /\.jsx?$/,
        include: paths.srcDir,
        loader: 'babel-loader',
        options: {
          cacheDirectory: true,
          cacheCompression: !env.isDev
        }
      },
      {
        test: /\.css$/,
        use: getStyleLoaders({
          env,
          cssOptions: { importLoaders: 1 }
        })
      },
      {
        test: [/\.gif$/, /\.jpe?g$/, /\.png$/],
        loader: 'url-loader',
        options: {
          limit: 10000,
          name: 'static/media/[name].[hash:8].[ext]'
        }
      },
      {
        test: [/\.svg$/, /\.ttf$/, /\.woff$/, /\.woff2$/, /\.eot$/],
        loader: 'file-loader',
        options: {
          name: 'static/media/[name].[hash:8].[ext]'
        }
      }
    ]
  },
  plugins: (() => {
    const plugins = [
      new CaseSensitivePathsPlugin(),
      new HtmlPlugin({
        inject: true,
        minify: !env.isDev && getHtmlMinifierOption(),
        template: paths.appHtml
      }),
      new InterpolateHtmlPlugin(HtmlPlugin, {
        PUBLIC_URL: paths.publicUrl.replace(/^\//, '')
      }),
      new ManifestPlugin({
        fileName: 'asset-manifest.json',
        publicPath: paths.publicUrl
      }),
      new ModuleNotFoundPlugin(paths.appDir),
      new webpack.DefinePlugin(stringify(env.variables)),
      new webpack.IgnorePlugin(/^\.\/locale$/, /moment$/)
    ]
    if (env.isDev) {
      plugins.push(
        new WatchMissingNodeModulesPlugin(paths.nodeModulesDir),
        new webpack.HotModuleReplacementPlugin()
      )
    } else {
      plugins.push(
        new MiniCssExtractPlugin({
          filename: 'static/css/[name].[contenthash:8].css',
          chunkFilename: 'static/css/[name].[contenthash:8].chunk.css'
        }),
        new WorkboxPlugin.GenerateSW({
          cacheId: packageJson.name,
          swDest: paths.serviceWorkerUrl.replace(/^\//, ''),
          importWorkboxFrom: 'local',
          importsDirectory: 'static/sw',
          clientsClaim: true,
          skipWaiting: true,
          navigateFallback: paths.publicUrl + 'index.html',
          navigateFallbackBlacklist: [/^\/api/, /\/[^/]+\.[^/]+$/]
        })
      )
    }
    if (env.showWebpackVisualizer) {
      plugins.push(new BundleAnalyzerPlugin())
    }
    return plugins
  })(),
  devServer: {
    stats,
    contentBase: paths.publicDir,
    watchContentBase: true,
    host: process.env.HOST || '0.0.0.0',
    port: parseInt(process.env.PORT, 10) || 3000,
    publicPath: paths.publicUrl,
    historyApiFallback: true,
    hot: true,
    compress: true,
    clientLogLevel: 'none',
    overlay: false,
    before: (app, server) => {
      app.use(evalSourceMapMiddleware(server))
      app.use(errorOverlayMiddleware())
    }
  }
}
