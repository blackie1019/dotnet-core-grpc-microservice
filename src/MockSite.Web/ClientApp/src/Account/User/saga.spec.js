import * as routerActions from 'connected-react-router'
import paths from '@app/App/constants/paths'
import dialogActions from '@app/Common/Dialog/action'
import createSagaTester from '@app/Common/Utils/test/createSagaTester'
import actions from './action'
import {
  fetchUserListAPI,
  fetchUserAPI,
  createUserAPI,
  updateUserAPI,
  deleteUsersAPI
} from './api'
import reducer from './reducer'
import saga from './saga'

jest.mock('./api')

describe('User Saga', () => {
  describe('UserActions.fetchListRequest handler', () => {
    it('should handle success response', async () => {
      const users = [{ id: 1, name: 'u1' }, { id: 2, name: 'u2' }]
      const successAction = actions.fetchListSuccess(users)
      const expectedState = reducer(undefined, successAction)
      fetchUserListAPI.mockResolvedValue(users)

      const tester = createSagaTester({ reducer, saga })
      await tester.next(actions.fetchListRequest())

      expect(fetchUserListAPI).toBeCalled()
      expect(tester.getCalledActions()).toContainEqual(successAction)
      expect(tester.getState()).toEqual(expectedState)
    })

    it('should handle failure response', async () => {
      const error = new Error(401)
      fetchUserListAPI.mockRejectedValue(error)

      const tester = createSagaTester({ reducer, saga })
      await tester.next(actions.fetchListRequest())

      expect(fetchUserListAPI).toBeCalled()
      expect(tester.getCalledActions()).toContainEqual(
        actions.fetchListFailure(error)
      )
    })
  })

  describe('UserActions.fetchCurrentRequest handler', () => {
    it('should handle success response', async () => {
      const user = { id: 1, name: 'u1' }
      const successAction = actions.fetchCurrentSuccess(user)
      const expectedState = reducer(undefined, successAction)
      fetchUserAPI.mockResolvedValue(user)

      const tester = await createSagaTester({ reducer, saga })
      await tester.next(actions.fetchCurrentRequest(user.id))

      expect(fetchUserAPI).toBeCalledWith(user.id)
      expect(tester.getCalledActions()).toContainEqual(successAction)
      expect(tester.getState()).toEqual(expectedState)
    })

    it('should handle failure response', async () => {
      const userId = 1
      const error = new Error(401)
      fetchUserAPI.mockRejectedValue(error)

      const tester = await createSagaTester({ reducer, saga })
      await tester.next(actions.fetchCurrentRequest(userId))

      expect(fetchUserAPI).toBeCalledWith(userId)
      expect(tester.getCalledActions()).toContainEqual(
        actions.fetchCurrentFailure(error)
      )
    })
  })

  describe('UserActions.createRequest handler', () => {
    it('should handle success response', async () => {
      const user = { name: 'u1', email: 'a@b.c' }
      createUserAPI.mockResolvedValue()

      const tester = await createSagaTester({ reducer, saga })
      await tester.next(actions.createRequest(user))

      const calledActions = tester.getCalledActions()
      expect(createUserAPI).toBeCalledWith(user)
      expect(calledActions).toContainEqual(actions.createSuccess())
      expect(calledActions).toContainEqual(routerActions.push(paths.USER_LIST))
    })

    it('should handle failure response', async () => {
      const user = { name: 'u1', email: 'a@b.c' }
      const error = new Error(401)
      createUserAPI.mockRejectedValue(error)

      const tester = await createSagaTester({ reducer, saga })
      await tester.next(actions.createRequest(user))

      expect(createUserAPI).toBeCalledWith(user)
      expect(tester.getCalledActions()).toContainEqual(
        actions.createFailure(error)
      )
    })
  })

  describe('UserActions.updateRequest handler', () => {
    it('should handle success response', async () => {
      const user = { id: 1, name: 'u1', email: 'a@b.c' }
      updateUserAPI.mockResolvedValue()

      const tester = await createSagaTester({ reducer, saga })
      await tester.next(actions.updateRequest(user))

      const calledActions = tester.getCalledActions()
      expect(updateUserAPI).toBeCalledWith(user)
      expect(calledActions).toContainEqual(actions.updateSuccess())
      expect(calledActions).toContainEqual(routerActions.push(paths.USER_LIST))
    })

    it('should handle failure response', async () => {
      const user = { id: 1, name: 'u1', email: 'a@b.c' }
      const error = new Error(401)
      updateUserAPI.mockRejectedValue(error)

      const tester = await createSagaTester({ reducer, saga })
      await tester.next(actions.updateRequest(user))

      expect(updateUserAPI).toBeCalledWith(user)
      expect(tester.getCalledActions()).toContainEqual(
        actions.updateFailure(error)
      )
    })
  })

  describe('UserActions.deleteRequest handler', () => {
    it('should handle success response', async () => {
      const userIds = [1, 3]
      const users = [{ id: 1, name: 'u1' }, { id: 2, name: 'u2' }]
      const successAction = actions.deleteSuccess(userIds)
      const initialState = reducer(undefined, actions.fetchListSuccess(users))
      const expectedState = reducer(initialState, successAction)
      deleteUsersAPI.mockResolvedValue()

      const tester = await createSagaTester({ initialState, reducer, saga })
      await tester.next(actions.deleteRequest(userIds))
      await tester.dispatch(dialogActions.confirm())

      expect(deleteUsersAPI).toBeCalledWith(userIds)
      expect(tester.getCalledActions()).toContainEqual(successAction)
      expect(tester.getState()).toEqual(expectedState)
    })

    it('should handle failure response', async () => {
      const userIds = [1, 3]
      const error = new Error(401)
      deleteUsersAPI.mockRejectedValue(error)

      const tester = await createSagaTester({ reducer, saga })
      await tester.next(actions.deleteRequest(userIds))
      await tester.dispatch(dialogActions.confirm())

      expect(deleteUsersAPI).toBeCalledWith(userIds)
      expect(tester.getCalledActions()).toContainEqual(
        actions.deleteFailure(error)
      )
    })

    it('should cancel request', async () => {
      const userIds = [1, 3]
      deleteUsersAPI.mockResolvedValue()

      const tester = await createSagaTester({ reducer, saga })
      await tester.next(actions.deleteRequest(userIds))
      await tester.dispatch(dialogActions.cancel())

      expect(deleteUsersAPI).toBeCalledTimes(0)
      expect(tester.getCalledActions()).toContainEqual(actions.deleteCancel())
    })
  })
})
