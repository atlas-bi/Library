"use client"

import { ArrowUpRightFromSquare, CirclePlay, Pencil, Settings } from "lucide-react"
import type { ReactNode } from "react"
import {
  ActionRail,
  RailExternalLink,
  RailIconLink,
  RailTooltipButton,
} from "@/components/interactions/action-rail"
import { EntityFeedbackDialog } from "@/components/interactions/entity-feedback-dialog"
import { EntityProfileSheet } from "@/components/interactions/entity-profile-sheet"
import { ReportRunDialog } from "@/components/interactions/report-run-dialog"
import { RequestAccessDialog } from "@/components/interactions/request-access-dialog"
import { ShareMailDialog } from "@/components/interactions/share-mail-dialog"
import { StarToggleButton } from "@/components/interactions/star-toggle-button"
import { isInteractionFeatureEnabled } from "@/lib/interactions/features"
import type { ReportDetail } from "@/lib/reports/types"

function resolveDisabledRunLabel(report: ReportDetail): string {
  if (report.epicMasterFile === "IDB") {
    return "Open a related dashboard that uses this."
  }
  if (report.editReportUrl) {
    return "Open in report library."
  }
  if (report.epicMasterFile) {
    return "Run from the Hyperspace report library."
  }
  return "Run report unavailable"
}

function ReportRunRailButton({ report, title }: { report: ReportDetail; title: string }) {
  const attachments = report.attachments ?? []
  if (attachments.length > 0) {
    return <ReportRunDialog reportName={title} attachments={attachments} />
  }

  if (report.canRun && report.runUrl) {
    return (
      <RailExternalLink href={report.runUrl} label="Run report" className="size-11">
        <CirclePlay className="size-7 text-success" strokeWidth={1.5} />
        <span className="sr-only">Run report</span>
      </RailExternalLink>
    )
  }

  return (
    <RailTooltipButton label={resolveDisabledRunLabel(report)} disabled className="size-11">
      <CirclePlay className="size-7 text-muted-foreground/50" strokeWidth={1.5} />
      <span className="sr-only">Run report unavailable</span>
    </RailTooltipButton>
  )
}

export function ReportActionRail({
  report,
  title,
  profilePanel,
}: {
  report: ReportDetail
  title: string
  profilePanel: ReactNode
}) {
  const features = report.features ?? {}
  const shareUrl = `/reports?id=${report.id}`

  return (
    <ActionRail label="Report actions">
      <ReportRunRailButton report={report} title={title} />

      <EntityProfileSheet entityName={title} entityLabel="report profile">
        {profilePanel}
      </EntityProfileSheet>

      <StarToggleButton
        type="report"
        id={report.id}
        initialStarred={report.isStarred ?? false}
        initialCount={report.starCount ?? 0}
        iconOnly
      />

      {report.canEditDocumentation ? (
        <RailIconLink href={`/reports/edit?id=${report.id}`} label="Open Atlas editor">
          <Pencil className="size-5" />
          <span className="sr-only">Open Atlas editor</span>
        </RailIconLink>
      ) : null}

      {isInteractionFeatureEnabled(features.sharingEnabled) ? (
        <ShareMailDialog shareName={title} shareUrl={shareUrl} iconOnly />
      ) : null}

      {isInteractionFeatureEnabled(features.requestAccessEnabled) ? (
        <RequestAccessDialog reportName={title} reportUrl={shareUrl} />
      ) : null}

      {isInteractionFeatureEnabled(features.feedbackEnabled) ? (
        <EntityFeedbackDialog entityName={title} entityUrl={shareUrl} />
      ) : null}

      {report.editReportUrl ? (
        <RailExternalLink href={report.editReportUrl} label="Open report editor">
          <ArrowUpRightFromSquare className="size-5" />
          <span className="sr-only">Open report editor</span>
        </RailExternalLink>
      ) : null}

      {report.manageReportUrl ? (
        <RailExternalLink href={report.manageReportUrl} label="Manage report">
          <Settings className="size-5" />
          <span className="sr-only">Manage report</span>
        </RailExternalLink>
      ) : null}
    </ActionRail>
  )
}
