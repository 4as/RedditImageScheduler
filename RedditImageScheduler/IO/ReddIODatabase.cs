using System;
using RedditImageScheduler.Data;
using SQLite;

namespace RedditImageScheduler.IO {
	public class ReddIODatabase {
		private readonly string sFile;

		private SQLiteConnection sqlConnection;
		private ReddIOEntries ioEntries;

		public ReddIODatabase(string file) {
			sFile = file;
		}

		// ================================================================================
		// GETTERS / SETTERS
		public string FilePath => sFile;
		public bool IsOpen => sqlConnection != null;
		
		public ReddDataEntries Entries => ioEntries.Data;

		// ================================================================================
		// PUBLIC
		public void Open() {
			try {
				sqlConnection = new SQLiteConnection(sFile);
				//TODO: remember to remove those drops in release
				sqlConnection.DropTable<ReddDataEntry>();

				sqlConnection.CreateTable<ReddDataEntry>();
				
				
				ioEntries = new ReddIOEntries(sqlConnection);
				ioEntries.OnError += OnEntriesError;
			}
			catch( Exception ) {
				sqlConnection = null;
				Console.WriteLine("Unable to initialize RedditIO database.");
				OnErrorInitialize?.Invoke();
			}
		}
		
		public void Close() {
			if( ioEntries != null ) {
				ioEntries.OnError -= OnEntriesError;
			}
			sqlConnection.Close();
			sqlConnection.Dispose();
		}

		// ===============================================
		// EVENTS
		public delegate void DatabaseEvent();

		public event DatabaseEvent OnErrorInitialize;
		public event DatabaseEvent OnErrorChange;

		// ===============================================
		// CALLBACKS
		private void OnEntriesError() {
			OnErrorChange?.Invoke();
		}
	}
}