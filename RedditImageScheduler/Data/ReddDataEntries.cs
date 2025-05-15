using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using RedditImageScheduler.IO;
using RedditImageScheduler.Utils;

namespace RedditImageScheduler.Data {
	public class ReddDataEntries : IReadOnlyList<ReddDataEntry>, INotifyCollectionChanged {
		private readonly ReddIOEntries ioEntries;
		private List<ReddDataEntry> listEntries;
		public ReddDataEntries(ReddIOEntries entries) {
			ioEntries = entries;
		}
		
		public int Count => listEntries.Count;
		
		public ReddDataEntry this[int index] => listEntries[index];
		public ReddDataEntry this[uint index] => listEntries[(int)index];

		public void Open() {
			listEntries = ioEntries.GetAll();

			foreach( var entry in listEntries ) {
				entry.Validate();
				Update(entry);
			}
		}

		public ReddDataEntry Create(uint hours_spacing) {
			ReddDataEntry entry = Add(string.Empty, string.Empty, FindNextTime(hours_spacing), null);
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
			entry.Validate();
			
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

		public void Consume(ReddDataEntry entry) {
			int idx = listEntries.IndexOf(entry);
			if( idx != -1 ) {
				ReddDebug.Trace("Consuming entry: " + entry.Title);
				entry.IsPosted = true;
				ioEntries.Update(entry);
			}
		}

		public void Trim(uint days_old) {
			for( int i = listEntries.Count - 1; i > -1; i-- ) {
				ReddDataEntry entry = listEntries[i];
				if( entry.IsPosted ) {
					var diff = DateTime.Now - entry.Date;
					if( diff.Days > days_old ) {
						ReddDebug.Trace("Trimming away entry: " + entry.Title);
						listEntries.RemoveAt(i);
						ioEntries.Remove(entry);
					}
				}
			}
		}

		public void Remove(ReddDataEntry entry) {
			int idx = listEntries.IndexOf(entry);
			if( idx == -1 ) return;
			
			listEntries.RemoveAt(idx);
			
			ioEntries.Remove(entry);
		}

		public int IndexOf(uint entry_id) {
			for( int i = listEntries.Count - 1; i > -1; i-- ) {
				ReddDataEntry entry = listEntries[i];
				if( entry.Id == entry_id ) return i;
			}

			return -1;
		}
		
		public int IndexOf(ReddDataEntry entry) {
			return listEntries.IndexOf(entry);
		}

		public IEnumerator<ReddDataEntry> GetEnumerator() => listEntries.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => listEntries.GetEnumerator();

		// ===============================================
		// NON-PUBLIC METHODS
		protected DateTime FindNextTime(uint hours_spacing) {
			long timestamp = 0;
			ReddDataEntry target = null;
			foreach( var entry in listEntries ) {
				if( entry.Timestamp > timestamp ) {
					target = entry;
					timestamp = entry.Timestamp;
				}
			}

#if DEBUG
			return DateTime.Now.AddMinutes(1);
#else
			if( target == null ) {
				return DateTime.Now.AddHours(hours_spacing);
			}
			else {
				return target.Date.AddHours(hours_spacing);
			}
#endif
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