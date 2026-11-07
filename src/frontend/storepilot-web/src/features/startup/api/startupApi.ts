import { get } from '../../../shared/api/apiClient'

export type HealthResponse = {
  status: string
}

export type SetupStatusResponse = {
  isSetupCompleted: boolean
}

export function checkHealth(signal?: AbortSignal) {
  return get<HealthResponse>('/system/health', { signal })
}

export function getSetupStatus(signal?: AbortSignal) {
  return get<SetupStatusResponse>(
    '/system/setup-status',
    { signal },
  )
}
