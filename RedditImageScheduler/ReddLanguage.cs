namespace RedditImageScheduler {
	public static class ReddLanguage {
		public static readonly string NAME_APP = "Reddit Image Scheduler";
		public static readonly string NAME_OPTIONS = "Options";
		public static readonly string NAME_LOGIN = "Login";
		
		public static readonly string BUTTON_ADD = "Add";
		public static readonly string BUTTON_SAVE = "Save";
		public static readonly string BUTTON_REMOVE = "Remove";
		public static readonly string BUTTON_CANCEL = "Cancel";
		public static readonly string BUTTON_SUGGEST = "Suggest";
		public static readonly string BUTTON_TIMETABLE = "<- Timetable";
		public static readonly string BUTTON_EDIT = "<- Edit";
		public static readonly string BUTTON_LOGIN = "Connect";
		public static readonly string BUTTON_REGISTER = "Register";

		public static readonly string TEXT_NO_TITLE = "[EMPTY]";
		public static readonly string TEXT_NONE = "None";
		public static readonly string TEXT_PLACEHOLDER_TITLE = "Title";
		public static readonly string TEXT_PLACEHOLDER_SOURCE = "Source URL";
		public static readonly string TEXT_CONFIRM_DELETE = "Click again to confirm";
		public static readonly string TEXT_CONNECTING = "Connecting...";
		
		public static readonly string LABEL_DATE = "Post on:";
		public static readonly string LABEL_HOUR = "Hour:";
		public static readonly string LABEL_TITLE = "Title:";
		public static readonly string LABEL_SOURCE = "Source:";
		public static readonly string LABEL_NEXT = "Next:";
		public static readonly string LABEL_ENTRY_SPACING = "Entry spacing:";
		public static readonly string LABEL_POSTING_SPACING = "Posting overlap spacing:";
		public static readonly string LABEL_TRIM_OLD = "History trim:";
		public static readonly string LABEL_APP_ID = "App ID *:";
		public static readonly string LABEL_APP_SECRET = "App secret:";
		public static readonly string LABEL_APP_REDIRECT = "Redirect URL:";

		public static readonly string MENU_LOGOUT = "&Log out";
		public static readonly string MENU_LOAD = "&Open...";
		public static readonly string MENU_SAVE = "&Move to...";
		public static readonly string MENU_QUIT = "&Quit";
		public static readonly string MENU_HOME = "&Homepage";
		public static readonly string MENU_ABOUT = "&About";
		public static readonly string MENU_OPTIONS = "&Preferences";
		public static readonly string MENU_EDIT = "&Edit";
		
		public static readonly string STATUS_VALID_SAVED = "Entry is valid. Current schedule: {0}.";
		public static readonly string STATUS_VALID_UNSAVED = "Entry is valid (modified). Target schedule: {0}.";
		public static readonly string STATUS_INVALID = "Entry is invalid: {0}.";
		public static readonly string STATUS_INVALID_TITLE = "invalid title";
		public static readonly string STATUS_INVALID_SOURCE = "invalid source";
		public static readonly string STATUS_INVALID_DATE = "invalid date";
		public static readonly string STATUS_INVALID_IMAGE = "invalid image";

		public static readonly string STATE_POSTED = "Entry was posted on: {0}.";
		public static readonly string STATE_VALID = "Entry is valid, will be posted on: {0}.";
		public static readonly string STATE_VALID_HOURS = "Entry is valid, will be posted in {0} hours.";
		public static readonly string STATE_VALID_MINUTES = "Entry is valid, will be posted in {0} minutes.";
		public static readonly string STATE_VALID_SECONDS = "Entry is valid, will be posted in {0} seconds.";
		public static readonly string STATE_INVALID = "Entry is invalid and won't be posted until fixed.";

		public static readonly string NEXT_FULL = "\"{0}\" (Posting on: {1})";
		public static readonly string NEXT_HOURS = "\"{0}\" (Posting in: {1} hours)";
		public static readonly string NEXT_MINUTES = "\"{0}\" (Posting in: {1} minutes)";
		public static readonly string NEXT_SECONDS = "\"{0}\" (Posting in: {1} seconds)";
		
		public static readonly string MESSAGE_UNSAVED_CHANGES = "You have unsaved changes. Save them now?";
		
		public static readonly string INFO_ENTRY_SPACING = "New entries in the timetable are created with the date of posting set to X hours after the latest entry.";
		public static readonly string INFO_POSTING_SPACING = "If multiple posts are scheduled for the same time, they will be spaced out by X minutes.";
		public static readonly string INFO_OLD_TRIMMING = "Old entries that were successfully posted are deleted from the timetable after X days.";
		
		public static readonly string ERROR = "Error";
		public static readonly string ERROR_FILE_NOT_FOUND = "Unable to open file: {0}";
		public static readonly string ERROR_SETTINGS_LOAD_FAILED = "Failed to access '{0}.' Make sure the current folder can be read from and written to. Your settings won't be saved.";
		public static readonly string ERROR_DATABASE_INITIALIZATION_FAILED = "Failed to access '{0}.' Make sure the current folder can be read from and written to. Your schedule won't be saved.";
		public static readonly string ERROR_DATABASE_MODIFY_FAILED = "Failed to modify the schedule file. Is '{0}' still accessible?";
		public static readonly string ERROR_FILE_SAVE_FAILED = "Failed to move the schedule file. Is '{0}' accessible?";
	}
}