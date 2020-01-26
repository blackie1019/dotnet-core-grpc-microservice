import * as routerActions from 'connected-react-router'
import { all, call, put, takeLatest, takeLeading } from 'redux-saga/effects'
import paths from '@app/App/constants/paths'
import dialogActions from '@app/Common/Dialog/action'
import { takeDialogResult } from '@app/Common/Dialog/effects'
import snackbarActions from '@app/Common/Snackbar/action'
import actions from './action'
import {
  fetchLocalizationListAPI,
  fetchLocalizationAPI,
  createLocalizationAPI,
  updateLocalizationAPI,
  deleteLocalizationsAPI
} from './api'

function* handleFetchLocalizationList() {
  try {
    const localizations = yield call(fetchLocalizationListAPI)
    yield put(actions.fetchListSuccess(localizations))
  } catch (error) {
    yield put(actions.fetchListFailure(error))
    yield put(snackbarActions.showFailure({ message: 'Fetch Failed!' }))
  }
}

function* handleFetchCurrentLocalization({ payload: displayKey }) {
  try {
    const localization = yield call(fetchLocalizationAPI, displayKey)
    yield put(actions.fetchCurrentSuccess(localization))
  } catch (error) {
    yield put(actions.fetchCurrentFailure(error))
    yield put(snackbarActions.showFailure({ message: 'Fetch Failed!' }))
  }
}

function* handleCreateLocalization({ payload }) {
  try {
    yield call(createLocalizationAPI, payload)
    yield put(actions.createSuccess())
    yield put(snackbarActions.showSuccess({ message: 'Create Success!' }))
    yield put(routerActions.push(paths.LOCALIZATION_LIST))
  } catch (error) {
    yield put(actions.createFailure(error))
    yield put(snackbarActions.showFailure({ message: 'Create Failed!' }))
  }
}

function* handleUpdateLocalization({ payload }) {
  try {
    yield call(updateLocalizationAPI, payload)
    yield put(actions.updateSuccess())
    yield put(snackbarActions.showSuccess({ message: 'Update Success!' }))
    yield put(routerActions.push(paths.LOCALIZATION_LIST))
  } catch (error) {
    yield put(actions.updateFailure(error))
    yield put(snackbarActions.showFailure({ message: 'Update Failed!' }))
  }
}

function* handleDeleteLocalizations({ payload: displayKeys }) {
  try {
    yield put(dialogActions.showConfirm({ confirmButtonText: 'Delete' }))
    const confirm = yield takeDialogResult()
    if (!confirm) return yield put(actions.deleteCancel())

    yield call(deleteLocalizationsAPI, displayKeys)
    yield put(actions.deleteSuccess(displayKeys))
    yield put(snackbarActions.showSuccess({ message: 'Delete Success!' }))
  } catch (error) {
    yield put(actions.deleteFailure(error))
    yield put(snackbarActions.showFailure({ message: 'Delete Failed!' }))
  }
}

export default function*() {
  yield all([
    takeLeading(`${actions.fetchListRequest}`, handleFetchLocalizationList),
    takeLatest(
      `${actions.fetchCurrentRequest}`,
      handleFetchCurrentLocalization
    ),
    takeLeading(`${actions.createRequest}`, handleCreateLocalization),
    takeLeading(`${actions.updateRequest}`, handleUpdateLocalization),
    takeLeading(`${actions.deleteRequest}`, handleDeleteLocalizations)
  ])
}
