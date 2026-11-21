interface StatCardProps {
  title: string
  value: number
  description: string
  icon: string
}

function StatCard({
  title,
  value,
  description,
  icon,
}: StatCardProps) {
  return (
    <article className="stat-card">
      <div className="stat-card__header">
        <span className="stat-card__icon" aria-hidden="true">
          {icon}
        </span>

        <span className="stat-card__title">
          {title}
        </span>
      </div>

      <strong className="stat-card__value">
        {value.toLocaleString('ar-EG')}
      </strong>

      <p className="stat-card__description">
        {description}
      </p>
    </article>
  )
}

export default StatCard
