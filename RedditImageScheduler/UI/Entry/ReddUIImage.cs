using System;
using System.IO;
using System.Reflection;
using Eto.Drawing;
using Eto.Forms;
using RedditImageScheduler.Data;
using RedditImageScheduler.Utils;

namespace RedditImageScheduler.UI.Entry {
	public class ReddUIImage : ImageView {
		private static readonly Icon ICON_PLACEHOLDER = Icon.FromResource("RedditImageScheduler.Resources.drop_icon.png", Assembly.GetExecutingAssembly());

		private readonly OpenFileDialog etoDialog = new OpenFileDialog();
		
		private ReddUtilBitmap utilBitmap;
		private bool hasChanged = false;
		private bool isPressed = false;

		public ReddUIImage() {
			etoDialog.MultiSelect = false;
			etoDialog.Filters.Add(ReddConfig.IMAGE_FILTER);
			
			Image = ICON_PLACEHOLDER;
		}

		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			MouseDown += OnImageDown;
			MouseLeave += OnImageLeave;
			MouseUp += OnImageUp;
		}

		protected override void OnUnLoad(EventArgs e) {
			base.OnUnLoad(e);
			MouseDown -= OnImageDown;
			MouseLeave -= OnImageLeave;
			MouseUp -= OnImageUp;
		}

		// ===============================================
		// GETTERS / SETTERS
		public bool HasChanged => hasChanged;
		public bool IsValid => utilBitmap.Bitmap != null;
		public ReddUtilBitmap Bitmap => utilBitmap;

		// ===============================================
		// PUBLIC METHODS
		public void Unset() {
			SetImage( new ReddUtilBitmap() );
			hasChanged = false;
		}
		
		public void Set(ReddUtilBitmap bitmap) {
			if( !bitmap.IsValid ) {
				Unset();
				return;
			}
			
			SetImage(bitmap);
		}

		public void Set(string file_path) {
			if( file_path == null ) {
				Unset();
				return;
			}
			
			try {
				byte[] bytes = File.ReadAllBytes( file_path );
				SetImage( new ReddUtilBitmap(file_path, bytes) );
				hasChanged = true;
			}
			catch( Exception ) {
				MessageBox.Show(string.Format(ReddLanguage.ERROR_FILE_NOT_FOUND, file_path), ReddLanguage.ERROR, MessageBoxType.Error);
			}
		}
		
		public void Set(Image image) {
			if( image == null ) {
				Unset();
				return;
			}
			
			SetImage(new ReddUtilBitmap(image));
			hasChanged = true;
		}
		
		public byte[] ToByteArray() => utilBitmap.ToByteArray();

		// ===============================================
		// NON-PUBLIC METHODS
		private void SetImage(ReddUtilBitmap bitmap) {
			utilBitmap = bitmap;
			if( utilBitmap.Bitmap == null ) {
				Image = ICON_PLACEHOLDER;
				BackgroundColor = ReddConfig.UI_BG_DEFAULT;
			}
			else {
				Image = utilBitmap.Bitmap;
				BackgroundColor = ReddConfig.UI_BG_EMPTY;
			}

			OnImageChanged?.Invoke();
		}

		// ===============================================
		// EVENTS
		public delegate void UIImageEvent();
		public event UIImageEvent OnImageChanged;

		// ===============================================
		// CALLBACKS
		private void OnImageDown(object sender, MouseEventArgs e) {
			isPressed = true;
		}
		
		private void OnImageLeave(object sender, MouseEventArgs e) {
			isPressed = false;
		}
		
		private void OnImageUp(object sender, MouseEventArgs e) {
			if( !isPressed ) return;

			isPressed = false;
			
			if( !Bounds.Contains((Point)e.Location) ) return;
			
			var result = etoDialog.ShowDialog(ParentWindow);
			if( etoDialog.FileName != null && (result == DialogResult.Yes || result == DialogResult.Ok) ) {
				Set(etoDialog.FileName);
			}
		}
	}
}