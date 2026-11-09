
const API_BASE_URL = '/api'

type RequestOptions = {
  method?: 'GET' | 'POST' | 'PUT' | 'DELETE'
  body?: unknown
  signal?: AbortSignal
}

export class ApiError extends Error {
  public readonly status: number

  constructor(status: number, message: string) {
    super(message)

    this.status = status
    this.name = 'ApiError'
  }
}

// Extract a useful error message from the API response.
async function getErrorMessage(response: Response): Promise<string> {
  const contentType = response.headers.get('content-type') ?? ''

  try {
    if (
      contentType.includes('application/json') ||
      contentType.includes('application/problem+json')
    ) {
      const data = await response.json()

      if (typeof data.detail === 'string' && data.detail.trim()) {
        return data.detail
      }

      if (typeof data.message === 'string' && data.message.trim()) {
        return data.message
      }

      if (typeof data.title === 'string' && data.title.trim()) {
        return data.title
      }

      if (data.errors && typeof data.errors === 'object') {
        const firstError = Object.values(data.errors)
          .flat()
          .find(value => typeof value === 'string')

        if (firstError) {
          return firstError
        }
      }
    } else {
      const text = await response.text()

      if (text.trim()) {
        return text
      }
    }
  } catch {
    // Ignore response parsing errors and use the fallback message.
  }

  return `Request failed with status code ${response.status}.`
}

// Send an HTTP request and handle its response.
async function request<T>(
  endpoint: string,
  options: RequestOptions = {},
): Promise<T> {
  const method = options.method ?? 'GET'
  const headers = new Headers({
    Accept: 'application/json',
  })

  let body: BodyInit | undefined

  if (options.body instanceof FormData) {
    // Let the browser set the multipart boundary automatically.
    body = options.body
  } else if (options.body !== undefined) {
    headers.set('Content-Type', 'application/json')
    body = JSON.stringify(options.body)
  }

  const response = await fetch(`${API_BASE_URL}${endpoint}`, {
    method,
    headers,
    body,
    signal: options.signal,
  })

  if (!response.ok) {
    const message = await getErrorMessage(response)
    throw new ApiError(response.status, message)
  }

  // DELETE endpoints may return 204 No Content.
  if (response.status === 204) {
    return undefined as T
  }

  // Some successful endpoints may not return a response body.
  if (response.status === 205) {
    return undefined as T
  }

  return (await response.json()) as T
}

export function get<T>(
  endpoint: string,
  signal?: AbortSignal,
): Promise<T> {
  return request<T>(endpoint, {
    method: 'GET',
    signal,
  })
}

export function post<T>(
  endpoint: string,
  body: unknown,
): Promise<T> {
  return request<T>(endpoint, {
    method: 'POST',
    body,
  })
}

export function put<T>(
  endpoint: string,
  body: unknown,
): Promise<T> {
  return request<T>(endpoint, {
    method: 'PUT',
    body,
  })
}

export function postForm<T>(
  endpoint: string,
  formData: FormData,
): Promise<T> {
  return request<T>(endpoint, {
    method: 'POST',
    body: formData,
  })
}

export function remove<T = void>(
  endpoint: string,
): Promise<T> {
  return request<T>(endpoint, {
    method: 'DELETE',
  })
}
