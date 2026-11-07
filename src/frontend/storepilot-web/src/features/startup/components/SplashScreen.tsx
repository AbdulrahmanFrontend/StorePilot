import type { StartupState } from '../hooks/useStartup'

type SplashScreenProps = {
  status: StartupState
  onRetry: () => void
}

export default function SplashScreen({
  status,
  onRetry,
}: SplashScreenProps) {
  return (
    <main className="splash-screen">
      <section className="splash-card">

        <div className="brand-mark">
          <span>S</span>
        </div>

        <h1 className="app-name">StorePilot</h1>

        <p className="app-description">
          Smart Store Management
        </p>

        <div className="startup-status" aria-live="polite">

          {status === 'checking' && (
            <>
              <span className="loader" />

              <p>جاري تهيئة النظام...</p>

              <span className="status-hint">
                يرجى الانتظار
              </span>
            </>
          )}

          {status === 'ready' && (
            <>
              <span className="status-icon success">
                ✓
              </span>

              <p>النظام جاهز</p>

              <span className="status-hint">
                تم الاتصال بالخادم بنجاح
              </span>
            </>
          )}

          {status === 'error' && (
            <>
              <span className="status-icon error">
                !
              </span>

              <p>تعذر الاتصال بالنظام</p>

              <span className="status-hint">
                تأكد من تشغيل خدمة النظام ثم حاول مرة أخرى
              </span>

              <button
                className="retry-button"
                onClick={onRetry}
              >
                إعادة المحاولة
              </button>
            </>
          )}

        </div>

        <span className="version-label">
          Version 1.0.0
        </span>

      </section>
    </main>
  )
}
