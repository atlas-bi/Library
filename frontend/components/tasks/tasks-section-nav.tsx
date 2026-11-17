"use client"

export type TaskTabId =
  | "recommendretire"
  | "dead"
  | "maintenancerequired"
  | "auditonly"
  | "missingschedule"
  | "notanalytics"
  | "topundocumented"
  | "newundocumented"
  | "canmakereports"

export const TASK_TAB_ORDER: TaskTabId[] = [
  "recommendretire",
  "dead",
  "maintenancerequired",
  "auditonly",
  "missingschedule",
  "notanalytics",
  "topundocumented",
  "newundocumented",
  "canmakereports",
]

const TAB_LABELS: Record<TaskTabId, string> = {
  recommendretire: "To Retire",
  dead: "Unused",
  maintenancerequired: "To Maintain",
  auditonly: "Audit Only",
  missingschedule: "No Maint Schedule",
  notanalytics: "Edited by Others",
  topundocumented: "To Document",
  newundocumented: "New Undocumented",
  canmakereports: "Users with Create",
}

export function getDefaultTaskTab(): TaskTabId {
  return "recommendretire"
}

export function getHashTaskTab(hash: string | null): TaskTabId | null {
  const normalized = hash?.replace(/^#/, "")
  if (!normalized) return null
  return TASK_TAB_ORDER.find((tab) => tab === normalized) ?? null
}

export function TasksSectionNav({
  activeTab,
  onTabChangeAction,
}: {
  activeTab: TaskTabId
  onTabChangeAction: (tab: TaskTabId) => void
}) {
  return (
    <nav aria-label="Maintenance task sections" className="atlas-home-tab-nav">
      <ul className="flex flex-wrap items-center text-[0.95rem]">
        {TASK_TAB_ORDER.map((tab, index) => (
          <li key={tab}>
            {index > 0 ? (
              <span className="mx-1.5 text-[var(--atlas-home-muted-light)]">/</span>
            ) : null}
            <a
              href={`#${tab}`}
              onClick={(event) => {
                event.preventDefault()
                window.history.replaceState(null, "", `#${tab}`)
                onTabChangeAction(tab)
              }}
              className={
                activeTab === tab
                  ? "text-[0.9rem] font-medium text-[var(--atlas-home-link)] hover:text-[var(--atlas-home-link-hover)] hover:underline"
                  : "text-[0.9rem] text-[var(--atlas-home-link)] hover:text-[var(--atlas-home-link-hover)] hover:underline"
              }
              aria-current={activeTab === tab ? "page" : undefined}
            >
              {TAB_LABELS[tab]}
            </a>
          </li>
        ))}
      </ul>
    </nav>
  )
}

export function TaskTabPanel({
  tab,
  activeTab,
  children,
}: {
  tab: TaskTabId
  activeTab: TaskTabId
  children: React.ReactNode
}) {
  if (tab !== activeTab) return null
  return (
    <section id={tab} className="space-y-4 pt-2">
      {children}
    </section>
  )
}
