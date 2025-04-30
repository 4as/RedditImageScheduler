using System.IO;
using System.Reflection;
using Eto.Drawing;
using Eto.Forms;
using RedditImageScheduler.Data;
using RedditImageScheduler.Utils;

namespace RedditImageScheduler.UI.Core {
	public class ReddUIImage : ImageView {
		private static readonly Icon ICON_PLACEHOLDER = Icon.FromResource("RedditImageScheduler.Resources.drop_icon.png", Assembly.GetExecutingAssembly());

		private readonly ReddUtilBitmap utilBitmap = new ReddUtilBitmap();
		private ReddDataEntry dataEntry;

		public ReddUIImage() {
			Image = ICON_PLACEHOLDER;
		}
		
		public bool HasFile => utilBitmap.HasFormat;
		
		public byte[] ToByteArray() => dataEntry != null ? dataEntry.Image : utilBitmap.ToByteArray();

		public void Unset() {
			utilBitmap.Unset();
			Image = ICON_PLACEHOLDER;
			dataEntry = null;
		}

		public void Set(string file_path) {
			if( file_path == null ) {
				Unset();
				return;
			}
			
			try {
				FileStream file = File.Open(file_path, FileMode.Open);
				utilBitmap.Set(file);
				Image = utilBitmap.Bitmap;
				dataEntry = null;
			}
			catch( System.Exception ) {
				MessageBox.Show(string.Format(ReddLanguage.ERROR_FILE_NOT_FOUND, file_path), ReddLanguage.ERROR, MessageBoxType.Error);
			}
		}

		public void Set(ReddDataEntry entry) {
			if( entry?.Image == null ) {
				Unset();
				return;
			}
			
			dataEntry = entry;
			utilBitmap.Set(dataEntry);
			Image = utilBitmap.Bitmap;
		}
	}
}