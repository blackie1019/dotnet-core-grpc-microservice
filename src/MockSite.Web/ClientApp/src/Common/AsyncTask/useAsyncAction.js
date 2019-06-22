import { useCallback, useState } from 'react'
import useReduxState from '@app/Common/Hooks/useReduxState'
import { getTaskId } from './reducer'

const defaultState = {
  loading: false,
  success: false,
  failure: false
}

export default actionCreator => {
  const [asyncId, setAsyncId] = useState()
  const asyncState = useReduxState(state => state.asyncTask)

  const asyncActionCreator = useCallback(
    (...args) => {
      const action = actionCreator(...args)
      if (!action) return
      setAsyncId(getTaskId(action))
      return action
    },
    [actionCreator]
  )

  return [asyncState[asyncId] || defaultState, asyncActionCreator]
}
