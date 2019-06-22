import * as routerActions from 'connected-react-router'
import { all, call, put, takeLatest, takeLeading } from 'redux-saga/effects'
import paths from '@app/App/constants/paths'
import dialogActions from '@app/Common/Dialog/action'
import { takeDialogResult } from '@app/Common/Dialog/effects'
import snackbarActions from '@app/Common/Snackbar/action'
import actions from './action'
import {
  fetchUserListAPI,
  fetchUserAPI,
  createUserAPI,
  updateUserAPI,
  deleteUsersAPI
} from './api'

function* handleFetchUsers() {
  try {
    const users = yield call(fetchUserListAPI)
    yield put(actions.fetchListSuccess(users))
  } catch (error) {
    yield put(actions.fetchListFailure(error))
    yield put(snackbarActions.showFailure({ message: 'Fetch Failed!' }))
  }
}

function* handleFetchCurrentUser({ payload: id }) {
  try {
    const user = yield call(fetchUserAPI, id)
    yield put(actions.fetchCurrentSuccess(user))
  } catch (error) {
    yield put(actions.fetchCurrentFailure(error))
    yield put(snackbarActions.showFailure({ message: 'Fetch Failed!' }))
  }
}

function* handleCreateUser({ payload }) {
  try {
    yield call(createUserAPI, payload)
    yield put(actions.createSuccess())
    yield put(snackbarActions.showSuccess({ message: 'Create Success!' }))
    yield put(routerActions.push(paths.USER_LIST))
  } catch (error) {
    yield put(actions.createFailure(error))
    yield put(snackbarActions.showFailure({ message: 'Create Failed!' }))
  }
}

export function* handleUpdateUser({ payload }) {
  try {
    yield call(updateUserAPI, payload)
    yield put(actions.updateSuccess())
    yield put(snackbarActions.showSuccess({ message: 'Update Success!' }))
    yield put(routerActions.push(paths.USER_LIST))
  } catch (error) {
    yield put(actions.updateFailure(error))
    yield put(snackbarActions.showFailure({ message: 'Update Failed!' }))
  }
}

function* handleDeleteUsers({ payload: ids }) {
  try {
    yield put(dialogActions.showConfirm({ confirmButtonText: 'Delete' }))
    const confirm = yield takeDialogResult()
    if (!confirm) return yield put(actions.deleteCancel())

    yield call(deleteUsersAPI, ids)
    yield put(actions.deleteSuccess(ids))
    yield put(snackbarActions.showSuccess({ message: 'Delete Success!' }))
  } catch (error) {
    yield put(actions.deleteFailure(error))
    yield put(snackbarActions.showFailure({ message: 'Delete Failed!' }))
  }
}

export default function*() {
  yield all([
    takeLeading(`${actions.fetchListRequest}`, handleFetchUsers),
    takeLatest(`${actions.fetchCurrentRequest}`, handleFetchCurrentUser),
    takeLeading(`${actions.createRequest}`, handleCreateUser),
    takeLeading(`${actions.updateRequest}`, handleUpdateUser),
    takeLeading(`${actions.deleteRequest}`, handleDeleteUsers)
  ])
}
