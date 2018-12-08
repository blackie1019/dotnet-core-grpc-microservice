import { sendErrorToLog } from './action';

describe(`[core/errorHandler/action]`, () => {
  it(`sendErrorToLog action should be return object with type property 'SEND_ERROR_TO_LOG' and dynamic payload`, () => {
    const actual = sendErrorToLog('some errors');
    expect(actual).toEqual({
      type: 'SEND_ERROR_TO_LOG',
      payload: 'some errors'
    });
  });
});
