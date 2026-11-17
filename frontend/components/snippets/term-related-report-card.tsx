import { EntityCardFooter } from "@/components/interactions/entity-card-footer"
import { ReportSnippetRunAction } from "@/components/interactions/report-snippet-run-action"
import { ProfileAnalyticsPanel } from "@/components/profile/profile-analytics-panel"
import type { TermFeaturesDto, TermRelatedReportDto } from "@/lib/terms/types"
import { truncateText } from "@/lib/text"
import { SnippetMediaCard } from "./snippet-media-card"

export function TermRelatedReportCard({
  report,
  features,
}: {
  report: TermRelatedReportDto
  features?: TermFeaturesDto
}) {
  const href = report.url ?? `/reports?id=${report.id}`
  const title = report.name?.trim() || `Report ${report.id}`
  const excerpt = report.bodyText?.trim()
    ? truncateText(report.bodyText)
    : report.description?.trim()
      ? truncateText(report.description)
      : "Open to view details."
  const tags = report.type ? [report.type] : ["report"]
  const canRequestAccess = (report.attachmentCount ?? 0) > 0 || !!report.canRun

  return (
    <SnippetMediaCard
      title={title}
      href={href}
      tags={tags}
      showCertified={report.isCertified}
      headerLeading={
        <ReportSnippetRunAction
          reportId={report.id}
          canRun={report.canRun}
          attachmentCount={report.attachmentCount}
        />
      }
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
          features={features}
          canRequestAccess={canRequestAccess}
          profilePanel={<ProfileAnalyticsPanel id={report.id} type="report" />}
        />
      }
    />
  )
}
