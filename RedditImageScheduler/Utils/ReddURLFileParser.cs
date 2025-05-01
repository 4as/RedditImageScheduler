using System;
using System.IO;
using IniParser;

namespace RedditImageScheduler.Utils {
	public static class ReddURLFileParser {
		private static readonly string KEY_URL = "URL";
		private static readonly IniDataParser PARSER = new IniDataParser();

		public static bool IsURL(Uri file_uri) {
			return Path.GetExtension(file_uri.LocalPath).ToLower() == ".url";
		}

		public static string GetUrl(Uri file) {
			try {
				string content = File.ReadAllText(file.LocalPath);
				if( IsURL(file) ) {
					IniData ini = PARSER.Parse(content);
					string url = null;
					foreach( var section in ini.Sections ) {
						if( section.Properties.Contains(KEY_URL) ) {
							url = section.Properties[KEY_URL];
							break;
						}
					}

					if( url != null ) {
						return url;
					}
				}
				else {
					if( Uri.TryCreate(content, UriKind.Absolute, out var validation) ) {
						return validation.AbsoluteUri;
					}
				}
			}
			catch { }

			return null;
		}
	}
}