import { combineReducers } from 'redux'
import { connectRouter } from 'connected-react-router'
import asyncTask from '@app/Common/AsyncTask/reducer'
import authentication from '@app/Common/Authentication/reducer'
import dialog from '@app/Common/Dialog/reducer'
import snackbar from '@app/Common/Snackbar/reducer'
import user from '@app/Account/User/reducer'
import localization from '@app/Account/Localization/reducer'
import currency from '@app/Account/Currency/reducer'

export default history =>
  combineReducers({
    asyncTask,
    authentication,
    dialog,
    router: connectRouter(history),
    snackbar,
    user,
    localization,
    currency
  })
