import { array, bool, func, object, oneOfType } from 'prop-types'
import { cloneElement } from 'react'
import { connect } from 'react-redux'
import hasPermission from './hasPermission'

function Secured({ dispatch, credential, policy, children, ...props }) {
  if (typeof children === 'function') return children(credential)
  if (hasPermission(credential, policy)) return cloneElement(children, props)
  return null
}

Secured.propTypes = {
  credential: object,
  policy: oneOfType([array, bool, func])
}

export default connect(state => ({
  credential: state.authentication.credential
}))(Secured)
