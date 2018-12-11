import {
  APP_START,
  GET_DATA_START,
  GET_DATA_SUCCESS,
  GET_DATA_FAIL
} from 'core/example/actionType.js';

export const appStart = () => ({
  type: APP_START
});
export const getDataStart = () => ({
  type: GET_DATA_START
});
export const getDataSuccess = payload => ({
  type: GET_DATA_SUCCESS,
  payload
});
export const getDataFail = () => ({
  type: GET_DATA_FAIL
});

export default {
  appStart,
  getDataStart,
  getDataSuccess,
  getDataFail
};
