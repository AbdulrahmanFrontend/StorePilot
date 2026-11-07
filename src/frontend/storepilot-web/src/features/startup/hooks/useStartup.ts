import { useEffect, useState } from 'react'

import {
  checkHealth,
  getSetupStatus,
} from '../api/startupApi'

export type StartupState =
  | 'checking'
  | 'ready'
  | 'error'

export type StartupDestination =
  | 'setup'
  | 'dashboard'

const RETRY_DELAY = 1000
const REQUEST_TIMEOUT = 3000
const MAX_STARTUP_TIME = 15000

export function useStartup() {
  const [status, setStatus] =
    useState<StartupState>('checking')

  const [destination, setDestination] =
    useState<StartupDestination | null>(null)

  const [retryKey, setRetryKey] = useState(0)

  useEffect(() => {
    let isActive = true

    let retryTimer: number | undefined

    let requestController: AbortController | undefined

    const startedAt = Date.now()

    async function requestWithTimeout<T>(
      request: (signal: AbortSignal) => Promise<T>,
    ): Promise<T> {
      const controller = new AbortController()

      requestController = controller

      const timer = window.setTimeout(
        () => controller.abort(),
        REQUEST_TIMEOUT,
      )

      try {
        return await request(controller.signal)
      } finally {
        window.clearTimeout(timer)

        if (requestController === controller) {
          requestController = undefined
        }
      }
    }

    async function checkStartup() {
      try {
        // المرحلة الأولى: التأكد من جاهزية الـ API.
        const health = await requestWithTimeout(checkHealth)

        if (!isActive) return

        if (health.status !== 'Ready') {
          throw new Error('API is not ready')
        }

        // المرحلة الثانية: معرفة حالة الإعداد الأولي.
        const setup = await requestWithTimeout(getSetupStatus)

        if (!isActive) return

        setDestination(
          setup.isSetupCompleted ? 'dashboard' : 'setup',
        )

        setStatus('ready')
      } catch {
        if (!isActive) return

        const elapsedTime = Date.now() - startedAt

        if (elapsedTime >= MAX_STARTUP_TIME) {
          setStatus('error')
          return
        }

        retryTimer = window.setTimeout(
          checkStartup,
          RETRY_DELAY,
        )
      }
    }

    void checkStartup()

    return () => {
      isActive = false

      requestController?.abort()

      if (retryTimer !== undefined) {
        window.clearTimeout(retryTimer)
      }
    }
  }, [retryKey])

  function retry() {
    setStatus('checking')
    setDestination(null)
    setRetryKey((current) => current + 1)
  }

  return {
    status,
    destination,
    retry,
  }
}
