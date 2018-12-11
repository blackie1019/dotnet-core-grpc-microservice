import {
  APP_START,
  GET_DATA_SUCCESS,
  GET_DATA_FAIL
} from 'core/example/actionType.js';

const initState = {
  isStarted: false,
  isGetDataSuccess: false,
  data: []
};

const reducer = (state = initState, { type, payload }) => {
  switch (type) {
    case APP_START:
      return {
        ...state,
        isStarted: true
      };
    case GET_DATA_SUCCESS:
      return {
        ...state,
        isGetDataSuccess: true,
        data: payload
      };
    case GET_DATA_FAIL:
      return {
        ...state,
        isGetDataSuccess: false
      };
    default:
      return state;
  }
};

export default reducer;
