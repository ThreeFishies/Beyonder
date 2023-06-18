using System;
using System.IO;
using HarmonyLib;
using ShinyShoe;
using UnityEngine;
using BepInEx;
using Void.Init;

namespace Void.Clan
{
	public static class Mod_Sprites_Setup
	{
		// Token: 0x0600000A RID: 10 RVA: 0x00002268 File Offset: 0x00000468
		public static Sprite LoadNewSprite(string FilePath, string FileName, float PixelsPerUnit = 100f)
		{
			Texture2D texture2D = Mod_Sprites_Setup.LoadTexture(Path.Combine(FilePath,FileName));
			return Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0f, 0f), PixelsPerUnit);
		}

		// Token: 0x0600000B RID: 11 RVA: 0x000022B8 File Offset: 0x000004B8
		private static Texture2D LoadTexture(string filePath)
		{
			bool flag = File.Exists(filePath);
			Texture2D result;
			if (flag)
			{
				byte[] data = File.ReadAllBytes(filePath);
				Texture2D texture2D = new Texture2D(2, 2);
				texture2D.LoadImage(data);
				result = texture2D;
			}
			else
			{
				result = null;
			}
			return result;
		}
	}
}
