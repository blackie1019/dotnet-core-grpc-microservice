import { Layout } from 'antd'
import { push as gotoPage } from 'connected-react-router'
import { arrayOf, shape, object, func } from 'prop-types'
import React from 'react'
import { connect } from 'react-redux'
import useMemoCall from '@app/Common/Hooks/useMemoCall'
import getCurrentNav from '@app/Common/Router/getCurrentNav'
import SwitchRoute from '@app/Common/Router/SwitchRoute'
import ConsolePageSidebar from './ConsolePageSidebar'
import ConsolePageTopbar from './ConsolePageTopbar'
import getSiderbarNavGroups from './getSiderbarNavGroups'

function ConsolePageContainer({ location, credential, route, gotoPage }) {
  const navGroups = useMemoCall(
    getSiderbarNavGroups,
    route.routes,
    credential,
    gotoPage
  )
  const currentNav = useMemoCall(getCurrentNav, location.pathname, route.routes)
  return (
    <Layout style={{ maxHeight: '100vh', backgroundColor: '#f2f4f8' }}>
      <ConsolePageSidebar
        title="MOCKSITE"
        currentGroupKey={currentNav?.group?.displayKey}
        currentNavKey={currentNav?.item?.displayKey}
        navGroups={navGroups}
      />
      <Layout
        style={{
          maxHeight: '100vh',
          backgroundColor: 'transparent',
          zIndex: 1,
          overflow: 'hidden'
        }}
      >
        <ConsolePageTopbar location={location} routes={route.routes} />
        <Layout.Content
          style={{
            maxHeight: 'calc(100vh - 64px)',
            padding: '24px',
            overflowX: 'hidden',
            overflowY: 'auto'
          }}
        >
          <SwitchRoute parent={route} routes={route.routes} />
        </Layout.Content>
      </Layout>
    </Layout>
  )
}

ConsolePageContainer.propTypes = {
  credential: object,
  route: shape({
    routes: arrayOf(object.isRequired).isRequired
  }).isRequired,
  gotoPage: func.isRequired
}

export default connect(
  state => ({ credential: state.authentication.credential }),
  { gotoPage }
)(ConsolePageContainer)
