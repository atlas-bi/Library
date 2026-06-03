import Image from "next/image"
import Link from "next/link"
import { HomeFooter } from "@/components/home/home-footer"
import { HomeNavbarClient } from "@/components/home/home-navbar-client"
import { HomeSearchClient } from "@/components/home/home-search-client"
import { HomeTabsClient } from "@/components/home/home-tabs-client"
import type { HomeTabRequestContext, HomeTabsVisibility } from "@/lib/home/types"

type HomeShellProps = {
  displayName: string
  isSignedIn: boolean
  isAdministrator: boolean
  adminEnabled: boolean
  requestContext: HomeTabRequestContext
  visibleTabs: HomeTabsVisibility
}

export function HomeShell({
  displayName,
  isSignedIn,
  isAdministrator,
  adminEnabled,
  requestContext,
  visibleTabs,
}: HomeShellProps) {
  return (
    <div className="atlas-home-surface flex min-h-screen flex-col bg-white font-sans">
      <header className="sticky top-0 z-20 border-b border-[var(--atlas-home-border-soft)] bg-white shadow-[0_2px_12px_rgba(10,10,10,0.04)]">
        <div className="mx-auto flex min-h-[var(--atlas-home-navbar-height)] w-full max-w-[1280px] items-center gap-2 px-4">
          <Link href="/" className="atlas-home-brand flex min-w-[156px] items-center gap-2">
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

          <div className="flex flex-1 items-center self-stretch px-0.5 lg:pt-0">
            <HomeSearchClient />
          </div>

          <HomeNavbarClient
            displayName={displayName}
            isSignedIn={isSignedIn}
            isAdministrator={isAdministrator}
            adminEnabled={adminEnabled}
          />
        </div>
      </header>

      <main className="mx-auto w-full max-w-[1280px] flex-1 px-4 py-4">
        <h1 className="atlas-home-heading">Hi, {displayName}!</h1>

        <div>
          <HomeTabsClient requestContext={requestContext} visibleTabs={visibleTabs} />
        </div>
      </main>

      <HomeFooter />
    </div>
  )
}
