namespace RedditImageScheduler {
	public static class ReddLanguage {
		public static readonly string APP_NAME = "Reddit Image Scheduler";
		public static readonly string ADD = "Add";
		public static readonly string SAVE = "Save";
		public static readonly string REMOVE = "Remove";
		public static readonly string TITLE = "Title";
		public static readonly string SOURCE = "Source URL";
		public static readonly string DATE = "Post on:";
		public static readonly string HOUR = "Hour:";

		public static readonly string MENU_EDIT = "&Edit...";
		public static readonly string MENU_TIMETABLE = "&Timetable...";
		public static readonly string MENU_QUIT = "&Quit";
		public static readonly string MENU_HOME = "&Homepage";
		public static readonly string MENU_ABOUT = "&About";
		public static readonly string MENU_OPTIONS = "&Options...";
		
		public static readonly string STATUS_VALID_SAVED = "Entry is valid. Current schedule: {0}.";
		public static readonly string STATUS_VALID_UNSAVED = "Entry is valid. Old schedule: {0}, new schedule: {1}.";
		public static readonly string STATUS_INVALID = "Entry is invalid: {0}.";
		public static readonly string STATUS_INVALID_TITLE = "invalid title";
		public static readonly string STATUS_INVALID_SOURCE = "invalid source";
		public static readonly string STATUS_INVALID_DATE = "invalid date";
		public static readonly string STATUS_INVALID_IMAGE = "invalid image";
		
		public static readonly string MESSAGE_UNSAVED_CHANGES = "You have unsaved changes. Save them now?";
		
		public static readonly string ERROR = "Error";
		public static readonly string ERROR_FILE_NOT_FOUND = "Unable to open file: {0}";
	}
}