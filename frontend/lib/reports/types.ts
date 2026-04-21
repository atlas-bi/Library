export type ReportFeatureFlags = {
  termsEnabled?: boolean;
  userProfilesEnabled?: boolean;
  feedbackEnabled?: boolean;
  requestAccessEnabled?: boolean;
  sharingEnabled?: boolean;
};

export type PeopleRef = {
  id: number;
  username: string;
  fullName: string;
  email: string;
};

export type MaintenanceStatus = {
  isRequired: boolean;
  message?: string | null;
  lastMaintenanceDate?: string | null;
  nextMaintenanceDate?: string | null;
  schedule?: { id: number; name: string } | null;
};

export type ReportDetail = {
  id: number;
  name: string;
  displayTitle?: string | null;
  displayName?: string | null;
  description?: string | null;
  detailedDescription?: string | null;
  repositoryDescription?: string | null;
  typeName?: string | null;
  typeShortName?: string | null;
  availability?: string | null;
  lastModified?: string | null;
  lastLoadDate?: string | null;
  url?: string;

  runs?: number;

  // capability flags
  canRun?: boolean;
  canEditDocumentation?: boolean;
  canViewGroups?: boolean;
  canViewUserProfiles?: boolean;
  isStarred?: boolean;
  starCount?: number;

  // derived action URLs
  runUrl?: string | null;
  editReportUrl?: string | null;
  manageReportUrl?: string | null;
  recordViewerUrl?: string | null;

  // feature flags
  features?: ReportFeatureFlags;

  // header tags
  headerTags?: Array<{
    id: number;
    name?: string | null;
    description?: string | null;
    priority?: number | null;
    showInHeader?: string | boolean | null;
  }>;

  // maintenance
  maintenanceStatus?: MaintenanceStatus | null;

  // people
  author?: PeopleRef | null;
  lastModifiedBy?: PeopleRef | null;
  requester?: PeopleRef | null;

  // collections / groups / hierarchy
  groups?: Array<{ id: number; name?: string | null; email?: string | null; type?: string | null }>;
  collections?: Array<{ id: number; name?: string | null; rank?: number | null }>;
  children?: Array<{ id: number; name?: string | null; displayTitle?: string | null; type?: string | null; url?: string | null; lastModified?: string | null }>;
  parents?: Array<{ id: number; name?: string | null; displayTitle?: string | null; type?: string | null; url?: string | null; lastModified?: string | null }>;

  // document-related
  images?: Array<{ id: number; ordinal?: number | null; source?: string | null }>;

  // queries / terms
  queries?: Array<{ id: number; name?: string | null; language?: string | null; sourceServer?: string | null; source?: string | null }>;
  componentQueries?: Array<{ id: number; name?: string | null; language?: string | null; sourceServer?: string | null; source?: string | null }>;
  terms?: Array<{ id: number; name?: string | null; summary?: string | null }>;

  // full document payload (not rendered yet)
  document?: unknown;
};

export type ReportListItem = {
  id: number;
  name: string;
  displayTitle?: string | null;
  displayName?: string | null;
  description?: string | null;
  typeShortName?: string | null;
  lastModified?: string | null;
};

