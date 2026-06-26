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
      <aside aria-label={label} className={className ?? "atlas-action-rail sticky z-10 shrink-0"}>
        <span className="atlas-action-rail-label">{label}</span>
        <div className="flex w-full flex-col items-center gap-1">{children}</div>
      </aside>
    </TooltipProvider>
  )
}

export function ActionRailGroup({
  children,
  separated = false,
}: {
  children: ReactNode
  separated?: boolean
}) {
  return (
    <div
      className={
        separated
          ? "atlas-action-rail-group atlas-action-rail-group-separated flex w-full flex-col items-center gap-1 pt-2"
          : "atlas-action-rail-group flex w-full flex-col items-center gap-1"
      }
    >
      {children}
    </div>
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
        <Button
          asChild
          variant="ghost"
          size="icon"
          className={className ?? "atlas-action-rail-button"}
        >
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
        <Button
          asChild
          variant="ghost"
          size="icon"
          className={className ?? "atlas-action-rail-button"}
        >
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
          className={className ?? "atlas-action-rail-button"}
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
