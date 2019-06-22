import SagaTester from 'redux-saga-tester'

export default ({ initialState, reducer, saga, ...config }) => {
  const tester = new SagaTester({
    initialState: initialState || reducer?.(undefined, {}),
    reducers: reducer,
    ...config
  })
  const task = tester.start(saga)
  tester.next = action => {
    tester.dispatch(action)
    task.end()
    return task.toPromise()
  }
  tester.cancel = () => task.queue.cancelAll()
  return tester
}
