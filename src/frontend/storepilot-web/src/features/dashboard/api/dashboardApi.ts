import { get } from '../../../shared/api/apiClient'
import type { DashboardSummary } from '../types/dashboard'

export function getDashboardSummary(
  signal?: AbortSignal,
): Promise<DashboardSummary> {
  return get<DashboardSummary>('/dashboard/summary', signal)
}
