import PropTypes from 'prop-types'
import React, { Component } from 'react'
import { connect } from 'react-redux'
import actions from './action'

class ErrorBoundary extends Component {
  static propTypes = {
    logError: PropTypes.func.isRequired
  }

  static getDerivedStateFromError() {
    return { hasError: true }
  }

  state = { hasError: false }

  componentDidCatch(error, info) {
    this.props.logError(
      `[Component Error] ${error.toString()}, ${info.componentStack}`
    )
  }

  render() {
    if (this.state.hasError) return <h1>Something went wrong.</h1>
    return this.props.children
  }
}

export default connect(
  null,
  actions
)(ErrorBoundary)
