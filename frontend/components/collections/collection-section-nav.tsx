export function CollectionSectionNav({
  hasInitiative,
  hasReports,
  hasTerms,
}: {
  hasInitiative: boolean
  hasReports: boolean
  hasTerms: boolean
}) {
  const links = [
    { href: "#details", label: "Details" },
    ...(hasReports ? [{ href: "#reports", label: "Linked Reports" }] : []),
    ...(hasTerms ? [{ href: "#terms", label: "Linked Terms" }] : []),
    ...(hasInitiative ? [{ href: "#initiative", label: "Owning Initiative" }] : []),
  ]

  return (
    <nav aria-label="Collection sections" className="flex flex-wrap gap-3 text-sm">
      {links.map((link) => (
        <a key={link.href} href={link.href} className="text-link hover:underline">
          {link.label}
        </a>
      ))}
    </nav>
  )
}
