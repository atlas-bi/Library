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
}: {
  type: InteractionEntityType
  id: number
  initialStarred: boolean
  initialCount: number
}) {
  const router = useRouter()
  const [isStarred, setIsStarred] = useState(initialStarred)
  const [count, setCount] = useState(initialCount)
  const [error, setError] = useState<string | null>(null)
  const [pending, startTransition] = useTransition()

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
