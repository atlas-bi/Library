"use client"

import { BarChart3 } from "lucide-react"
import type { ReactNode } from "react"
import { InteractionTooltip } from "@/components/interactions/interaction-tooltip"
import { Button } from "@/components/ui/button"
import { Sheet, SheetContent, SheetHeader, SheetTitle, SheetTrigger } from "@/components/ui/sheet"

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
    <Sheet>
      <InteractionTooltip label={tooltipLabel} placement={tooltipPlacement}>
        <SheetTrigger asChild>{trigger}</SheetTrigger>
      </InteractionTooltip>
      <SheetContent side="right" className="w-full overflow-y-auto sm:max-w-4xl lg:max-w-5xl">
        <SheetHeader>
          <SheetTitle>Profile — {entityName}</SheetTitle>
        </SheetHeader>
        <div className="mt-4">{children}</div>
      </SheetContent>
    </Sheet>
  )
}
