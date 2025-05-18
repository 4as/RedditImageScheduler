using RedditImageScheduler.IO;

namespace RedditImageScheduler.Data {
	public class ReddDataOptions {
		private readonly ReddIOOptions ioSettings;
		public ReddDataOptions(ReddIOOptions settings) {
			ioSettings = settings;
		}
		
		public string AppId => ioSettings.AppId;
		public string AppSecret => ioSettings.AppSecret;
		
		public string DatabasePath => ioSettings.DatabasePath;
		
		public uint EntrySpacingHours => ioSettings.EntrySpacingHours;
		public uint PostingSpacingMinutes => ioSettings.PostingSpacingMinutes;
		public uint TrimmingOldDays => ioSettings.TrimmingOldDays;

		public void SetApp(string app_id, string app_secret) {
			ioSettings.SetApp(app_id, app_secret);
		}

		public void SetDatabase(string database_path) {
			ioSettings.SetDatabase(database_path);
		}

		public void SetOptions(uint entry_spacing, uint posting_spacing, uint trimming_old_days) {
			ioSettings.SetOptions(entry_spacing, posting_spacing, trimming_old_days);
		}
		
		public event ReddIOOptions.IOOptionsEvent EventUpdate {
			add => ioSettings.EventUpdate += value;
			remove => ioSettings.EventUpdate -= value;
		}
	}
}