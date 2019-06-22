import { push } from 'connected-react-router'
import paths from '@app/App/constants/paths'
import createSagaTester from '@app/Common/Utils/test/createSagaTester'
import actions from './action'
import {
  initAuthAPI,
  lockAuthAPI,
  loginAPI,
  logoutAPI,
  renewTokenAPI,
  changePasswordAPI
} from './api'
import reducer from './reducer'
import saga from './saga'

jest.mock('./api')

describe('Authentication Saga', () => {
  describe('AuthActions.init handler', () => {
    it('should restore credential and auto logout when token expire', async () => {
      const credential = { token: '123', expiryTime: Date.now() }
      const successAction = actions.initSuccess(credential)
      const loginState = reducer(undefined, successAction)
      const logoutState = reducer(loginState, actions.logout())
      initAuthAPI.mockResolvedValue(credential)

      const tester = createSagaTester({ reducer, saga })
      await tester.next(actions.init())
      const actualLoginState = tester.getState()
      await tester.waitFor(`${actions.logout}`)

      expect(initAuthAPI).toBeCalled()
      expect(tester.getCalledActions()).toContainEqual(successAction)
      expect(actualLoginState).toEqual(loginState)
      expect(tester.getState()).toEqual(logoutState)
    })
  })

  describe('AuthActions.login handler', () => {
    it('should handle success response and auto logout when token expire', async () => {
      const payload = { userCode: '1', password: '123' }
      const credential = { token: '123', expiryTime: Date.now() }
      const successAction = actions.loginSuccess(credential)
      const loginState = reducer(undefined, successAction)
      const logoutState = reducer(loginState, actions.logout())
      loginAPI.mockResolvedValue(credential)

      const tester = createSagaTester({ reducer, saga })
      await tester.next(actions.login(payload))
      const actualLoginState = tester.getState()
      await tester.waitFor(`${actions.logout}`)

      const calledActions = tester.getCalledActions()
      expect(loginAPI).toBeCalledWith(payload)
      expect(calledActions).toContainEqual(successAction)
      expect(calledActions).toContainEqual(push(paths.INDEX))
      expect(actualLoginState).toEqual(loginState)
      expect(tester.getState()).toEqual(logoutState)
    })

    it('should handle failure response', async () => {
      const error = new Error(400)
      loginAPI.mockRejectedValue(error)

      const tester = createSagaTester({ reducer, saga })
      await tester.next(actions.login({}))

      expect(loginAPI).toBeCalled()
      expect(tester.getCalledActions()).toContainEqual(
        actions.loginFailure(error)
      )
    })
  })

  describe('AuthActions.logout handler', () => {
    it('should clear credential', async () => {
      const tester = createSagaTester({ reducer, saga })
      await tester.next(actions.logout())

      expect(logoutAPI).toBeCalled()
      expect(tester.getCalledActions()).toContainEqual(push(paths.LOGIN))
    })
  })

  describe('AuthActions.changePassword handler', () => {
    it('should handle success response', async () => {
      const payload = { password: '123' }

      const tester = createSagaTester({ reducer, saga })
      await tester.next(actions.changePassword(payload))

      const calledActions = tester.getCalledActions()
      expect(changePasswordAPI).toBeCalledWith(payload)
      expect(calledActions).toContainEqual(actions.changePasswordSuccess())
      expect(calledActions).toContainEqual(push(paths.INDEX))
    })

    it('should handle failure response', async () => {
      const payload = { password: '123' }
      const error = new Error(401)
      changePasswordAPI.mockRejectedValue(error)

      const tester = createSagaTester({ reducer, saga })
      await tester.next(actions.changePassword(payload))

      expect(changePasswordAPI).toBeCalledWith(payload)
      expect(tester.getCalledActions()).toContainEqual(
        actions.changePasswordFailure(error)
      )
    })
  })

  describe('AuthActions.lockScreen handler', () => {
    it('should lock credential', async () => {
      const lockingCredential = { locking: true }
      lockAuthAPI.mockResolvedValue(lockingCredential)

      const tester = createSagaTester({ reducer, saga })
      await tester.next(actions.lockScreen())

      expect(lockAuthAPI).toBeCalled()
      expect(tester.getCalledActions()).toContainEqual(
        actions.lockScreenSuccess(lockingCredential)
      )
    })
  })

  describe('AuthActions.unlockScreen handler', () => {
    it('should handle success response and auto logout when token expired', async () => {
      const payload = { userCode: '1', password: '123' }
      const credential = { token: '123', expiryTime: Date.now() }
      const successAction = actions.unlockScreenSuccess(credential)
      const unlockState = reducer(undefined, successAction)
      const logoutState = reducer(unlockState, actions.logout())
      renewTokenAPI.mockResolvedValue(credential)

      const tester = createSagaTester({ reducer, saga })
      await tester.next(actions.unlockScreen(payload))
      const actualUnlockState = tester.getState()
      await tester.waitFor(`${actions.logout}`)

      const calledActions = tester.getCalledActions()
      expect(renewTokenAPI).toBeCalledWith(payload)
      expect(calledActions).toContainEqual(successAction)
      expect(calledActions).toContainEqual(push(paths.INDEX))
      expect(actualUnlockState).toEqual(unlockState)
      expect(tester.getState()).toEqual(logoutState)
    })

    it('should handle failure response', async () => {
      const error = new Error(400)
      renewTokenAPI.mockRejectedValue(error)

      const tester = createSagaTester({ reducer, saga })
      await tester.next(actions.unlockScreen({}))

      expect(renewTokenAPI).toBeCalled()
      expect(tester.getCalledActions()).toContainEqual(
        actions.unlockScreenFailure(error)
      )
    })
  })

  describe('Unauthentication handler', () => {
    it('should logout when receive 401 error', async () => {
      const error = new Error()
      error.response = { status: 401 }
      const initialState = {
        authentication: reducer(
          undefined,
          actions.loginSuccess({ token: '123' })
        )
      }

      const tester = createSagaTester({ reducer, saga, initialState })
      await tester.next({ type: 'ANY', error: true, payload: error })

      expect(tester.getCalledActions()).toContainEqual(actions.logout())
    })
  })
})
