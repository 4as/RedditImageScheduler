using System;
using System.IO;
using System.Reflection;
using Eto.Drawing;
using Eto.Forms;
using RedditImageScheduler.Data;
using RedditImageScheduler.UI.Core;

namespace RedditImageScheduler.UI {
	public class ReddUIContentEntry : DynamicLayout {
		private readonly DynamicLayout etoButtons = new DynamicLayout();
		private readonly Button etoSave = new Button();
		private readonly Button etoDelete = new Button();
		private readonly TextBox etoTitle = new TextBox();
		private readonly TextBox etoSource = new TextBox();
		private readonly DynamicLayout etoDate = new DynamicLayout();
		private readonly Label etoLabelDate = new Label();
		private readonly ReddUIDate uiDatePicker = new ReddUIDate();
		private readonly Label etoLabelHours = new Label();
		private readonly NumericStepper etoDateHours = new NumericStepper();
		private readonly ReddUIImage uiImage = new ReddUIImage();
		private readonly OpenFileDialog etoDialog = new OpenFileDialog();
		
		private ReddDataEntry dataEntry;

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
			
			uiDatePicker.Mode = DateTimePickerMode.Date;
			uiDatePicker.MinDate = DateTime.Today;
			etoDate.Add(uiDatePicker);
			
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
			
			AddCentered(uiImage);
			
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

			DateTime date = entry.GetDate();
			uiDatePicker.Value = date;
			etoDateHours.Value = date.Hour;
			etoTitle.Text = entry.Title;
			etoSource.Text = entry.Source;
			uiImage.Set(dataEntry);
			
			etoSave.Click += OnButtonSave;
			etoDelete.Click += OnButtonDelete;
			uiImage.MouseUp += OnImage;
			
			Visible = true;
		}

		public void Unset() {
			uiImage.MouseUp -= OnImage;
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
			uiImage.Size = new Size(size, size);
		}
		
		private void OnButtonSave(object sender, EventArgs e) {
			dataEntry.Title = etoTitle.Text;
			dataEntry.Source = etoSource.Text;
			dataEntry.Image = uiImage.ToByteArray(); 
			dataEntry.SetDate( uiDatePicker.GetDateAtHour((int)etoDateHours.Value) );
			OnSave?.Invoke();
		}

		private void OnButtonDelete(object sender, EventArgs e) {
			OnDelete?.Invoke();
		}
		
		private void OnImage(object sender, MouseEventArgs e) {
			var result = etoDialog.ShowDialog(ParentWindow);
			if( etoDialog.FileName != null && (result == DialogResult.Yes || result == DialogResult.Ok) ) {
				uiImage.Set(etoDialog.FileName);
			}
		}
	}
}