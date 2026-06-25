import { ChevronRight } from "lucide-react"

/** In-page section links matching Razor `Collections/_Links.cshtml`. */
export function CollectionSectionNav({
  hasReports,
  hasTerms,
}: {
  hasReports: boolean
  hasTerms: boolean
}) {
  const links = [
    { href: "#details", label: "Details" },
    ...(hasReports ? [{ href: "#reports", label: "Linked Reports" }] : []),
    ...(hasTerms ? [{ href: "#terms", label: "Linked Terms" }] : []),
  ]

  return (
    <nav aria-label="Collection sections" className="text-sm">
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
