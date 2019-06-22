import { array } from 'prop-types'
import React from 'react'
import styled from 'styled-components'
import Box from '@app/Components/Box/Box'
import FullScreenPageTopbar from './FullScreenPageTopbar'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  align-items: center;
  width: 100vw;
  min-height: 100%;
  background-image: url(https://source.unsplash.com/collection/2411320/800);
  background-position: center;
  background-size: cover;
  &::before {
    position: absolute;
    top: 0;
    left: 0;
    display: block;
    width: 100%;
    height: 100%;
    background-color: rgba(0, 0, 0, 0.4);
    content: '';
  }
`

export default function FullScreenPageContainer({ navs, children }) {
  return (
    <Box height="100vh" overflowY="auto">
      <Container>
        <FullScreenPageTopbar navs={navs} />
        <Box flex="1" display="flex" alignItems="center" padding="0 0 64px">
          {children}
        </Box>
      </Container>
    </Box>
  )
}

FullScreenPageContainer.propTypes = {
  navs: array
}
