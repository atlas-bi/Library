"use server"

import { revalidatePath } from "next/cache"
import {
  searchDirectors,
  searchInteractionRecipients,
  sendShareMail,
  submitAccessRequest,
  submitFeedback,
  toggleStar,
} from "@/lib/interactions/api"
import { buildStarToggleRequest } from "@/lib/interactions/star-toggle-request"
import type {
  AccessRequestRequest,
  FeedbackRequest,
  InteractionEntityType,
  ShareMailRequest,
} from "@/lib/interactions/types"

export async function toggleStarAction(
  type: InteractionEntityType,
  id: number,
  options?: { search?: string },
) {
  const result = await toggleStar(buildStarToggleRequest(type, id, options))
  if (!result.ok) {
    return { error: result.message }
  }

  revalidatePath("/")
  revalidatePath("/search")
  if (type === "report") {
    revalidatePath("/reports")
  } else if (type === "term") {
    revalidatePath("/terms")
  } else if (type === "initiative") {
    revalidatePath("/initiatives")
  } else if (type === "user") {
    revalidatePath("/users")
  } else if (type === "group") {
    revalidatePath("/groups")
  } else if (type === "collection") {
    revalidatePath("/collections")
  }

  return { data: result.data }
}

export async function searchRecipientsAction(query: string, includeGroups = true) {
  return searchInteractionRecipients(query, includeGroups)
}

export async function searchDirectorsAction(query: string) {
  return searchDirectors(query)
}

export async function sendShareMailAction(body: ShareMailRequest) {
  const result = await sendShareMail(body)
  if (!result.ok) {
    return { error: result.message }
  }
  return { data: result.data }
}

export async function submitFeedbackAction(body: FeedbackRequest) {
  const result = await submitFeedback(body)
  if (!result.ok) {
    return { error: result.message }
  }
  return { data: result.data }
}

export async function submitAccessRequestAction(body: AccessRequestRequest) {
  const result = await submitAccessRequest(body)
  if (!result.ok) {
    return { error: result.message }
  }
  return { data: result.data }
}
