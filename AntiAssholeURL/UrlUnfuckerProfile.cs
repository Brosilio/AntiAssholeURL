using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Web;
using System.Xml.Serialization;

namespace AntiAssholeURL
{
	public class UrlUnfuckerProfile
	{

		[XmlAttribute("name")]
		public string name;

		[XmlAttribute("keep-fragments")]
		public bool keepFragments;

		[XmlAttribute("try-hard")]
		public bool tryHard;

		public string[] urls;
		public string[] queries;
		public int[] segments;

		public bool CanUnfuck(Uri fucked)
		{
			foreach (string s in urls)
			{
				string test = fucked.Host + fucked.AbsolutePath;
				if (test.StartsWith(s, StringComparison.OrdinalIgnoreCase))
					return true;
			}

			return false;
		}

		public Uri Unfuck(Uri fucked)
		{
			UriBuilder b = new UriBuilder
			{
				Scheme = fucked.Scheme,
				Host = fucked.Host
			};

			if (!fucked.IsDefaultPort)
				b.Port = fucked.Port;

			if (keepFragments)
				b.Fragment = fucked.Fragment;

			foreach (int keep in segments)
			{
				if (keep < 0)
					continue;

				// URL can't be unfucked properly
				if (keep >= fucked.Segments.Length)
				{
					// Attempt anyways, but skip this segment
					if (tryHard) continue;

					return null;
				}

				b.Path = Path.Combine(b.Path, fucked.Segments[keep]);
			}

			if (queries.Length > 0)
			{
				StringBuilder query = new StringBuilder();
				NameValueCollection p = HttpUtility.ParseQueryString(fucked.Query);

				foreach (string key in queries)
				{
					string val = p.Get(key);
					if (val != null)
					{
						query.Append(key);
						query.Append("=");
						query.Append(val);
						query.Append("&"); // Trailing & shouldn't matter...?
					}
				}

				b.Query = query.ToString();
			}

			return b.Uri;
		}
	}
}
