import type { Metadata } from "next"
import { EtlThemePanel } from "@/components/settings/etl-theme-panel"
import { GroupRolesPanel } from "@/components/settings/group-roles-panel"
import { RolesPanel } from "@/components/settings/roles-panel"
import { SearchSettingsPanel } from "@/components/settings/search-settings-panel"
import { SiteMessagesPanel } from "@/components/settings/site-messages-panel"
import { TagsSettingsPanel } from "@/components/settings/tags-settings-panel"
import { UserRolesPanel } from "@/components/settings/user-roles-panel"
import {
  getDefaultEtl,
  getEtl,
  getGroupRoles,
  getPermissions,
  getRoles,
  getSearch,
  getSiteMessages,
  getTags,
  getTheme,
  getUserRoles,
} from "@/lib/settings/api"

export const metadata: Metadata = { title: "Settings" }

export default async function SettingsPage() {
  const [
    messagesResult,
    etlResult,
    themeResult,
    defaultEtlResult,
    searchResult,
    rolesResult,
    permissionsResult,
    userRolesResult,
    groupRolesResult,
    orgValues,
    runFreqs,
    frags,
    fragTags,
    maintSchedules,
    maintStatuses,
    finImpacts,
    stratImps,
    tags,
  ] = await Promise.all([
    getSiteMessages(),
    getEtl(),
    getTheme(),
    getDefaultEtl(),
    getSearch(),
    getRoles(),
    getPermissions(),
    getUserRoles(),
    getGroupRoles(),
    getTags("organizational-values"),
    getTags("estimated-run-frequencies"),
    getTags("fragilities"),
    getTags("fragility-tags"),
    getTags("maintenance-schedules"),
    getTags("maintenance-log-statuses"),
    getTags("financial-impacts"),
    getTags("strategic-importances"),
    getTags("tags"),
  ])

  return (
    <>
      {/* Tab panels — the SettingsTabController client component handles visibility via 'hidden' */}
      <div id="roles" className="panel-tab-data hidden">
        {rolesResult.ok && permissionsResult.ok ? (
          <RolesPanel initialRoles={rolesResult.data} permissions={permissionsResult.data} />
        ) : (
          <p className="text-red-500">
            {!rolesResult.ok ? rolesResult.message : !permissionsResult.ok ? permissionsResult.message : ""}
          </p>
        )}
      </div>

      <div id="user-roles" className="panel-tab-data hidden">
        {rolesResult.ok && userRolesResult.ok ? (
          <UserRolesPanel initialAssignments={userRolesResult.data} availableRoles={rolesResult.data} />
        ) : (
          <p className="text-red-500">{!userRolesResult.ok ? userRolesResult.message : ""}</p>
        )}
      </div>

      <div id="user-groups" className="panel-tab-data hidden">
        {rolesResult.ok && groupRolesResult.ok ? (
          <GroupRolesPanel initialAssignments={groupRolesResult.data} availableRoles={rolesResult.data} />
        ) : (
          <p className="text-red-500">{!groupRolesResult.ok ? groupRolesResult.message : ""}</p>
        )}
      </div>

      <div id="meta-fields" className="panel-tab-data hidden">
        <TagsSettingsPanel
          organizationalValues={orgValues.ok ? orgValues.data : []}
          estimatedRunFrequencies={runFreqs.ok ? runFreqs.data : []}
          fragilities={frags.ok ? frags.data : []}
          fragilityTags={fragTags.ok ? fragTags.data : []}
          maintenanceSchedules={maintSchedules.ok ? maintSchedules.data : []}
          maintenanceLogStatuses={maintStatuses.ok ? maintStatuses.data : []}
          financialImpacts={finImpacts.ok ? finImpacts.data : []}
          strategicImportances={stratImps.ok ? stratImps.data : []}
          tags={tags.ok ? tags.data : []}
        />
      </div>

      <div id="site-message" className="panel-tab-data hidden">
        {messagesResult.ok ? (
          <SiteMessagesPanel initialMessages={messagesResult.data} />
        ) : (
          <p className="text-red-500">{messagesResult.message}</p>
        )}
      </div>

      <div id="search" className="panel-tab-data hidden">
        {searchResult.ok ? (
          <SearchSettingsPanel initialData={searchResult.data} />
        ) : (
          <p className="text-red-500">{searchResult.message}</p>
        )}
      </div>

      <div id="theme" className="panel-tab-data hidden">
        {themeResult.ok ? (
          <EtlThemePanel
            initialEtl={null}
            initialTheme={themeResult.data.value ?? null}
            defaultEtl={null}
            themeOnly
          />
        ) : (
          <p className="text-red-500">{themeResult.message}</p>
        )}
      </div>

      <div id="etl" className="panel-tab-data hidden">
        {etlResult.ok ? (
          <EtlThemePanel
            initialEtl={etlResult.data.value ?? null}
            initialTheme={null}
            defaultEtl={defaultEtlResult.ok ? defaultEtlResult.data : null}
            etlOnly
          />
        ) : (
          <p className="text-red-500">{etlResult.message}</p>
        )}
      </div>
    </>
  )
}
