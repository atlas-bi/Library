import { MarkdownContent } from "@/components/content/markdown-content"
import type { ReportDetail } from "@/lib/reports/types"

export function ReportDescriptionSection({ report }: { report: ReportDetail }) {
  const doc = report.document
  const sections = [
    { title: "Developer Description", content: doc?.developerDescription },
    { title: "Key Assumptions", content: doc?.keyAssumptions },
    { title: "System Description", content: report.description },
    { title: "System Detailed Description", content: report.detailedDescription },
    { title: "Repository Description", content: report.repositoryDescription },
  ].filter((section) => Boolean(section.content?.trim()))

  if (sections.length === 0) return null

  return (
    <section id="description" className="space-y-4 scroll-mt-24">
      <h2 className="text-2xl font-semibold text-[var(--atlas-home-text-strong)]">Description</h2>
      {sections.map((section) => (
        <div key={section.title} className="space-y-2">
          <h3 className="text-lg font-semibold">{section.title}</h3>
          <MarkdownContent content={section.content ?? ""} />
        </div>
      ))}
    </section>
  )
}
