export type InteractionEntityType = "report" | "collection" | "term"

export interface StarToggleRequest {
  type: InteractionEntityType
  id: number
}

export interface StarToggleResponse {
  type: InteractionEntityType
  id: number
  isStarred: boolean
  count: number
}

export interface ShareRecipientInput {
  userId: number
  type: "u" | "g"
}

export interface ShareMailRequest {
  draftId?: number
  to: ShareRecipientInput[]
  subject: string
  message: string
  text: string
  share: boolean
  shareName: string
  shareUrl: string
}

export interface ShareMailResponse {
  message: string
  recipientCount: number
  shareCount: number
}

export interface FeedbackRequest {
  reportName: string
  reportUrl: string
  description: string
}

export interface AccessRequestRequest {
  reportName: string
  reportUrl: string
  directorName: string
}

export interface InteractionRecipientDto {
  id: number
  name: string
  type: string
  email?: string | null
}
