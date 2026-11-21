import type { ReactNode } from 'react'

import AppHeader from './components/AppHeader'
import AppSidebar from './components/AppSidebar'
import './styles/layout.css'

interface AppLayoutProps {
  children: ReactNode
  title: string
  description?: string
}

function AppLayout({
  children,
  title,
  description,
}: AppLayoutProps) {
  return (
    <div className="app-layout">
      <AppSidebar />

      <div className="app-layout__content">
        <AppHeader
          title={title}
          description={description}
        />

        <main className="app-layout__main">
          {children}
        </main>
      </div>
    </div>
  )
}

export default AppLayout
