import Link from "next/link"
import { formatAnalyticsCount, formatAnalyticsPercent } from "@/lib/analytics/format"
import type { AnalyticsBarItemDto } from "@/lib/analytics/types"

export function AnalyticsBarDataTable({ items }: { items: AnalyticsBarItemDto[] }) {
  if (items.length === 0) {
    return (
      <p className="py-2 text-sm text-[#7a7a7a]">No data for this range.</p>
    )
  }

  const columnTitle = items[0]?.titleOne ?? "Name"
  const valueTitle = items[0]?.titleTwo ?? "Count"
  const showPercent = items.some((item) => item.percent != null)

  return (
    <table className="analytics-bar-table w-full border-collapse text-sm">
      <thead>
        <tr>
          <th className="pb-2 text-left font-bold text-[#363636]">{columnTitle}</th>
          <th className="pb-2 text-right font-bold text-[#363636]">{valueTitle}</th>
        </tr>
      </thead>
      <tbody>
        {items.map((item) => (
          <tr key={`${item.key}-${item.href ?? ""}`} className="group">
            <td className="py-2 pr-2 align-middle text-[#363636]">
              {item.href ? (
                <Link href={item.href} className="text-[#3273dc] hover:underline">
                  {item.key}
                </Link>
              ) : (
                item.key
              )}
            </td>
            <td className="py-2 text-right align-middle whitespace-nowrap">
              <strong className="mr-2 inline-block min-w-[1.5rem] text-right">
                {formatAnalyticsCount(item.count)}
              </strong>
              {showPercent && item.percent != null ? (
                <span className="analytics-bar-percent relative inline-block min-w-[100px] py-2 text-center align-middle">
                  <span
                    className="analytics-bar-percent-fill absolute inset-y-0 left-0"
                    style={{ width: `${Math.min(100, Math.round(item.percent * 100))}%` }}
                  />
                  <span className="relative text-xs font-bold text-[#b5b5b5]">
                    {formatAnalyticsPercent(item.percent)}
                  </span>
                </span>
              ) : null}
            </td>
          </tr>
        ))}
      </tbody>
    </table>
  )
}
