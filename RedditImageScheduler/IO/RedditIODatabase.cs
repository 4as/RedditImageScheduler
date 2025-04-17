using System.Collections.Generic;
using System.Linq;
using RedditImageScheduler.Data;
using SQLite;

namespace RedditImageScheduler.IO {
	public class RedditIODatabase {
		private readonly SQLiteConnection sqlConnection;
		private readonly string sFile;

		public RedditIODatabase(string file) {
			sFile = file;

			sqlConnection = new SQLiteConnection(sFile);
			//TODO: remember to remove those drops in release
			sqlConnection.DropTable<Entry>();
			
			sqlConnection.CreateTable<Entry>();
		}

		// ================================================================================
		// GETTERS / SETTERS
		public string FilePath => sFile;
		public SQLiteConnection Connection => sqlConnection;

		// ================================================================================
		// PUBLIC
		public void Dispose() => sqlConnection.Dispose();

		public RedditDatabase Initialize() {
			RedditDatabase dataset = new RedditDatabase(this);
			return dataset;
		}

		public int Add(string title, string source, byte[] image) {
			Entry raw = new Entry();
			raw.Title = title;
			raw.Source = source;
			raw.Image = image;
			sqlConnection.Insert(raw);
			return raw.Id;
		}

		public Entry Get(int entry_id) {
			return sqlConnection.Get<Entry>(entry_id);
		}

		public List<Entry> GetAll() {
			return sqlConnection.Query<Entry>("SELECT * FROM " + nameof(Entry));
		}

		public void Update(Entry entry) {
			sqlConnection.Update(entry);
		}

		[Table(nameof(Entry))]
		public class Entry {
			[PrimaryKey, AutoIncrement]
			[Column(nameof(Id))]
			public int Id { get; set; }
			
			[Column(nameof(Title))]
			public string Title { get; set; }
			
			[Column(nameof(Source))]
			public string Source { get; set; }

			[Column(nameof(Image))]
			public byte[] Image { get; set; }
		}
	}
}