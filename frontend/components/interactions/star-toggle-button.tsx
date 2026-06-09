"use client"

import { Star } from "lucide-react"
import { useRouter } from "next/navigation"
import { useState, useTransition } from "react"
import { toggleStarAction } from "@/app/interactions/actions"
import { Button } from "@/components/ui/button"
import type { InteractionEntityType } from "@/lib/interactions/types"

export function StarToggleButton({
  type,
  id,
  initialStarred,
  initialCount,
  iconOnly = false,
  variant = "button",
}: {
  type: InteractionEntityType
  id: number
  initialStarred: boolean
  initialCount: number
  iconOnly?: boolean
  variant?: "button" | "card-footer"
}) {
  const router = useRouter()
  const [isStarred, setIsStarred] = useState(initialStarred)
  const [count, setCount] = useState(initialCount)
  const [error, setError] = useState<string | null>(null)
  const [pending, startTransition] = useTransition()

  const toggleStar = () => {
    setError(null)
    startTransition(() => {
      void (async () => {
        const result = await toggleStarAction(type, id)
        if (result.error) {
          setError(result.error)
          return
        }
        if (result.data) {
          setIsStarred(result.data.isStarred)
          setCount(result.data.count)
        }
        router.refresh()
      })()
    })
  }

  if (variant === "card-footer") {
    return (
      <div className="flex flex-col">
        <button
          type="button"
          disabled={pending}
          onClick={toggleStar}
          className="atlas-home-footer-cell inline-flex w-full cursor-pointer items-center justify-center gap-2 border-r border-[var(--atlas-home-border-soft)] text-center text-sm text-[var(--atlas-home-link)] hover:bg-[var(--atlas-home-surface-muted)] disabled:cursor-not-allowed disabled:opacity-60"
        >
          <Star
            className={`h-4 w-4 ${isStarred ? "atlas-home-star-icon" : "text-[var(--atlas-home-muted)]"}`}
            strokeWidth={1.8}
            fill={isStarred ? "currentColor" : "none"}
          />
          <span>
            {isStarred ? "Starred" : "Star"} {count}
          </span>
        </button>
        {error ? <span className="px-2 text-xs text-destructive">{error}</span> : null}
      </div>
    )
  }

  if (iconOnly) {
    return (
      <div className="flex flex-col items-center gap-1">
        <Button
          type="button"
          variant="ghost"
          size="icon"
          className="relative size-10"
          disabled={pending}
          onClick={toggleStar}
        >
          <Star className={`size-5 ${isStarred ? "fill-amber-400 text-amber-500" : ""}`} />
          <span className="sr-only">Star this collection</span>
          {count > 0 ? (
            <span className="absolute -top-1 -right-1 rounded-full bg-muted px-1 text-[10px] leading-none">
              {count}
            </span>
          ) : null}
        </Button>
        {error ? <span className="text-xs text-destructive">{error}</span> : null}
      </div>
    )
  }

  return (
    <div className="flex flex-col items-end gap-1">
      <Button
        type="button"
        variant={isStarred ? "default" : "outline"}
        size="sm"
        disabled={pending}
        onClick={toggleStar}
      >
        <Star className={`mr-1 size-3.5 ${isStarred ? "fill-current" : ""}`} />
        {count}
      </Button>
      {error ? <span className="text-xs text-destructive">{error}</span> : null}
    </div>
  )
}
