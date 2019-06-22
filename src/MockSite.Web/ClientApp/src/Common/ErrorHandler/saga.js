import { all, takeEvery, call } from 'redux-saga/effects'
import action from './action'

export function* handleLogError({ payload }) {
  // eslint-disable-next-line no-console
  yield call(console.error, payload)
}

export default function* watch() {
  yield all([takeEvery(`${action.logError}`, handleLogError)])
}
