import type { CollectionListItemDto, CollectionReportDto } from "@/lib/collections/types"
import type { ReportDetail, ReportLinkSummary } from "@/lib/reports/types"

export function toCollectionReportSnippet(link: ReportLinkSummary): CollectionReportDto {
  return {
    id: link.id,
    name: link.displayTitle ?? link.name,
    description: link.description,
    attachmentCount: link.attachmentCount ?? 0,
    canRun: link.canRun,
    epicMasterFile: link.epicMasterFile,
  }
}

export function toCollectionListItem(
  collection: NonNullable<ReportDetail["collections"]>[number],
): CollectionListItemDto {
  return {
    id: collection.id,
    name: collection.name ?? `Collection ${collection.id}`,
    description: collection.description,
    isStarred: collection.isStarred,
    starCount: collection.starCount,
  }
}
