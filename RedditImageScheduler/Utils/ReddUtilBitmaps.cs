using System.IO;
using Eto.Drawing;
using RedditImageScheduler.Data;

namespace RedditImageScheduler.Utils {
	public static class ReddUtilBitmaps {
		private static readonly ReddUtilCache<uint> CACHE_ENTRIES = new ReddUtilCache<uint>(ReddConfig.CACHE_SIZE);
		private static readonly ReddUtilCache<string> CACHE_FILES = new ReddUtilCache<string>(ReddConfig.CACHE_SIZE);
		
		public static Bitmap Get(ReddDataEntry entry) {
			Bitmap bmp = CACHE_ENTRIES.Get(entry.Id);
			if( bmp == null ) {
				bmp = new Bitmap(entry.Image);
				CACHE_ENTRIES.Add(entry.Id, bmp);
			}

			return bmp;
		}

		public static Bitmap Get(FileStream stream) {
			Bitmap bmp = CACHE_FILES.Get(stream.Name);
			if( bmp == null ) {
				bmp = new Bitmap(stream);
				CACHE_FILES.Add(stream.Name, bmp);
			}

			return bmp;
		}

		public static void Add(ReddDataEntry entry) {
			if( entry.Image == null ) return;
			CACHE_ENTRIES.Add(entry.Id, new Bitmap(entry.Image));
		}
	}
}