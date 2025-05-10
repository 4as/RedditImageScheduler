using Eto.Drawing;

namespace RedditImageScheduler.Utils {
	public readonly struct ReddUtilBitmap {
		private readonly Bitmap bmpBitmap;
		public ReddUtilBitmap(uint id, byte[] image_bytes) {
			bmpBitmap = image_bytes != null ? ReddUtilBitmaps.Get(id, image_bytes) : null;
		}

		public ReddUtilBitmap(string filepath, byte[] image_bytes) {
			if( filepath == null || image_bytes == null) {
				bmpBitmap = null;
			}
			else {
				bmpBitmap = ReddUtilBitmaps.Get(filepath, image_bytes);
			}
		}
		
		public ReddUtilBitmap(Bitmap image) {
			bmpBitmap = image;
		}

		public ReddUtilBitmap(Image image) {
			bmpBitmap = new Bitmap(image);
		}
		
		public bool IsValid => bmpBitmap != null;
		public Bitmap Bitmap => bmpBitmap;
		
		public static implicit operator Bitmap(ReddUtilBitmap bitmap) => bitmap.Bitmap;
		public static implicit operator ReddUtilBitmap(Bitmap bitmap) => new ReddUtilBitmap(bitmap);
		public static implicit operator ReddUtilBitmap(Image image) => new ReddUtilBitmap(image);

		public byte[] ToByteArray() {
			return bmpBitmap?.ToByteArray(ImageFormat.Png);
		}

		public static ImageFormat GetImageFormat(string extension) {
			switch( extension ) {
				case ".png": return ImageFormat.Png;
				case ".jpg": return ImageFormat.Jpeg;
				case ".jpeg": return ImageFormat.Jpeg;
				case ".gif": return ImageFormat.Gif;
				case ".bmp": return ImageFormat.Bitmap;
				default: return ImageFormat.Bitmap;
			}
		}
	}
}