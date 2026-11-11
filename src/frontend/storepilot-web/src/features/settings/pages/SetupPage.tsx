import { useEffect, useState } from 'react'
import type { ChangeEvent, SubmitEvent } from 'react'

import {
  getStoreSettings,
  updateStoreSettings,
  uploadStoreLogo,
} from '../api/settingsApi'

import { ApiError } from '../../../shared/api/apiClient'

import type { StoreSettings } from '../types/settings'

import './setup-page.css'

type SetupPageProps = {
  onCompleted: () => void
}

function SetupPage({ onCompleted }: SetupPageProps) {
  // Store settings form values.
  const [shopName, setShopName] = useState('')
  const [ownerName, setOwnerName] = useState('')

  // Selected logo file and its existing server URL.
  const [logoFile, setLogoFile] = useState<File | null>(null)
  const [existingLogoUrl, setExistingLogoUrl] = useState<string | null>(null)

  // Loading and saving states.
  const [isLoading, setIsLoading] = useState(true)
  const [isSaving, setIsSaving] = useState(false)

  // UI feedback states.
  const [error, setError] = useState<string | null>(null)
  const [message, setMessage] = useState<string | null>(null)

  // Temporary URL used to preview the selected logo.
  const [previewUrl, setPreviewUrl] = useState<string | null>(null)

  // Used to retry loading settings after a failed request.
  const [loadRetryKey, setLoadRetryKey] = useState(0)

  // Release the temporary URL when it changes or the component unmounts.
  useEffect(() => {
    if (!previewUrl) {
      return
    }

    return () => {
      URL.revokeObjectURL(previewUrl)
    }
  }, [previewUrl])

  // Load the current store settings when the page mounts.
  useEffect(() => {
    let isActive = true

    async function loadSettings() {
      try {
        setIsLoading(true)
        setError(null)

        const settings: StoreSettings = await getStoreSettings()

        if (!isActive) return

        setShopName(settings.shopName)
        setOwnerName(settings.ownerName)
        setExistingLogoUrl(settings.logoUrl)
      } catch (err) {
        if (!isActive) return

        setError(
          err instanceof Error
            ? err.message
            : 'Unable to load store settings.',
        )
      } finally {
        if (isActive) {
          setIsLoading(false)
        }
      }
    }

    void loadSettings()

    return () => {
      isActive = false
    }
  }, [loadRetryKey])

  // Handle the selected logo file.
  function handleLogoChange(event: ChangeEvent<HTMLInputElement>) {
    const file = event.currentTarget.files?.[0] ?? null

    setLogoFile(file)
    setPreviewUrl(file ? URL.createObjectURL(file) : null)

    setError(null)
    setMessage(null)
  }

  // Validate the form before sending any requests.
  function validateForm(): string | null {
    if (!shopName.trim()) {
      return 'من فضلك أدخل اسم اسم المحل.'
    }

    if (!ownerName.trim()) {
      return 'من فضلك ادخل اسم مالك المحل.'
    }

    if (shopName.trim().length > 100) {
      return 'اسم المحل لا ينبغي أن يتعدى ال 100 حرف.'
    }

    if (ownerName.trim().length > 100) {
      return 'اسم مالك المحل لا ينبغي أن يتعدى ال 100 حرف.'
    }

    if (logoFile) {
      const allowedTypes = ['image/png', 'image/jpeg', 'image/webp']

      if (!allowedTypes.includes(logoFile.type)) {
        return 'من فضلك اختر صورا من نوع png, jpeg, webp.'
      }

      if (logoFile.size > 2 * 1024 * 1024) {
        return 'حجم اللوجو لا ينبغي أن يتعدى ال 2 MB.'
      }
    }

    return null
  }

  // Handle form submission.
  async function handleSubmit(event: SubmitEvent<HTMLFormElement>) {
    event.preventDefault()

    setError(null)
    setMessage(null)

    const validationError = validateForm()

    if (validationError) {
      setError(validationError)
      return
    }

    try {
      setIsSaving(true)

      // Step 1: Save the store information.
      await updateStoreSettings({
        shopName: shopName.trim(),
        ownerName: ownerName.trim(),
      })

      // Step 2: Upload the logo only when a new file was selected.
      if (logoFile) {
        try {
          await uploadStoreLogo(logoFile)
        } catch (err) {
          setMessage(
            'Store information was saved, but the logo could not be uploaded.',
          )

          setError(
            err instanceof Error
              ? err.message
              : 'Logo upload failed.',
          )

          return
        }
      }

      // Notify the parent component that setup has completed.
      onCompleted()
    } catch (err) {
      if (err instanceof ApiError) {
        setError(err.message)
      } else {
        setError(
          err instanceof Error
            ? err.message
            : 'An unexpected error occurred.',
        )
      }
    } finally {
      setIsSaving(false)
    }
  }

  // Prefer the newly selected logo over the existing server logo.
  const displayedLogoUrl = previewUrl ?? existingLogoUrl

  if (isLoading) {
    return (
      <main className="setup-page" dir="rtl">
        <div className="setup-card">
          <p className="setup-loading">جاري تحميل إعدادات المحل...</p>
        </div>
      </main>
    )
  }

  return (
    <main className="setup-page" dir="rtl">
      <section className="setup-card">
        <header className="setup-header">
          <div className="setup-brand-mark">S</div>

          <h1>إعداد المحل</h1>

          <p>
            أدخل البيانات الأساسية للمحل للبدء باستخدام StorePilot.
          </p>
        </header>

        {error && !existingLogoUrl && !shopName && !ownerName && (
          <div className="setup-error" role="alert">
            {error}

            <button
              type="button"
              className="setup-retry"
              onClick={() => setLoadRetryKey(current => current + 1)}
            >
              إعادة المحاولة
            </button>
          </div>
        )}

        <form onSubmit={handleSubmit} className="setup-form">
          <div className="form-field">
            <label htmlFor="shopName">
              اسم المحل
            </label>

            <input
              id="shopName"
              type="text"
              value={shopName}
              onChange={event => setShopName(event.target.value)}
              placeholder="أدخل اسم المحل"
              maxLength={100}
              required
              disabled={isSaving}
            />
          </div>

          <div className="form-field">
            <label htmlFor="ownerName">
              اسم صاحب المحل
            </label>

            <input
              id="ownerName"
              type="text"
              value={ownerName}
              onChange={event => setOwnerName(event.target.value)}
              placeholder="أدخل اسم صاحب المحل"
              maxLength={100}
              required
              disabled={isSaving}
            />
          </div>

          <div className="form-field">
            <label htmlFor="storeLogo">
              شعار المحل
              <span className="optional-label"> (اختياري)</span>
            </label>

            <input
              id="storeLogo"
              type="file"
              accept="image/png,image/jpeg,image/webp"
              onChange={handleLogoChange}
              disabled={isSaving}
            />

            <p className="field-hint">
              PNG أو JPG أو WebP، بحد أقصى 2 MB.
            </p>
          </div>

          <div className="logo-preview">
            {displayedLogoUrl ? (
              <img
                src={displayedLogoUrl}
                alt="Store logo preview"
              />
            ) : (
              <div className="logo-placeholder">
                لا يوجد شعار
              </div>
            )}
          </div>

          {message && (
            <div className="setup-message">
              {message}
            </div>
          )}

          {error && (
            <div className="setup-error" role="alert">
              {error}
            </div>
          )}

          <button
            type="submit"
            className="setup-submit"
            disabled={isSaving}
          >
            {isSaving ? 'جاري الحفظ...' : 'حفظ ومتابعة'}
          </button>
        </form>

        <footer className="setup-footer">
          StorePilot · Version 1.0.0
        </footer>
      </section>
    </main>
  )
}

export default SetupPage
