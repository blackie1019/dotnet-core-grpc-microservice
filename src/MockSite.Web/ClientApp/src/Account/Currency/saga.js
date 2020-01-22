import * as routerActions from 'connected-react-router'
import { all, call, put, takeLatest, takeLeading } from 'redux-saga/effects'
import paths from '@app/App/constants/paths'
import dialogActions from '@app/Common/Dialog/action'
import { takeDialogResult } from '@app/Common/Dialog/effects'
import snackbarActions from '@app/Common/Snackbar/action'
import actions from './action'
import {
  fetchCurrencyListAPI,
  fetchCurrencyAPI,
  createCurrencyAPI,
  updateCurrencyAPI,
  deleteCurrenciesAPI
} from './api'

function* handleFetchCurrencies() {
  try {
    const currencies = yield call(fetchCurrencyListAPI)
    yield put(actions.fetchListSuccess(currencies))
  } catch (error) {
    yield put(actions.fetchListFailure(error))
    yield put(snackbarActions.showFailure({ message: 'Fetch Failed!' }))
  }
}

function* handleFetchCurrentCurrency({ payload: currencyCode }) {
  try {
    const currency = yield call(fetchCurrencyAPI, currencyCode)
    yield put(actions.fetchCurrentSuccess(currency))
  } catch (error) {
    yield put(actions.fetchCurrentFailure(error))
    yield put(snackbarActions.showFailure({ message: 'Fetch Failed!' }))
  }
}

function* handleCreateCurrency({ payload }) {
  try {
    yield call(createCurrencyAPI, payload)
    yield put(actions.createSuccess())
    yield put(snackbarActions.showSuccess({ message: 'Create Success!' }))
    yield put(routerActions.push(paths.CURRENCY_LIST))
  } catch (error) {
    yield put(actions.createFailure(error))
    yield put(snackbarActions.showFailure({ message: 'Create Failed!' }))
  }
}

export function* handleUpdateCurrency({ payload }) {
  try {
    yield call(updateCurrencyAPI, payload)
    yield put(actions.updateSuccess())
    yield put(snackbarActions.showSuccess({ message: 'Update Success!' }))
    yield put(routerActions.push(paths.CURRENCY_LIST))
  } catch (error) {
    yield put(actions.updateFailure(error))
    yield put(snackbarActions.showFailure({ message: 'Update Failed!' }))
  }
}

function* handleDeleteCurrencies({ payload: currencyCodes }) {
  try {
    yield put(dialogActions.showConfirm({ confirmButtonText: 'Delete' }))
    const confirm = yield takeDialogResult()
    if (!confirm) return yield put(actions.deleteCancel())

    yield call(deleteCurrenciesAPI, currencyCodes)
    yield put(actions.deleteSuccess(currencyCodes))
    yield put(snackbarActions.showSuccess({ message: 'Delete Success!' }))
  } catch (error) {
    yield put(actions.deleteFailure(error))
    yield put(snackbarActions.showFailure({ message: 'Delete Failed!' }))
  }
}

export default function*() {
  yield all([
    takeLeading(`${actions.fetchListRequest}`, handleFetchCurrencies),
    takeLatest(`${actions.fetchCurrentRequest}`, handleFetchCurrentCurrency),
    takeLeading(`${actions.createRequest}`, handleCreateCurrency),
    takeLeading(`${actions.updateRequest}`, handleUpdateCurrency),
    takeLeading(`${actions.deleteRequest}`, handleDeleteCurrencies)
  ])
}
