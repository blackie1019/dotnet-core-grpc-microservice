const MiniCssExtractPlugin = require('mini-css-extract-plugin')

module.exports = ({ env, cssOptions, preProcessor }) => {
  const loaders = [
    {
      loader: 'css-loader',
      options: { ...cssOptions, sourceMap: env.shouldUseSourceMap }
    },
    {
      loader: 'postcss-loader',
      options: {
        ident: 'postcss',
        plugins: () => [
          require('postcss-flexbugs-fixes'),
          require('postcss-preset-env')({
            autoprefixer: { flexbox: 'no-2009' },
            stage: 3
          })
        ],
        sourceMap: env.shouldUseSourceMap
      }
    }
  ]
  if (env.isDev) {
    loaders.unshift('style-loader')
  } else {
    loaders.unshift(MiniCssExtractPlugin.loader)
  }
  if (preProcessor) {
    loaders.push({
      loader: preProcessor,
      options: { sourceMap: env.shouldUseSourceMap }
    })
  }
  return loaders
}
