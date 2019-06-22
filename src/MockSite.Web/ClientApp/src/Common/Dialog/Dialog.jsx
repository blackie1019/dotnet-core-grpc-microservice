import { Modal } from 'antd'
import { bool, func, string } from 'prop-types'
import { useEffect, useRef } from 'react'
import { connect } from 'react-redux'
import useWillUnmount from '../Hooks/useWillUnmount'

function Dialog({
  show,
  mode,
  message,
  subMessage,
  showCancelButton,
  confirmButtonText,
  cancelButtonText,
  confirmButtonStyle,
  cancelButtonStyle,
  onConfirm,
  onCancel
}) {
  const modalRef = useRef()

  useEffect(() => {
    if (show) {
      modalRef.current = Modal[mode]({
        okCancel: showCancelButton,
        title: message,
        content: subMessage,
        okText: confirmButtonText,
        okType: confirmButtonStyle,
        cancelText: cancelButtonText,
        cancelButtonProps: { type: cancelButtonStyle },
        onOk: onConfirm,
        onCancel
      })
    } else {
      modalRef.current?.destroy()
      modalRef.current = null
    }
  }, [show])

  useWillUnmount(() => {
    modalRef.current?.destroy()
    modalRef.current = null
  })

  return null
}

Dialog.propTypes = {
  show: bool.isRequired,
  mode: string,
  message: string.isRequired,
  subMessage: string,
  showCancelButton: bool,
  confirmButtonText: string,
  cancelButtonText: string,
  confirmButtonStyle: string,
  cancelButtonStyle: string,
  onConfirm: func.isRequired,
  onCancel: func.isRequired
}

export default connect(
  state => state.dialog,
  null,
  ({ confirmActionType, cancelActionType, ...props }, { dispatch }) => ({
    ...props,
    onConfirm: () => dispatch({ type: confirmActionType }),
    onCancel: () => dispatch({ type: cancelActionType })
  })
)(Dialog)
