import React from 'react'
import styled from 'styled-components'

const customCSSProperties = {
  marginVertical: value => `margin-top:${value};margin-bottom:${value};`,
  marginHorizontal: value => `margin-right:${value};margin-left:${value};`,
  paddingVertical: value => `padding-top:${value};padding-bottom:${value};`,
  paddingHorizontal: value => `padding-right:${value};padding-left:${value};`,
  gapVertical: value => `>*+*{margin-top:${value};};`,
  gapHorizontal: value => `>*+*{margin-left:${value};};`,
  css: value => value
}

const supportedCSSProperties = Object.keys({
  ...document.body.style,
  ...customCSSProperties
}).reduce((dict, key) => {
  dict[key] = true
  return dict
}, {})

const toCSSProperty = ([key, value]) => {
  if (!value && value !== 0) return ''
  if (typeof value === 'number') value = `${value}px`
  if (customCSSProperties[key]) return customCSSProperties[key](value)
  key = key.replace(/[A-Z]/g, c => '-' + c.toLowerCase())
  return `${key}: ${value};`
}

export default React.memo(styled.div`
  ${props =>
    Object.entries(props)
      .filter(([key]) => supportedCSSProperties[key])
      .map(toCSSProperty)
      .join('')};
`)
