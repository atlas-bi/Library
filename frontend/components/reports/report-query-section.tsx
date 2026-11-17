"use client"

import Link from "next/link"
import type { ReportDetail, ReportQueryItem } from "@/lib/reports/types"

function queryText(query: ReportQueryItem): string {
  return query.query?.trim() || query.source?.trim() || ""
}

async function copyQueryText(text: string) {
  if (!text) return
  await navigator.clipboard.writeText(text)
}

function QueryCode({ text }: { text: string }) {
  const lines = text.split("\n")

  return (
    <div className="overflow-x-auto rounded-md bg-muted/40 text-xs text-foreground">
      <ol aria-label="Query lines" className="py-2">
        {lines.map((line, index) => (
          <li key={`${index}-${line}`} className="grid grid-cols-[2.5rem_minmax(0,1fr)]">
            <span
              aria-hidden
              className="select-none border-r border-[var(--atlas-home-border-soft)] px-2 text-right font-mono text-muted-foreground"
            >
              {index + 1}
            </span>
            <code className="px-3 font-mono whitespace-pre">{line.length > 0 ? line : " "}</code>
          </li>
        ))}
      </ol>
    </div>
  )
}

function PrimaryQueryBlock({
  query,
  index,
}: {
  query: ReportQueryItem
  index: number
}) {
  const text = queryText(query)

  return (
    <div className="space-y-3 rounded-md border border-[var(--atlas-home-border-soft)] bg-white p-4">
      <div className="flex flex-wrap items-center justify-between gap-2">
        <h3 className="text-lg font-semibold">
          Query #{index + 1} | {query.name ?? `Query ${query.id}`}
        </h3>
        {text ? (
          <button
            type="button"
            className="rounded-md border border-[var(--atlas-home-border-soft)] px-3 py-1.5 text-sm hover:bg-muted/40"
            onClick={() => void copyQueryText(text)}
          >
            Copy Query
          </button>
        ) : null}
      </div>
      {query.language ? <div className="text-sm text-muted-foreground">Language: {query.language}</div> : null}
      {query.sourceServer ? <div className="text-sm text-muted-foreground">Server: {query.sourceServer}</div> : null}
      {text ? <QueryCode text={text} /> : null}
    </div>
  )
}

function ComponentQueryBlock({ query }: { query: ReportQueryItem }) {
  const text = queryText(query)
  const reportId = query.reportObjectId ?? query.id
  const titleSuffix =
    query.epicMasterFile || query.epicRecordId != null
      ? ` (${[query.epicMasterFile, query.epicRecordId].filter(Boolean).join(" ")})`
      : ""

  return (
    <div className="space-y-3 rounded-md border border-[var(--atlas-home-border-soft)] bg-white p-4">
      <div className="flex flex-wrap items-center justify-between gap-2">
        <h3 className="text-lg font-semibold">
          <Link href={`/reports?id=${reportId}`} className="text-link hover:underline">
            {query.name ?? `Query ${query.id}`}
            {titleSuffix}
          </Link>
        </h3>
        {text ? (
          <button
            type="button"
            className="rounded-md border border-[var(--atlas-home-border-soft)] px-3 py-1.5 text-sm hover:bg-muted/40"
            onClick={() => void copyQueryText(text)}
          >
            Copy Query
          </button>
        ) : null}
      </div>
      {query.description?.trim() ? <p className="text-sm text-muted-foreground">{query.description}</p> : null}
      {text ? <QueryCode text={text} /> : null}
    </div>
  )
}

export function ReportQuerySection({ report }: { report: ReportDetail }) {
  const queries = report.queries ?? []
  const componentQueries = report.componentQueries ?? []
  if (queries.length + componentQueries.length === 0) return null

  return (
    <section id="query" className="space-y-4 scroll-mt-24">
      <h2 className="text-2xl font-semibold text-[var(--atlas-home-text-strong)]">Query</h2>
      <div className="space-y-4">
        {queries.map((query, index) => (
          <PrimaryQueryBlock key={query.id} query={query} index={index} />
        ))}
        {componentQueries.map((query) => (
          <ComponentQueryBlock key={`component-${query.id}-${query.reportObjectId ?? 0}`} query={query} />
        ))}
      </div>
    </section>
  )
}
