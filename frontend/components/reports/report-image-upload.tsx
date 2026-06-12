"use client"

import { useRouter } from "next/navigation"
import { useRef, useState, useTransition } from "react"
import { uploadReportImageAction } from "@/app/reports/actions"
import { Button } from "@/components/ui/button"
import { Label } from "@/components/ui/label"

export function ReportImageUpload({ reportId }: { reportId: number }) {
  const router = useRouter()
  const inputRef = useRef<HTMLInputElement>(null)
  const [error, setError] = useState<string | null>(null)
  const [message, setMessage] = useState<string | null>(null)
  const [pending, startTransition] = useTransition()

  return (
    <div className="space-y-2">
      <Label htmlFor={`report-image-${reportId}`}>Upload image (max 1 MB)</Label>
      <input
        ref={inputRef}
        id={`report-image-${reportId}`}
        type="file"
        accept="image/*"
        className="block w-full text-sm"
        disabled={pending}
      />
      {error ? <p className="text-sm text-destructive">{error}</p> : null}
      {message ? <p className="text-sm text-muted-foreground">{message}</p> : null}
      <Button
        type="button"
        size="sm"
        variant="outline"
        disabled={pending}
        onClick={() => {
          const input = inputRef.current
          if (!input?.files?.[0]) {
            setError("Choose an image first.")
            return
          }
          setError(null)
          setMessage(null)
          const formData = new FormData()
          formData.append("file", input.files[0])
          startTransition(() => {
            void (async () => {
              const result = await uploadReportImageAction(reportId, formData)
              if ("error" in result && result.error) {
                setError(result.error)
                return
              }
              setMessage("Image uploaded.")
              input.value = ""
              router.refresh()
            })()
          })
        }}
      >
        {pending ? "Uploading…" : "Upload"}
      </Button>
    </div>
  )
}
