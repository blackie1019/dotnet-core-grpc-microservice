import { call, all, takeEvery } from 'redux-saga/effects';
import { SEND_ERROR_TO_LOG } from 'core/errorHandler/actionType';

export function* handleSendErrorToLog() {
  try {
    // call API to write log.
    yield call(console.log, 'Send log to server successfully.');
  } catch (error) {
    yield call(console.log, 'Send log to server fail.');
  }
}

export default function* watch() {
  yield all([takeEvery(SEND_ERROR_TO_LOG, handleSendErrorToLog)]);
}
