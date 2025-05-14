using Eto.Drawing;
using Eto.Forms;
using RedditImageScheduler.Utils.Data;

namespace RedditImageScheduler {
	public static class ReddConfig {
		public static readonly string FILE_DATABASE = "database.sqlite";
		public static readonly string FILE_SETTINGS = "settings.ini";
		public static readonly string SETTINGS_SECTION = "Options";
		public static readonly string HOMEPAGE = "https://github.com/4as/RedditImageScheduler";
		public static readonly FileFilter IMAGE_FILTER = new FileFilter("Images", ".png", ".jpg", ".jpeg", ".gif", ".bmp");
		
		public static readonly int WIDTH = 900;
		public static readonly int HEIGHT = 500;
		public static readonly uint CACHE_SIZE = 32;
		
		public static readonly uint ENTRY_SPACING_HOURS = 12;
		public static readonly uint ENTRY_TRIMMING_DAYS_OLD = 2;
		public static readonly uint ENTRY_POSTING_COOLDOWN_MINUTES = 5;
		public static readonly uint ENTRY_TITLE_LENGTH = 4;
		
		public static readonly ReddClamping<uint> OPTION_ENTRY_SPACING_HOURS = new ReddClamping<uint>(1, 24);
		public static readonly ReddClamping<uint> OPTION_POSTING_SPACING_MINUTES = new ReddClamping<uint>(1, 120);
		public static readonly ReddClamping<uint> OPTION_OLD_TRIMMING_DAYS = new ReddClamping<uint>(1, 30);

		public static readonly Color UI_TEXT_IMPORTANT = new Color(1f, 0f, 0f, 1f);
		public static readonly Color UI_BG_DEFAULT = new Color(1f, 1f, 1f, 1f);
		public static readonly Color UI_BG_EMPTY = new Color(0f, 0f, 0f, 0f);
		public static readonly Color UI_BG_INVALID = new Color(1f, 0.75f, 0.7f, 1f);
		public static readonly Color UI_BG_SELECTED = new Color(0f, 0f, 1f, 1f);
		public static readonly Color UI_BG_POSTED = new Color(0.75f, 0.75f, 1f, 1f);
		public static readonly Color UI_BG_NEXT = new Color(1f, 1f, 0.75f, 1f);
	}
}