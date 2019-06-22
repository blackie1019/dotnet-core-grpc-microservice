import { css } from 'styled-components'

export default css`
  && {
    margin: 0 20px;
    max-width: 400px;
    width: calc(100vw - 20px);
    @media (min-width: 480px) {
      margin: 0 auto;
    }
  }
`
