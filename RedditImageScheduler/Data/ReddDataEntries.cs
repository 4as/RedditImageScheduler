using System.Collections;
using System.Collections.Generic;
using RedditImageScheduler.IO;

namespace RedditImageScheduler.Data {
	public readonly struct ReddDataEntries : IReadOnlyList<ReddDataEntry> {
		private readonly ReddIODatabase ioDatabase;
		private readonly List<ReddIODatabase.Entry> listEntries;
		
		public ReddDataEntries(ReddIODatabase database) {
			ioDatabase = database;
			listEntries = ioDatabase.GetAll();
		}
		
		public int Count => listEntries.Count;
		
		public ReddDataEntry this[int index] => new ReddDataEntry(listEntries[index]);
		public ReddDataEntry this[uint index] => new ReddDataEntry(listEntries[(int)index]);

		public void Add(ReddDataEntry entry) {
			int id = ioDatabase.Add(entry.Title, entry.SourceURL, entry.Image);
			if( id != 0 ) {
				listEntries.Add(ioDatabase.Get(id));
			}
		}
		
		public IEnumerator<ReddDataEntry> GetEnumerator() {
			foreach( var entry in listEntries ) {
				yield return new ReddDataEntry(entry);
			}
		}

		IEnumerator IEnumerable.GetEnumerator() {
			foreach( var entry in listEntries ) {
				yield return new ReddDataEntry(entry);
			}
		}
	}
}