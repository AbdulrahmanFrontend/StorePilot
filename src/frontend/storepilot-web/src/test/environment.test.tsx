import { render, screen } from '@testing-library/react'
import { describe, expect, it } from 'vitest'

describe('Testing environment', () => {
  it('should render a React component and query its content', () => {
    render(<p>StorePilot Testing Environment</p>)

    expect(
      screen.getByText('StorePilot Testing Environment')
    ).toBeInTheDocument()
  })
})
