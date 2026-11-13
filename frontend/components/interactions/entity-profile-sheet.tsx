"use client"

import { BarChart3, X } from "lucide-react"
import type { ReactNode } from "react"
import { InteractionTooltip } from "@/components/interactions/interaction-tooltip"
import { Button } from "@/components/ui/button"
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog"

export function EntityProfileSheet({
  entityName,
  entityLabel = "profile",
  children,
  variant = "default",
}: {
  entityName: string
  entityLabel?: string
  children: ReactNode
  variant?: "default" | "footer"
}) {
  const tooltipLabel = `Open ${entityLabel}`
  const tooltipPlacement = variant === "footer" ? "footer" : "rail"
  const trigger =
    variant === "footer" ? (
      <button
        type="button"
        className="inline-flex cursor-pointer items-center text-[var(--atlas-home-muted)] hover:text-[var(--atlas-home-link)]"
      >
        <BarChart3 className="h-4 w-4" strokeWidth={1.8} />
        <span className="sr-only">{tooltipLabel}</span>
      </button>
    ) : (
      <Button type="button" variant="ghost" size="icon" className="atlas-action-rail-button">
        <BarChart3 className="size-5" />
        <span className="sr-only">{tooltipLabel}</span>
      </Button>
    )

  return (
    <Dialog>
      <InteractionTooltip label={tooltipLabel} placement={tooltipPlacement}>
        <DialogTrigger asChild>{trigger}</DialogTrigger>
      </InteractionTooltip>
      {/* Match Razor's modal-large: 90vw × 90vh, centred, scrollable */}
      <DialogContent
        className="flex max-h-[95vh] w-[95vw] max-w-[95vw] flex-col overflow-hidden p-0"
        aria-describedby={undefined}
      >
        <DialogHeader className="shrink-0 border-b px-6 py-4">
          <DialogTitle className="font-serif text-2xl font-bold">Profile</DialogTitle>
        </DialogHeader>
        <div className="min-h-0 flex-1 overflow-y-auto px-6 py-4">{children}</div>
      </DialogContent>
    </Dialog>
  )
}
