import { useCallback, useEffect, useState } from 'react'
import { getDashboardSummary } from '../api/dashboardApi'
import type { DashboardSummary } from '../types/dashboard'

type DashboardSummaryState =
  | { status: 'loading' }
  | { status: 'success'; data: DashboardSummary }
  | { status: 'error'; message: string }

export function useDashboardSummary() {
  const [state, setState] = useState<DashboardSummaryState>({
    status: 'loading',
  })

  const [retryCount, setRetryCount] = useState(0)

  const retry = useCallback(() => {
    setState({ status: 'loading' })
    setRetryCount(current => current + 1)
  }, [])

  useEffect(() => {
    const controller = new AbortController()

    getDashboardSummary(controller.signal)
      .then(data => {
        if (controller.signal.aborted) {
          return
        }

        setState({
          status: 'success',
          data,
        })
      })
      .catch(error => {
        if (controller.signal.aborted) {
          return
        }

        setState({
          status: 'error',
          message:
            error instanceof Error
              ? error.message
              : 'حدث خطأ غير متوقع أثناء تحميل بيانات لوحة التحكم.',
        })
      })

    return () => {
      controller.abort()
    }
  }, [retryCount])

  return {
    ...state,
    retry,
  }
}
