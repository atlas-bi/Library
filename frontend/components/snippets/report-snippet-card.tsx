import { EntityCardFooter } from "@/components/interactions/entity-card-footer"
import { ReportSnippetRunAction } from "@/components/interactions/report-snippet-run-action"
import { ProfileAnalyticsPanel } from "@/components/profile/profile-analytics-panel"
import type { CollectionFeatureFlagsDto, CollectionReportDto } from "@/lib/collections/types"
import { getReportImagePlaceholderPath, getReportThumbnailPath } from "@/lib/reports/report-image-url"
import { truncateText } from "@/lib/text"
import { SnippetMediaCard } from "./snippet-media-card"

export function ReportSnippetCard({
  report,
  features,
  descriptionContent,
}: {
  report: CollectionReportDto
  features?: CollectionFeatureFlagsDto | null
  /** When set (e.g. search highlights), replaces the default description excerpt. */
  descriptionContent?: React.ReactNode
}) {
  const title = report.name?.trim() || `Report ${report.id}`
  const href = `/reports?id=${report.id}`
  const defaultExcerpt = report.description?.trim()
    ? truncateText(report.description)
    : "Open to view details."
  const tags = ["report", ...(report.canRun ? ["can run"] : [])]
  const canRequestAccess = (report.attachmentCount ?? 0) > 0 || !!report.canRun

  return (
    <SnippetMediaCard
      title={title}
      href={href}
      tags={tags}
      headerLeading={
        <ReportSnippetRunAction
          reportId={report.id}
          canRun={report.canRun}
          attachmentCount={report.attachmentCount}
          runUrl={report.runUrl}
          epicMasterFile={report.epicMasterFile}
          editReportUrl={report.editReportUrl}
        />
      }
      imageSrc={getReportThumbnailPath(report.id, { size: "128x128" })}
      imageFallbackSrc={getReportImagePlaceholderPath({ size: "128x128" })}
      imageAlt={`${title} thumbnail`}
      excerpt={
        descriptionContent ?? (
          <>
            {defaultExcerpt} <span className="text-link font-medium">read more</span>
          </>
        )
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
