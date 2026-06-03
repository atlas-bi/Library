import Image from "next/image"
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
      <div className="mx-auto grid w-full max-w-[1280px] gap-5 px-4 py-7 md:grid-cols-[1fr_minmax(84px,1fr)_170px]">
        <div className="space-y-2">
          <Link href="/" className="atlas-home-brand flex items-center gap-2">
            <Image
              src="/favicon.ico"
              alt="Atlas logo"
              width={35}
              height={35}
              className="h-[35px] w-[35px]"
            />
            <h2 className="m-0 text-[0.95rem] leading-6 font-medium transition-colors">
              <span className="atlas-home-brand-slash mx-1">/</span>
              library
            </h2>
          </Link>
          <p className="text-[0.8125rem] text-[var(--atlas-home-text)]">
            Atlas was created by the Riverside Healthcare Analytics team.
          </p>
          <p className="text-[0.8125rem] text-[var(--atlas-home-muted)]">
            &copy; {year} Example Healthcare | Release 3.15.2-alpha.1
          </p>
        </div>

        <div className="hidden md:block" />

        <div className="grid content-start gap-1.5 text-center">
          {footerSections.map((section) => (
            <div key={section.title} className="space-y-1">
              <strong className="block text-[0.95rem] font-semibold text-[var(--atlas-home-title)]">
                {section.title}
              </strong>
              {section.links.map((link) => (
                <a
                  key={link.label}
                  href={link.href}
                  target="_blank"
                  rel="noreferrer"
                  className="block text-[0.8125rem] text-[var(--atlas-home-link)] hover:text-[var(--atlas-home-link-hover)] hover:underline"
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
