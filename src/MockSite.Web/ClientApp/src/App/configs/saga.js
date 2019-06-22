import { all, call, fork, put } from 'redux-saga/effects'
import authentication from '@app/Common/Authentication/saga'
import errorActions from '@app/Common/ErrorHandler/action'
import errorHandler from '@app/Common/ErrorHandler/saga'
import user from '@app/Account/User/saga'

export default function* rootSaga() {
  try {
    yield all([authentication, errorHandler, user].map(call))
  } catch (error) {
    yield fork(rootSaga)
    yield put(errorActions.logError(error))
  }
}
