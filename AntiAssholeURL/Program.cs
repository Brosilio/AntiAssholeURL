using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AntiAssholeURL
{
	static class Program
	{
		internal static List<UrlUnfuckerProfile> UnfuckerProfiles;

		[STAThread]
		static void Main()
		{
			UnfuckerProfiles = new List<UrlUnfuckerProfile>();

			if(!Directory.Exists("unfuckers")) Directory.CreateDirectory("unfuckers");

			foreach(string file in Directory.GetFiles("unfuckers"))
			{
				if(Path.GetExtension(file).Equals(".xml", StringComparison.OrdinalIgnoreCase))
				{
						UnfuckerProfiles.Add(XML.Read<UrlUnfuckerProfile>(File.ReadAllText(file)));
					try
					{
					} catch (Exception ex)
					{
						throw ex;
					}
				}
			}

			if(UnfuckerProfiles.Count == 0)
			{
				MessageBox.Show("No URL profiles found.\nVisit the following website to download the default set of profiles:\nhttps://github.com/brosilio/antiassholeurl", "AntiAssholeURL");
				//return;
			}

			Application.SetHighDpiMode(HighDpiMode.SystemAware);
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Form1());
		}
	}
}
