import { Icon } from 'antd'
import React from 'react'
import CreateUserPage from '@app/Account/User/CreateUserPage'
import UpdateUserPage from '@app/Account/User/UpdateUserPage'
import UserListPage from '@app/Account/User/UserListPage'
import paths from '../constants/paths'
import policies from '../constants/policies'
import ChangePasswordPage from '../pages/ChangePassword/ChangePasswordPage'
import ConsolePageContainer from '../pages/Console/ConsolePageContainer'
import EmptyPage from '../pages/Empty/EmptyPage'
import LockScreenPage from '../pages/LockScreen/LockScreenPage'
import LoginPage from '../pages/Login/LoginPage'

export default [
  {
    policy: credential => !credential || credential.locking,
    displayKey: 'Login',
    path: paths.LOGIN,
    component: LoginPage
  },
  {
    policy: credential => !!credential,
    displayKey: 'Lock Screen',
    path: paths.LOCK_SCREEN,
    component: LockScreenPage
  },
  {
    policy: true,
    displayKey: 'Change Password',
    path: paths.CHANGE_PASSWORD,
    component: ChangePasswordPage
  },
  {
    policy: true,
    forbiddenFallback: credential =>
      credential ? paths.LOCK_SCREEN : paths.LOGIN,
    notFoundFallback: paths.INDEX,
    component: ConsolePageContainer,
    routes: [
      {
        path: paths.INDEX,
        displayKey: 'Home',
        component: EmptyPage
      },
      {
        policy: [policies.USER_READONLY],
        displayKey: 'Account',
        icon: <Icon type="profile" />,
        path: paths.ACCOUNT,
        exact: false,
        routes: [
          {
            displayKey: 'User',
            path: paths.USER_LIST,
            exact: false,
            routes: [
              {
                displayKey: 'User List',
                component: UserListPage
              },
              {
                policy: [policies.USER_MODIFY],
                displayKey: 'Create User',
                path: paths.CREATE_USER,
                component: CreateUserPage
              },
              {
                policy: [policies.USER_MODIFY],
                displayKey: 'Update User',
                path: paths.UPDATE_USER,
                component: UpdateUserPage
              }
            ]
          }
        ]
      }
    ]
  }
]
