import type { ReportDetail, ReportUpdateBody } from "@/lib/reports/types"

function ynFlag(value: unknown): "Y" | "N" {
  return String(value ?? "N").toUpperCase() === "Y" ? "Y" : "N"
}

function defaultImageIds(report: ReportDetail): number[] {
  const images = report.images ?? []
  return [...images]
    .sort((a, b) => (a.ordinal ?? 0) - (b.ordinal ?? 0))
    .map((image) => image.id)
}

export function buildReportUpdateBodyFromDetail(
  report: ReportDetail,
  overrides?: Pick<Partial<ReportUpdateBody>, "imageIds">,
): ReportUpdateBody {
  const doc = report.document ?? {}

  return {
    gitLabProjectUrl: doc.gitLabProjectUrl?.trim() || null,
    developerDescription: doc.developerDescription?.trim() || null,
    keyAssumptions: doc.keyAssumptions?.trim() || null,
    developerNotes: doc.developerNotes?.trim() || null,
    operationalOwnerUserId: doc.operationalOwner?.id ?? null,
    requesterUserId: doc.requester?.id ?? null,
    organizationalValueId: doc.organizationalValue?.id ?? null,
    estimatedRunFrequencyId: doc.estimatedRunFrequency?.id ?? null,
    fragilityId: doc.fragility?.id ?? null,
    maintenanceScheduleId: doc.maintenanceSchedule?.id ?? null,
    executiveVisibilityYn: ynFlag(doc.executiveVisibilityYn),
    enabledForHyperspace: ynFlag(doc.enabledForHyperspace),
    doNotPurge: ynFlag(doc.doNotPurge),
    hidden: ynFlag(doc.hidden),
    termIds: (report.terms ?? []).map((term) => term.id),
    collectionIds: (report.collections ?? []).map((collection) => collection.id),
    fragilityTagIds: (doc.fragilityTags ?? []).map((tag) => tag.id),
    imageIds: overrides?.imageIds ?? defaultImageIds(report),
    serviceRequestIds: (doc.serviceRequests ?? []).map((request) => request.id),
    newServiceRequest: null,
    newMaintenanceLog: null,
  }
}