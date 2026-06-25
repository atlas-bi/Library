"use client"

import { useState, useTransition } from "react"
import { deleteCollectionAction } from "@/app/collections/actions"
import { Button } from "@/components/ui/button"

export function DeleteCollectionButton({
  collectionId,
  collectionName,
}: {
  collectionId: number
  collectionName: string
}) {
  const [error, setError] = useState<string | null>(null)
  const [pending, startTransition] = useTransition()

  return (
    <div className="space-y-2">
      <Button
        type="button"
        variant="destructive"
        disabled={pending}
        onClick={() => {
          const label = collectionName.trim() || `collection ${collectionId}`
          if (!window.confirm(`Delete ${label}? This cannot be undone.`)) return
          setError(null)
          startTransition(() => {
            void (async () => {
              const result = await deleteCollectionAction(collectionId)
              if (result?.error) {
                setError(result.error)
              }
            })()
          })
        }}
      >
        {pending ? "Deleting…" : "Delete collection"}
      </Button>
      {error ? <p className="text-sm text-destructive">{error}</p> : null}
    </div>
  )
}
