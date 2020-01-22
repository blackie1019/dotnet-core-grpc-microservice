import { Icon } from 'antd'
import React from 'react'
import CreateUserPage from '@app/Account/User/CreateUserPage'
import UpdateUserPage from '@app/Account/User/UpdateUserPage'
import UserListPage from '@app/Account/User/UserListPage'
import LocalizationListPage from '@app/Account/Localization/LocalizationListPage'
import CreateLocalizationPage from '@app/Account/Localization/CreateLocalizationPage'
import UpdateLocalizationPage from '@app/Account/Localization/UpdateLocalizationPage'
import CreateCurrencyPage from '@app/Account/Currency/CreateCurrencyPage'
import UpdateCurrencyPage from '@app/Account/Currency/UpdateCurrencyPage'
import CurrencyListPage from '@app/Account/Currency/CurrencyListPage'
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
          },
          {
            displayKey: 'Localization',
            path: paths.LOCALIZATION_LIST,
            exact: false,
            routes: [
              {
                displayKey: 'Localization List',
                component: LocalizationListPage
              },
              {
                policy: [policies.USER_MODIFY],
                displayKey: 'Create Localization',
                path: paths.CREATE_LOCALIZATION,
                component: CreateLocalizationPage
              },
              {
                policy: [policies.USER_MODIFY],
                displayKey: 'Update Localization',
                path: paths.UPDATE_LOCALIZATION,
                component: UpdateLocalizationPage
              }
            ]
          },
          {
            displayKey: 'Currency',
            path: paths.CURRENCY_LIST,
            exact: false,
            routes: [
              {
                displayKey: 'Currency List',
                component: CurrencyListPage
              },
              {
                policy: [policies.USER_MODIFY],
                displayKey: 'Create Currency',
                path: paths.CREATE_CURRENCY,
                component: CreateCurrencyPage
              },
              {
                policy: [policies.USER_MODIFY],
                displayKey: 'Update Currency',
                path: paths.UPDATE_CURRENCY,
                component: UpdateCurrencyPage
              }
            ]
          }
        ]
      }
    ]
  }
]
