import { createStore, applyMiddleware } from 'redux';
import createSagaMiddleware from 'redux-saga';
import reducer from 'core/rootReducer';
import rootSaga from 'core/rootSaga';
const sagaMiddleware = createSagaMiddleware();
const middleware = applyMiddleware(sagaMiddleware);

const store = createStore(reducer, middleware);

let sagaTask = sagaMiddleware.run(function*() {
  yield rootSaga();
});
if (process.env.NODE_ENV !== 'production' && module.hot) {
  module.hot.accept('core/rootReducer', () => {
    const nextReducer = require('core/rootReducer').default;
    store.replaceReducer(nextReducer);
  });
  module.hot.accept('core/rootSaga', () => {
    const getNewSagas = require('core/rootSaga').default;
    sagaTask.cancel();
    sagaTask.done.then(() => {
      sagaTask = sagaMiddleware.run(function* replacedSaga() {
        yield getNewSagas();
      });
    });
  });
}
export default store;
