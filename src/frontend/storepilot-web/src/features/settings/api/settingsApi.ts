
import {
  get,
  put,
  postForm,
  remove,
} from '../../../shared/api/apiClient'

import type {
  StoreSettings,
  UpdateStoreSettingsRequest,
} from '../types/settings'

const SETTINGS_ENDPOINT = '/system/settings'
const LOGO_ENDPOINT = '/system/settings/logo'

// Retrieve the current store settings.
export function getStoreSettings(): Promise<StoreSettings> {
  return get<StoreSettings>(SETTINGS_ENDPOINT)
}

// Update the store name and owner name.
export function updateStoreSettings(
  data: UpdateStoreSettingsRequest,
): Promise<StoreSettings> {
  return put<StoreSettings>(SETTINGS_ENDPOINT, data)
}

// Upload a new store logo.
export function uploadStoreLogo(file: File): Promise<void> {
  const formData = new FormData()

  formData.append('file', file)

  return postForm<void>(LOGO_ENDPOINT, formData)
}

// Delete the current store logo.
export function deleteStoreLogo(): Promise<void> {
  return remove<void>(LOGO_ENDPOINT)
}
