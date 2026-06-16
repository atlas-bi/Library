"use client"

import { EntityFeedbackDialog } from "@/components/interactions/entity-feedback-dialog"

/** @deprecated Use EntityFeedbackDialog from components/interactions */
export function CollectionFeedbackDialog({
  collectionName,
  collectionUrl,
}: {
  collectionName: string
  collectionUrl: string
}) {
  return <EntityFeedbackDialog entityName={collectionName} entityUrl={collectionUrl} />
}
