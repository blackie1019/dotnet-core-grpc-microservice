import { take } from 'redux-saga/effects'
import action from './action'

function* takeDialogResult() {
  const { type } = yield take([`${action.confirm}`, `${action.cancel}`])
  return type === `${action.confirm}`
}

export { takeDialogResult }
