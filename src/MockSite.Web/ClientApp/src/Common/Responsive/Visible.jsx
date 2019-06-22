import { string } from 'prop-types'
import useMedia from './useMedia'

export default function Visible({ media, children }) {
  const matches = useMedia(media)
  return matches ? children : null
}

Visible.propTypes = {
  media: string.isRequired
}
