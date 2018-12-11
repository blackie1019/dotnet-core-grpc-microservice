import reducer from './reducer';

const initState = {
  isStarted: false,
  isGetDataSuccess: false,
  data: []
};

describe(`[core/example/reducer]`, () => {
  it(`empty action object should return initial state`, () => {
    const actual = reducer(initState, {});
    expect(actual).toEqual({
      isStarted: false,
      isGetDataSuccess: false,
      data: []
    });
  });
  it(`app start action should let isStarted property to be true.`, () => {
    const actual = reducer(initState, { type: 'APP_START' });
    expect(actual).toEqual({
      isStarted: true,
      isGetDataSuccess: false,
      data: []
    });
  });
  it(`get data success action with array data should let isGetDataSuccess to be true and data will be replace with payload`, () => {
    const actual = reducer(initState, {
      type: 'GET_DATA_SUCCESS',
      payload: [{ id: 1 }, { id: 2 }]
    });
    expect(actual).toEqual({
      isStarted: false,
      isGetDataSuccess: true,
      data: [{ id: 1 }, { id: 2 }]
    });
  });
  it(`get data success action with object data should let isGetDataSuccess to be true and data will be replace with payload`, () => {
    const actual = reducer(initState, {
      type: 'GET_DATA_SUCCESS',
      payload: { id: 1, message: 'mockedMessage', type: 0 }
    });
    expect(actual).toEqual({
      isStarted: false,
      isGetDataSuccess: true,
      data: { id: 1, message: 'mockedMessage', type: 0 }
    });
  });
  it(`get data fail action and initial state should let isGetDataSuccess to be false`, () => {
    const actual = reducer(initState, {
      type: 'GET_DATA_FAIL'
    });
    expect(actual).toEqual({
      isStarted: false,
      isGetDataSuccess: false,
      data: []
    });
  });
  it(`get data fail action and some data state should only let isGetDataSuccess to be false`, () => {
    const mockedState = {
      isStarted: true,
      isGetDataSuccess: true,
      data: { id: 1, message: 'mockedMessage', type: 0 }
    };
    const actual = reducer(mockedState, {
      type: 'GET_DATA_FAIL'
    });
    expect(actual).toEqual({
      isStarted: true,
      isGetDataSuccess: false,
      data: { id: 1, message: 'mockedMessage', type: 0 }
    });
  });
});
