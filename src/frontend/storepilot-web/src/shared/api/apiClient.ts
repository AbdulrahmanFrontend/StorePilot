const API_BASE_URL = '/api'

// إعدادات إضافية يمكن تمريرها للطلب.
type RequestOptions = {
  signal?: AbortSignal
}

export async function get<T>(
  endpoint: string,
  options?: RequestOptions,
): Promise<T> {
  const response = await fetch(`${API_BASE_URL}${endpoint}`, {
    method: 'GET',
    headers: {
      Accept: 'application/json',
    },
    signal: options?.signal,
  })

  if (!response.ok) {
    throw new Error(`HTTP Error: ${response.status}`)
  }

  return (await response.json()) as T
}
