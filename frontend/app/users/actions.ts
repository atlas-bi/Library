"use server"

import {
  createUserFolder,
  deleteUserFolder,
  removeUserSharedObject,
  reorderUserFavorites,
  reorderUserFolders,
  toggleAdminMode,
  toggleUserFavorite,
  updateUserFavoriteFolderAssignment,
  updateUserFolder,
} from "@/lib/users/api"
import type {
  CreateUserFavoriteFolderRequest,
  ReorderUserFavoriteFolderItem,
  ReorderUserFavoriteItem,
  ToggleUserFavoriteRequest,
  UpdateUserFavoriteFolderAssignmentRequest,
  UpdateUserFavoriteFolderRequest,
} from "@/lib/users/types"

export async function createUserFolderAction(
  userId: number,
  isCurrentUser: boolean,
  body: CreateUserFavoriteFolderRequest,
) {
  return createUserFolder(userId, isCurrentUser, body)
}

export async function updateUserFolderAction(
  userId: number,
  isCurrentUser: boolean,
  folderId: number,
  body: UpdateUserFavoriteFolderRequest,
) {
  return updateUserFolder(userId, isCurrentUser, folderId, body)
}

export async function deleteUserFolderAction(
  userId: number,
  isCurrentUser: boolean,
  folderId: number,
) {
  return deleteUserFolder(userId, isCurrentUser, folderId)
}

export async function reorderUserFoldersAction(
  userId: number,
  isCurrentUser: boolean,
  body: ReorderUserFavoriteFolderItem[],
) {
  return reorderUserFolders(userId, isCurrentUser, body)
}

export async function reorderUserFavoritesAction(
  userId: number,
  isCurrentUser: boolean,
  body: ReorderUserFavoriteItem[],
) {
  return reorderUserFavorites(userId, isCurrentUser, body)
}

export async function updateUserFavoriteFolderAssignmentAction(
  userId: number,
  isCurrentUser: boolean,
  body: UpdateUserFavoriteFolderAssignmentRequest,
) {
  return updateUserFavoriteFolderAssignment(userId, isCurrentUser, body)
}

export async function toggleUserFavoriteAction(
  userId: number,
  isCurrentUser: boolean,
  body: ToggleUserFavoriteRequest,
) {
  return toggleUserFavorite(userId, isCurrentUser, body)
}

export async function removeUserSharedObjectAction(id: number) {
  return removeUserSharedObject(id)
}

export async function toggleAdminModeAction() {
  return toggleAdminMode()
}
