"use client"

import { type ReactNode, useEffect, useRef } from "react"

interface Props {
  defaultTab: string
  children: ReactNode
}

export function SettingsTabController({ defaultTab, children }: Props) {
  const containerRef = useRef<HTMLDivElement>(null)

  function activateTab(tabId: string) {
    const container = containerRef.current
    if (!container) return

    // Hide all panels by adding 'hidden'
    const panels = container.querySelectorAll<HTMLElement>(".panel-tab-data")
    panels.forEach((p) => {
      p.classList.add("hidden")
      p.classList.remove("block")
    })

    // Show target
    const target = container.querySelector<HTMLElement>(`#${tabId}`)
    if (target) {
      target.classList.remove("hidden")
      target.classList.add("block")
    }

    // Update nav active state
    document.querySelectorAll(".panel-tab").forEach((a) => {
      const link = a as HTMLAnchorElement
      const isActive = link.hash === `#${tabId}`
      const iconSpan = link.querySelector("span")

      if (isActive) {
        link.classList.add("text-[#485fc7]", "bg-[#f5f5f5]")
        link.classList.remove("text-[#363636]", "hover:bg-[#f5f5f5]")
        if (iconSpan) {
          iconSpan.classList.add("text-[#485fc7]")
          iconSpan.classList.remove("text-[#4a4a4a]")
        }
      } else {
        link.classList.remove("text-[#485fc7]", "bg-[#f5f5f5]")
        link.classList.add("text-[#363636]", "hover:bg-[#f5f5f5]")
        if (iconSpan) {
          iconSpan.classList.remove("text-[#485fc7]")
          iconSpan.classList.add("text-[#4a4a4a]")
        }
      }
    })
  }

  useEffect(() => {
    // Read hash on mount
    const hash = window.location.hash.replace("#", "") || defaultTab
    activateTab(hash)

    // Listen for hash changes
    const handleHashChange = () => {
      const id = window.location.hash.replace("#", "") || defaultTab
      activateTab(id)
    }

    window.addEventListener("hashchange", handleHashChange)

    // Intercept panel-tab link clicks
    const navLinks = document.querySelectorAll<HTMLAnchorElement>("a.panel-tab")
    const clickHandlers = Array.from(navLinks).map((link) => {
      const handler = (e: Event) => {
        e.preventDefault()
        const id = link.hash.replace("#", "")
        window.history.pushState(null, "", `#${id}`)
        activateTab(id)
      }
      link.addEventListener("click", handler)
      return { link, handler }
    })

    return () => {
      window.removeEventListener("hashchange", handleHashChange)
      clickHandlers.forEach(({ link, handler }) => {
        link.removeEventListener("click", handler)
      })
    }
  }, [defaultTab])

  return <div ref={containerRef}>{children}</div>
}
