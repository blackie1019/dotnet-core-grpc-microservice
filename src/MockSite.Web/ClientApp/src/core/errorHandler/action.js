import { SEND_ERROR_TO_LOG } from 'core/errorHandler/actionType';

export const sendErrorToLog = error => ({
  type: SEND_ERROR_TO_LOG,
  payload: error
});

export default {
  sendErrorToLog
};
