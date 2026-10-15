"use client"

import { useEffect, useState } from "react"
import { HomeTabPanel } from "@/components/home/home-tab-panel"
import { fetchHomeTabPanel } from "@/lib/home/client-api"
import { HOME_TABS } from "@/lib/home/constants"
import type { HomePanelData, HomeTabId, HomeTabsVisibility } from "@/lib/home/types"

type PanelState = {
  status: "idle" | "loading" | "ready" | "error"
  data?: HomePanelData
  error?: string
}

type HomeTabsClientProps = {
  visibleTabs?: HomeTabsVisibility
}

function getDefaultTab(visibleTabs: HomeTabsVisibility): HomeTabId {
  return HOME_TABS.find((tab) => visibleTabs[tab.id])?.id ?? "stars"
}

export function HomeTabsClient({
  visibleTabs = {
    stars: true,
    subscriptions: true,
    "report-runs": true,
    groups: true,
  },
}: HomeTabsClientProps) {
  const [activeTab, setActiveTab] = useState<HomeTabId>(getDefaultTab(visibleTabs))
  const [panels, setPanels] = useState<Record<HomeTabId, PanelState>>({
    stars: { status: "idle" },
    subscriptions: { status: "idle" },
    "report-runs": { status: "idle" },
    groups: { status: "idle" },
  })

  useEffect(() => {
    const state = panels[activeTab]
    if (state.status !== "idle") return

    setPanels((current) => ({
      ...current,
      [activeTab]: { status: "loading" },
    }))

    fetchHomeTabPanel(activeTab)
      .then((payload) => {
        if (!payload.ok) {
          setPanels((current) => ({
            ...current,
            [activeTab]: { status: "error", error: payload.error },
          }))
          return
        }

        setPanels((current) => ({
          ...current,
          [activeTab]: { status: "ready", data: payload.data },
        }))
      })
      .catch(() => {
        setPanels((current) => ({
          ...current,
          [activeTab]: { status: "error", error: "unknown" },
        }))
      })
  }, [activeTab, panels])

  const tabs = HOME_TABS.filter((tab) => visibleTabs[tab.id])
  const activePanel = panels[activeTab]

  return (
    <section>
      <nav aria-label="Homepage tabs" className="atlas-home-tab-nav">
        <ul className="flex flex-wrap items-center text-[0.95rem]">
          {tabs.map((tab, index) => (
            <li key={tab.id}>
              {index > 0 ? (
                <span className="mx-1.5 text-[var(--atlas-home-muted-light)]">/</span>
              ) : null}
              <button
                type="button"
                role="tab"
                aria-selected={activeTab === tab.id}
                className={`cursor-pointer text-[0.9rem] ${activeTab === tab.id ? "font-medium text-[var(--atlas-home-link)]" : "text-[var(--atlas-home-link)]"} hover:text-[var(--atlas-home-link-hover)] hover:underline`}
                onClick={() => setActiveTab(tab.id)}
              >
                {tab.label}
              </button>
            </li>
          ))}
        </ul>
      </nav>

      <div role="tabpanel" aria-label={HOME_TABS.find((tab) => tab.id === activeTab)?.label}>
        {activePanel.status === "loading" || activePanel.status === "idle" ? (
          <div className="atlas-home-card px-6 py-7 text-sm text-[var(--atlas-home-text)]">
            Loading {HOME_TABS.find((tab) => tab.id === activeTab)?.label}...
          </div>
        ) : null}

        {activePanel.status === "error" ? (
          <div className="atlas-home-card px-6 py-7 text-sm text-[var(--atlas-home-text)]">
            {activePanel.error === "http_401" ? (
              <>
                <a
                  href="/auth/login"
                  className="font-medium text-[var(--atlas-home-link)] hover:underline"
                >
                  Sign in
                </a>{" "}
                to view your {HOME_TABS.find((tab) => tab.id === activeTab)?.label?.toLowerCase()}.
              </>
            ) : (
              <>Unable to load {HOME_TABS.find((tab) => tab.id === activeTab)?.label}.</>
            )}
          </div>
        ) : null}

        {activePanel.status === "ready" && activePanel.data ? (
          <HomeTabPanel panel={activePanel.data} />
        ) : null}
      </div>
    </section>
  )
}
