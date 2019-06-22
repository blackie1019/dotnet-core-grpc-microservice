import { Icon } from 'antd'
import { array } from 'prop-types'
import React, { useContext } from 'react'
import { __RouterContext } from 'react-router'
import useMemoCall from '@app/Common/Hooks/useMemoCall'
import getBreadcrumbs from './getBreadcrumbs'

export default function BackButton({ routes, ...props }) {
  const { history, location } = useContext(__RouterContext)
  const breadcrumbs = useMemoCall(getBreadcrumbs, location.pathname, routes)
  if (breadcrumbs.length < 2) return null
  return (
    <Icon
      {...props}
      type="arrow-left"
      onClick={() => history.replace(breadcrumbs[breadcrumbs.length - 2].path)}
    />
  )
}

BackButton.propTypes = {
  routes: array.isRequired
}
