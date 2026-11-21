import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { beforeEach, describe, expect, it, vi } from 'vitest'
import { MemoryRouter } from 'react-router-dom'

import { getDashboardSummary } from '../api/dashboardApi'
import type { DashboardSummary } from '../types/dashboard'
import DashboardPage from './DashboardPage'

vi.mock('../api/dashboardApi', () => ({
  getDashboardSummary: vi.fn(),
}))

const mockedGetDashboardSummary = vi.mocked(getDashboardSummary)

const summary: DashboardSummary = {
  activeProductsCount: 25,
  activeGroupsCount: 8,
  activeUnitsCount: 4,
}

describe('DashboardPage', () => {
  beforeEach(() => {
    vi.resetAllMocks()
  })

  it('should display a loading state while fetching dashboard data', () => {
    mockedGetDashboardSummary.mockImplementation(
      () => new Promise<DashboardSummary>(() => {}),
    )

    render (
        <MemoryRouter initialEntries={['/dashboard']}>
            <DashboardPage />
        </MemoryRouter>,
    )

    expect(screen.getByRole('status')).toHaveTextContent(
      'جاري تحميل بيانات لوحة التحكم...',
    )
  })

  it('should display dashboard statistics after successful loading', async () => {
    mockedGetDashboardSummary.mockResolvedValue(summary)

    render (
        <MemoryRouter initialEntries={['/dashboard']}>
            <DashboardPage />
        </MemoryRouter>,
    )

    expect(
      await screen.findByText('الأصناف المفعلة'),
    ).toBeInTheDocument()

    expect(screen.getByText('المجموعات المفعلة')).toBeInTheDocument()
    expect(screen.getByText('الوحدات المفعلة')).toBeInTheDocument()

    const productsCard = screen
      .getByText('الأصناف المفعلة')
      .closest('article')

    expect(productsCard).not.toBeNull()

    expect(productsCard).toHaveTextContent(
      summary.activeProductsCount.toLocaleString('ar-EG'),
    )
  })

  it('should display an error and retry loading when requested', async () => {
    const user = userEvent.setup()

    mockedGetDashboardSummary
      .mockRejectedValueOnce(new Error('Network error'))
      .mockResolvedValueOnce(summary)

    render (
        <MemoryRouter initialEntries={['/dashboard']}>
            <DashboardPage />
        </MemoryRouter>,
    )

    expect(await screen.findByRole('alert')).toHaveTextContent(
      'Network error',
    )

    await user.click(
      screen.getByRole('button', { name: 'إعادة المحاولة' }),
    )

    expect(
      await screen.findByText('الأصناف المفعلة'),
    ).toBeInTheDocument()

    expect(mockedGetDashboardSummary).toHaveBeenCalledTimes(2)
  })
})
