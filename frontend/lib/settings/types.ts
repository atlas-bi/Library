// Settings API type definitions — mirrors web/Contracts/Api/Settings/SettingsDtos.cs

export interface SettingValueDto {
  value: string | null
}

export interface SiteMessageDto {
  id: number
  value: string
  description: string | null
}

export interface SiteMessageRequest {
  value: string
  description?: string | null
}

export interface SearchReportTypeDto {
  id: number
  name: string
  shortName: string | null
  visible: boolean
}

export interface SearchSettingsDto {
  visibility: Record<string, string>
  reportTypes: SearchReportTypeDto[]
}

export interface ParameterDto {
  id: number
  name: string
  description: string | null
  used: number
}

export interface ParameterRequest {
  name: string
  description?: string | null
}

export interface PermissionDto {
  id: number
  name: string
}

export interface RoleDto {
  id: number
  name: string
  permissions: PermissionDto[]
}

export interface RoleRequest {
  name: string
}

export interface UserRoleAssignmentDto {
  userId: number
  name: string
  roles: PermissionDto[]
}

export interface GroupRoleAssignmentDto {
  groupId: number
  name: string
  roles: PermissionDto[]
}
