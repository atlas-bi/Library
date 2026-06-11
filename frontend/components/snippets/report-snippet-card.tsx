import { EntityCardFooter } from "@/components/interactions/entity-card-footer"
import { ProfileAnalyticsPanel } from "@/components/profile/profile-analytics-panel"
import type { CollectionFeatureFlagsDto, CollectionReportDto } from "@/lib/collections/types"
import { truncateText } from "@/lib/text"
import { SnippetMediaCard } from "./snippet-media-card"

export function ReportSnippetCard({
  report,
  features,
}: {
  report: CollectionReportDto
  features?: CollectionFeatureFlagsDto | null
}) {
  const title = report.name?.trim() || `Report ${report.id}`
  const href = `/reports?id=${report.id}`
  const excerpt = report.description?.trim()
    ? truncateText(report.description)
    : "Open to view details."
  const tags = ["report", ...(report.canRun ? ["can run"] : [])]
  const canRequestAccess = (report.attachmentCount ?? 0) > 0 || !!report.canRun

  return (
    <SnippetMediaCard
      title={title}
      href={href}
      tags={tags}
      excerpt={
        <>
          {excerpt} <span className="text-link font-medium">read more</span>
        </>
      }
      footer={
        <EntityCardFooter
          entityType="report"
          id={report.id}
          title={title}
          href={href}
          isStarred={report.isStarred}
          starCount={report.starCount}
          features={features ?? undefined}
          canRequestAccess={canRequestAccess}
          profilePanel={<ProfileAnalyticsPanel id={report.id} type="report" />}
        />
      }
    />
  )
}
