export type StoreSettings = {
  shopName: string
  ownerName: string
  logoUrl: string | null
  isSetupCompleted: boolean
}

export type UpdateStoreSettingsRequest = {
  shopName: string
  ownerName: string
}
