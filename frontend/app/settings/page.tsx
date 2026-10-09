import type { Metadata } from "next"
import { EtlThemePanel } from "@/components/settings/etl-theme-panel"
import { GroupRolesPanel } from "@/components/settings/group-roles-panel"
import { RolesPanel } from "@/components/settings/roles-panel"
import { SearchSettingsPanel } from "@/components/settings/search-settings-panel"
import { SiteMessagesPanel } from "@/components/settings/site-messages-panel"
import { TagsSettingsPanel } from "@/components/settings/tags-settings-panel"
import { UserRolesPanel } from "@/components/settings/user-roles-panel"
import { getCurrentUser } from "@/lib/auth"
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
import { getSettingsAccess, hasAnySettingsAccess, META_FIELD_TAG_TYPES } from "@/lib/settings/nav"

export const metadata: Metadata = { title: "Settings" }

export default async function SettingsPage() {
  const user = await getCurrentUser()
  const access = getSettingsAccess(user)

  if (!hasAnySettingsAccess(access)) {
    return null
  }

  const needsRoles = access.canEditRoles || access.canEditUsers || access.canEditGroups

  const [
    rolesResult,
    permissionsResult,
    userRolesResult,
    groupRolesResult,
    messagesResult,
    etlResult,
    themeResult,
    defaultEtlResult,
    searchResult,
    ...tagResults
  ] = await Promise.all([
    needsRoles ? getRoles() : null,
    access.canEditRoles ? getPermissions() : null,
    access.canEditUsers ? getUserRoles() : null,
    access.canEditGroups ? getGroupRoles() : null,
    access.canManageSiteSettings ? getSiteMessages() : null,
    access.canManageSiteSettings ? getEtl() : null,
    access.canManageSiteSettings ? getTheme() : null,
    access.canManageSiteSettings ? getDefaultEtl() : null,
    access.canManageSiteSettings ? getSearch() : null,
    ...(access.canManageMetaFields
      ? META_FIELD_TAG_TYPES.map((tagType) => getTags(tagType))
      : []),
  ])

  const [
    orgValues,
    runFreqs,
    frags,
    fragTags,
    maintSchedules,
    maintStatuses,
    finImpacts,
    stratImps,
    tags,
  ] = tagResults

  const tagLoadErrors = access.canManageMetaFields
    ? [
        orgValues && !orgValues.ok && orgValues.message,
        runFreqs && !runFreqs.ok && runFreqs.message,
        frags && !frags.ok && frags.message,
        fragTags && !fragTags.ok && fragTags.message,
        maintSchedules && !maintSchedules.ok && maintSchedules.message,
        maintStatuses && !maintStatuses.ok && maintStatuses.message,
        finImpacts && !finImpacts.ok && finImpacts.message,
        stratImps && !stratImps.ok && stratImps.message,
        tags && !tags.ok && tags.message,
      ].filter((message): message is string => Boolean(message))
    : []

  return (
    <>
      {access.canEditRoles && (
        <div id="roles" className="panel-tab-data hidden">
          {rolesResult?.ok && permissionsResult?.ok ? (
            <RolesPanel initialRoles={rolesResult.data} permissions={permissionsResult.data} />
          ) : (
            <p className="text-red-500">
              {rolesResult && !rolesResult.ok
                ? rolesResult.message
                : permissionsResult && !permissionsResult.ok
                  ? permissionsResult.message
                  : ""}
            </p>
          )}
        </div>
      )}

      {access.canEditUsers && (
        <div id="user-roles" className="panel-tab-data hidden">
          {rolesResult?.ok && userRolesResult?.ok ? (
            <UserRolesPanel
              initialAssignments={userRolesResult.data}
              availableRoles={rolesResult.data}
            />
          ) : (
            <p className="text-red-500">
              {userRolesResult && !userRolesResult.ok ? userRolesResult.message : ""}
            </p>
          )}
        </div>
      )}

      {access.canEditGroups && (
        <div id="user-groups" className="panel-tab-data hidden">
          {rolesResult?.ok && groupRolesResult?.ok ? (
            <GroupRolesPanel
              initialAssignments={groupRolesResult.data}
              availableRoles={rolesResult.data}
            />
          ) : (
            <p className="text-red-500">
              {groupRolesResult && !groupRolesResult.ok ? groupRolesResult.message : ""}
            </p>
          )}
        </div>
      )}

      {access.canManageMetaFields && (
        <div id="meta-fields" className="panel-tab-data hidden">
          {tagLoadErrors.length > 0 && (
            <p className="text-red-500 mb-4">{tagLoadErrors.join(" ")}</p>
          )}
          <TagsSettingsPanel
            organizationalValues={orgValues?.ok ? orgValues.data : []}
            estimatedRunFrequencies={runFreqs?.ok ? runFreqs.data : []}
            fragilities={frags?.ok ? frags.data : []}
            fragilityTags={fragTags?.ok ? fragTags.data : []}
            maintenanceSchedules={maintSchedules?.ok ? maintSchedules.data : []}
            maintenanceLogStatuses={maintStatuses?.ok ? maintStatuses.data : []}
            financialImpacts={finImpacts?.ok ? finImpacts.data : []}
            strategicImportances={stratImps?.ok ? stratImps.data : []}
            tags={tags?.ok ? tags.data : []}
          />
        </div>
      )}

      {access.canManageSiteSettings && (
        <>
          <div id="site-message" className="panel-tab-data hidden">
            {messagesResult?.ok ? (
              <SiteMessagesPanel initialMessages={messagesResult.data} />
            ) : (
              <p className="text-red-500">{messagesResult?.message ?? ""}</p>
            )}
          </div>

          <div id="search" className="panel-tab-data hidden">
            {searchResult?.ok ? (
              <SearchSettingsPanel initialData={searchResult.data} />
            ) : (
              <p className="text-red-500">{searchResult?.message ?? ""}</p>
            )}
          </div>

          <div id="theme" className="panel-tab-data hidden">
            {themeResult?.ok ? (
              <EtlThemePanel
                initialEtl={null}
                initialTheme={themeResult.data.value ?? null}
                defaultEtl={null}
                themeOnly
              />
            ) : (
              <p className="text-red-500">{themeResult?.message ?? ""}</p>
            )}
          </div>

          <div id="etl" className="panel-tab-data hidden">
            {etlResult?.ok ? (
              <EtlThemePanel
                initialEtl={etlResult.data.value ?? null}
                initialTheme={null}
                defaultEtl={defaultEtlResult?.ok ? defaultEtlResult.data : null}
                etlOnly
              />
            ) : (
              <p className="text-red-500">{etlResult?.message ?? ""}</p>
            )}
          </div>
        </>
      )}
    </>
  )
}
