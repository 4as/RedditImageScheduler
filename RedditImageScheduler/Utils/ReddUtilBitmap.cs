using System.IO;
using Eto.Drawing;
using RedditImageScheduler.Data;

namespace RedditImageScheduler.Utils {
	public class ReddUtilBitmap {
		private Bitmap bmpBitmap;
		
		public Bitmap Bitmap => bmpBitmap;

		public void Unset() {
			bmpBitmap = null;
		}
		
		public void Set(FileStream file) {
			if( file == null) {
				Unset();
				return;
			}

			bmpBitmap = ReddUtilBitmaps.Get(file);
		}

		public void Set(ReddDataEntry entry) {
			if( entry?.Image == null ) {
				Unset();
				return;
			}
			
			bmpBitmap = ReddUtilBitmaps.Get(entry);
		}

		public void Set(Image image) {
			if( image == null ) {
				Unset();
				return;
			}
			
			bmpBitmap = new Bitmap(image);
		}

		public byte[] ToByteArray() {
			return bmpBitmap?.ToByteArray(ImageFormat.Png) ?? null;
		}
	}
}