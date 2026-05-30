import Link from "next/link"
import { Badge } from "@/components/ui/badge"
import { Card, CardContent, CardFooter, CardHeader } from "@/components/ui/card"
import type { CollectionReportDto } from "@/lib/collections/types"
import { truncateText } from "@/lib/text"

export function ReportSnippetCard({ report }: { report: CollectionReportDto }) {
  const title = report.name?.trim() || `Report ${report.id}`
  const excerpt = report.description?.trim()
    ? truncateText(report.description)
    : "Open to view details."

  return (
    <Card className="h-full">
      <CardHeader className="gap-2 border-b py-3">
        <div className="flex flex-wrap items-start justify-between gap-2">
          <Link href={`/reports?id=${report.id}`} className="font-medium hover:underline">
            {title}
          </Link>
          <div className="flex flex-wrap gap-1">
            <Badge variant="secondary">report</Badge>
            {report.canRun ? <Badge variant="outline">Can run</Badge> : null}
          </div>
        </div>
      </CardHeader>
      <CardContent className="py-3">
        <Link
          href={`/reports?id=${report.id}`}
          className="text-sm text-muted-foreground hover:underline"
        >
          {excerpt} <span className="text-link">read more</span>
        </Link>
      </CardContent>
      <CardFooter className="border-t py-2 text-xs text-muted-foreground">
        {typeof report.rank === "number" ? `Rank ${report.rank}` : null}
        {report.isStarred ? <span className="ml-auto">Starred</span> : null}
      </CardFooter>
    </Card>
  )
}
