import Fuse from 'fuse.js'
import { useMemo } from 'react'

export default ({ data, keys, keyword }) => {
  return useMemo(() => {
    if (!keyword) return data
    const fuse = new Fuse(data, {
      threshold: 0.1,
      keys
    })
    return fuse.search(keyword)
  }, [data, keyword])
}
