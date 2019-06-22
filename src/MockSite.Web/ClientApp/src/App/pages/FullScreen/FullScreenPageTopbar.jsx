import { Layout, Typography } from 'antd'
import { arrayOf, element, shape, string } from 'prop-types'
import React from 'react'
import { Link } from 'react-router-dom'
import styled from 'styled-components'
import Visible from '@app/Common/Responsive/Visible'
import Box from '@app/Components/Box/Box'
import paths from '../../constants/paths'
import DrawerNav from './DrawerNav'
import MenuNav from './MenuNav'

const Header = styled(Layout.Header)`
  && {
    z-index: 1;
    width: 100%;
    padding: 0 24px;
    max-width: 1600px;
    background-color: transparent;
    @media (min-width: 576px) {
      padding: 0 48px;
    }
  }
`

export default function FullScreenPageTopbar({ navs = [] }) {
  return (
    <Header>
      <Box display="flex" alignItems="center" height="100%">
        <Link to={paths.INDEX}>
          <Typography.Title style={{ margin: 0, color: 'white' }} level={3}>
            MOCKSITE
          </Typography.Title>
        </Link>
        <Box flex="1" display="flex" justifyContent="flex-end">
          <Visible media="(min-width: 576px)">
            <MenuNav
              style={{ backgroundColor: 'transparent' }}
              mode="horizontal"
              theme="dark"
              navs={navs}
            />
          </Visible>
          <Visible media="(max-width: 575px)">
            <DrawerNav navs={navs} />
          </Visible>
        </Box>
      </Box>
    </Header>
  )
}

FullScreenPageTopbar.propTypes = {
  navs: arrayOf(
    shape({
      path: string.isRequired,
      icon: element.isRequired,
      displayKey: string.isRequired
    }).isRequired
  )
}
