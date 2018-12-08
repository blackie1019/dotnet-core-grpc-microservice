import React, { Component } from 'react';
import PropTypes from 'prop-types';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import actionCreator from 'core/rootAction';

class ErrorBoundary extends Component {
  state = { hasError: false };

  static getDerivedStateFromError(error) {
    // Update state so the next render will show the fallback UI.
    return { hasError: true };
  }

  componentDidCatch(error, info) {
    // You can also log the error to an error reporting service
    this.props.sendErrorToLog(
      `[Client Error] ${error.toString()}, ${info.componentStack}`
    );
  }

  render() {
    if (this.state.hasError) {
      // You can render any custom fallback UI
      return <h1>Something went wrong.</h1>;
    }

    return this.props.children;
  }
}

ErrorBoundary.propTypes = {
  children: PropTypes.node.isRequired,
  sendErrorToLog: PropTypes.func.isRequired
};

ErrorBoundary.displayName = 'ErrorBoundary';

const mapDispatchToProps = dispatch => ({
  ...bindActionCreators(actionCreator.errorHandler, dispatch)
});

export default connect(
  null,
  mapDispatchToProps
)(ErrorBoundary);
