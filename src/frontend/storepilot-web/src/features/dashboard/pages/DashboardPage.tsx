import StatCard from '../components/StatCard'
import { useDashboardSummary } from '../hooks/useDashboardSummary'

import AppLayout from '../../layout/AppLayout'

import '../styles/dashboard.css'

function DashboardPage() {
  const dashboard = useDashboardSummary()

  return (
    <AppLayout
      title="لوحة التحكم"
      description="نظرة عامة على بيانات النظام."
    >
      {dashboard.status === 'loading' && (
        <p role="status">
          جاري تحميل بيانات لوحة التحكم...
        </p>
      )}

      {dashboard.status === 'error' && (
        <div role="alert">
          <p>{dashboard.message}</p>

          <button type="button" onClick={dashboard.retry}>
            إعادة المحاولة
          </button>
        </div>
      )}

      {dashboard.status === 'success' && (
        <section
          className="dashboard__stats"
          aria-label="إحصائيات النظام"
        >
          <StatCard
            title="الأصناف المفعلة"
            value={dashboard.data.activeProductsCount}
            description="إجمالي الأصناف النشطة في النظام"
            icon="📦"
          />
        </section>
      )}
    </AppLayout>
  )
}

export default DashboardPage
