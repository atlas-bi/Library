"use client"

import type { ReactNode } from "react"
import { InteractionTooltip } from "@/components/interactions/interaction-tooltip"

export function FooterIconActions({ children }: { children: ReactNode }) {
  return (
    <div className="atlas-home-footer-cell atlas-footer-icon-group inline-flex items-center justify-center gap-3 text-center text-sm">
      {children}
    </div>
  )
}

export function FooterIconAction({ label, children }: { label: string; children: ReactNode }) {
  return (
    <InteractionTooltip label={label} placement="footer">
      {children}
    </InteractionTooltip>
  )
}
