import actions from './action'
import reducer from './reducer'

describe('Authentication Reducer', () => {
  const initialState = {
    didInit: false,
    credential: undefined
  }

  it('should handle authActions.initSuccess', () => {
    const credential = { token: '123' }
    const expectedState = { didInit: true, credential }
    const actualState = reducer(initialState, actions.initSuccess(credential))
    expect(actualState).toEqual(expectedState)
  })

  it('should handle authActions.loginSuccess', () => {
    const credential = { token: '123' }
    const actualState = reducer(initialState, actions.loginSuccess(credential))
    expect(actualState.credential).toEqual(credential)
  })

  it('should handle authActions.lockScreenSuccess', () => {
    const credential = { token: '' }
    const actualState = reducer(
      initialState,
      actions.lockScreenSuccess(credential)
    )
    expect(actualState.credential).toEqual(credential)
  })

  it('should handle authActions.unlockScreenSuccess', () => {
    const credential = { token: '123' }
    const actualState = reducer(
      initialState,
      actions.unlockScreenSuccess(credential)
    )
    expect(actualState.credential).toEqual(credential)
  })

  it('should handle authActions.logout', () => {
    const credential = { token: '123' }
    const loginState = { didInit: true, credential }
    const expectedState = { didInit: true, credential: undefined }
    const actualState = reducer(loginState, actions.logout())
    expect(actualState).toEqual(expectedState)
  })
})
