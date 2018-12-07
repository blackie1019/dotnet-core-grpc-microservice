import { APP_START } from 'core/test/actionType.js';

const initState = {
  isStarted: false
};

const reducer = (state = initState, { type, payload }) => {
  switch (type) {
    case APP_START:
      return {
        ...state,
        isStarted: payload
      };
    default:
      return state;
  }
};

export default reducer;
