import { handleActions, combineActions } from 'redux-actions'
import actions from './action'

const initialState = {
  show: false,
  mode: undefined,
  title: '',
  message: '',
  subMessage: '',
  showCancelButton: false,
  confirmButtonText: 'Confirm',
  cancelButtonText: 'Cancel',
  confirmButtonStyle: 'primary',
  cancelButtonStyle: 'default',
  confirmActionType: `${actions.ok}`,
  cancelActionType: `${actions.cancel}`
}

export default handleActions(
  {
    [actions.showDialog]: (_state, { payload = {} }) => ({
      ...initialState,
      show: true,
      mode: 'success',
      message: payload.message || 'Success!',
      subMessage: payload.subMessage
    }),
    [actions.showFailure]: (_state, { payload = {} }) => ({
      ...initialState,
      show: true,
      mode: 'error',
      message: payload.message || 'Failure!',
      subMessage: payload.subMessage
    }),
    [actions.showConfirm]: (_state, { payload = {} }) => ({
      ...initialState,
      show: true,
      mode: 'confirm',
      message: payload.message || 'Are you sure?',
      subMessage: payload.subMessage || 'This action cannot be undone.',
      showCancelButton: true,
      confirmButtonText:
        payload.confirmButtonText || initialState.confirmButtonText,
      confirmButtonStyle: payload.confirmButtonStyle || 'danger',
      confirmActionType: `${actions.confirm}`
    }),
    [combineActions(actions.cancel, actions.confirm, actions.ok)]: () =>
      initialState
  },
  initialState
)
