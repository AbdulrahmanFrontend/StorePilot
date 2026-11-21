import AppLayout from '../AppLayout'

interface SectionPlaceholderPageProps {
  title: string
  description: string
}

function SectionPlaceholderPage({
  title,
  description,
}: SectionPlaceholderPageProps) {
  return (
    <AppLayout
      title={title}
      description={description}
    >
      <section className="section-placeholder">
        <h2 className="section-placeholder__title">
          {title}
        </h2>

        <p className="section-placeholder__message">
          هذا القسم قيد التطوير وسيتم تنفيذه لاحقًا.
        </p>
      </section>
    </AppLayout>
  )
}

export default SectionPlaceholderPage
