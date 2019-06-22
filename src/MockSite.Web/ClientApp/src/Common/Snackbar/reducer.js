import { handleActions } from 'redux-actions'
import actions from './action'

const initialState = {
  id: 0,
  mode: 'success',
  message: undefined
}

export default handleActions(
  {
    [actions.showSuccess]: (state, { payload = {} }) => ({
      id: state.id + 1,
      mode: 'success',
      message: payload.message || 'Success!'
    }),
    [actions.showWarning]: (state, { payload = {} }) => ({
      id: state.id + 1,
      mode: 'warning',
      message: payload.message || 'Warning!'
    }),
    [actions.showFailure]: (state, { payload = {} }) => ({
      id: state.id + 1,
      mode: 'error',
      message: payload.message || 'Failure!'
    })
  },
  initialState
)
