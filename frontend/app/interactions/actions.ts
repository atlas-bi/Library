"use server"

import { revalidatePath } from "next/cache"
import {
  searchInteractionRecipients,
  sendShareMail,
  submitFeedback,
  toggleStar,
} from "@/lib/interactions/api"
import type {
  FeedbackRequest,
  InteractionEntityType,
  ShareMailRequest,
} from "@/lib/interactions/types"

export async function toggleStarAction(type: InteractionEntityType, id: number) {
  const result = await toggleStar({ type, id })
  if (!result.ok) {
    return { error: result.message }
  }

  if (type === "report") {
    revalidatePath("/reports")
  } else {
    revalidatePath("/collections")
  }

  return { data: result.data }
}

export async function searchRecipientsAction(query: string, includeGroups = true) {
  return searchInteractionRecipients(query, includeGroups)
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
