"use client"

import { ShareMailDialog } from "@/components/interactions/share-mail-dialog"

export function ShareModal({
  isOpen,
  onClose,
  initiativeName,
  initiativeId,
}: {
  isOpen: boolean
  onClose: () => void
  initiativeName: string
  initiativeId: number
}) {
  if (!isOpen) return null

  return (
    <ShareMailDialog
      shareName={initiativeName}
      shareUrl={`/initiatives?id=${initiativeId}`}
      open={isOpen}
      showTrigger={false}
      onOpenChange={(open) => {
        if (!open) onClose()
      }}
    />
  )
}
