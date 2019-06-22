import { useEffect } from 'react'

export default willUnmount => useEffect(() => willUnmount, [])
