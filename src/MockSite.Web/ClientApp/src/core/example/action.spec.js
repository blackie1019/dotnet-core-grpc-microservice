import { appStart, getDataStart, getDataSuccess, getDataFail } from './action';

describe(`[core/example/action]`, () => {
  it(`appStart action should be return object with type 'APP_START'`, () => {
    const actual = appStart();
    expect(actual).toEqual({
      type: 'APP_START'
    });
  });
  it(`getDataStart action should be return object with type 'GET_DATA_START'`, () => {
    const actual = getDataStart();
    expect(actual).toEqual({
      type: 'GET_DATA_START'
    });
  });
  it(`getDataSuccess action should be return object with type 'GET_DATA_SUCCESS' and dynamic payload`, () => {
    const mockedPayload = {
      message: 'dynamic payload'
    };
    const actual = getDataSuccess(mockedPayload);
    expect(actual).toEqual({
      type: 'GET_DATA_SUCCESS',
      payload: {
        message: 'dynamic payload'
      }
    });
  });
  it(`getDataFail action should be return object with type 'GET_DATA_FAIL'`, () => {
    const actual = getDataFail();
    expect(actual).toEqual({
      type: 'GET_DATA_FAIL'
    });
  });
});
