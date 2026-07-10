"use server"

import { deleteInitiative, searchInitiativeCollections, createInitiative, updateInitiative } from "./api"
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
