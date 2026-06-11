"use client"

import { CirclePlay } from "lucide-react"
import type { ReactNode } from "react"
import { Button } from "@/components/ui/button"
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog"
import { Tooltip, TooltipContent, TooltipTrigger } from "@/components/ui/tooltip"
import type { ReportAttachment } from "@/lib/reports/types"

function formatAttachmentDate(value?: string | null): string {
  if (!value) return ""
  const date = new Date(value)
  return Number.isNaN(date.getTime()) ? "" : date.toLocaleString()
}

export function ReportRunDialog({
  reportName,
  attachments,
  trigger,
}: {
  reportName: string
  attachments: ReportAttachment[]
  trigger?: ReactNode
}) {
  const sorted = [...attachments].sort((a, b) => {
    const aTime = a.creationDate ? new Date(a.creationDate).getTime() : 0
    const bTime = b.creationDate ? new Date(b.creationDate).getTime() : 0
    return bTime - aTime
  })

  return (
    <Dialog>
      <Tooltip>
        <TooltipTrigger asChild>
          <DialogTrigger asChild>
            {trigger ?? (
              <Button type="button" variant="ghost" size="icon" className="size-11">
                <CirclePlay className="size-7 text-success" strokeWidth={1.5} />
                <span className="sr-only">Run report</span>
              </Button>
            )}
          </DialogTrigger>
        </TooltipTrigger>
        <TooltipContent side="right">Run report</TooltipContent>
      </Tooltip>
      <DialogContent className="max-w-lg">
        <DialogHeader>
          <DialogTitle>Run this report</DialogTitle>
        </DialogHeader>
        <div className="space-y-4 text-sm text-[var(--atlas-home-text)]">
          <p>
            This is a Crystal Report. Here are the recent run outputs for{" "}
            <strong>{reportName}</strong>.
          </p>
          {sorted.length > 0 ? (
            <ul className="list-disc space-y-2 pl-5">
              {sorted.map((attachment) => {
                const label = attachment.name?.trim() || `Output ${attachment.id}`
                const dateLabel = formatAttachmentDate(attachment.creationDate)
                const href = attachment.runUrl?.trim()

                return (
                  <li key={attachment.id}>
                    {href ? (
                      <a
                        href={href}
                        target="_blank"
                        rel="noopener noreferrer"
                        className="font-medium text-[var(--atlas-home-link)] hover:underline"
                      >
                        {label}
                      </a>
                    ) : (
                      <span>{label}</span>
                    )}
                    {dateLabel ? (
                      <span className="text-[var(--atlas-home-muted)]"> · {dateLabel}</span>
                    ) : null}
                  </li>
                )
              })}
            </ul>
          ) : (
            <p className="text-[var(--atlas-home-muted)]">No run outputs are available yet.</p>
          )}
          <div className="space-y-3 border-t border-[var(--atlas-home-border-soft)] pt-4 text-[var(--atlas-home-muted)]">
            <p>
              If you need a <strong className="text-[var(--atlas-home-text)]">newer</strong>{" "}
              dataset, request a new run from the Report Library.
            </p>
            <ol className="list-decimal space-y-1 pl-5">
              <li>Open the Report Library</li>
              <li>Search for this report</li>
              <li>Click &quot;view&quot; &gt; click &quot;Request&quot;</li>
            </ol>
            <p>~ after 5 minutes ~</p>
            <ol className="list-decimal space-y-1 pl-5">
              <li>Open My Reports</li>
              <li>In the lower section of the screen you will find the report listed</li>
              <li>Click &quot;View Output&quot;</li>
            </ol>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  )
}
