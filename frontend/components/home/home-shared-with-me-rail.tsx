import Link from "next/link"
import type { HomeSharedWithMeItem } from "@/lib/home/types"

export function HomeSharedWithMeRail({ items }: { items: HomeSharedWithMeItem[] }) {
  if (items.length === 0) {
    return null
  }

  return (
    <aside className="sticky top-20 self-start space-y-3 rounded-md border border-[var(--atlas-home-border-soft)] bg-white p-4">
      <strong className="block text-sm text-[var(--atlas-home-text-strong)]">Shared With Me</strong>
      <ul className="space-y-2 text-sm">
        {items.map((item) => (
          <li key={item.id}>
            {item.href ? (
              <Link href={item.href} className="text-[var(--atlas-home-link)] hover:underline">
                {item.name}
              </Link>
            ) : (
              item.name
            )}
            {item.sharedFrom ? (
              <div className="text-xs text-[var(--atlas-home-muted)]">
                Shared by {item.sharedFrom}
                {item.shareDate ? ` on ${item.shareDate}` : ""}
              </div>
            ) : null}
          </li>
        ))}
      </ul>
    </aside>
  )
}