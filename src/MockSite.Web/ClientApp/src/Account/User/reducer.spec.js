import actions from './action'
import reducer from './reducer'

describe('User Reducer', () => {
  it('should handle userActions.fetchListSuccess', () => {
    const initialState = { list: [] }
    const expectedUsers = [{ id: 1, name: 'u1' }]
    const actualState = reducer(
      initialState,
      actions.fetchListSuccess(expectedUsers)
    )
    expect(actualState.list).toEqual(expectedUsers)
  })

  it('should handle userActions.fetchCurrentRequest', () => {
    const initialState = { current: { id: 1, name: 'u1' } }
    const actualState = reducer(initialState, actions.fetchCurrentRequest(2))
    expect(actualState.current).toBeUndefined()
  })

  it('should handle userActions.fetchCurrentSuccess', () => {
    const initialState = { current: undefined }
    const expectedUser = { id: 1, name: 'u1' }
    const actualState = reducer(
      initialState,
      actions.fetchCurrentSuccess(expectedUser)
    )
    expect(actualState.current).toEqual(expectedUser)
  })

  it('should handle userActions.deleteSuccess', () => {
    const initialState = {
      list: [
        { id: 1, name: 'u1' },
        { id: 2, name: 'u2' },
        { id: 3, name: 'u3' }
      ]
    }
    const deletedIds = [1, 3]
    const expectedUsers = [{ id: 2, name: 'u2' }]
    const actualState = reducer(initialState, actions.deleteSuccess(deletedIds))
    expect(actualState.list).toEqual(expectedUsers)
  })
})
