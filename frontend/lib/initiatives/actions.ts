"use server"

import {
  createInitiative,
  deleteInitiative,
  searchInitiativeCollections,
  toggleStar,
  shareMail,
  updateInitiative,
  type ShareMailRequestDto,
} from "./api"
import type { InitiativeWriteBody } from "./types"

export async function deleteInitiativeAction(id: number) {
  return deleteInitiative(id)
}

export async function searchInitiativeCollectionsAction(query: string) {
  return searchInitiativeCollections(query)
}

export async function createInitiativeAction(body: InitiativeWriteBody) {
  return createInitiative(body)
}

export async function updateInitiativeAction(id: number, body: InitiativeWriteBody) {
  return updateInitiative(id, body)
}

export async function toggleStarAction(id: number) {
  return toggleStar(id)
}

export async function shareMailAction(data: ShareMailRequestDto) {
  return shareMail(data)
}
