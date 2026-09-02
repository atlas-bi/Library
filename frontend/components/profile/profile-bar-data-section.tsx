import Link from "next/link"
import { ChevronsUpDown } from "lucide-react"
import type { ProfileBarItemDto } from "@/lib/profile/types"

function getBarTitle(item: ProfileBarItemDto) {
  return item.title ?? item.key
}

function getBarSubtitle(item: ProfileBarItemDto) {
  return item.titleTwo ?? item.dateTitle ?? item.date ?? item.dateLabel ?? null
}

export function ProfileBarDataSection({
  title, // Fallback title
  items,
  defaultTitleTwo = "Runs",
}: {
  title: string
  items: ProfileBarItemDto[]
  defaultTitleTwo?: string
}) {
  if (items.length === 0) return null

  const first = items[0]
  const titleOne = first?.titleOne || title
  const dateTitle = first?.dateTitle || null
  const titleTwo = first?.titleTwo || defaultTitleTwo

  return (
    <div className="w-full">
      <table className="w-full border-collapse text-xs md:text-sm">
        <thead>
          <tr className="border-b-0 text-left">
            <th className="pb-3 pr-2 font-bold whitespace-nowrap">
              <div className="flex items-center gap-1 cursor-default">
                {titleOne}
                <ChevronsUpDown className="size-3 text-muted-foreground" />
              </div>
            </th>
            {dateTitle ? (
              <th className="pb-3 px-2 font-bold whitespace-nowrap">
                <div className="flex items-center gap-1 cursor-default">
                  {dateTitle}
                  <ChevronsUpDown className="size-3 text-muted-foreground" />
                </div>
              </th>
            ) : null}
            <th className="pb-3 pl-2 font-bold whitespace-nowrap text-right w-1">
              <div className="flex items-center justify-end gap-1 cursor-default">
                {titleTwo}
                <ChevronsUpDown className="size-3 text-muted-foreground" />
              </div>
            </th>
          </tr>
        </thead>
        <tbody>
          {items.map((item, idx) => {
            const label = getBarTitle(item)
            const hasDateColumn = !!dateTitle
            const dateValue = item.date || ""
            
            // Format number e.g. 1,000
            const countFormatted = typeof item.count === "number" ? item.count.toLocaleString() : "0"

            return (
              <tr key={item.key || idx} className="group">
                <td className="py-2 pr-2 align-middle max-w-[150px] md:max-w-[200px] truncate">
                  {item.href ? (
                    <Link href={item.href} className="font-medium text-[var(--atlas-home-link,theme(colors.blue.600))] hover:underline">
                      {label}
                    </Link>
                  ) : (
                    <span className="font-medium text-[var(--atlas-home-link,theme(colors.blue.600))]">{label}</span>
                  )}
                </td>
                
                {hasDateColumn ? (
                  <td className="py-2 px-2 align-middle whitespace-nowrap text-muted-foreground">
                    {dateValue}
                  </td>
                ) : null}

                <td className="py-2 pl-2 align-middle text-right whitespace-nowrap">
                  <div className="flex items-center justify-end gap-2">
                    <strong className="text-foreground">{countFormatted}</strong>
                    {typeof item.percent === "number" ? (
                      <div className="relative inline-flex items-center justify-center overflow-hidden bg-[rgba(38,128,235,0.1)] rounded-sm min-w-[36px] h-5">
                        <div 
                          className="absolute left-0 top-0 bottom-0 bg-[rgba(38,128,235,0.2)]" 
                          style={{ width: `${Math.round(item.percent * 100)}%` }} 
                        />
                        <span className="relative z-10 text-[10px] font-bold text-muted-foreground px-1">
                          {Math.round(item.percent * 100)}%
                        </span>
                      </div>
                    ) : null}
                  </div>
                </td>
              </tr>
            )
          })}
        </tbody>
      </table>
    </div>
  )
}

export function ProfileBarDataGrid({
  type,
  id,
  users,
  reports,
  fails,
}: {
  type: string
  id: number
  users: ProfileBarItemDto[]
  reports: ProfileBarItemDto[]
  fails: ProfileBarItemDto[]
}) {
  const showUsers = type !== "user" && users.length > 0
  const showReports = (type === "user" || type === "term" || type === "collection" || (type === "report" && id === -1)) && reports.length > 0
  const showFails = fails.length > 0

  // If no data to show, render nothing (which matches Razor empty state)
  if (!showUsers && !showReports && !showFails) {
    return null
  }

  return (
    <div className="grid gap-x-8 gap-y-6 md:grid-cols-3 pt-4">
      {showUsers ? <ProfileBarDataSection title="Top Users" items={users} /> : null}
      {showReports ? <ProfileBarDataSection title="Top Reports" items={reports} /> : null}
      {showFails ? <ProfileBarDataSection title="Failed Runs" items={fails} defaultTitleTwo="Fails" /> : null}
    </div>
  )
}
