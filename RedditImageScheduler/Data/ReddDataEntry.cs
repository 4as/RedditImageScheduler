using RedditImageScheduler.IO;

namespace RedditImageScheduler.Data {
	public readonly struct ReddDataEntry {
		public readonly string SourceURL;
		public readonly string Title;
		public readonly byte[] Image;

		public ReddDataEntry(ReddIODatabase.Entry ioEntry) {
			SourceURL = ioEntry.Source;
			Title = ioEntry.Title;
			Image = ioEntry.Image;
		}

		public ReddDataEntry(string sourceUrl, string title, byte[] image) {
			SourceURL = sourceUrl;
			Title = title;
			Image = image;
		}
	}
}