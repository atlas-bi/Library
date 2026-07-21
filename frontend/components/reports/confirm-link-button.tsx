"use client"

import { AppAlertDialog } from "@/components/ui/app-alert-dialog"

type ConfirmLinkButtonProps = {
  href: string
  buttonLabel: string
  title: string
  description: string
  className?: string
}

export function ConfirmLinkButton({
  href,
  buttonLabel,
  title,
  description,
  className,
}: ConfirmLinkButtonProps) {
  return (
    <AppAlertDialog
      triggerLabel={buttonLabel}
      title={title}
      description={description}
      confirmLabel="Open link"
      cancelLabel="Cancel"
      triggerClassName={className}
      onConfirm={() => window.open(href, "_blank", "noopener,noreferrer")}
    />
  )
}
