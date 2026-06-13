import type { CollectionReportDto } from "@/lib/collections/types"
import { truncateText } from "@/lib/text"
import { SnippetMediaCard } from "./snippet-media-card"

export function ReportSnippetCard({ report }: { report: CollectionReportDto }) {
  const title = report.name?.trim() || `Report ${report.id}`
  const excerpt = report.description?.trim()
    ? truncateText(report.description)
    : "Open to view details."
  const tags = ["report", ...(report.canRun ? ["can run"] : [])]

  return (
    <SnippetMediaCard
      title={title}
      href={`/reports?id=${report.id}`}
      tags={tags}
      excerpt={
        <>
          {excerpt} <span className="text-link font-medium">read more</span>
        </>
      }
      footer={
        <div className="flex w-full items-center justify-between text-xs text-muted-foreground">
          {typeof report.rank === "number" ? <span>Rank {report.rank}</span> : <span />}
          {report.isStarred ? <span className="font-medium text-amber-600">Starred</span> : null}
        </div>
      }
    />
  )
}
