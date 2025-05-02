using System.IO;
using Eto.Drawing;
using RedditImageScheduler.Data;

namespace RedditImageScheduler.Utils {
	public static class ReddUtilBitmaps {
		private static readonly ReddUtilCache<uint> CACHE_ENTRIES = new ReddUtilCache<uint>(ReddConfig.CACHE_SIZE);
		private static readonly ReddUtilCache<string> CACHE_FILES = new ReddUtilCache<string>(ReddConfig.CACHE_SIZE);

		public static uint NumEntries => CACHE_ENTRIES.Length;
		public static uint NumFiles => CACHE_FILES.Length;
		
		public static Bitmap Get(uint id, byte[] image_bytes) {
			Bitmap bmp = CACHE_ENTRIES.Get(id);
			if( bmp == null ) {
				bmp = new Bitmap(image_bytes);
				CACHE_ENTRIES.Add(id, bmp);
			}

			return bmp;
		}

		public static Bitmap Get(string filepath, byte[] image_bytes) {
			Bitmap bmp = CACHE_FILES.Get(filepath);
			if( bmp == null ) {
				bmp = new Bitmap(image_bytes);
				CACHE_FILES.Add(filepath, bmp);
			}

			return bmp;
		}

		public static void Add(uint id, byte[] image_bytes) {
			if( image_bytes == null ) return;
			Bitmap bmp = new Bitmap(image_bytes);
			CACHE_ENTRIES.Add(id, bmp);
		}
	}
}