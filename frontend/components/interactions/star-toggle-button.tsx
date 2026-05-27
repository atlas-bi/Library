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
}: {
  type: InteractionEntityType
  id: number
  initialStarred: boolean
  initialCount: number
  iconOnly?: boolean
}) {
  const router = useRouter()
  const [isStarred, setIsStarred] = useState(initialStarred)
  const [count, setCount] = useState(initialCount)
  const [error, setError] = useState<string | null>(null)
  const [pending, startTransition] = useTransition()

  if (iconOnly) {
    return (
      <div className="flex flex-col items-center gap-1">
        <Button
          type="button"
          variant="ghost"
          size="icon"
          className="relative size-10"
          disabled={pending}
          onClick={() => {
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
          }}
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
        onClick={() => {
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
        }}
      >
        <Star className={`mr-1 size-3.5 ${isStarred ? "fill-current" : ""}`} />
        {count}
      </Button>
      {error ? <span className="text-xs text-destructive">{error}</span> : null}
    </div>
  )
}
