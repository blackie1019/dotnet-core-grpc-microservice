export default ['USER_READONLY', 'USER_MODIFY', 'USER_DELETE'].reduce(
  (dict, key) => {
    dict[key] = key
    return dict
  },
  {}
)
