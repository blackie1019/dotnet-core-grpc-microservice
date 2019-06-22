import { handleActions, combineActions } from 'redux-actions'
import actions from './action'

const initialState = {
  didInit: false,
  credential: undefined
}

export default handleActions(
  {
    [actions.initSuccess]: (_state, action) => ({
      didInit: true,
      credential: action.payload
    }),
    [combineActions(
      actions.loginSuccess,
      actions.lockScreenSuccess,
      actions.unlockScreenSuccess
    )]: (state, action) => ({
      ...state,
      credential: action.payload
    }),
    [actions.logout]: state => ({
      ...state,
      credential: undefined
    })
  },
  initialState
)

export const tokenSelector = state => state.authentication.credential?.token
