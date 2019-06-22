export const isValidEmail = value =>
  /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/.test(
    value
  )

export const isValidPassword = value => {
  const minMaxLength = /^[\s\S]{8,20}$/
  const upper = /[A-Z]/
  const lower = /[a-z]/
  const number = /[0-9]/
  const special = /[ !"#$%&'()*+,\-./:;<=>?@[\\\]^_`{|}~]/
  return (
    minMaxLength.test(value) &&
    upper.test(value) &&
    lower.test(value) &&
    number.test(value) &&
    special.test(value)
  )
}
