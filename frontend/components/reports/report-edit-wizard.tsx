"use client"

import Link from "next/link"
import { useMemo, useState } from "react"
import { ReportEditForm } from "@/components/reports/report-edit-form"
import { ReportImageUpload } from "@/components/reports/report-image-upload"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Label } from "@/components/ui/label"
import type { ReportDetail, ReportEditLookupOptions } from "@/lib/reports/types"
import { cn } from "@/lib/utils"

const STEPS = [
  { id: "description", label: "Description" },
  { id: "meta", label: "Meta" },
  { id: "images", label: "Images" },
  { id: "maintenance", label: "Maintenance" },
  { id: "complete", label: "Complete" },
] as const

type WizardStep = (typeof STEPS)[number]["id"]

type FormStep = "description" | "meta" | "maintenance"

function isFormStep(step: WizardStep): step is FormStep {
  return step === "description" || step === "meta" || step === "maintenance"
}

export function ReportEditWizard({
  report,
  cancelHref,
  lookupOptions,
}: {
  report: ReportDetail
  cancelHref: string
  lookupOptions: ReportEditLookupOptions
}) {
  const [step, setStep] = useState<WizardStep>("description")
  const title = report.displayTitle || report.displayName || report.name
  const doc = (report.document ?? {}) as Record<string, unknown>

  const maintenanceSummary = useMemo(() => {
    const schedule = doc.maintenanceSchedule as { name?: string } | undefined
    const logs = (doc.maintenanceLogs as Array<unknown> | undefined) ?? []
    return {
      schedule: schedule?.name ?? "Not set",
      logCount: logs.length,
    }
  }, [doc.maintenanceLogs, doc.maintenanceSchedule])

  const formMode: FormStep = isFormStep(step) ? step : "description"

  return (
    <div className="space-y-6">
      <h1 className="text-3xl font-bold tracking-tight">Editing {title}</h1>

      <nav aria-label="Report edit steps" className="overflow-x-auto">
        <ol className="flex min-w-max items-center gap-2 text-sm">
          <li>
            <Button asChild variant="outline" size="sm">
              <Link href={cancelHref}>Cancel</Link>
            </Button>
          </li>
          {STEPS.map((wizardStep) => (
            <li key={wizardStep.id}>
              <Button
                type="button"
                size="sm"
                variant={step === wizardStep.id ? "default" : "outline"}
                aria-current={step === wizardStep.id ? "step" : undefined}
                onClick={() => {
                  setStep(wizardStep.id)
                }}
              >
                {wizardStep.label}
              </Button>
            </li>
          ))}
        </ol>
      </nav>

      <div className={cn(!isFormStep(step) && "hidden")} aria-hidden={!isFormStep(step)}>
        {step === "maintenance" ? (
          <div className="mb-4 grid gap-4 sm:grid-cols-2">
            <div className="space-y-1">
              <Label>Maintenance schedule</Label>
              <p className="text-sm text-muted-foreground">{maintenanceSummary.schedule}</p>
            </div>
            <div className="space-y-1">
              <Label>Existing maintenance logs</Label>
              <p className="text-sm text-muted-foreground">{maintenanceSummary.logCount}</p>
            </div>
          </div>
        ) : null}
        <ReportEditForm
          reportId={report.id}
          initial={report}
          cancelHref={cancelHref}
          lookupOptions={lookupOptions}
          mode={formMode}
          onBack={
            step === "description"
              ? undefined
              : () => {
                  if (step === "meta") setStep("description")
                  if (step === "maintenance") setStep("images")
                }
          }
          onContinue={() => {
            if (step === "description") setStep("meta")
            if (step === "meta") setStep("images")
            if (step === "maintenance") setStep("complete")
          }}
        />
      </div>

      {step === "images" ? (
        <Card>
          <CardHeader>
            <CardTitle>Images</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <ReportImageUpload reportId={report.id} />
            <div className="flex flex-wrap gap-2">
              <Button type="button" variant="outline" onClick={() => setStep("meta")}>
                Back
              </Button>
              <Button type="button" onClick={() => setStep("maintenance")}>
                Next
              </Button>
            </div>
          </CardContent>
        </Card>
      ) : null}

      {step === "complete" ? (
        <Card>
          <CardHeader>
            <CardTitle>Complete</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <p className="text-sm text-muted-foreground">
              Save your documentation on the previous steps, then return to the report when you are
              ready.
            </p>
            <div className="flex flex-wrap gap-2">
              <Button asChild>
                <Link href={`/reports?id=${report.id}`}>Return to report</Link>
              </Button>
              <Button type="button" variant="outline" onClick={() => setStep("maintenance")}>
                Back
              </Button>
            </div>
          </CardContent>
        </Card>
      ) : null}
    </div>
  )
}
