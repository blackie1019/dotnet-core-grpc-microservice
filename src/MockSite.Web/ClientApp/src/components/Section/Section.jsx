import { Col, Layout, Row, Typography } from 'antd'
import { bool, string } from 'prop-types'
import React from 'react'
import routes from '@app/App/configs/routes'
import BackButton from '@app/Common/Router/BackButton'
import Box from '@app/Components/Box/Box'

function SectionBackButton() {
  const consoleRoutes = routes[routes.length - 1].routes
  return (
    <BackButton
      style={{ marginBottom: 8, marginRight: 8, padding: 8 }}
      routes={consoleRoutes}
    />
  )
}

function SectionTitle({ text, children }) {
  return (
    <Box marginBottom={8}>
      <Typography.Title style={{ margin: 0 }} level={3}>
        {text}
        {children}
      </Typography.Title>
    </Box>
  )
}

SectionTitle.propTypes = {
  text: string.isRequired
}

function SectionRow({ children }) {
  return (
    <Row type="flex" style={{ marginTop: 4, marginBottom: 4 }}>
      {children}
    </Row>
  )
}

function SectionCol({ style, children, ...props }) {
  return (
    <Col
      style={{
        display: 'flex',
        alignItems: 'center',
        paddingTop: 4,
        paddingBottom: 4,
        ...style
      }}
      {...props}
    >
      {children}
    </Col>
  )
}

function SectionActionBar({ children }) {
  const elements = React.Children.toArray(children)
  return (
    <SectionRow>
      <SectionCol span={24} sm={12}>
        {elements[0]}
      </SectionCol>
      {elements[1] && (
        <SectionCol
          span={24}
          sm={{ push: 1, span: 11 }}
          md={{ push: 2, span: 10 }}
          lg={{ push: 4, span: 8 }}
        >
          {elements[1]}
        </SectionCol>
      )}
    </SectionRow>
  )
}

function SectionFilter({ style, align, label, children, ...props }) {
  const textStyle = { marginRight: 8 }
  if (align) {
    textStyle.width = 60
    textStyle.textAlign = 'right'
  }
  return (
    <SectionCol {...props} style={{ ...style, padding: 4 }}>
      {label && (
        <Typography.Text style={textStyle} strong>
          {label}
        </Typography.Text>
      )}
      {children}
    </SectionCol>
  )
}

SectionFilter.propTypes = {
  align: bool,
  label: string
}

function SectionButtonGroup({ children }) {
  return (
    <Box display="flex" css={'>*+*{margin-left:8px!important;}'}>
      {children}
    </Box>
  )
}

function SectionBody(props) {
  return (
    <Box marginTop={8}>
      <Box overflow="hidden">{props.children}</Box>
    </Box>
  )
}

export default function Section({ style, children }) {
  let backButton
  let title
  const restChildren = []
  React.Children.forEach(children, child => {
    switch (child.type) {
      case SectionBackButton:
        backButton = child
        break
      case SectionTitle:
        title = child
        break
      default:
        restChildren.push(child)
        break
    }
  })
  return (
    <Layout
      style={{
        borderRadius: 8,
        padding: 24,
        backgroundColor: 'white',
        ...style
      }}
    >
      {(title || backButton) && (
        <Box display="flex" alignItems="center">
          {backButton}
          {title}
        </Box>
      )}
      {restChildren}
    </Layout>
  )
}

Section.Back = SectionBackButton
Section.Title = SectionTitle
Section.ActionBar = SectionActionBar
Section.Row = SectionRow
Section.Col = SectionCol
Section.Filter = SectionFilter
Section.ButtonGroup = SectionButtonGroup
Section.Body = SectionBody
