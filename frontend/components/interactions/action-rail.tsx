"use client"

import Link from "next/link"
import type { ReactNode } from "react"
import { Button } from "@/components/ui/button"
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from "@/components/ui/tooltip"

export function ActionRail({
  label,
  children,
  className,
}: {
  label: string
  children: ReactNode
  className?: string
}) {
  return (
    <TooltipProvider>
      <aside
        aria-label={label}
        className={
          className ??
          "sticky top-8 z-10 flex w-[4.5rem] shrink-0 flex-col items-center gap-1.5 rounded-xl border border-border/80 bg-card/95 p-2.5 shadow-lg backdrop-blur-sm"
        }
      >
        <span className="mb-1 w-full border-b border-border/60 pb-2 text-center text-[10px] font-semibold uppercase tracking-wider text-muted-foreground">
          Actions
        </span>
        {children}
      </aside>
    </TooltipProvider>
  )
}

export function RailIconLink({
  href,
  label,
  children,
  className,
}: {
  href: string
  label: string
  children: ReactNode
  className?: string
}) {
  return (
    <Tooltip>
      <TooltipTrigger asChild>
        <Button asChild variant="ghost" size="icon" className={className ?? "size-10"}>
          <Link href={href}>{children}</Link>
        </Button>
      </TooltipTrigger>
      <TooltipContent side="right">{label}</TooltipContent>
    </Tooltip>
  )
}

export function RailExternalLink({
  href,
  label,
  children,
  className,
}: {
  href: string
  label: string
  children: ReactNode
  className?: string
}) {
  return (
    <Tooltip>
      <TooltipTrigger asChild>
        <Button asChild variant="ghost" size="icon" className={className ?? "size-10"}>
          <a href={href} target="_blank" rel="noopener noreferrer">
            {children}
          </a>
        </Button>
      </TooltipTrigger>
      <TooltipContent side="right">{label}</TooltipContent>
    </Tooltip>
  )
}

export function RailTooltipButton({
  label,
  children,
  disabled,
  onClick,
  className,
}: {
  label: string
  children: ReactNode
  disabled?: boolean
  onClick?: () => void
  className?: string
}) {
  return (
    <Tooltip>
      <TooltipTrigger asChild>
        <Button
          type="button"
          variant="ghost"
          size="icon"
          className={className ?? "size-10"}
          disabled={disabled}
          onClick={onClick}
        >
          {children}
        </Button>
      </TooltipTrigger>
      <TooltipContent side="right">{label}</TooltipContent>
    </Tooltip>
  )
}
