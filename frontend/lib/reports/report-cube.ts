import type { ReportDetail } from "@/lib/reports/types"

export function isCubeReportType(report: ReportDetail): boolean {
  const label = [report.typeName, report.typeShortName].filter(Boolean).join(" ").toLowerCase()
  return label.includes("cube") || label.includes("ssas")
}

export function defaultCubeFileName(report: ReportDetail): string {
  return `${report.name}.odc`
}
