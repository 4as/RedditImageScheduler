using System.IO;

namespace RedditImageScheduler.Utils.Data {
	public class ReddFilepath {
		private readonly string sDefault;
		private string sFilepath;
		public ReddFilepath(string default_filepath) {
			sDefault = default_filepath;
			sFilepath = sDefault;
		}

		public string Filepath {
			get => sFilepath;
			set => sFilepath = File.Exists(value) ? value : sDefault;
		}
	}
}