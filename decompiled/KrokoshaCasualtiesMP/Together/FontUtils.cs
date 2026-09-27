using System.Text;
using TMPro;
using UnityEngine;

namespace Together;

public static class FontUtils
{
	public static string ReplaceMissingCharacters(TMP_FontAsset font, string input)
	{
		if ((Object)(object)font == (Object)null || string.IsNullOrEmpty(input))
		{
			return input;
		}
		StringBuilder stringBuilder = new StringBuilder(input.Length);
		foreach (char c in input)
		{
			if (font.HasCharacter(c, true, false))
			{
				stringBuilder.Append(c);
			}
			else
			{
				stringBuilder.Append('?');
			}
		}
		return stringBuilder.ToString();
	}

	public static string ReplaceMissingCharacters(Font font, string input)
	{
		if ((Object)(object)font == (Object)null || string.IsNullOrEmpty(input))
		{
			return input;
		}
		StringBuilder stringBuilder = new StringBuilder(input.Length);
		font.RequestCharactersInTexture(input, font.fontSize, (FontStyle)0);
		CharacterInfo val = default(CharacterInfo);
		foreach (char c in input)
		{
			if (font.GetCharacterInfo(c, ref val, font.fontSize))
			{
				stringBuilder.Append(c);
			}
			else
			{
				stringBuilder.Append('?');
			}
		}
		return stringBuilder.ToString();
	}

	public static bool HasCharacter(Font font, char c)
	{
		if ((Object)(object)font == (Object)null)
		{
			return false;
		}
		font.RequestCharactersInTexture(c.ToString(), font.fontSize, (FontStyle)0);
		CharacterInfo val = default(CharacterInfo);
		return font.GetCharacterInfo(c, ref val, font.fontSize);
	}
}
