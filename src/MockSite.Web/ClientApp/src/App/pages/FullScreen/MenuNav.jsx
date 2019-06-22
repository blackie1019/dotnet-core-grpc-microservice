import { Menu } from 'antd'
import { arrayOf, element, shape, string } from 'prop-types'
import React from 'react'
import { Link } from 'react-router-dom'
import Box from '@app/Components/Box/Box'

export default function MenuNav({ navs, ...props }) {
  return (
    <Menu {...props}>
      {navs.map(nav => (
        <Menu.Item key={nav.path}>
          <Link to={nav.path}>
            <Box display="flex" alignItems="center">
              {nav.icon}
              {nav.displayKey}
            </Box>
          </Link>
        </Menu.Item>
      ))}
    </Menu>
  )
}

MenuNav.propTypes = {
  navs: arrayOf(
    shape({
      path: string.isRequired,
      icon: element.isRequired,
      displayKey: string.isRequired
    }).isRequired
  )
}
