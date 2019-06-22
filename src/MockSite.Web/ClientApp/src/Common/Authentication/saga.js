import { push } from 'connected-react-router'
import {
  all,
  call,
  cancel,
  delay,
  fork,
  put,
  select,
  take,
  takeEvery
} from 'redux-saga/effects'
import paths from '@app/App/constants/paths'
import snackbarActions from '@app/Common/Snackbar/action'
import {
  initAuthAPI,
  lockAuthAPI,
  loginAPI,
  logoutAPI,
  renewTokenAPI,
  changePasswordAPI
} from './api'
import actions from './action'

function* logoutWhenExpired(expiryTime) {
  yield delay(expiryTime - Date.now())
  yield put(actions.logout())
}

function* autoLogout(credential) {
  const task = yield fork(logoutWhenExpired, credential?.expiryTime)
  yield take([`${actions.lockScreen}`, `${actions.logout}`])
  yield cancel(task)
}

function* handleInit() {
  let credential = yield call(initAuthAPI)
  if (credential) yield fork(autoLogout, credential)
  yield put(actions.initSuccess(credential))
}

function* handleLogin({ payload }) {
  try {
    const credential = yield call(loginAPI, payload)
    yield put(actions.loginSuccess(credential))
    yield put(push(paths.INDEX))
    yield fork(autoLogout, credential)
  } catch (error) {
    yield put(actions.loginFailure(error))
    yield put(snackbarActions.showFailure({ message: 'Login failed!' }))
  }
}

function* handleLogout() {
  yield call(logoutAPI)
  yield put(push(paths.LOGIN))
}

function* handleChangePassword({ payload }) {
  try {
    yield call(changePasswordAPI, payload)
    yield put(actions.changePasswordSuccess())
    yield put(snackbarActions.showSuccess({ message: 'Password updated!' }))
    yield put(push(paths.INDEX))
  } catch (error) {
    yield put(actions.changePasswordFailure(error))
    yield put(
      snackbarActions.showFailure({ message: 'Password update failed!' })
    )
  }
}

function* handleLockScreen() {
  const credential = yield call(lockAuthAPI)
  yield put(actions.lockScreenSuccess(credential))
}

function* handleUnlockScreen({ payload }) {
  try {
    const newCredential = yield call(renewTokenAPI, payload)
    yield put(actions.unlockScreenSuccess(newCredential))
    yield put(push(paths.INDEX))
    yield fork(autoLogout, newCredential)
  } catch (error) {
    yield put(actions.unlockScreenFailure(error))
    yield put(snackbarActions.showFailure({ message: 'Wrong password!' }))
  }
}

function* handleUnauthenticationError({ error, payload }) {
  if (!error || payload?.response?.status !== 401) return
  const { credential } = yield select(state => state.authentication)
  if (!credential) return
  yield put(actions.logout())
}

export default function* watch() {
  yield all([
    takeEvery(`${actions.init}`, handleInit),
    takeEvery(`${actions.login}`, handleLogin),
    takeEvery(`${actions.logout}`, handleLogout),
    takeEvery(`${actions.changePassword}`, handleChangePassword),
    takeEvery(`${actions.lockScreen}`, handleLockScreen),
    takeEvery(`${actions.unlockScreen}`, handleUnlockScreen),
    takeEvery('*', handleUnauthenticationError)
  ])
}
