import {
  Navigate,
  Route,
  Routes,
} from 'react-router-dom'

import DashboardPage from './features/dashboard/pages/DashboardPage'
import SectionPlaceholderPage from './features/layout/pages/SectionPlaceholderPage'

function AppRoutes() {
  return (
    <Routes>
      <Route
        path="/"
        element={<Navigate to="/dashboard" replace />}
      />

      <Route
        path="/dashboard"
        element={<DashboardPage />}
      />

      <Route
        path="/products"
        element={
          <SectionPlaceholderPage
            title="الأصناف"
            description="إدارة أصناف المتجر."
          />
        }
      />

      <Route
        path="/groups"
        element={
          <SectionPlaceholderPage
            title="المجموعات"
            description="إدارة مجموعات الأصناف."
          />
        }
      />

      <Route
        path="/units"
        element={
          <SectionPlaceholderPage
            title="الوحدات"
            description="إدارة وحدات القياس."
          />
        }
      />

      <Route
        path="/customers"
        element={
          <SectionPlaceholderPage
            title="العملاء"
            description="إدارة بيانات العملاء."
          />
        }
      />

      <Route
        path="/suppliers"
        element={
          <SectionPlaceholderPage
            title="الموردون"
            description="إدارة بيانات الموردين."
          />
        }
      />

      <Route
        path="/sales"
        element={
          <SectionPlaceholderPage
            title="المبيعات"
            description="إدارة عمليات البيع."
          />
        }
      />

      <Route
        path="/purchases"
        element={
          <SectionPlaceholderPage
            title="المشتريات"
            description="إدارة عمليات الشراء."
          />
        }
      />

      <Route
        path="*"
        element={<Navigate to="/dashboard" replace />}
      />
    </Routes>
  )
}

export default AppRoutes
