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
		private readonly ReddUtilBitmap utilBitmap = new ReddUtilBitmap();
		private ReddDataEntry dataEntry;
		private bool hasChanged = false;
		private bool isPressed = false;

		public ReddUIImage() {
			etoDialog.MultiSelect = false;
			etoDialog.Filters.Add(new FileFilter("Images", ".png", ".jpg", ".jpeg", ".gif", ".bmp"));
			
			Image = ICON_PLACEHOLDER;
			AllowDrop = true;
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

		// ===============================================
		// PUBLIC METHODS
		public void Unset() {
			utilBitmap.Unset();
			SetImage(utilBitmap);
			dataEntry = null;
			hasChanged = false;
		}

		public void Set(string file_path) {
			if( file_path == null ) {
				Unset();
				return;
			}
			
			try {
				using( FileStream file = File.OpenRead(file_path) ) {
					utilBitmap.Set(file);
					SetImage(utilBitmap);
					dataEntry = null;
				}
			}
			catch( Exception ) {
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
			SetImage(utilBitmap);
		}
		
		public byte[] ToByteArray() => dataEntry != null ? dataEntry.Image : utilBitmap.ToByteArray();

		// ===============================================
		// NON-PUBLIC METHODS
		private void SetImage(ReddUtilBitmap bitmap) {
			if( bitmap.Bitmap == null ) {
				Image = ICON_PLACEHOLDER;
				BackgroundColor = ReddConfig.UI_BG_DEFAULT;
			}
			else {
				Image = bitmap.Bitmap;
				BackgroundColor = new Color(0f, 0f, 0f, 0f);
			}

			OnImageChanged?.Invoke();
		}

		// ===============================================
		// EVENTS
		public delegate void UIImageEvent();
		public event UIImageEvent OnImageChanged;

		// ===============================================
		// CALLBACKS
		protected override void OnDragEnter(DragEventArgs e) {
			base.OnDragEnter(e);
			if( e.Data.ContainsUris || e.Data.ContainsImage ) {
				e.Effects = DragEffects.Copy;
			}
			else {
				e.Effects = DragEffects.None;
			}
		}

		protected override void OnDragDrop(DragEventArgs e) {
			base.OnDragDrop(e);
			if( e.Data.ContainsUris ) {
				Uri uri = e.Data.Uris[0];
				if( uri.IsFile ) {
					Set(uri.LocalPath);
					hasChanged = true;
				}
			}
			else if( e.Data.ContainsImage ) {
				utilBitmap.Set(e.Data.Image);
				SetImage(utilBitmap);
				dataEntry = null;
				hasChanged = true;
			}
		}
		
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
				hasChanged = true;
			}
		}
	}
}