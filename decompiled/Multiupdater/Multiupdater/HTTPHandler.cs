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
		HttpClient client = new HttpClient();
		try
		{
			client.DefaultRequestHeaders.UserAgent.ParseAdd("MyGitHubClient/1.0");
			client.Timeout = new TimeSpan(0, 0, 10);
			HttpResponseMessage val;
			try
			{
				val = await client.GetAsync("https://api.github.com/repos/krokosha666/cas-unk-krokosha-multiplayer-coop/releases/latest");
			}
			catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
			{
				throw new TimeoutException();
			}
			catch (HttpRequestException)
			{
				throw new TimeoutException();
			}
			JObject val2 = JObject.Parse(await val.Content.ReadAsStringAsync());
			Version item = Version.Parse(((object)val2["tag_name"]).ToString().TrimStart(new char[1] { 'v' }));
			JToken obj = val2["assets"];
			foreach (JToken item2 in (JArray)((obj is JArray) ? obj : null))
			{
				if (((object)item2[(object)"name"]).ToString().Contains("KrokMP"))
				{
					return (item, ((object)item2[(object)"browser_download_url"]).ToString());
				}
			}
			throw new InvalidDataException("No suitable tag version retrieved!");
		}
		finally
		{
			((IDisposable)client)?.Dispose();
		}
	}
}
