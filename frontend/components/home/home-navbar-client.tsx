"use client"

import { ChevronDown, List, UserRound, Wrench } from "lucide-react"
import Link from "next/link"
import { useEffect, useRef, useState } from "react"

type HomeNavbarClientProps = {
  displayName: string
  isSignedIn: boolean
  isAdministrator: boolean
  adminEnabled: boolean
}

type MenuId = "tools" | "library" | "user" | null

type DropdownItem = {
  label: string
  href: string
  dividerBefore?: boolean
}

function DropdownMenu({
  open,
  children,
  onMouseEnter,
  onMouseLeave,
}: {
  open: boolean
  children: React.ReactNode
  onMouseEnter?: () => void
  onMouseLeave?: () => void
}) {
  if (!open) return null

  return (
    <div
      role="menu"
      className="atlas-home-dropdown absolute top-full right-0 z-30 mt-0 min-w-52 py-2"
      onMouseEnter={onMouseEnter}
      onMouseLeave={onMouseLeave}
    >
      {children}
    </div>
  )
}

function DropdownLink({ href, children }: { href: string; children: React.ReactNode }) {
  return (
    <Link href={href} className="atlas-home-dropdown-item block px-4 py-2 text-sm">
      {children}
    </Link>
  )
}

function DropdownItems({ items }: { items: DropdownItem[] }) {
  return items.map((item) => (
    <div key={item.label}>
      {item.dividerBefore ? <hr className="atlas-home-dropdown-divider my-1" /> : null}
      <DropdownLink href={item.href}>{item.label}</DropdownLink>
    </div>
  ))
}

export function HomeNavbarClient({
  displayName,
  isSignedIn,
  isAdministrator,
  adminEnabled,
}: HomeNavbarClientProps) {
  const [openMenu, setOpenMenu] = useState<MenuId>(null)
  const containerRef = useRef<HTMLDivElement | null>(null)
  const closeTimerRef = useRef<number | null>(null)
  const toolsItems: DropdownItem[] = [
    { label: "Settings", href: "/settings" },
    { label: "Analytics", href: "/analytics" },
    { label: "Tasks", href: "/tasks" },
    {
      label: "Report Activity",
      href: "/profile",
      dividerBefore: true,
    },
  ]
  const libraryItems: DropdownItem[] = [
    { label: "Initiatives", href: "/initiatives" },
    { label: "Collections", href: "/collections" },
    { label: "Terms", href: "/terms" },
    {
      label: "About Analytics",
      href: "/about_analytics",
      dividerBefore: true,
    },
  ]
  const userItems: DropdownItem[] = [
    { label: "Your profile", href: "/users" },
    { label: "Your stars", href: "/users#stars" },
    {
      label: "Settings",
      href: "/users/settings",
      dividerBefore: true,
    },
  ]

  useEffect(() => {
    function handleOutsideClick(event: MouseEvent) {
      if (!containerRef.current?.contains(event.target as Node)) {
        setOpenMenu(null)
      }
    }

    document.addEventListener("mousedown", handleOutsideClick)
    return () => {
      document.removeEventListener("mousedown", handleOutsideClick)
      if (closeTimerRef.current !== null) {
        window.clearTimeout(closeTimerRef.current)
      }
    }
  }, [])

  const toggleMenu = (menu: Exclude<MenuId, null>) => {
    setOpenMenu((current) => (current === menu ? null : menu))
  }

  const openMenuOnHover = (menu: Exclude<MenuId, null>) => {
    if (closeTimerRef.current !== null) {
      window.clearTimeout(closeTimerRef.current)
      closeTimerRef.current = null
    }
    setOpenMenu(menu)
  }

  const scheduleCloseMenus = () => {
    if (closeTimerRef.current !== null) {
      window.clearTimeout(closeTimerRef.current)
    }

    closeTimerRef.current = window.setTimeout(() => {
      setOpenMenu(null)
      closeTimerRef.current = null
    }, 120)
  }

  const handleAdminToggle = () => {
    const currentPath =
      typeof window === "undefined"
        ? "/"
        : `${window.location.pathname}${window.location.search}${window.location.hash}`

    window.location.assign(`/auth/admin-mode?returnTo=${encodeURIComponent(currentPath)}`)
  }

  return (
    <div
      ref={containerRef}
      className="flex items-center gap-1 text-sm text-[var(--atlas-home-text)]"
    >
      <div className="relative">
        <button
          type="button"
          aria-label="Tools menu"
          className="atlas-home-navbar-link inline-flex cursor-pointer items-center gap-1 px-2 py-2"
          onClick={() => toggleMenu("tools")}
          onMouseEnter={() => openMenuOnHover("tools")}
          onMouseLeave={scheduleCloseMenus}
        >
          <Wrench className="h-[1.1rem] w-[1.1rem]" strokeWidth={1.8} />
          <ChevronDown className="h-3.5 w-3.5" strokeWidth={1.8} />
        </button>
        <DropdownMenu
          open={openMenu === "tools"}
          onMouseEnter={() => openMenuOnHover("tools")}
          onMouseLeave={scheduleCloseMenus}
        >
          <DropdownItems items={toolsItems} />
        </DropdownMenu>
      </div>

      <div className="relative">
        <button
          type="button"
          aria-label="Library menu"
          className="atlas-home-navbar-link inline-flex cursor-pointer items-center gap-1 px-2 py-2"
          onClick={() => toggleMenu("library")}
          onMouseEnter={() => openMenuOnHover("library")}
          onMouseLeave={scheduleCloseMenus}
        >
          <List className="h-[1.1rem] w-[1.1rem]" strokeWidth={1.8} />
          <ChevronDown className="h-3.5 w-3.5" strokeWidth={1.8} />
        </button>
        <DropdownMenu
          open={openMenu === "library"}
          onMouseEnter={() => openMenuOnHover("library")}
          onMouseLeave={scheduleCloseMenus}
        >
          <DropdownItems items={libraryItems} />
        </DropdownMenu>
      </div>

      <div className="relative">
        <button
          type="button"
          aria-label="User menu"
          className="atlas-home-navbar-link inline-flex cursor-pointer items-center gap-1 px-2 py-2"
          onClick={() => toggleMenu("user")}
          onMouseEnter={() => openMenuOnHover("user")}
          onMouseLeave={scheduleCloseMenus}
        >
          <UserRound className="h-[1.1rem] w-[1.1rem]" strokeWidth={1.8} />
          <ChevronDown className="h-3.5 w-3.5" strokeWidth={1.8} />
        </button>
        <DropdownMenu
          open={openMenu === "user"}
          onMouseEnter={() => openMenuOnHover("user")}
          onMouseLeave={scheduleCloseMenus}
        >
          <div className="px-4 py-2 text-sm font-medium text-[var(--atlas-home-title)]">
            {displayName}
          </div>
          <DropdownItems items={userItems} />
          <hr className="atlas-home-dropdown-divider my-1" />
          {isSignedIn ? (
            <DropdownLink href="/auth/logout">Sign out</DropdownLink>
          ) : (
            <DropdownLink href="/auth/login">Sign in</DropdownLink>
          )}
        </DropdownMenu>
      </div>

      {isAdministrator ? (
        <button
          type="button"
          onClick={handleAdminToggle}
          className="ml-3 inline-flex cursor-pointer items-center gap-2"
        >
          <span
            className={`atlas-home-admin-toggle relative inline-flex h-7 w-12 items-center rounded-full ${
              adminEnabled ? "bg-[var(--atlas-home-link)]" : "bg-[var(--atlas-home-muted-light)]"
            }`}
          >
            <span
              className={`absolute h-5 w-5 rounded-full bg-white transition-transform ${
                adminEnabled ? "translate-x-6" : "translate-x-1"
              }`}
            />
          </span>
          <strong className="text-base font-semibold text-[var(--atlas-home-title)]">Admin</strong>
        </button>
      ) : null}

      {!isSignedIn ? (
        <Link
          href="/auth/login"
          className="px-2 text-sm text-[var(--atlas-home-link)] hover:underline"
        >
          Sign in
        </Link>
      ) : null}
    </div>
  )
}
