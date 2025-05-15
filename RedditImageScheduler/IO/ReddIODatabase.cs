using System;
using System.IO;
using RedditImageScheduler.Data;
using SQLite;

namespace RedditImageScheduler.IO {
	public class ReddIODatabase {
		private readonly ReddIOEntries ioEntries = new ReddIOEntries();

		private SQLiteConnection sqlConnection;
		private string sFile;

		// ================================================================================
		// GETTERS / SETTERS
		public string FilePath => sFile;
		public bool IsOpen => sqlConnection != null;

		public ReddDataEntries Entries => ioEntries.Data;

		// ================================================================================
		// PUBLIC
		public void Open(string filepath) {
			Close();
			try {
				SQLiteConnectionString options = new SQLiteConnectionString(filepath, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite, true);
				sqlConnection = new SQLiteConnection(options);

				//sqlConnection.DropTable<ReddDataEntry>();

				sqlConnection.CreateTable<ReddDataEntry>();

				sFile = filepath;

				ioEntries.Open(sqlConnection);
				ioEntries.OnError += OnEntriesError;
			}
			catch( Exception ) {
				sqlConnection = null;
				Console.WriteLine("Unable to initialize RedditIO database.");
				EventErrorInitialize?.Invoke();
			}
		}

		public void Close() {
			ioEntries.OnError -= OnEntriesError;
			if( sqlConnection != null ) {
				sqlConnection.Close();
				sqlConnection.Dispose();
				sqlConnection = null;
			}

			sFile = null;
		}

		// ===============================================
		// EVENTS
		public delegate void DatabaseEvent();

		public event DatabaseEvent EventErrorInitialize;
		public event DatabaseEvent EventErrorChange;

		// ===============================================
		// CALLBACKS
		private void OnEntriesError() {
			EventErrorChange?.Invoke();
		}
	}
}