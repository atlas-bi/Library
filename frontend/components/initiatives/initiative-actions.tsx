"use client"

import { useState } from "react"
import { useRouter } from "next/navigation"
import { Button } from "@/components/ui/button"
import { deleteInitiativeAction } from "@/lib/initiatives/actions"
import type { InitiativeDetailDto } from "@/lib/initiatives/types"

export function InitiativeActions({ data }: { data: InitiativeDetailDto }) {
  const router = useRouter()
  const [isDeleting, setIsDeleting] = useState(false)

  const handleDelete = async () => {
    if (!window.confirm("Are you sure you want to delete this initiative?")) {
      return
    }

    setIsDeleting(true)
    const result = await deleteInitiativeAction(data.id)
    setIsDeleting(false)

    if (result.error) {
      alert(`Error deleting initiative: ${result.error}`)
    } else {
      router.push("/initiatives")
      router.refresh()
    }
  }

  const handleEdit = () => {
    router.push(`/initiatives/edit?id=${data.id}`)
  }

  return (
    <div className="flex items-center gap-2">
      {data.canEditInitiative && (
        <Button variant="outline" onClick={handleEdit}>
          Edit
        </Button>
      )}
      {data.canDeleteInitiative && (
        <Button variant="destructive" onClick={handleDelete} disabled={isDeleting}>
          {isDeleting ? "Deleting..." : "Delete"}
        </Button>
      )}
    </div>
  )
}
