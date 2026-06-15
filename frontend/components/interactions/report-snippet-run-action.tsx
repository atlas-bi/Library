"use client"

import { CirclePlay } from "lucide-react"
import Link from "next/link"
import { InteractionTooltip } from "@/components/interactions/interaction-tooltip"

function resolveDisabledRunLabel(report: {
  epicMasterFile?: string | null
  editReportUrl?: string | null
}): string {
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

export function ReportSnippetRunAction({
  reportId,
  canRun = false,
  attachmentCount = 0,
  runUrl,
  epicMasterFile,
  editReportUrl,
}: {
  reportId: number
  canRun?: boolean
  attachmentCount?: number
  runUrl?: string | null
  epicMasterFile?: string | null
  editReportUrl?: string | null
}) {
  const detailHref = `/reports?id=${reportId}`
  const showRun = attachmentCount > 0 || canRun || !!epicMasterFile || !!editReportUrl

  if (!showRun) {
    return null
  }

  const playIcon = (enabled: boolean) => (
    <CirclePlay
      className={`h-8 w-8 ${enabled ? "text-[var(--atlas-home-success)]" : "text-[var(--atlas-home-muted)]"}`}
      strokeWidth={1.5}
    />
  )

  if (runUrl) {
    return (
      <InteractionTooltip label="Run report" placement="rail">
        <a
          href={runUrl}
          target="_blank"
          rel="noreferrer"
          aria-label="Run report"
          className="inline-flex shrink-0 cursor-pointer items-center"
        >
          {playIcon(true)}
        </a>
      </InteractionTooltip>
    )
  }

  if (canRun || attachmentCount > 0) {
    return (
      <InteractionTooltip label="Run report" placement="rail">
        <Link
          href={detailHref}
          aria-label="Run report"
          className="inline-flex shrink-0 cursor-pointer items-center"
        >
          {playIcon(true)}
        </Link>
      </InteractionTooltip>
    )
  }

  return (
    <InteractionTooltip
      label={resolveDisabledRunLabel({ epicMasterFile, editReportUrl })}
      placement="rail"
    >
      <button
        type="button"
        disabled
        aria-label="Run report unavailable"
        className="inline-flex shrink-0 cursor-not-allowed items-center border-0 bg-transparent p-0 opacity-70"
      >
        {playIcon(false)}
      </button>
    </InteractionTooltip>
  )
}
