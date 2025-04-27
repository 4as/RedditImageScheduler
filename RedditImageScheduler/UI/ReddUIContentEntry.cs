using System;
using System.IO;
using System.Reflection;
using Eto.Drawing;
using Eto.Forms;
using RedditImageScheduler.Data;

namespace RedditImageScheduler.UI {
	public class ReddUIContentEntry : DynamicLayout {
		private readonly DynamicLayout etoButtons = new DynamicLayout();
		private readonly Button etoSave = new Button();
		private readonly Button etoDelete = new Button();
		private readonly TextBox etoTitle = new TextBox();
		private readonly TextBox etoSource = new TextBox();
		private readonly DynamicLayout etoDate = new DynamicLayout();
		private readonly Label etoLabelDate = new Label();
		private readonly DateTimePicker etoDatePicker = new DateTimePicker();
		private readonly Label etoLabelHours = new Label();
		private readonly NumericStepper etoDateHours = new NumericStepper();
		private readonly ImageView etoImage = new ImageView();
		private readonly OpenFileDialog etoDialog = new OpenFileDialog();
		
		private readonly Bitmap bmpPlaceholder = new Bitmap(128,128,PixelFormat.Format24bppRgb);
		private ReddDataEntry dataEntry;
		private Bitmap bmpCurrent;
		private ImageFormat enumFormat;

		public ReddUIContentEntry() {
			Spacing = new Size(2, 2);

			etoDialog.MultiSelect = false;
			etoDialog.Filters.Add(new FileFilter("Images", ".png", ".jpg", ".jpeg", ".gif", ".bmp"));

			BeginVertical();
			
			etoButtons.BeginHorizontal();
			etoSave.Text = ReddLanguage.SAVE;
			etoButtons.Add(etoSave);
			etoButtons.AddSpace();
			etoDelete.Text = ReddLanguage.REMOVE;
			etoButtons.Add(etoDelete);
			etoButtons.EndHorizontal();
			Add(etoButtons);
			
			etoTitle.PlaceholderText = ReddLanguage.TITLE;
			Add(etoTitle);
			
			etoSource.PlaceholderText = ReddLanguage.SOURCE;
			Add(etoSource);
			
			etoDate.BeginHorizontal();
			
			etoLabelDate.VerticalAlignment = VerticalAlignment.Center;
			etoLabelDate.Text = ReddLanguage.DATE;
			etoDate.Add(etoLabelDate);
			
			etoDatePicker.Mode = DateTimePickerMode.Date;
			etoDatePicker.MinDate = DateTime.Today;
			//todo: kind of ugly - move it to a dedicated extending class? 
			PropertyInfo prop = etoDatePicker.ControlObject.GetType().GetProperty("ShowCheckBox");
			if( prop != null && prop.CanWrite ) {
				prop.SetValue(etoDatePicker.ControlObject, false, null);
			}
			etoDate.Add(etoDatePicker);
			
			etoLabelHours.VerticalAlignment = VerticalAlignment.Center;
			etoLabelHours.Text = ReddLanguage.HOUR;
			etoDate.Add(etoLabelHours);
			
			etoDateHours.Increment = 1;
			etoDateHours.Wrap = true;
			etoDateHours.DecimalPlaces = 0;
			etoDateHours.MaximumDecimalPlaces = 0;
			etoDateHours.MinValue = 0;
			etoDateHours.MaxValue = 23;
			etoDate.Add(etoDateHours);
			
			etoDate.EndHorizontal();
			Add(etoDate);
			
			AddCentered(etoImage);
			
			EndVertical();
			
			Visible = false;
		}
		
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			OnAlign();
		}

		// ===============================================
		// GETTERS / SETTERS
		public ReddDataEntry Data => dataEntry;

		// ===============================================
		// PUBLIC METHODS
		public void Set(ReddDataEntry entry) {
			if( dataEntry == entry ) return;
			
			Unset();

			if( entry == null ) return;
			
			dataEntry = entry;
			//todo: create a Bitmap pooler, rather than instancing new bitmaps every time
			bmpCurrent = dataEntry.Image == null ? null : new Bitmap(entry.Image);

			DateTime date = entry.GetDate();
			etoDatePicker.Value = date;
			etoDateHours.Value = date.Hour;
			etoTitle.Text = entry.Title;
			etoSource.Text = entry.Source;
			etoImage.Image = bmpCurrent ?? bmpPlaceholder;
			
			etoSave.Click += OnButtonSave;
			etoDelete.Click += OnButtonDelete;
			etoImage.MouseUp += OnImage;
			
			Visible = true;
		}

		public void Unset() {
			etoImage.MouseUp -= OnImage;
			etoSave.Click -= OnButtonSave;
			etoDelete.Click -= OnButtonDelete;
			Visible = false;
			dataEntry = null;
		}

		// ===============================================
		// EVENTS
		public delegate void UIEntryEvent();
		public event UIEntryEvent OnSave;
		public event UIEntryEvent OnDelete;

		// ===============================================
		// CALLBACKS
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			OnAlign();
		}

		protected virtual void OnAlign() {
			etoTitle.Width = Width;
			etoSource.Width = Width;
			int height = Height - (etoTitle.Height + etoSource.Height + etoDate.Height);
			int size = Width < height ? Width : height;
			etoImage.Size = new Size(size, size);
		}
		
		private void OnButtonSave(object sender, EventArgs e) {
			dataEntry.Title = etoTitle.Text;
			dataEntry.Source = etoSource.Text;
			dataEntry.Image = bmpCurrent?.ToByteArray(enumFormat);
			DateTime date = etoDatePicker.Value.GetValueOrDefault();
			date = new DateTime(date.Year, date.Month, date.Day, (int)etoDateHours.Value, date.Minute, 0, date.Kind);
			dataEntry.SetDate(date);
			OnSave?.Invoke();
		}

		private void OnButtonDelete(object sender, EventArgs e) {
			OnDelete?.Invoke();
		}
		
		private void OnImage(object sender, MouseEventArgs e) {
			var result = etoDialog.ShowDialog(ParentWindow);
			if( etoDialog.FileName != null && (result == DialogResult.Yes || result == DialogResult.Ok) ) {
				FileStream file;
				try {
					file = File.Open(etoDialog.FileName, FileMode.Open);
				}
				catch( Exception ) {
					MessageBox.Show(string.Format(ReddLanguage.ERROR_FILE_NOT_FOUND, etoDialog.FileName), ReddLanguage.ERROR, MessageBoxType.Error);
					return;
				}
				
				string ext = Path.GetExtension(etoDialog.FileName);
				switch( ext ) {
					case ".png": enumFormat = ImageFormat.Png; break;
					case ".jpg": enumFormat = ImageFormat.Jpeg; break;
					case ".jpeg": enumFormat = ImageFormat.Jpeg; break;
					case ".gif": enumFormat = ImageFormat.Gif; break;
					case ".bmp": enumFormat = ImageFormat.Bitmap; break;
					default: return;
				}
				
				bmpCurrent = new Bitmap(file);
				etoImage.Image = bmpCurrent;
			}
		}
	}
}