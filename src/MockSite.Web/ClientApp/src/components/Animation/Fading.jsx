import { bool, number, oneOfType } from 'prop-types'
import React, { useEffect, useState } from 'react'
import styled from 'styled-components'

const toNumber = (value, defaultValue) =>
  typeof value === 'number' ? value : defaultValue

const beginOfXY = ({ $in, $up, $down, $right, $left }) => {
  const offset = Math.abs(toNumber($up || $down || $right || $left, 50))
  if ($up) return `0,${$in ? offset : 0}px`
  if ($down) return `0,${$in ? -offset : 0}px`
  if ($right) return `${$in ? -offset : 0}px,0`
  if ($left) return `${$in ? offset : 0}px,0`
  return '0,0'
}

const endOfXY = ({ $out, $up, $down, $right, $left }) => {
  const offset = Math.abs(toNumber($up || $down || $right || $left, 50))
  if ($up) return `0,${$out ? -offset : 0}px`
  if ($down) return `0,${$out ? offset : 0}px`
  if ($right) return `${$out ? offset : 0}px,0`
  if ($left) return `${$out ? -offset : 0}px,0`
  return '0,0'
}

const beginOfOpacity = props => (props.$in ? 0 : 1)

const endOfOpacity = props => (props.$out ? 0 : 1)

const Animated = styled.div`
  transition: all 300ms linear;
  transform: translate3d(${beginOfXY}, 0);
  opacity: ${beginOfOpacity};
  &.end {
    transform: translate3d(${endOfXY}, 0);
    opacity: ${endOfOpacity};
  }
`

export default function Fading({
  duration = 300,
  in: _in,
  out,
  up,
  down,
  right,
  left,
  children
}) {
  const [className, setClassName] = useState(null)
  useEffect(() => {
    const timerId = setTimeout(() => setClassName('end'), duration)
    return () => clearTimeout(timerId)
  }, [])
  return (
    <Animated
      className={className}
      $in={_in}
      $out={out}
      $up={up}
      $down={down}
      $right={right}
      $left={left}
    >
      {children}
    </Animated>
  )
}

Fading.propTypes = {
  duration: number,
  in: bool,
  out: bool,
  up: oneOfType([bool, number]),
  down: oneOfType([bool, number]),
  right: oneOfType([bool, number]),
  left: oneOfType([bool, number])
}
