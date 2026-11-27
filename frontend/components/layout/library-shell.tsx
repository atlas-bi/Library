import Link from "next/link"
import type { ReactNode } from "react"
import { HomeFooter } from "@/components/home/home-footer"
import { HomeNavbarClient } from "@/components/home/home-navbar-client"
import { HomeSearchClient } from "@/components/home/home-search-client"

type LibraryShellProps = {
  displayName: string
  isSignedIn: boolean
  isAdministrator: boolean
  adminEnabled: boolean
  searchPlaceholder?: string
  children: ReactNode
}

export function LibraryShell({
  displayName,
  isSignedIn,
  isAdministrator,
  adminEnabled,
  searchPlaceholder = "type to search..",
  children,
}: LibraryShellProps) {
  return (
    <div className="atlas-home-surface flex min-h-screen flex-col bg-white font-sans">
      <header className="sticky top-0 z-20 h-16 border-b border-[var(--atlas-home-border-soft)] bg-white shadow-[0_2px_12px_rgba(10,10,10,0.04)]">
        <div className="flex h-16 items-center px-4 container mx-auto">
          <div className="flex min-w-0 flex-1 items-center gap-4 md:mx-6 lg:gap-6">
            <Link href="/" className="group flex shrink-0 items-center">
              <img src="/logo.svg" alt="Atlas logo" className="h-9 w-9 shrink-0" />
              <span
                className="atlas-brand-lockup m-0 scroll-m-20 text-2xl font-medium text-slate-700 transition-colors hover:text-slate-900 max-lg:hidden"
                aria-hidden="true"
              >
                <span className="mx-2 text-slate-500 group-hover:text-sky-700">/</span>
                library
              </span>
            </Link>

            <div className="min-w-0 flex-1">
              <HomeSearchClient placeholder={searchPlaceholder} />
            </div>

            <HomeNavbarClient
              displayName={displayName}
              isSignedIn={isSignedIn}
              isAdministrator={isAdministrator}
              adminEnabled={adminEnabled}
            />
          </div>
        </div>
      </header>

      <main className="container mx-auto w-full flex-1 py-4">{children}</main>

      <HomeFooter />
    </div>
  )
}
