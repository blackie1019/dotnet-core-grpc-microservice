import { call, all, takeEvery } from 'redux-saga/effects';
import errorHandlerSaga, { handleSendErrorToLog } from './errorHandler';
import { SEND_ERROR_TO_LOG } from 'core/errorHandler/actionType';
import { sendClientErrorToServer } from 'api/api';

describe(`[core/effects/saga]`, () => {
  it(`root watch function`, () => {
    const iterator = errorHandlerSaga();
    expect(iterator.next().value).toEqual(
      all([takeEvery(SEND_ERROR_TO_LOG, handleSendErrorToLog)])
    );
  });

  it(`handleSendErrorToLog without error`, () => {
    const mockedPayload = { payload: 'client error ... etc' };
    const iterator = handleSendErrorToLog(mockedPayload);

    // first yield
    expect(iterator.next().value).toEqual(
      call(sendClientErrorToServer, 'client error ... etc')
    );
    // second yield
    expect(iterator.next().value).toEqual(
      call(console.log, 'Send log to server successfully.')
    );
  });

  it(`handleSendErrorToLog with error`, () => {
    const mockedPayload = { payload: 'client error ... etc' };
    const mockedError = new Error('unexpected error');
    const iterator = handleSendErrorToLog(mockedPayload);

    // first yield
    expect(iterator.next().value).toEqual(
      call(sendClientErrorToServer, 'client error ... etc')
    );
    // second yield
    expect(iterator.throw(mockedError).value).toEqual(
      call(console.log, 'Send log to server fail.')
    );
  });
});
