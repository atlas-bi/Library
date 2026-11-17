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
  const parameters = report.parameters ?? []

  if (sections.length === 0 && parameters.length === 0) return null

  const showValueColumn = parameters.some((parameter) => Boolean(parameter.value?.trim()))

  return (
    <section id="description" className="space-y-4 scroll-mt-24">
      <h2 className="text-2xl font-semibold text-[var(--atlas-home-text-strong)]">Description</h2>
      {sections.map((section) => (
        <div key={section.title} className="space-y-2">
          <h3 className="text-lg font-semibold">{section.title}</h3>
          <MarkdownContent content={section.content ?? ""} />
        </div>
      ))}
      {parameters.length > 0 ? (
        <div className="space-y-2">
          <h3 className="text-lg font-semibold">Parameters</h3>
          <div className="overflow-x-auto rounded-md border border-[var(--atlas-home-border-soft)]">
            <table className="min-w-full text-sm" aria-label="report parameters">
              <thead>
                <tr className="border-b border-[var(--atlas-home-border-soft)]">
                  <th className="px-3 py-2 text-left font-medium">Name</th>
                  {showValueColumn ? (
                    <th className="px-3 py-2 text-left font-medium">Value</th>
                  ) : null}
                </tr>
              </thead>
              <tbody>
                {parameters.map((parameter) => (
                  <tr key={parameter.id} className="border-b border-[var(--atlas-home-border-soft)] last:border-b-0">
                    <td className="px-3 py-2">{parameter.name}</td>
                    {showValueColumn ? <td className="px-3 py-2">{parameter.value}</td> : null}
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      ) : null}
    </section>
  )
}
