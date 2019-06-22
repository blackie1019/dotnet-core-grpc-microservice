import { message as notification } from 'antd'
import { number, oneOf, string } from 'prop-types'
import { useEffect, useRef } from 'react'
import { connect } from 'react-redux'
import useWillUnmount from '@app/Common/Hooks/useWillUnmount'
import actions from './action'

notification.config({ maxCount: 1 })

function Snackbar({ id, mode, message }) {
  const closeFnRef = useRef()
  useEffect(() => {
    if (id) {
      closeFnRef.current?.()
      closeFnRef.current = notification[mode](message)
    }
  }, [id])
  useWillUnmount(() => {
    closeFnRef.current?.()
  })
  return null
}

Snackbar.propTypes = {
  id: number.isRequired,
  mode: oneOf(['success', 'warning', 'error']),
  message: string
}

export default connect(
  state => state.snackbar,
  actions
)(Snackbar)
