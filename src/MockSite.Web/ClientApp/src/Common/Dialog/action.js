import { createActions } from 'redux-actions'

export default createActions(
  'SHOW_CONFIRM',
  'SHOW_SUCCESS',
  'SHOW_FAILURE',
  'OK',
  'CONFIRM',
  'CANCEL',
  'CLOSE',
  { prefix: 'DIALOG' }
)
