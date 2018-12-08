import { call, all, put, takeEvery } from 'redux-saga/effects';
import exampleSaga, { handleGetDataStart } from './example';
import {
  GET_DATA_START,
  GET_DATA_SUCCESS,
  GET_DATA_FAIL
} from 'core/example/actionType';
import { getData } from 'api/api';

describe(`[core/example/saga]`, () => {
  it(`root saga function`, () => {
    const iterator = exampleSaga();
    expect(iterator.next().value).toEqual(
      all([takeEvery(GET_DATA_START, handleGetDataStart)])
    );
  });

  it(`handleGetDataStart without error`, () => {
    const iterator = handleGetDataStart();
    const mockedResponse = { message: 'ok' };
    // first yield
    expect(iterator.next().value).toEqual(call(getData));
    // second yield
    expect(iterator.next(mockedResponse).value).toEqual(
      put({ type: GET_DATA_SUCCESS, payload: { message: 'ok' } })
    );
  });

  it(`handleGetDataStart with error`, () => {
    const mockedError = new Error('unexpected error');
    const iterator = handleGetDataStart();

    // first yield
    expect(iterator.next().value).toEqual(call(getData));
    // second yield
    expect(iterator.throw(mockedError).value).toEqual(
      put({ type: GET_DATA_FAIL })
    );
  });
});
