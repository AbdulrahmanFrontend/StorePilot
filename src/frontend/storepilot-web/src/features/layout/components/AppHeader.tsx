interface AppHeaderProps {
  title: string
  description?: string
}

function AppHeader({
  title,
  description,
}: AppHeaderProps) {
  return (
    <header className="app-header">
      <div className="app-header__text">
        <h1 className="app-header__title">
          {title}
        </h1>

        {description && (
          <p className="app-header__description">
            {description}
          </p>
        )}
      </div>
    </header>
  )
}

export default AppHeader
