using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;

namespace Multiupdater;

internal class FileOperations
{
	private static string GetZipFilename(string url)
	{
		return url.Substring(url.LastIndexOf('/') + 1, url.LastIndexOf(".zip") + 3 - url.LastIndexOf('/'));
	}

	public static void InitTempDirectory()
	{
		if (Directory.Exists(Constants.TempDirectory))
		{
			Directory.Delete(Constants.TempDirectory, recursive: true);
		}
		Directory.CreateDirectory(Constants.TempDirectory);
	}

	public static void ClearTempDirectory()
	{
		Directory.Delete(Constants.TempDirectory, recursive: true);
	}

	public static async Task<string> DownloadArchive(string url)
	{
		HttpClient client = new HttpClient();
		try
		{
			Uri uri = new Uri(url);
			string zipFilename = GetZipFilename(url);
			char directorySeparatorChar = Path.DirectorySeparatorChar;
			string tempFolderFilePath = string.Join(directorySeparatorChar.ToString(), Constants.TempDirectory, zipFilename);
			client.Timeout = new TimeSpan(0, 0, 30);
			try
			{
				HttpResponseMessage val = await client.GetAsync(uri);
				using (FileStream fs = new FileStream(tempFolderFilePath, FileMode.Create))
				{
					await val.Content.CopyToAsync((Stream)fs);
				}
				return tempFolderFilePath;
			}
			catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
			{
				throw new TimeoutException();
			}
			catch (HttpRequestException)
			{
				throw new TimeoutException();
			}
		}
		finally
		{
			((IDisposable)client)?.Dispose();
		}
	}

	public static void UnzipFiles(string zippedPath)
	{
		char directorySeparatorChar = Path.DirectorySeparatorChar;
		string destinationDirectoryName = string.Join(directorySeparatorChar.ToString(), Constants.TempDirectory, "MULTIPLAYER_STAGED");
		ZipFile.ExtractToDirectory(zippedPath, destinationDirectoryName);
	}
}
