using System;
using System.Collections.Generic;
using RedditImageScheduler.Data;
using SQLite;

namespace RedditImageScheduler.IO {
	public class ReddIOEntries {
		private readonly SQLiteConnection sqlConnection;
		private readonly ReddDataEntries dataEntries;
		public ReddIOEntries(SQLiteConnection connection) {
			sqlConnection = connection;
			dataEntries = new ReddDataEntries(this);
		}
		
		public ReddDataEntries Data => dataEntries;

		public List<ReddDataEntry> GetAll() {
			try {
				sqlConnection.Query<ReddDataEntry>("SELECT * FROM " + nameof(ReddDataEntry));
			}
			catch( Exception ) {
				OnError?.Invoke();
			}

			return new List<ReddDataEntry>();
		} 

		public void Insert(ReddDataEntry entry) {
			try {
				int added = sqlConnection.Insert(entry);
				if( added == 0 ) {
					OnError?.Invoke();
				}
				else {
					OnUpdate?.Invoke();
				}
			}
			catch( Exception ) {
				OnError?.Invoke();
			}
		}

		public void Update(ReddDataEntry entry) {
			try {
				sqlConnection.Update(entry);
				OnUpdate?.Invoke();
			}
			catch( Exception ) {
				OnError?.Invoke();
			}
		}

		public void Remove(ReddDataEntry entry) {
			try {
				sqlConnection.Delete(entry);
				OnUpdate?.Invoke();
			}
			catch( Exception ) {
				OnError?.Invoke();
			}
		}

		// ===============================================
		// EVENTS
		public delegate void IOEntriesEvent();

		public event IOEntriesEvent OnUpdate;
		public event IOEntriesEvent OnError;
	}
}