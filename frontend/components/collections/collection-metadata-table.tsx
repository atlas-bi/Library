import type { ReactNode } from "react"
import type { CollectionDetailDto } from "@/lib/collections/types"

export function CollectionMetadataTable({ collection }: { collection: CollectionDetailDto }) {
  const rows: Array<{ label: string; value: ReactNode }> = []

  const updatedBy = collection.lastUpdatedBy?.fullName?.trim() || collection.lastUpdatedBy?.username
  if (updatedBy) {
    rows.push({ label: "Last Updated By", value: updatedBy })
  }
  if (collection.lastModifiedDisplay) {
    rows.push({ label: "Last Updated", value: collection.lastModifiedDisplay })
  }
  if (collection.hidden === "Y") {
    rows.push({ label: "Hidden from Search?", value: "Yes" })
  }

  if (rows.length === 0) return null

  return (
    <div className="mt-4 overflow-x-auto rounded-lg border bg-card shadow-sm">
      <table className="w-full text-sm" aria-label="Collection details">
        <tbody>
          {rows.map((row, index) => (
            <tr
              key={row.label}
              className={index < rows.length - 1 ? "border-b border-border/60" : undefined}
            >
              <th
                scope="row"
                className="w-52 whitespace-nowrap bg-muted/30 px-4 py-3 text-left font-medium text-muted-foreground"
              >
                {row.label}
              </th>
              <td className="px-4 py-3 text-foreground">{row.value}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}
