"use client"

import type { ReactNode } from "react"
import { Tooltip, TooltipContent, TooltipTrigger } from "@/components/ui/tooltip"

export type InteractionTooltipPlacement = "rail" | "footer"

function tooltipSide(placement: InteractionTooltipPlacement): "right" | "top" {
  return placement === "footer" ? "top" : "right"
}

export function InteractionTooltip({
  label,
  placement = "rail",
  children,
}: {
  label: string
  placement?: InteractionTooltipPlacement
  children: ReactNode
}) {
  return (
    <Tooltip>
      <TooltipTrigger asChild>{children}</TooltipTrigger>
      <TooltipContent side={tooltipSide(placement)}>{label}</TooltipContent>
    </Tooltip>
  )
}
