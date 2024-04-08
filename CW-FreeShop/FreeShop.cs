﻿using HarmonyLib;
using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using MelonLoader;

namespace CW_FreeShop
{
	public class FreeShop : MelonMod
	{
		internal const string Title = "FreeShop";
		internal const string Version = "1.0.0";
		internal const string Author = "ShingekiNoRex";
	}

	public static class FreeShopPatch
	{
		[HarmonyPatch(typeof(ShopHandler), "OnAddToCartItemClicked")]
		[HarmonyPrefix]
		static bool OnAddToCartItemClickedPrefix(byte itemID, Dictionary<byte, ShopItem> ___m_ItemsForSaleDictionary, PhotonView ___m_PhotonView, SFX_Instance ___addSFX, ShopHandler __instance)
		{
			if (!___m_ItemsForSaleDictionary.ContainsKey(itemID))
				return true;

			___m_PhotonView.RPC("RPCA_AddItemToCart", RpcTarget.All, new object[]
			{
					itemID
			});
			___addSFX.Play(__instance.transform.position, false, 1f, null);
			return false;
		}

		[HarmonyPatch(typeof(ShopHandler), "OnOrderCartClicked")]
		[HarmonyPrefix]
		static bool OnOrderCartClickedPrefix(ShoppingCart ___m_ShoppingCart, PhotonView ___m_PhotonView, SFX_Instance ___purchaseSFX, ShopHandler __instance)
		{
			if (___m_ShoppingCart.IsEmpty)
				return true;

			___purchaseSFX.Play(__instance.transform.position, false, 1f, null);
			byte[] itemIDs = (from item in ___m_ShoppingCart.Cart
							  select item.ItemID).ToArray<byte>();
			___m_PhotonView.RPC("RPCA_SpawnDrone", RpcTarget.All, new object[]
			{
				itemIDs
			});
			__instance.OnClearCartClicked();
			return false;
		}
	}
}
