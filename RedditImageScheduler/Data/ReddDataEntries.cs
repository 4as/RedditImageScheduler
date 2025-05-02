using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using RedditImageScheduler.IO;
using RedditImageScheduler.Utils;

namespace RedditImageScheduler.Data {
	public class ReddDataEntries : IReadOnlyList<ReddDataEntry>, INotifyCollectionChanged {
		private readonly ReddIOEntries ioEntries;
		private readonly List<ReddDataEntry> listEntries;
		public ReddDataEntries(ReddIOEntries entries) {
			ioEntries = entries;
			listEntries = entries.GetAll();
		}
		
		public int Count => listEntries.Count;
		
		public ReddDataEntry this[int index] => listEntries[index];
		public ReddDataEntry this[uint index] => listEntries[(int)index];

		public ReddDataEntry Create() {
			ReddDataEntry entry = Add(string.Empty, string.Empty, GetNextTime(), null);
			return entry;
		}
		
		public ReddDataEntry Add(string title, string source, DateTime date, byte[] image) {
			ReddDataEntry raw = new ReddDataEntry();
			raw.Title = title;
			raw.Source = source;
			raw.Image = image;
			raw.Date = date;
			listEntries.Add(raw);
			ioEntries.Insert(raw);
			return raw;
		}

		public void Update(ReddDataEntry entry) {
			int idx = listEntries.IndexOf(entry);
			if( idx == -1 ) {
				listEntries.Add(entry);
				ioEntries.Insert(entry);
			}
			else {
				ioEntries.Update(entry);
			}

			ReddUtilBitmaps.Add(entry.Id, entry.Image);
		}

		public void Remove(ReddDataEntry entry) {
			int idx = listEntries.IndexOf(entry);
			if( idx == -1 ) return;
			
			listEntries.RemoveAt(idx);
			
			ioEntries.Remove(entry);
		}

		public int IndexOf(ReddDataEntry entry) {
			return listEntries.IndexOf(entry);
		}

		public IEnumerator<ReddDataEntry> GetEnumerator() => listEntries.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => listEntries.GetEnumerator();

		// ===============================================
		// NON-PUBLIC METHODS
		protected DateTime GetNextTime() {
			long timestamp = 0;
			ReddDataEntry target = null;
			foreach( var entry in listEntries ) {
				if( entry.Timestamp > timestamp ) {
					target = entry;
					timestamp = entry.Timestamp;
				}
			}

			if( target == null ) {
				return DateTime.Now.AddHours(ReddConfig.ENTRY_HOURS_SPACING);
			}
			else {
				return target.Date.AddHours(ReddConfig.ENTRY_HOURS_SPACING);
			}
		}

		// ===============================================
		// EVENTS
		public event ReddIOEntries.IOEntriesEvent OnUpdate {
			add => ioEntries.OnUpdate += value;
			remove => ioEntries.OnUpdate -= value;
		}
		
		public event ReddIOEntries.IOEntriesEvent OnError {
			add => ioEntries.OnError += value;
			remove => ioEntries.OnError -= value;
		}

		public event NotifyCollectionChangedEventHandler CollectionChanged {
			add => ioEntries.CollectionChanged += value;
			remove => ioEntries.CollectionChanged -= value;
		}
	}
}