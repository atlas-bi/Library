import type { ReportDetail } from "@/lib/reports/types"

function QueryBlock({
  query,
}: {
  query: {
    id: number
    name?: string | null
    language?: string | null
    source?: string | null
    sourceServer?: string | null
  }
}) {
  return (
    <details className="rounded-md border border-[var(--atlas-home-border-soft)] bg-white">
      <summary className="cursor-pointer px-4 py-3 font-medium">
        {query.name ?? `Query ${query.id}`}
      </summary>
      <div className="space-y-2 border-t border-[var(--atlas-home-border-soft)] px-4 py-3 text-sm text-muted-foreground">
        {query.language ? <div>Language: {query.language}</div> : null}
        {query.sourceServer ? <div>Server: {query.sourceServer}</div> : null}
        {query.source ? (
          <pre className="overflow-x-auto rounded-md bg-muted/40 p-3 text-xs text-foreground">
            {query.source}
          </pre>
        ) : null}
      </div>
    </details>
  )
}

export function ReportQuerySection({ report }: { report: ReportDetail }) {
  const queries = [...(report.queries ?? []), ...(report.componentQueries ?? [])]
  if (queries.length === 0) return null

  return (
    <section id="query" className="space-y-4 scroll-mt-24">
      <h2 className="text-2xl font-semibold text-[var(--atlas-home-text-strong)]">Query</h2>
      <div className="space-y-3">
        {queries.map((query) => (
          <QueryBlock key={query.id} query={query} />
        ))}
      </div>
    </section>
  )
}
