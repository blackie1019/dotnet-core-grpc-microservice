import { Breadcrumb } from 'antd'
import { arrayOf, object } from 'prop-types'
import React from 'react'
import { Link } from 'react-router-dom'
import useMemoCall from '@app/Common/Hooks/useMemoCall'
import getBreadcrumbs from '@app/Common/Router/getBreadcrumbs'
import ProfileDropdown from './ProfileDropdown'
import Topbar from './StyledTopbar'

export default function ConsolePageTopbar({ location, routes }) {
  const breadcrumbs = useMemoCall(getBreadcrumbs, location.pathname, routes)
  return (
    <Topbar>
      <Breadcrumb separator="">
        {breadcrumbs.map(crumb => (
          <Breadcrumb.Item key={crumb.displayKey}>
            <Link to={crumb.path}>{crumb.displayKey}</Link>
          </Breadcrumb.Item>
        ))}
      </Breadcrumb>
      <Topbar.Right>
        <ProfileDropdown />
      </Topbar.Right>
    </Topbar>
  )
}

ConsolePageTopbar.propTypes = {
  routes: arrayOf(object.isRequired).isRequired
}
