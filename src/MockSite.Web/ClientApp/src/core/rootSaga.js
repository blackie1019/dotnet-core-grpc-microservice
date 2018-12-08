import { all, call, put, fork } from 'redux-saga/effects';
import errorHandler from 'core/effects/errorHandler';
import { SEND_ERROR_TO_LOG } from 'core/errorHandler/actionType';

export default function* rootSaga() {
  try {
    yield all([call(errorHandler)]);
  } catch (error) {
    yield fork(rootSaga);
    yield put({ type: SEND_ERROR_TO_LOG, payload: error });
  }
}
