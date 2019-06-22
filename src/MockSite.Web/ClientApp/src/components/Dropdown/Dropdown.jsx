import { Dropdown as AntdDropdown, Menu } from 'antd'
import React from 'react'
import hasPermission from '@app/Common/Authentication/hasPermission'
import useReduxState from '@app/Common/Hooks/useReduxState'
import Button from '@app/Components/Button/Button'

function DropdownTrigger() {
  return null
}

function DropdownDivider() {
  return null
}

function DropdownItem() {
  return null
}

export default function Dropdown({ children, ...props }) {
  const credential = useReduxState(state => state.authentication.credential)

  const menuItems = []
  let menuLength = 0
  let trigger
  React.Children.forEach(children, (child, index) => {
    switch (child.type) {
      case DropdownTrigger:
        return (trigger = child.props.children)
      case DropdownItem: {
        const { text, policy, hidden, ...props } = child.props
        const authorized = hasPermission(credential, policy)
        if (authorized && !hidden) {
          menuItems.push(
            <Menu.Item key={index} {...props}>
              {text}
            </Menu.Item>
          )
          menuLength++
        }
        break
      }
      case DropdownDivider:
        menuItems.push(<Menu.Divider key={index} />)
        break
    }
  })

  if (!menuLength) return null
  return (
    <AntdDropdown
      trigger={['click']}
      {...props}
      overlay={<Menu>{menuItems}</Menu>}
    >
      {trigger || <Button.Actions />}
    </AntdDropdown>
  )
}

Dropdown.Trigger = DropdownTrigger
Dropdown.Divider = DropdownDivider
Dropdown.Item = DropdownItem
