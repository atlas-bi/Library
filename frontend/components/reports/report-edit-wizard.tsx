"use client"

import Link from "next/link"
import { useCallback, useEffect, useMemo, useState } from "react"
import { ReportEditForm } from "@/components/reports/report-edit-form"
import { ReportImageUpload } from "@/components/reports/report-image-upload"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Label } from "@/components/ui/label"
import { getReportObjectImageThumbUrl } from "@/lib/reports/report-image-url"
import type { ReportDetail, ReportEditLookupOptions } from "@/lib/reports/types"

export const REPORT_EDIT_WIZARD_STEPS = [
  { id: "description", label: "Description" },
  { id: "meta", label: "Meta" },
  { id: "images", label: "Images" },
  { id: "maintenance", label: "Maintenance" },
  { id: "complete", label: "Complete" },
] as const

export type ReportEditWizardStep = (typeof REPORT_EDIT_WIZARD_STEPS)[number]["id"]

const STEP_IDS = REPORT_EDIT_WIZARD_STEPS.map((step) => step.id)

export function parseReportEditWizardStep(hash: string | null): ReportEditWizardStep | null {
  const normalized = hash?.replace(/^#/, "")
  if (!normalized) return null
  return STEP_IDS.find((step) => step === normalized) ?? null
}

export function getDefaultReportEditWizardStep(): ReportEditWizardStep {
  return "description"
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
  const [step, setStep] = useState<ReportEditWizardStep>(() => getDefaultReportEditWizardStep())
  const title = report.displayTitle || report.displayName || report.name
  const doc = (report.document ?? {}) as Record<string, unknown>
  const existingImages = report.images ?? []

  const maintenanceSummary = useMemo(() => {
    const schedule = doc.maintenanceSchedule as { name?: string } | undefined
    const logs = (doc.maintenanceLogs as Array<unknown> | undefined) ?? []
    return {
      schedule: schedule?.name ?? "Not set",
      logCount: logs.length,
    }
  }, [doc.maintenanceLogs, doc.maintenanceSchedule])

  const goToStep = useCallback((nextStep: ReportEditWizardStep) => {
    setStep(nextStep)
    if (typeof window !== "undefined") {
      window.history.replaceState(null, "", `#${nextStep}`)
    }
  }, [])

  useEffect(() => {
    const syncFromHash = () => {
      const hashStep = parseReportEditWizardStep(window.location.hash)
      if (hashStep) {
        setStep(hashStep)
        return
      }
      const defaultStep = getDefaultReportEditWizardStep()
      setStep(defaultStep)
      if (!window.location.hash) {
        window.history.replaceState(null, "", `#${defaultStep}`)
      }
    }

    syncFromHash()
    window.addEventListener("hashchange", syncFromHash)
    return () => window.removeEventListener("hashchange", syncFromHash)
  }, [])

  return (
    <div className="space-y-6">
      <h1 className="atlas-home-heading mb-0">Editing {title}</h1>

      <nav aria-label="Report edit steps" className="atlas-home-tab-nav overflow-x-auto">
        <ul className="flex min-w-max flex-wrap items-center text-[0.95rem]">
          <li>
            <Link
              href={cancelHref}
              className="mr-2 inline-flex rounded-md border border-[var(--atlas-home-border-soft)] px-3 py-1.5 text-[0.9rem] text-[var(--atlas-home-link)] hover:bg-muted/40"
            >
              Cancel
            </Link>
          </li>
          {REPORT_EDIT_WIZARD_STEPS.map((wizardStep, index) => (
            <li key={wizardStep.id}>
              {index > 0 ? (
                <span className="mx-1.5 text-[var(--atlas-home-muted-light)]">/</span>
              ) : null}
              <a
                href={`#${wizardStep.id}`}
                onClick={(event) => {
                  event.preventDefault()
                  goToStep(wizardStep.id)
                }}
                className={
                  step === wizardStep.id
                    ? "text-[0.9rem] font-medium text-[var(--atlas-home-link)] hover:underline"
                    : "text-[0.9rem] text-[var(--atlas-home-link)] hover:text-[var(--atlas-home-link-hover)] hover:underline"
                }
                aria-current={step === wizardStep.id ? "step" : undefined}
              >
                {wizardStep.label}
              </a>
            </li>
          ))}
        </ul>
      </nav>

      {step === "description" ? (
        <section id="description" className="scroll-mt-24">
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
        </section>
      ) : null}

      {step === "meta" ? (
        <section id="meta" className="scroll-mt-24">
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
        </section>
      ) : null}

      {step === "images" ? (
        <section id="images" className="scroll-mt-24">
          <Card>
            <CardHeader>
              <CardTitle>Images</CardTitle>
            </CardHeader>
            <CardContent className="space-y-4">
              {existingImages.length > 0 ? (
                <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
                  {existingImages.map((image) => (
                    <div key={image.id} className="rounded-md border border-[var(--atlas-home-border-soft)] p-2">
                      <img
                        src={getReportObjectImageThumbUrl(image.id)}
                        alt={`Report image ${image.id}`}
                        className="h-auto w-full max-h-64 rounded-md border border-[var(--atlas-home-border-soft)] object-cover"
                      />
                    </div>
                  ))}
                </div>
              ) : (
                <p className="text-sm text-muted-foreground">No images uploaded yet.</p>
              )}
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
        </section>
      ) : null}

      {step === "maintenance" ? (
        <section id="maintenance" className="scroll-mt-24">
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
        </section>
      ) : null}

      {step === "complete" ? (
        <section id="complete" className="scroll-mt-24">
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
        </section>
      ) : null}
    </div>
  )
}
