"use client"

import type { ReactNode } from "react"
import { EntityProfileSheet } from "@/components/interactions/entity-profile-sheet"

/** @deprecated Use EntityProfileSheet from components/interactions */
export function CollectionProfileSheet({
  collectionName,
  children,
  variant = "default",
}: {
  collectionName: string
  children: ReactNode
  variant?: "default" | "footer"
}) {
  return (
    <EntityProfileSheet
      entityName={collectionName}
      entityLabel="collection profile"
      variant={variant}
    >
      {children}
    </EntityProfileSheet>
  )
}
