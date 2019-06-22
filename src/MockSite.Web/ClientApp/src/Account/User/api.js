import AppAPI from '@app/Common/Utils/http/AppAPI'

const userPath = 'User'

export const fetchUserListAPI = () =>
  AppAPI.get(`${userPath}/GetUsers`).useAuth()

export const fetchUserAPI = id =>
  AppAPI.get(`${userPath}/GetUser/${id}`).useAuth()

export const createUserAPI = data =>
  AppAPI.post(`${userPath}/CreateUser`)
    .useAuth()
    .body(data)

export const updateUserAPI = data =>
  AppAPI.post(`${userPath}/UpdateUser`)
    .useAuth()
    .body(data)

export const deleteUsersAPI = ids =>
  AppAPI.post(`${userPath}/DeleteUser/${ids[0]}`).useAuth()
