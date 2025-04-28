using System.IO;
using Eto.Drawing;
using RedditImageScheduler.Data;

namespace RedditImageScheduler.Utils {
	public class ReddUtilBitmap {
		private Bitmap bmpBitmap;
		private ImageFormat enumFormat;
		private bool hasFormat;
		
		public Bitmap Bitmap => bmpBitmap;
		public bool HasFormat => hasFormat;

		public void Unset() {
			bmpBitmap = null;
			hasFormat = false;
		}
		
		public void Set(FileStream file) {
			if( file == null ) {
				Unset();
				return;
			}
			
			string ext = Path.GetExtension(file.Name);
			hasFormat = true;
			switch( ext ) {
				case ".png": enumFormat = ImageFormat.Png; break;
				case ".jpg": enumFormat = ImageFormat.Jpeg; break;
				case ".jpeg": enumFormat = ImageFormat.Jpeg; break;
				case ".gif": enumFormat = ImageFormat.Gif; break;
				case ".bmp": enumFormat = ImageFormat.Bitmap; break;
				default: Unset(); return;
			}

			bmpBitmap = ReddUtilBitmaps.GetBitmap(file);
		}

		public void Set(ReddDataEntry entry) {
			if( entry?.Image == null ) {
				Unset();
				return;
			}
			
			hasFormat = false;
			bmpBitmap = ReddUtilBitmaps.GetBitmap(entry);
		}

		public byte[] ToByteArray() {
			return hasFormat ? bmpBitmap.ToByteArray(enumFormat) : null;
		}
	}
}