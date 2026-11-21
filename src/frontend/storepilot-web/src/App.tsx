import { useEffect, useState } from 'react'

import { useStartup } from './features/startup/hooks/useStartup'
import SplashScreen from './features/startup/components/SplashScreen'
import SetupPage from './features/settings/pages/SetupPage'
import AppRoutes from './AppRoutes'

import './App.css'

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
    if (status !== 'ready' || destination === null) {
      return
    }

    const timer = window.setTimeout(() => {
      setScreen(destination)
    }, MIN_SPLASH_DISPLAY_TIME)

    return () => {
      window.clearTimeout(timer)
    }
  }, [status, destination])

  if (screen === 'splash') {
    return (
      <SplashScreen
        status={status}
        onRetry={retry}
      />
    )
  }

  if (screen === 'setup') {
    return (
      <SetupPage
        onCompleted={() => setScreen('dashboard')}
      />
    )
  }

  return <AppRoutes />
}

export default App
