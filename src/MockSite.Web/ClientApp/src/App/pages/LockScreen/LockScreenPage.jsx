import { Icon } from 'antd'
import { func, string } from 'prop-types'
import React, { useEffect } from 'react'
import { connect } from 'react-redux'
import styled from 'styled-components'
import paths from '@app/App/constants/paths'
import action from '@app/Common/Authentication/action'
import Fading from '@app/Components/Animation/Fading'
import centeredFormStyle from '../FullScreen/centeredFormStyle'
import FullScreenPageContainer from '../FullScreen/FullScreenPageContainer'
import BaseLockScreenForm from './LockScreenForm'

const LockScreenForm = styled(BaseLockScreenForm)`
  ${centeredFormStyle};
`

const navs = [
  {
    path: paths.LOGIN,
    icon: <Icon type="login" />,
    displayKey: 'Login'
  }
]

function LockScreenPage({ username, lockScreen, unlockScreen }) {
  useEffect(() => {
    lockScreen()
  }, [])
  return (
    <FullScreenPageContainer navs={navs}>
      <Fading in down>
        <LockScreenForm username={username} onSubmit={unlockScreen} />
      </Fading>
    </FullScreenPageContainer>
  )
}

LockScreenPage.propTypes = {
  username: string.isRequired,
  lockScreen: func.isRequired,
  unlockScreen: func.isRequired
}

export default connect(
  state => ({ username: state.authentication.credential?.name }),
  { lockScreen: action.lockScreen, unlockScreen: action.unlockScreen }
)(LockScreenPage)
