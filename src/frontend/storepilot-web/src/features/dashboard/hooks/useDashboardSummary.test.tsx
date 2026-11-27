import { act, renderHook, waitFor } from '@testing-library/react'
import { beforeEach, describe, expect, it, vi } from 'vitest'

import { getDashboardSummary } from '../api/dashboardApi'
import type { DashboardSummary } from '../types/dashboard'
import { useDashboardSummary } from './useDashboardSummary'

vi.mock('../api/dashboardApi', () => ({
  getDashboardSummary: vi.fn(),
}))

const mockedGetDashboardSummary = vi.mocked(getDashboardSummary)

const summary: DashboardSummary = {
  activeProductsCount: 25,
}

describe('useDashboardSummary', () => {
  beforeEach(() => {
    vi.resetAllMocks()
  })

  it('should load dashboard summary successfully', async () => {
    mockedGetDashboardSummary.mockResolvedValue(summary)

    const { result } = renderHook(() => useDashboardSummary())

    expect(result.current.status).toBe('loading')

    await waitFor(() => {
      expect(result.current.status).toBe('success')
    })

    expect(result.current).toMatchObject({
      status: 'success',
      data: summary,
    })
  })

  it('should expose an error when the request fails', async () => {
    mockedGetDashboardSummary.mockRejectedValue(
      new Error('Server error'),
    )

    const { result } = renderHook(() => useDashboardSummary())

    await waitFor(() => {
      expect(result.current.status).toBe('error')
    })

    expect(result.current).toMatchObject({
      status: 'error',
      message: 'Server error',
    })
  })

  it('should retry the request successfully', async () => {
    mockedGetDashboardSummary
      .mockRejectedValueOnce(new Error('Network error'))
      .mockResolvedValueOnce(summary)

    const { result } = renderHook(() => useDashboardSummary())

    await waitFor(() => {
      expect(result.current.status).toBe('error')
    })

    act(() => {
      result.current.retry()
    })

    await waitFor(() => {
      expect(result.current.status).toBe('success')
    })

    expect(result.current).toMatchObject({
      status: 'success',
      data: summary,
    })

    expect(mockedGetDashboardSummary).toHaveBeenCalledTimes(2)
  })

  it('should abort the request when the hook unmounts', () => {
    let requestSignal: AbortSignal | undefined

    mockedGetDashboardSummary.mockImplementation(signal => {
      requestSignal = signal

      return new Promise<DashboardSummary>(() => {})
    })

    const { unmount } = renderHook(() => useDashboardSummary())

    expect(requestSignal).toBeDefined()
    expect(requestSignal?.aborted).toBe(false)

    unmount()

    expect(requestSignal?.aborted).toBe(true)
  })
})
