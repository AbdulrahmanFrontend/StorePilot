import SidebarItem from './SidebarItem'

function AppSidebar() {
  return (
    <aside className="app-sidebar">
      <div className="app-sidebar__brand">
        <h2 className="app-sidebar__title">
          StorePilot
        </h2>

        <span className="app-sidebar__subtitle">
          نظام إدارة المتجر
        </span>
      </div>

      <nav
        className="app-sidebar__navigation"
        aria-label="القائمة الرئيسية"
      >
        <SidebarItem
          label="لوحة التحكم"
          icon="📊"
          to="/dashboard"
        />

        <SidebarItem
          label="الأصناف"
          icon="📦"
          to="/products"
        />

        <SidebarItem
          label="المجموعات"
          icon="🗂️"
          to="/groups"
        />

        <SidebarItem
          label="الوحدات"
          icon="📏"
          to="/units"
        />

        <SidebarItem
          label="العملاء"
          icon="👥"
          to="/customers"
        />

        <SidebarItem
          label="الموردون"
          icon="🏢"
          to="/suppliers"
        />

        <SidebarItem
          label="المبيعات"
          icon="🛒"
          to="/sales"
        />

        <SidebarItem
          label="المشتريات"
          icon="🧾"
          to="/purchases"
        />
      </nav>
    </aside>
  )
}

export default AppSidebar
