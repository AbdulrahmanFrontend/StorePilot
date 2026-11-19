import { useEffect, useState } from 'react'
import { useStartup } from './features/startup/hooks/useStartup'
import SplashScreen from './features/startup/components/SplashScreen'
import SetupPage from './features/settings/pages/SetupPage'

import './App.css'

// الشاشات الممكن عرضها في التطبيق.
type AppScreen = 'splash' | 'setup' | 'dashboard'

const MIN_SPLASH_DISPLAY_TIME = 1000

function App() {
  const {
    status,
    destination,
    retry,
  } = useStartup()

  const [screen, setScreen] =
    useState<AppScreen>('splash')

  useEffect(() => {
    // لا ننتقل حتى ينتهي فحص بدء التشغيل.
    if (status !== 'ready' || destination === null) {
      return
    }

    // نترك شاشة Splash ظاهرة قليلًا بعد الجاهزية.
    const timer = window.setTimeout(() => {
      setScreen(destination)
    }, MIN_SPLASH_DISPLAY_TIME)

    return () => {
      window.clearTimeout(timer)
    }
  }, [status, destination])

  // عرض Splash أثناء الفحص أو عند حدوث خطأ.
  if (screen === 'splash') {
    return (
      <SplashScreen
        status={status}
        onRetry={retry}
      />
    )
  }

  // شاشة مؤقتة حتى ننفذ الإعداد الأولي.
  if (screen === 'setup') {
    return (
      <SetupPage
        onCompleted={() => setScreen('dashboard')}
      />
    )
  }

  // شاشة مؤقتة حتى ننفذ Dashboard.
  return (
    <main>
      <h1>StorePilot Dashboard</h1>
      <p>ستظهر هنا الشاشة الرئيسية للنظام.</p>
    </main>
  )
}

export default App
