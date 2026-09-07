import { ChevronRight } from "lucide-react"

/** In-page section links matching Razor `Terms/_Links.cshtml`. */
export function TermSectionNav({
  hasDescription,
  hasReports,
}: {
  hasDescription: boolean
  hasReports: boolean
}) {
  const links = [
    ...(hasDescription ? [{ href: "#details", label: "Details" }] : []),
    ...(hasReports ? [{ href: "#reports", label: "Linked Reports" }] : []),
    { href: "#meta", label: "Metadata" },
  ]

  return (
    <nav aria-label="breadcrumbs" className="breadcrumb text-sm mb-0">
      <ul className="flex flex-wrap items-center gap-1">
        {links.map((link, index) => (
          <li key={link.href} className="flex items-center">
            {index > 0 ? (
              <span className="mx-2 text-[var(--atlas-home-muted-light)]">/</span>
            ) : null}
            <a
              href={link.href}
              className="text-[var(--atlas-home-link)] hover:text-[var(--atlas-home-link-hover)] hover:underline"
            >
              {link.label}
            </a>
          </li>
        ))}
      </ul>
    </nav>
  )
}
