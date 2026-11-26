import type { ReactNode } from "react"
import { ReportSnippetCard } from "@/components/snippets/report-snippet-card"
import { searchResultToCollectionReport } from "@/lib/search/search-report-snippet"
import type { SearchResultDto } from "@/lib/search/types"

export function SearchReportResultCard({
  result,
  descriptionContent,
}: {
  result: SearchResultDto
  descriptionContent?: ReactNode
}) {
  const report = searchResultToCollectionReport(result)

  return (
    <ReportSnippetCard
      report={report}
      descriptionContent={descriptionContent}
    />
  )
}