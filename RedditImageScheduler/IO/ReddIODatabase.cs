using System;
using System.IO;
using RedditImageScheduler.Data;
using SQLite;

namespace RedditImageScheduler.IO {
	public class ReddIODatabase {
		private SQLiteConnection sqlConnection;
		private ReddIOEntries ioEntries;
		private string sFile;

		// ================================================================================
		// GETTERS / SETTERS
		public string FilePath => sFile;
		public bool IsOpen => sqlConnection != null;
		
		public ReddDataEntries Entries => ioEntries.Data;

		// ================================================================================
		// PUBLIC
		public void Open(string filepath) {
			try {
				SQLiteConnectionString options = new SQLiteConnectionString(filepath, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite, true);
				sqlConnection = new SQLiteConnection(options);
				
				//sqlConnection.DropTable<ReddDataEntry>();
				
				sqlConnection.CreateTable<ReddDataEntry>();

				sFile = filepath;
				
				ioEntries = new ReddIOEntries(sqlConnection);
				ioEntries.OnError += OnEntriesError;
			}
			catch( Exception ) {
				sqlConnection = null;
				Console.WriteLine("Unable to initialize RedditIO database.");
				EventErrorInitialize?.Invoke();
			}
		}
		
		public void Close() {
			if( ioEntries != null ) {
				ioEntries.OnError -= OnEntriesError;
			}
			sqlConnection.Close();
			sqlConnection.Dispose();

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