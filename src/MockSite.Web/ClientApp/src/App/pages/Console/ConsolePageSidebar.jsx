import anime from 'animejs'
import { Menu, Typography } from 'antd'
import { arrayOf, element, func, shape, string } from 'prop-types'
import React, { useState } from 'react'
import Sider from './StyledSider'

const menuAnimation = {
  enter: (node, done) => {
    anime({
      targets: node,
      height: [
        { value: 0, duration: 0 },
        { value: node.offsetHeight, duration: 100 }
      ],
      opacity: [
        { value: 0, duration: 0 },
        { value: 1, duration: 50, delay: 50 }
      ],
      easing: 'easeOutQuart',
      complete: done
    })
  },
  leave: (node, done) => {
    anime({
      targets: node,
      height: [
        { value: node.offsetHeight, duration: 0 },
        { value: 0, duration: 100 },
        { value: node.offsetHeight, duration: 0 }
      ],
      opacity: [{ value: 1, duration: 0 }, { value: 0, duration: 50 }],
      easing: 'easeOutQuart',
      complete: done
    })
  }
}

export default function ConsolePageSidebar({
  title,
  currentGroupKey,
  currentNavKey,
  navGroups
}) {
  const [collapsed, setCollapsed] = useState(false)
  return (
    <Sider
      collapsed={collapsed}
      onCollapse={collapse => setCollapsed(collapse)}
    >
      <Sider.Header>
        <Typography.Title level={3} style={{ color: 'white' }}>
          {title}
        </Typography.Title>
      </Sider.Header>
      <Sider.Body>
        <Menu
          mode="inline"
          theme="dark"
          defaultOpenKeys={[currentGroupKey]}
          defaultSelectedKeys={[currentNavKey]}
          forceSubMenuRender
          openAnimation={menuAnimation}
        >
          {navGroups.map(navGroup => (
            <Menu.SubMenu
              key={navGroup.displayKey}
              title={
                <>
                  <span>{navGroup.displayKey}</span>
                  {navGroup.icon}
                </>
              }
            >
              {navGroup.navs.map(nav => (
                <Menu.Item key={nav.displayKey} onClick={nav.onClick}>
                  {nav.displayKey}
                </Menu.Item>
              ))}
            </Menu.SubMenu>
          ))}
        </Menu>
      </Sider.Body>
    </Sider>
  )
}

ConsolePageSidebar.propTypes = {
  title: string.isRequired,
  currentGroupKey: string,
  currentNavKey: string,
  navGroups: arrayOf(
    shape({
      icon: element,
      displayKey: string.isRequired,
      navs: arrayOf(
        shape({
          displayKey: string.isRequired,
          onClick: func.isRequired
        }).isRequired
      ).isRequired
    }).isRequired
  ).isRequired
}
