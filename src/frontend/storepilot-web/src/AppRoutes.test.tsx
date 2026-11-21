import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { MemoryRouter } from 'react-router-dom'
import { beforeEach, describe, expect, it, vi } from 'vitest'

import { getDashboardSummary } from './features/dashboard/api/dashboardApi'
import type { DashboardSummary } from './features/dashboard/types/dashboard'
import AppRoutes from './AppRoutes'

vi.mock('./features/dashboard/api/dashboardApi', () => ({
  getDashboardSummary: vi.fn(),
}))

const mockedGetDashboardSummary = vi.mocked(getDashboardSummary)

const summary: DashboardSummary = {
  activeProductsCount: 25,
  activeGroupsCount: 8,
  activeUnitsCount: 4,
}

describe('AppRoutes', () => {
  beforeEach(() => {
    vi.resetAllMocks()
    mockedGetDashboardSummary.mockResolvedValue(summary)
  })

  it('should navigate to the selected section', async () => {
    const user = userEvent.setup()

    render(
      <MemoryRouter initialEntries={['/dashboard']}>
        <AppRoutes />
      </MemoryRouter>,
    )

    const productsLink = screen.getByRole('link', {
        name: 'الأصناف',
    })

    await user.click(productsLink)

    expect(
      await screen.findByText(
        'هذا القسم قيد التطوير وسيتم تنفيذه لاحقًا.',
      ),
    ).toBeInTheDocument()

    expect(
        screen.getByRole('link', { name: 'الأصناف' }),
    ).toHaveClass('sidebar-item--active')

    expect(
        screen.getByRole('link', { name: 'لوحة التحكم' }),
    ).not.toHaveClass('sidebar-item--active')
  })

  it('should redirect unknown routes to dashboard', async () => {
    render(
      <MemoryRouter initialEntries={['/unknown']}>
        <AppRoutes />
      </MemoryRouter>,
    )

    expect(
      await screen.findByText('الأصناف المفعلة'),
    ).toBeInTheDocument()
  })
})
