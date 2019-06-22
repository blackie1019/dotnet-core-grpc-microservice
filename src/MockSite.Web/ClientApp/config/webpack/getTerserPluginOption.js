module.exports = ({ shouldUseSourceMap = false }) => ({
  terserOptions: {
    mangle: { safari10: true },
    output: { ascii_only: true }
  },
  parallel: true,
  cache: true,
  sourceMap: shouldUseSourceMap
})
