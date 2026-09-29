import { ChevronRight } from "lucide-react"

export type ReportSectionLink = {
  href: string
  label: string
}

export function ReportSectionNav({ links }: { links: ReportSectionLink[] }) {
  if (links.length === 0) return null

  return (
    <nav aria-label="Report sections" className="text-sm">
      <ol className="flex flex-wrap items-center gap-1">
        {links.map((link, index) => (
          <li key={link.href} className="flex items-center gap-1">
            {index > 0 ? (
              <ChevronRight className="size-3.5 text-muted-foreground" aria-hidden />
            ) : null}
            <a
              href={link.href}
              className="rounded-md px-1.5 py-0.5 font-medium text-link hover:bg-muted hover:underline"
            >
              {link.label}
            </a>
          </li>
        ))}
      </ol>
    </nav>
  )
}

export function buildReportSectionLinks(report: {
  images?: Array<unknown> | null
  description?: string | null
  detailedDescription?: string | null
  repositoryDescription?: string | null
  document?: {
    developerDescription?: string | null
    keyAssumptions?: string | null
    maintenanceLogs?: Array<unknown> | null
  } | null
  terms?: Array<unknown> | null
  features?: { termsEnabled?: boolean }
  queries?: Array<unknown> | null
  componentQueries?: Array<unknown> | null
  parents?: Array<unknown> | null
  children?: Array<unknown> | null
  groups?: Array<unknown> | null
  collections?: Array<unknown> | null
  canViewGroups?: boolean
}): ReportSectionLink[] {
  const hasDescription =
    Boolean(report.document?.developerDescription?.trim()) ||
    Boolean(report.document?.keyAssumptions?.trim()) ||
    Boolean(report.description?.trim()) ||
    Boolean(report.detailedDescription?.trim()) ||
    Boolean(report.repositoryDescription?.trim())

  const links: ReportSectionLink[] = []

  if (report.images && report.images.length > 0) {
    links.push({ href: "#images", label: "Images" })
  }
  if (hasDescription) {
    links.push({ href: "#description", label: "Description" })
  }
  if (report.features?.termsEnabled !== false && report.terms && report.terms.length > 0) {
    links.push({ href: "#terms", label: "Terms" })
  }
  links.push({ href: "#details", label: "Details" })
  if (
    (report.queries && report.queries.length > 0) ||
    (report.componentQueries && report.componentQueries.length > 0)
  ) {
    links.push({ href: "#query", label: "Query" })
  }
  if (
    (report.parents && report.parents.length > 0) ||
    (report.children && report.children.length > 0) ||
    (report.canViewGroups && report.groups && report.groups.length > 0) ||
    (report.collections && report.collections.length > 0)
  ) {
    links.push({ href: "#relationships", label: "Relationships" })
  }
  if (report.document?.maintenanceLogs && report.document.maintenanceLogs.length > 0) {
    links.push({ href: "#maintenance", label: "Maintenance" })
  }

  return links
}
