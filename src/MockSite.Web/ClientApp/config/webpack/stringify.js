function stringify(input) {
  const type = typeof input
  if (type === 'string') return JSON.stringify(input)
  if (type === 'object') return stringifyObject(input)
  return input
}

function stringifyObject(input) {
  return Object.keys(input).reduce((output, key) => {
    output[key] = stringify(input[key])
    return output
  }, {})
}

module.exports = stringify
