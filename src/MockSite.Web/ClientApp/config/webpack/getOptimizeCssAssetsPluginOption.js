const safePostCssParser = require('postcss-safe-parser')

module.exports = ({ shouldUseSourceMap = false }) => {
  const sourceMapOption = {
    inline: false,
    annotation: true
  }
  return {
    cssProcessorOptions: {
      parser: safePostCssParser,
      map: shouldUseSourceMap ? sourceMapOption : false
    }
  }
}
