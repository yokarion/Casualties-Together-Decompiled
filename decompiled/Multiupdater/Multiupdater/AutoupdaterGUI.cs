using UnityEngine;

namespace Multiupdater;

internal class AutoupdaterGUI : MonoBehaviour
{
	private readonly int screenHeight = Screen.height;

	private readonly int bottomOffset = 100;

	public static string Status { get; set; }

	public static bool ShowUpdateButton { get; set; }

	public async void OnGUI()
	{
		GUI.Label(new Rect(5f, (float)(screenHeight - bottomOffset + 25), 1000f, 80f), new GUIContent("MP Updater: " + Status));
		if (ShowUpdateButton && GUI.Button(new Rect(5f, (float)(screenHeight - bottomOffset), 60f, 25f), new GUIContent("Update MP")) && AutoUpdater.latestVersionAndURL.Item2 != null)
		{
			await AutoUpdater.StartChecking();
		}
		if (GUI.Button(new Rect(105f, (float)(screenHeight - bottomOffset), 60f, 25f), new GUIContent("Close")))
		{
			Object.Destroy((Object)(object)this);
			ShowUpdateButton = false;
		}
	}
}
