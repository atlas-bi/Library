export type TaskCanMakeReportDto = {
  name: string
  userId: number | null
  role: string
  roleId: number | null
}

export type TaskRetireReportDto = {
  name: string
  maintenanceDate: string | null
  maintenanceDateString: string
  reportId: number
  comment: string | null
  fullName: string
}

export type TaskUnusedReportDto = {
  reportUrl: string
  name: string
  type: string
  modifiedBy: string
  lastModified: string
  server: string
  masterFile: string
  epicId: string
}

export type TaskMaintenanceReportDto = {
  reportId: number
  date: string
  name: string
  user: string
}

export type TaskAnalyticsReportDto = {
  reportUrl: string
  lastModified: string
  author: string
  modifiedBy: string
  name: string
  reportType: string
  epic: string
  runReportUrl: string | null
  editReportUrl: string | null
  recordViewerUrl: string | null
  runs: number
  epicMasterFile: string
  epicRecordId: string
}

export type TaskUndocumentedReportDto = {
  reportObjectId: number
  modifiedBy: string
  name: string
  reportType: string
  runs: number
  lastMaintained: string
  lastRun: string
  favorite: string
}

export type TasksResponseDto = {
  canMakeReports: TaskCanMakeReportDto[]
  recommendRetire: TaskRetireReportDto[]
  unused: TaskUnusedReportDto[]
  maintenanceRequired: TaskMaintenanceReportDto[]
  audit: TaskMaintenanceReportDto[]
  missingSchedule: TaskMaintenanceReportDto[]
  notInAnalytics: TaskAnalyticsReportDto[]
  topUndocumented: TaskUndocumentedReportDto[]
  newUndocumented: TaskUndocumentedReportDto[]
}
