module.exports = function expandEnvVariables(input) {
  if (typeof input !== 'object') return input
  return Object.keys(input).reduce((output, key) => {
    const value = input[key]
    const paths = key.split('.')
    let object = output
    for (let i = 0; i < paths.length; i++) {
      const path = paths[i]
      if (i !== paths.length - 1) {
        if (!object[path]) object[path] = {}
        object = object[path]
      } else {
        object[path] = expandEnvVariables(value)
      }
    }
    return output
  }, {})
}
