import Link from "next/link"
import type { ReactNode } from "react"
import type { TermDetailDto } from "@/lib/terms/types"

export function TermMetadataTable({ term }: { term: TermDetailDto }) {
  const canLinkProfiles =
    term.features?.userProfilesEnabled !== false && term.permissions?.canViewUserProfiles

  const rows: Array<{ label: string; value: ReactNode }> = []

  if (term.isApproved) {
    if (term.approvalDateDisplay) {
      rows.push({ label: "Approval Date", value: term.approvalDateDisplay })
    }
    rows.push({
      label: "Approved By",
      value:
        term.approvedBy?.id && canLinkProfiles ? (
          <Link href={`/users?id=${term.approvedBy.id}`} className="text-link hover:underline">
            {term.approvedBy.fullName || term.approvedBy.username}
          </Link>
        ) : (
          term.approvedBy?.fullName || term.approvedBy?.username || ""
        ),
    })
  } else {
    rows.push({ label: "Has Been Approved", value: "No" })
  }

  rows.push({
    label: "Last Updated By",
    value:
      term.lastUpdatedBy?.id && canLinkProfiles ? (
        <Link href={`/users?id=${term.lastUpdatedBy.id}`} className="text-link hover:underline">
          {term.lastUpdatedBy.fullName || term.lastUpdatedBy.username}
        </Link>
      ) : (
        term.lastUpdatedBy?.fullName || term.lastUpdatedBy?.username || ""
      ),
  })

  if (term.lastUpdatedDisplay) {
    rows.push({ label: "Last Updated", value: term.lastUpdatedDisplay })
  }

  return (
    <div className="mt-4 overflow-x-auto">
      <table className="text-sm" aria-label="Term details">
        <tbody>
          {rows.map((row) => (
            <tr key={row.label}>
              <th
                scope="row"
                className="w-48 whitespace-nowrap px-0 py-1.5 text-left font-medium text-[var(--atlas-home-text-strong)]"
              >
                {row.label}
              </th>
              <td className="px-4 py-1.5 text-[var(--atlas-home-text)]">{row.value}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}
