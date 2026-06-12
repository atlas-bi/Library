import { HomeGroupsPanelView } from "@/components/home/home-groups-panel"
import { HomeRunListPanelView } from "@/components/home/home-run-list-panel"
import { HomeStarsPanelView } from "@/components/home/home-stars-panel"
import { HomeSubscriptionsPanelView } from "@/components/home/home-subscriptions-panel"
import type { HomePanelData } from "@/lib/home/types"

export function HomeTabPanel({ panel }: { panel: HomePanelData }) {
  switch (panel.kind) {
    case "stars":
      return <HomeStarsPanelView panel={panel} />
    case "subscriptions":
      return <HomeSubscriptionsPanelView panel={panel} />
    case "report-runs":
      return <HomeRunListPanelView panel={panel} />
    case "groups":
      return <HomeGroupsPanelView panel={panel} />
  }
}
