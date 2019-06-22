import { Drawer, Icon } from 'antd'
import { array } from 'prop-types'
import React, { useState } from 'react'
import MenuNav from './MenuNav'

export default function DrawerNav({ navs }) {
  const [visible, setVisible] = useState(false)
  const toggleVisible = () => setVisible(!visible)
  if (!navs.length) return null
  return (
    <>
      <Icon type="menu" style={{ color: 'white' }} onClick={toggleVisible} />
      <Drawer
        bodyStyle={{ padding: '16px 24px' }}
        closable={false}
        visible={visible}
        onClose={toggleVisible}
      >
        <MenuNav style={{ borderRight: 0 }} navs={navs} />
      </Drawer>
    </>
  )
}

DrawerNav.propTypes = {
  navs: array.isRequired
}
