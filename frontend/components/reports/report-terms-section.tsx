import { TermSnippetCard } from "@/components/snippets/term-snippet-card"
import type { ReportDetail } from "@/lib/reports/types"

export function ReportTermsSection({ report }: { report: ReportDetail }) {
  if (report.features?.termsEnabled === false || !report.terms || report.terms.length === 0) {
    return null
  }

  return (
    <section id="terms" className="space-y-4 scroll-mt-24">
      <h2 className="text-2xl font-semibold text-[var(--atlas-home-text-strong)]">Terms</h2>
      <div className="grid gap-4 md:grid-cols-2">
        {report.terms.map((term) => (
          <TermSnippetCard
            key={term.id}
            term={{
              id: term.id,
              termId: term.id,
              name: term.name,
              summary: term.summary,
            }}
          />
        ))}
      </div>
    </section>
  )
}
