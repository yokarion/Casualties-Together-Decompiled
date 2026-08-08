using System.Collections;
using HarmonyLib;
using KrokoshaCasualtiesUtils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(MindwipeScript), "WipeRoutine")]
public static class MindwipeScript_WipeRoutine_MultiplayerPatch
{
	private class MindwipeMPTracker : MonoBehaviour
	{
		public GameObject go;

		private void Update()
		{
			MindwipeScript val = default(MindwipeScript);
			if (!((Component)this).TryGetComponent<MindwipeScript>(ref val))
			{
				if ((Object)(object)go != (Object)null)
				{
					Object.Destroy((Object)(object)go);
				}
				if ((Object)(object)this != (Object)null)
				{
					Object.Destroy((Object)(object)this);
				}
			}
		}
	}

	public static IEnumerator Copypasted_WipeRoutine(MindwipeScript __instance)
	{
		yield return (object)new WaitForSeconds(2f);
		Body body = ((Component)__instance).GetComponent<Body>();
		float timer = 0f;
		Limb[] limbs;
		while (timer < 3f)
		{
			body.Ragdoll();
			body.shock = 20f;
			body.hearingLoss += Time.deltaTime * 50f;
			if (body.IsBodyLocal())
			{
				PlayerCamera main = PlayerCamera.main;
				main.bonusAbber += Time.deltaTime * 8f;
			}
			limbs = body.limbs;
			Limb[] array = limbs;
			foreach (Limb obj in array)
			{
				Rigidbody2D rb = obj.rb;
				rb.angularVelocity += Random.Range(-400f, 400f);
				Rigidbody2D rb2 = obj.rb;
				rb2.velocity += Random.insideUnitCircle * 2f;
				obj.pain += Time.deltaTime * 100f;
			}
			timer += Time.deltaTime;
			yield return null;
		}
		limbs = body.limbs;
		for (int j = 0; j < limbs.Length; j++)
		{
			limbs[j].pain = 0f;
		}
		body.consciousness = 0f;
		body.energy = 0f;
		body.sleeping = true;
		body.hearingLoss = 80f;
		body.strokeAmount = 0f;
		if (body.alive)
		{
			body.brainHealth += 50f;
		}
		body.skills.INT = 0;
		body.skills.expINT = 0f;
		body.skills.UpdateExpBoundaries();
		if (body.IsBodyLocal())
		{
			if (KrokoshaScavMultiplayer.is_client)
			{
				foreach (Recipe recipe in Recipes.recipes)
				{
					recipe.hasMadeBefore = false;
				}
			}
			MusicManager.main.StopSong();
			Object obj2 = Object.Instantiate(Resources.Load("Special/MindwipeVignette"), ((Component)PlayerCamera.main.mainCanvas).transform);
			GameObject val = (GameObject)(object)((obj2 is GameObject) ? obj2 : null);
			val.transform.SetAsFirstSibling();
			val.AddComponent<MindwipeMPTracker>().go = val;
		}
		__instance.active = true;
	}

	private static bool Prefix(MindwipeScript __instance, ref IEnumerator __result)
	{
		if (!KrokoshaScavMultiplayer.network_system_is_running)
		{
			return true;
		}
		__result = Copypasted_WipeRoutine(__instance);
		return false;
	}
}
