const { RuleTester } = require('eslint')
const plugin = require('./')

RuleTester.setDefaultConfig({
  parser: 'babel-eslint'
})

new RuleTester().run('react-extra', plugin.rules['no-undefined-effect-deps'], {
  valid: ['useEffect(() => {}, [])'],
  invalid: [
    {
      code: 'useEffect(() => {})',
      errors: [{}]
    }
  ]
})
