using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;

namespace whp
{
	public sealed class Wallpaper
	{
		public enum Style
		{
			Tiled,
			Centered,
			Stretched
		}

		private const int SPI_SETDESKWALLPAPER = 20;

		private const int SPIF_UPDATEINIFILE = 1;

		private const int SPIF_SENDWININICHANGE = 2;

		private Wallpaper()
		{
		}

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

		public static void Set(Uri uri)
		{
			if (!Directory.Exists("C:\\WHP\\imgs"))
			{
				Directory.CreateDirectory("C:\\WHP\\imgs");
			}
			string path = uri.ToString().Substring(uri.ToString().LastIndexOf("/") + 1);
			string text = Path.Combine("C:\\WHP\\imgs", path);
			if (!File.Exists(text))
			{
				try
				{
					HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri.ToString());
					httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/43.0.2357.124 Safari/537.36";
					using (Stream responseStream = httpWebRequest.GetResponse().GetResponseStream())
					{
						using (FileStream fileStream = new FileStream(text, FileMode.Create, FileAccess.ReadWrite))
						{
							responseStream.CopyTo(fileStream);
							fileStream.Flush();
						}
					}
				}
				catch
				{
					return;
				}
			}
			Wallpaper.SystemParametersInfo(20, 0, text, 3);
		}
	}
}
