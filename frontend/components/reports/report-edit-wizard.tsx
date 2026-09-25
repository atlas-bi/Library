"use client"

import Link from "next/link"
import { useMemo, useState } from "react"
import { ReportEditForm } from "@/components/reports/report-edit-form"
import { ReportImageUpload } from "@/components/reports/report-image-upload"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Label } from "@/components/ui/label"
import type { ReportDetail, ReportEditLookupOptions } from "@/lib/reports/types"

const STEPS = [
  { id: "description", label: "Description" },
  { id: "meta", label: "Meta" },
  { id: "images", label: "Images" },
  { id: "maintenance", label: "Maintenance" },
  { id: "complete", label: "Complete" },
] as const

type WizardStep = (typeof STEPS)[number]["id"]

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

  const goToStep = (nextStep: WizardStep) => {
    setStep(nextStep)
  }

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
                onClick={() => {
                  goToStep(wizardStep.id)
                }}
              >
                {wizardStep.label}
              </Button>
            </li>
          ))}
        </ol>
      </nav>

      {step === "description" ? (
        <ReportEditForm
          reportId={report.id}
          initial={report}
          cancelHref={cancelHref}
          lookupOptions={lookupOptions}
          mode="description"
          onContinue={() => {
            goToStep("meta")
          }}
        />
      ) : null}

      {step === "meta" ? (
        <ReportEditForm
          reportId={report.id}
          initial={report}
          cancelHref={cancelHref}
          lookupOptions={lookupOptions}
          mode="meta"
          onBack={() => {
            goToStep("description")
          }}
          onContinue={() => {
            goToStep("images")
          }}
        />
      ) : null}

      {step === "images" ? (
        <Card>
          <CardHeader>
            <CardTitle>Images</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <ReportImageUpload reportId={report.id} />
            <div className="flex flex-wrap gap-2">
              <Button type="button" variant="outline" onClick={() => goToStep("meta")}>
                Back
              </Button>
              <Button type="button" onClick={() => goToStep("maintenance")}>
                Next
              </Button>
            </div>
          </CardContent>
        </Card>
      ) : null}

      {step === "maintenance" ? (
        <Card>
          <CardHeader>
            <CardTitle>Maintenance</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="grid gap-4 sm:grid-cols-2">
              <div className="space-y-1">
                <Label>Maintenance schedule</Label>
                <p className="text-sm text-muted-foreground">{maintenanceSummary.schedule}</p>
              </div>
              <div className="space-y-1">
                <Label>Existing maintenance logs</Label>
                <p className="text-sm text-muted-foreground">{maintenanceSummary.logCount}</p>
              </div>
            </div>
            <ReportEditForm
              reportId={report.id}
              initial={report}
              cancelHref={cancelHref}
              lookupOptions={lookupOptions}
              mode="maintenance"
              onBack={() => {
                goToStep("images")
              }}
              onContinue={() => {
                goToStep("complete")
              }}
            />
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
              Review your changes on the report detail page when you are ready.
            </p>
            <div className="flex flex-wrap gap-2">
              <Button asChild>
                <Link href={`/reports?id=${report.id}`}>Return to report</Link>
              </Button>
              <Button type="button" variant="outline" onClick={() => goToStep("maintenance")}>
                Back
              </Button>
            </div>
          </CardContent>
        </Card>
      ) : null}
    </div>
  )
}
