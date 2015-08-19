using HtmlAgilityPack;
using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace whp
{
	internal static class Program
	{
		private const string fullUrl = "http://wallpapers.wallhaven.cc/wallpapers/full/wallhaven-{0}.jpg";

		private const string searchUrl = "http://alpha.wallhaven.cc/search?categories=111&purity=010&sorting=views&resolutions={0}&order=desc&page={1}";

		[STAThread]
		private static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Thread thread = new Thread(new ThreadStart(() => 
			{
				while (true)
				{
					Program.t_Tick();
					Thread.Sleep(30000);
				}
			}));
			thread.Start();
		}

		private static void t_Tick()
		{
			Rectangle bounds = Screen.PrimaryScreen.Bounds;
			string doc = Program.GetDoc(string.Format("http://alpha.wallhaven.cc/search?categories=111&purity=010&sorting=views&resolutions={0}&order=desc&page={1}", string.Format("{0}x{1}", bounds.Width, bounds.Height), 1));
			HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();
			if (!string.IsNullOrEmpty(doc))
			{
				htmlDocument.LoadHtml(doc);
				string s = htmlDocument.DocumentNode.SelectSingleNode("//span[contains(@class,'thumb-listing-page-num')]").NextSibling.InnerText.Substring(3);
				string doc2 = Program.GetDoc(string.Format("http://alpha.wallhaven.cc/search?categories=111&purity=010&sorting=views&resolutions={0}&order=desc&page={1}", string.Format("{0}x{1}", bounds.Width, bounds.Height), new Random().Next(int.Parse(s))));
				if (!string.IsNullOrEmpty(doc2))
				{
					HtmlAgilityPack.HtmlDocument htmlDocument2 = new HtmlAgilityPack.HtmlDocument();
					htmlDocument2.LoadHtml(doc2);
					HtmlNodeCollection htmlNodeCollection = htmlDocument2.DocumentNode.SelectNodes("//figure");
					HtmlNode htmlNode = htmlNodeCollection[new Random().Next(htmlNodeCollection.Count)];
					Wallpaper.Set(new Uri(string.Format("http://wallpapers.wallhaven.cc/wallpapers/full/wallhaven-{0}.jpg", htmlNode.Attributes["data-wallpaper-id"].Value)));
				}
			}
		}

		public static string GetDoc(string url)
		{
			string result = null;
			try
			{
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
				using (StreamReader streamReader = new StreamReader(httpWebRequest.GetResponse().GetResponseStream()))
				{
					result = streamReader.ReadToEnd();
				}
			}
			catch
			{
			}
			return result;
		}
	}
}
