using Eto.Drawing;

namespace RedditImageScheduler {
	public static class ReddConfig {
		public static readonly string FILE = "database.sqlite";
		public static readonly int WIDTH = 900;
		public static readonly int HEIGHT = 500;
		public static readonly uint CACHE_SIZE = 32;
		
		public static readonly uint ENTRY_TITLE_LENGTH = 4;

		public static readonly Color UI_BG_DEFAULT = new Color(1f, 1f, 1f, 1f);
		public static readonly Color UI_BG_EMPTY = new Color(0f, 0f, 0f, 0f);
		public static readonly Color UI_BG_INVALID = new Color(1f, 0.75f, 0.7f, 1f);
	}
}