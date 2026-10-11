using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Multiupdater;

internal class HTTPHandler
{
	public static async Task<(Version, string)> GetLatestRelease()
	{
		using HttpClient client = new HttpClient();
		client.DefaultRequestHeaders.UserAgent.ParseAdd("MyGitHubClient/1.0");
		client.Timeout = new TimeSpan(0, 0, 10);
		HttpResponseMessage httpResponseMessage;
		try
		{
			httpResponseMessage = await client.GetAsync("https://api.github.com/repos/creaturefeaturelarry/casualties-together/releases/latest");
		}
		catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
		{
			throw new TimeoutException();
		}
		catch (HttpRequestException)
		{
			throw new TimeoutException();
		}
		JObject obj = JObject.Parse(await httpResponseMessage.Content.ReadAsStringAsync());
		Version item = Version.Parse(((object)obj["tag_name"]).ToString().TrimStart('v'));
		JToken obj2 = obj["assets"];
		foreach (JToken item2 in (JArray)((obj2 is JArray) ? obj2 : null))
		{
			if (((object)item2[(object)"name"]).ToString().Contains("CasualtiesTogether"))
			{
				return (item, ((object)item2[(object)"browser_download_url"]).ToString());
			}
		}
		throw new InvalidDataException("No suitable tag version retrieved!");
	}
}
