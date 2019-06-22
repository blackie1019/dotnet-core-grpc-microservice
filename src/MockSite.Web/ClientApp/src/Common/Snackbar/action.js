import { createActions } from 'redux-actions'

export default createActions('SHOW_SUCCESS', 'SHOW_WARNING', 'SHOW_FAILURE', {
  prefix: 'SNACKBAR'
})
