import type { CollectionReportDto } from "@/lib/collections/types"
import type { SearchResultDto } from "@/lib/search/types"

/** Maps Solr search hits to the collection report shape used by `ReportSnippetCard`. */
export function searchResultToCollectionReport(result: SearchResultDto): CollectionReportDto {
  const runUrl = result.url?.trim() || null
  const epicMasterFile = result.epicMasterFile?.trim() || null

  return {
    id: result.atlasId,
    name: result.name,
    description: result.description,
    isStarred: result.isStarred ?? undefined,
    epicMasterFile,
    runUrl,
    canRun: Boolean(runUrl),
    attachmentCount: 0,
  }
}