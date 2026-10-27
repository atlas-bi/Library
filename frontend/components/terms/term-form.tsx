"use client"

import { ArrowLeft, ArrowRight } from "lucide-react"
import Link from "next/link"
import { useState, useTransition } from "react"
import { createTermAction, updateTermAction } from "@/app/terms/actions"
import { MarkdownField } from "@/components/content/markdown-field"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Switch } from "@/components/ui/switch"
import type { TermDetailDto } from "@/lib/terms/types"

export function TermForm({
  mode,
  termId,
  initial,
  cancelHref,
  canApproveTerm,
}: {
  mode: "create" | "edit"
  termId?: number
  initial?: TermDetailDto | null
  cancelHref: string
  canApproveTerm: boolean
}) {
  const [name, setName] = useState(initial?.name ?? "")
  const [summary, setSummary] = useState(initial?.summary ?? "")
  const [technicalDefinition, setTechnicalDefinition] = useState(initial?.technicalDefinition ?? "")
  const [isApproved, setIsApproved] = useState((initial?.approvedYn ?? "N").toUpperCase() === "Y")

  const [formError, setFormError] = useState<string | null>(null)
  const [isPending, startTransition] = useTransition()

  const pageTitle = mode === "create" ? "New Term" : `Editing ${initial?.name?.trim() || "term"}`

  const submit = () => {
    setFormError(null)
    const trimmedName = name.trim()
    if (!trimmedName) {
      setFormError("Name is required.")
      return
    }

    const body = {
      name: trimmedName,
      summary: summary.trim() ? summary.trim() : null,
      technicalDefinition: technicalDefinition.trim() ? technicalDefinition.trim() : null,
      approvedYn: isApproved ? "Y" : "N",
    }

    startTransition(() => {
      void (async () => {
        if (mode === "create") {
          const result = await createTermAction(body)
          if (result?.error) setFormError(result.error)
          return
        }

        if (typeof termId !== "number") {
          setFormError("Missing term id.")
          return
        }

        const result = await updateTermAction(termId, body)
        if (result?.error) setFormError(result.error)
      })()
    })
  }

  return (
    <form className="space-y-8">
      <h1 className="text-[2.5rem] leading-[1.125] font-bold text-[#363636]">{pageTitle}</h1>

      <div className="flex flex-wrap items-stretch justify-between gap-4">
        <Button asChild variant="outline" size="lg" className="h-auto min-h-14 px-5 py-3">
          <Link href={cancelHref}>
            <ArrowLeft className="mr-3 size-5 shrink-0" />
            <span className="text-left">
              <span className="block font-semibold">Cancel</span>
              <span className="block text-xs font-normal text-muted-foreground">Go back</span>
            </span>
          </Link>
        </Button>
        <Button
          type="button"
          size="lg"
          className="h-auto min-h-14 px-5 py-3"
          disabled={isPending}
          onClick={submit}
        >
          <span className="text-left">
            <span className="block font-semibold">{isPending ? "Saving…" : "Save"}</span>
            <span className="block text-xs font-normal opacity-90">and continue</span>
          </span>
          <ArrowRight className="ml-3 size-5 shrink-0" />
        </Button>
      </div>

      <div className="space-y-2">
        <Label htmlFor="term-name">Name</Label>
        <Input
          id="term-name"
          value={name}
          placeholder="e.g LOSI"
          onChange={(event) => {
            setName(event.target.value)
          }}
          required
        />
      </div>

      <MarkdownField
        id="term-summary"
        label="Summary"
        value={summary}
        onChange={setSummary}
      />

      <MarkdownField
        id="term-technical"
        label="Technical Definition"
        value={technicalDefinition}
        onChange={setTechnicalDefinition}
      />

      {canApproveTerm ? (
        <div className="space-y-3">
          <Label className="text-base">Other Options</Label>
          <div className="flex items-center gap-3">
            <Switch
              id="term-approved"
              checked={isApproved}
              onCheckedChange={(checked) => {
                setIsApproved(checked)
              }}
            />
            <Label htmlFor="term-approved" className="cursor-pointer font-normal">
              Approved?
            </Label>
          </div>
        </div>
      ) : null}

      {formError ? <p className="text-sm text-destructive">{formError}</p> : null}
    </form>
  )
}
