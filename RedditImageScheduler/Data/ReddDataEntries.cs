using System.Collections;
using System.Collections.Generic;
using Eto.Drawing;
using RedditImageScheduler.IO;

namespace RedditImageScheduler.Data {
	public class ReddDataEntries : IReadOnlyList<ReddDataEntry> {
		private readonly ReddIOEntries ioEntries;
		private readonly List<ReddDataEntry> listEntries;
		public ReddDataEntries(ReddIOEntries entries) {
			ioEntries = entries;
			listEntries = entries.GetAll();
		}
		
		public int Count => listEntries.Count;
		
		public ReddDataEntry this[int index] => listEntries[index];
		public ReddDataEntry this[uint index] => listEntries[(int)index];
		
		public ReddDataEntry Add(string title, string source, uint timestamp, byte[] image) {
			ReddDataEntry raw = new ReddDataEntry();
			raw.Title = title;
			raw.Source = source;
			raw.Image = image;
			raw.Timestamp = timestamp;
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
		}

		public void Remove(ReddDataEntry entry) {
			int idx = listEntries.IndexOf(entry);
			if( idx == -1 ) return;
			
			listEntries.RemoveAt(idx);
		}

		public int IndexOf(ReddDataEntry entry) {
			return listEntries.IndexOf(entry);
		}

		public IEnumerator<ReddDataEntry> GetEnumerator() => listEntries.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => listEntries.GetEnumerator();

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
	}
}