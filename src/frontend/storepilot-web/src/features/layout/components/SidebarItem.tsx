import { NavLink } from 'react-router-dom'

interface SidebarItemProps {
  label: string
  icon: string
  to: string
}

function SidebarItem({
  label,
  icon,
  to,
}: SidebarItemProps) {
  return (
    <NavLink
      to={to}
      end
      className={({ isActive }) =>
        `sidebar-item ${
          isActive ? 'sidebar-item--active' : ''
        }`
      }
    >
      <span
        className="sidebar-item__icon"
        aria-hidden="true"
      >
        {icon}
      </span>

      <span className="sidebar-item__label">
        {label}
      </span>
    </NavLink>
  )
}

export default SidebarItem
