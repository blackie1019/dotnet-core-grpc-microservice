import { call, all, put, takeEvery } from 'redux-saga/effects';
import {
  GET_DATA_START,
  GET_DATA_SUCCESS,
  GET_DATA_FAIL
} from 'core/example/actionType';
import { getData } from 'api/api';

export function* handleGetDataStart() {
  try {
    const data = yield call(getData);
    yield put({
      type: GET_DATA_SUCCESS,
      payload: data
    });
  } catch (error) {
    yield put({
      type: GET_DATA_FAIL
    });
  }
}

export default function* watch() {
  yield all([takeEvery(GET_DATA_START, handleGetDataStart)]);
}
