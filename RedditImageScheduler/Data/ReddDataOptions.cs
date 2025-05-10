using RedditImageScheduler.IO;

namespace RedditImageScheduler.Data {
	public class ReddDataOptions {
		private readonly ReddIOOptions ioSettings;
		public ReddDataOptions(ReddIOOptions settings) {
			ioSettings = settings;
		}
		
		public string DatabasePath {
			get => ioSettings.DatabasePath;
			set => ioSettings.DatabasePath = value;
		}

		public uint EntrySpacingHours {
			get => ioSettings.EntrySpacingHours;
			set => ioSettings.EntrySpacingHours = value;
		}
		
		public uint PostingSpacingMinutes {
			get => ioSettings.PostingSpacingMinutes;
			set => ioSettings.PostingSpacingMinutes = value;
		}
		
		public uint TrimmingOldDays {
			get => ioSettings.TrimmingOldDays;
			set => ioSettings.TrimmingOldDays = value;
		}
		
		public event ReddIOOptions.IOOptionsEvent EventUpdate {
			add => ioSettings.EventUpdate += value;
			remove => ioSettings.EventUpdate -= value;
		}
	}
}