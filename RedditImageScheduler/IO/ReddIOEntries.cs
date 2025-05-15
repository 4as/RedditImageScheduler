using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using RedditImageScheduler.Data;
using SQLite;

namespace RedditImageScheduler.IO {
	public class ReddIOEntries : INotifyCollectionChanged {
		private readonly ReddDataEntries dataEntries;
		
		private SQLiteConnection sqlConnection;
		public ReddIOEntries() {
			dataEntries = new ReddDataEntries(this);
		}
		
		public ReddDataEntries Data => dataEntries;

		public void Open(SQLiteConnection connection) {
			sqlConnection = connection;
			dataEntries.Open();
		}

		public List<ReddDataEntry> GetAll() {
			try {
				return sqlConnection.Table<ReddDataEntry>().ToList();
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
					CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, entry));
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
				CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, null));
				OnUpdate?.Invoke();
			}
			catch( Exception ) {
				OnError?.Invoke();
			}
		}

		public void Remove(ReddDataEntry entry) {
			try {
				sqlConnection.Delete(entry);
				CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, entry));
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
		public event NotifyCollectionChangedEventHandler CollectionChanged;
	}
}