using System;
using System.Collections.Generic;
using RedditImageScheduler.Data;
using SQLite;

namespace RedditImageScheduler.IO {
	public class ReddIODatabase {
		private readonly string sFile;
		
		private SQLiteConnection sqlConnection;

		public ReddIODatabase(string file) {
			sFile = file;
		}

		// ================================================================================
		// GETTERS / SETTERS
		public string FilePath => sFile;
		public bool IsOpen => sqlConnection != null;
		public SQLiteConnection Connection => sqlConnection;

		public ReddDataEntries Entries => new ReddDataEntries(this);

		// ================================================================================
		// PUBLIC
		public void Close() {
			sqlConnection.Close();
			sqlConnection.Dispose();
		}

		public void Open() {
			try {
				sqlConnection = new SQLiteConnection(sFile);
				//TODO: remember to remove those drops in release
				sqlConnection.DropTable<Entry>();

				sqlConnection.CreateTable<Entry>();
			}
			catch( Exception ) {
				sqlConnection = null;
				Console.WriteLine( "Unable to initialize RedditIO database." );
				OnErrorInitialize?.Invoke();
			}
		}

		public int Add(string title, string source, byte[] image) {
			Entry raw = new Entry();
			raw.Title = title;
			raw.Source = source;
			raw.Image = image;
			
			int added = sqlConnection.Insert(raw);
			if( added == 0 ) {
				OnErrorAdd?.Invoke();
				return 0;
			}

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

		// ===============================================
		// CALLBACKS
		public delegate void DatabaseEvent();
		public event DatabaseEvent OnErrorInitialize;
		public event DatabaseEvent OnErrorAdd;

		// ===============================================
		// DEFINITIONS
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