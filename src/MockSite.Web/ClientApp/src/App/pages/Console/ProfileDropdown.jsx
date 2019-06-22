import { Avatar } from 'antd'
import { push as gotoPage } from 'connected-react-router'
import { func, string } from 'prop-types'
import React from 'react'
import { connect } from 'react-redux'
import authActions from '@app/Common/Authentication/action'
import Button from '@app/Components/Button/Button'
import Dropdown from '@app/Components/Dropdown/Dropdown'
import paths from '../../constants/paths'

function ProfileDropdown({ username, gotoPage, logout }) {
  const navs = [
    {
      displayKey: 'Change Password',
      onClick: () => gotoPage(paths.CHANGE_PASSWORD)
    },
    {
      displayKey: 'Lock Screen',
      onClick: () => gotoPage(paths.LOCK_SCREEN)
    },
    {
      displayKey: 'Log Out',
      onClick: logout
    }
  ]
  return (
    <Dropdown>
      <Dropdown.Trigger>
        <Button type="link" icon="caret-down" iconTheme="filled" iconRight>
          <Avatar size="small">{username[0]}</Avatar>
        </Button>
      </Dropdown.Trigger>
      {navs.map((nav, i) => (
        <Dropdown.Item key={i} text={nav.displayKey} onClick={nav.onClick} />
      ))}
    </Dropdown>
  )
}

ProfileDropdown.propTypes = {
  username: string.isRequired,
  gotoPage: func.isRequired,
  logout: func.isRequired
}

export default connect(
  state => ({ username: state.authentication.credential.name }),
  { gotoPage, logout: authActions.logout }
)(ProfileDropdown)
