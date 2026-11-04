"use client"

import { Download } from "lucide-react"
import { useState } from "react"
import { downloadReportCubeAction } from "@/app/reports/file-actions"
import { RailTooltipButton } from "@/components/interactions/action-rail"
import { defaultCubeFileName } from "@/lib/reports/report-cube"
import { cubeDownloadErrorMessage } from "@/lib/reports/report-file-messages"
import { saveDownloadedFile } from "@/lib/reports/save-downloaded-file"
import type { ReportDetail } from "@/lib/reports/types"

export function ReportCubeDownloadButton({ report }: { report: ReportDetail }) {
  const [pending, setPending] = useState(false)
  const [error, setError] = useState<string | null>(null)

  async function handleDownload() {
    setPending(true)
    setError(null)

    try {
      const result = await downloadReportCubeAction(report.id, defaultCubeFileName(report))
      if (!result.ok) {
        setError(cubeDownloadErrorMessage(result.code))
        return
      }
      saveDownloadedFile(result.data)
    } catch {
      setError(cubeDownloadErrorMessage("unknown"))
    } finally {
      setPending(false)
    }
  }

  return (
    <div className="flex flex-col items-center gap-1">
      <RailTooltipButton
        label="Download cube connection file"
        className="atlas-action-rail-button size-11"
        disabled={pending}
        onClick={() => void handleDownload()}
      >
        <Download className="size-6 text-[var(--atlas-home-link)]" strokeWidth={1.5} />
        <span className="sr-only">Download cube connection file</span>
      </RailTooltipButton>
      {error ? (
        <p className="max-w-[4.75rem] text-center text-[0.65rem] leading-tight text-destructive" role="alert">
          {error}
        </p>
      ) : null}
    </div>
  )
}
