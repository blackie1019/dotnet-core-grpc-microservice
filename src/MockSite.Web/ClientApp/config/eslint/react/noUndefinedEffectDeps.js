module.exports = {
  create: context => {
    function isUseEffectNode(node) {
      return node.type === 'CallExpression' && node.callee.name === 'useEffect'
    }

    function visitFunctionExpression(node) {
      const { parent } = node
      if (!isUseEffectNode(parent)) return
      if (parent.arguments.length !== 1) return
      context.report({
        node: parent.callee,
        message:
          "useEffect callback will be invoked every rendering if you don't give second argument. " +
          'Did you forget to pass an array of dependencies?'
      })
    }

    return {
      ArrowFunctionExpression: visitFunctionExpression,
      FunctionExpression: visitFunctionExpression
    }
  }
}
