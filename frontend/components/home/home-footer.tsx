import Link from "next/link"

const footerSections = [
  {
    title: "Status",
    links: [
      { label: "Documentation", href: "https://atlas.bi" },
      { label: "Source Code", href: "https://github.com/atlas-bi/atlas-bi-library" },
      { label: "Status", href: "https://status.atlas.bi/status/atlas" },
    ],
  },
]

export function HomeFooter() {
  const year = new Date().getFullYear()

  return (
    <footer className="atlas-home-footer mt-10 border-t border-[var(--atlas-home-border-soft)]">
      <div className="container mx-auto grid w-full gap-5 px-4 py-7 sm:px-6 md:px-8 md:grid-cols-[1fr_minmax(84px,1fr)_170px]">
        <div className="space-y-2">
          <Link href="/" className="group flex shrink-0 items-center">
            <img src="/logo.svg" alt="Atlas logo" className="h-9 w-9 shrink-0" />
            <span className="atlas-brand-lockup m-0 scroll-m-20 text-2xl font-medium text-slate-700 transition-colors hover:text-slate-900">
              <span className="mx-2 text-slate-500 group-hover:text-sky-700">/</span>
              library
            </span>
          </Link>
          <p className="atlas-home-footer-subtitle mt-4 mb-2">
            Atlas was created by the Riverside Healthcare Analytics team.
          </p>
          <p className="atlas-home-footer-copyright mt-4">
            &copy; {year} Example Healthcare | Release 3.15.2-alpha.1
          </p>
        </div>

        <div className="hidden md:block" />

        <div className="grid content-start gap-1.5 text-center">
          {footerSections.map((section) => (
            <div key={section.title} className="space-y-1">
              <strong className="atlas-home-footer-heading block">{section.title}</strong>
              {section.links.map((link) => (
                <a
                  key={link.label}
                  href={link.href}
                  target="_blank"
                  rel="noreferrer"
                  className="atlas-home-footer-link block"
                >
                  {link.label}
                </a>
              ))}
            </div>
          ))}
        </div>
      </div>
    </footer>
  )
}
