"use client"

import { ArrowUpRightFromSquare, CirclePlay, Pencil, Settings } from "lucide-react"
import type { ReactNode } from "react"
import {
  ActionRail,
  ActionRailGroup,
  RailExternalLink,
  RailIconLink,
  RailTooltipButton,
} from "@/components/interactions/action-rail"
import { EntityEngagementRailActions } from "@/components/interactions/entity-engagement-rail-actions"
import { ReportRunDialog } from "@/components/interactions/report-run-dialog"
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
      <RailExternalLink
        href={report.runUrl}
        label="Run report"
        className="atlas-action-rail-button size-11"
      >
        <CirclePlay className="size-7 text-success" strokeWidth={1.5} />
        <span className="sr-only">Run report</span>
      </RailExternalLink>
    )
  }

  return (
    <RailTooltipButton
      label={resolveDisabledRunLabel(report)}
      disabled
      className="atlas-action-rail-button size-11"
    >
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
  const hasAdminActions = !!report.editReportUrl || !!report.manageReportUrl

  return (
    <ActionRail label="Report actions">
      <ActionRailGroup>
        <ReportRunRailButton report={report} title={title} />
      </ActionRailGroup>

      <ActionRailGroup separated>
        <EntityEngagementRailActions
          entityType="report"
          entityId={report.id}
          entityName={title}
          entityUrl={shareUrl}
          profileLabel="report profile"
          profilePanel={profilePanel}
          isStarred={report.isStarred}
          starCount={report.starCount}
          features={features}
          showRequestAccess
          afterStar={
            report.canEditDocumentation ? (
              <RailIconLink href={`/reports/edit?id=${report.id}`} label="Open Atlas editor">
                <Pencil className="size-5" />
                <span className="sr-only">Open Atlas editor</span>
              </RailIconLink>
            ) : null
          }
        />
      </ActionRailGroup>

      {hasAdminActions && (report.editReportUrl || report.manageReportUrl) ? (
        <ActionRailGroup separated>
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
        </ActionRailGroup>
      ) : null}
    </ActionRail>
  )
}
