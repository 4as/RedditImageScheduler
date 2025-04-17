using RedditImageScheduler.IO;

namespace RedditImageScheduler.Data {
	public class RedditDatabase {
		private readonly RedditIODatabase ioDatabase;
		
		public RedditDatabase(RedditIODatabase database) {
			ioDatabase = database;
		}
	}
}