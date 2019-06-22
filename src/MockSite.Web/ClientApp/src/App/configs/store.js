import { createStore, applyMiddleware } from 'redux'
import createSagaMiddleware from 'redux-saga'
import { routerMiddleware } from 'connected-react-router'
import history from './history'
import createRootReducer from './reducer'
import rootSaga from './saga'

const sagaMiddleware = createSagaMiddleware()
const middlewares = [sagaMiddleware, routerMiddleware(history)]

if (app.env.ENABLE_LOGGER) {
  const { createLogger } = require('redux-logger')
  middlewares.push(createLogger({ collapsed: true }))
}

const store = createStore(
  createRootReducer(history),
  applyMiddleware(...middlewares)
)

let sagaTask = sagaMiddleware.run(function*() {
  yield rootSaga()
})

if (app.env.ENABLE_HOT_RELOAD) {
  module.hot?.accept('./reducer', () => {
    const nextReducer = require('./reducer').default
    store.replaceReducer(nextReducer)
  })

  module.hot?.accept('./saga', () => {
    const nextSaga = require('./saga').default
    sagaTask.cancel()
    sagaTask.done.then(() => {
      sagaTask = sagaMiddleware.run(function* replacedSaga() {
        yield nextSaga()
      })
    })
  })
}

export default store
