import reducer from 'core/reducer';
import { createStore, applyMiddleware } from 'redux';
import createSagaMiddleware from 'redux-saga';
import rootSaga from 'core/saga';
const sagaMiddleware = createSagaMiddleware();
const middleware = applyMiddleware(sagaMiddleware);

const store = createStore(reducer, middleware);

let sagaTask = sagaMiddleware.run(function*() {
  yield rootSaga();
});
if (process.env.NODE_ENV !== 'production' && module.hot) {
  module.hot.accept('core/reducer', () => {
    const nextReducer = require('core/reducer').default;
    store.replaceReducer(nextReducer);
  });
  module.hot.accept('core/saga', () => {
    const getNewSagas = require('core/saga').default;
    sagaTask.cancel();
    sagaTask.done.then(() => {
      sagaTask = sagaMiddleware.run(function* replacedSaga() {
        yield getNewSagas();
      });
    });
  });
}
export default store;
