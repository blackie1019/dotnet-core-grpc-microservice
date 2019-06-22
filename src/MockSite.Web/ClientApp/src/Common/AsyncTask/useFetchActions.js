import { equals } from 'ramda'
import { useEffect, useMemo, useRef, useState } from 'react'
import { getTaskId } from '@app/Common/AsyncTask/reducer'
import useReduxState from '@app/Common/Hooks/useReduxState'

const defaultAsyncState = {
  loading: false,
  success: false,
  failure: false
}

export default creator => {
  const asyncState = useReduxState(state => state.asyncTask)
  const prevTasksRef = useRef([])
  const [taskIds, setTaskIds] = useState([])

  const prevTasks = prevTasksRef.current
  const tasks = !creator ? [] : Array.isArray(creator[0]) ? creator : [creator]
  const todoTasks = tasks.filter((task, i) => {
    if (!prevTasks[i]) return true
    const [, ...preParams] = prevTasks[i]
    const [, ...params] = task
    return !equals(params, preParams)
  })
  prevTasksRef.current = tasks

  /* eslint-disable-next-line react-extra/no-undefined-effect-deps */
  useEffect(() => {
    if (!todoTasks.length) return
    const nextTaskIds = todoTasks
      .map(([creator, ...params]) => creator(...params))
      .filter(Boolean)
      .map(getTaskId)
      .filter(Boolean)
    setTaskIds(nextTaskIds)
  })

  return useMemo(
    () =>
      taskIds.reduce(
        (state, taskId) => {
          const taskState = asyncState[taskId] || {
            ...defaultAsyncState,
            failure: true
          }
          return {
            loading: state.loading || taskState.loading,
            success: state.success && taskState.success,
            failure: state.failure || taskState.failure
          }
        },
        { ...defaultAsyncState, success: true }
      ),
    [taskIds, asyncState]
  )
}
