using System;
using Eto.Drawing;
using Eto.Forms;
using RedditImageScheduler.Data;
using RedditImageScheduler.UI.Entry;
using RedditImageScheduler.Utils;

namespace RedditImageScheduler.UI {
	public class ReddUIContentEntry : DynamicLayout {
		private readonly DynamicLayout etoButtons = new DynamicLayout();
		private readonly Button etoSave = new Button();
		private readonly Button etoDelete = new Button();
		private readonly ReddUITitle uiTitle = new ReddUITitle();
		private readonly ReddUISource uiSource = new ReddUISource();
		private readonly ReddUIDate uiDate = new ReddUIDate();
		private readonly ReddUIImage uiImage = new ReddUIImage();
		private readonly ReddUIStatus uiStatus = new ReddUIStatus();

		private readonly ReddUtilDropHandler utilDrop = new ReddUtilDropHandler();
		private ReddDataEntry dataEntry;

		public ReddUIContentEntry() {
			Spacing = new Size(2, 2);
			
			BeginVertical();
			
			etoButtons.BeginHorizontal();
			etoSave.Text = ReddLanguage.SAVE;
			etoButtons.Add(etoSave);
			etoButtons.AddSpace();
			etoDelete.Text = ReddLanguage.REMOVE;
			etoButtons.Add(etoDelete);
			etoButtons.EndHorizontal();
			
			Add(etoButtons);
			Add(uiTitle);
			Add(uiSource);
			Add(uiDate);
			AddCentered( uiImage );
			Add(uiStatus);
			
			EndVertical();

			AllowDrop = true;
			Visible = false;
		}
		
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			OnAlign();
		}

		// ===============================================
		// GETTERS / SETTERS
		public ReddDataEntry Data => dataEntry;
		public bool HasChanges => dataEntry != null && uiStatus.HasChanges;
		public bool IsValid => uiTitle.IsValid && uiSource.IsValid && uiImage.IsValid && uiDate.IsValid;

		// ===============================================
		// PUBLIC METHODS
		public void Set(ReddDataEntry entry) {
			if( dataEntry == entry ) return;
			
			Unset();

			if( entry == null ) return;
			
			dataEntry = entry;
			
			uiTitle.Text = entry.Title;
			uiSource.Text = entry.Source;
			uiDate.Date = entry.Date;
			uiImage.Set(dataEntry.Bitmap);
			
			uiStatus.LastDate = entry.Date;
			uiStatus.CurrentDate = entry.Date;
			OnValidate();
			
			etoSave.Click += OnButtonSave;
			etoDelete.Click += OnButtonDelete;
			
			uiTitle.OnTitleChanged += OnModify;
			uiDate.OnDateChanged += OnModify;
			uiImage.OnImageChanged += OnModify;
			uiSource.OnSourceChanged += OnModify;

			utilDrop.OnDropFile += OnDropFile;
			utilDrop.OnDropURL += OnDropURL;
			utilDrop.OnDropImage += OnDropImage;
			
			Visible = true;
		}

		public void Unset() {
			uiStatus.HasChanges = false;
			
			uiTitle.OnTitleChanged -= OnModify;
			uiDate.OnDateChanged -= OnModify;
			uiImage.OnImageChanged -= OnModify;
			uiSource.OnSourceChanged -= OnModify;
			
			etoSave.Click -= OnButtonSave;
			etoDelete.Click -= OnButtonDelete;
			
			utilDrop.OnDropFile += OnDropFile;
			utilDrop.OnDropURL += OnDropURL;
			utilDrop.OnDropImage += OnDropImage;
			
			Visible = false;
			dataEntry = null;
		}

		public ReddDataEntry Commit() {
			dataEntry.Title = uiTitle.Text;
			dataEntry.Source = uiSource.Text;
			dataEntry.Bitmap = uiImage.Bitmap;
			dataEntry.Date = uiDate.Date;
			dataEntry.IsValid = IsValid;
			uiImage.Set(dataEntry.Bitmap);
			uiStatus.HasChanges = false;
			return dataEntry;
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

		protected override void OnDragEnter(DragEventArgs e) {
			base.OnDragEnter(e);
			utilDrop.HandleStart(e);
		}

		protected override void OnDragDrop(DragEventArgs e) {
			base.OnDragDrop(e);
			utilDrop.HandleDrop(e);
		}
		
		protected void OnDropFile(string filepath) {
			uiImage.Set(filepath);
		}

		protected void OnDropURL(string url) {
			uiSource.Text = url;
		}

		protected void OnDropImage(Image image) {
			uiImage.Set(image);
		}

		protected void OnAlign() {
			uiTitle.Width = Width;
			uiSource.Width = Width;
			uiStatus.Width = Width;
			int height = Height - (uiTitle.Height + uiSource.Height + uiDate.Height + uiStatus.Height + etoButtons.Height);
			int size = Width < height ? Width : height;
			uiImage.Size = new Size(size, size);
		}

		protected void OnValidate() {
			uiStatus.HasValidTitle = uiTitle.IsValid;
			uiStatus.HasValidSource = uiSource.IsValid;
			uiStatus.HasValidDate = uiDate.IsValid;
			uiStatus.HasValidImage = uiImage.IsValid;
		}
		
		private void OnModify() {
			uiStatus.HasChanges = uiTitle.Text != dataEntry.Title ||
								  uiSource.Text != dataEntry.Source ||
								  uiDate.Date != dataEntry.Date ||
								  uiImage.HasChanged;
			OnValidate();
		}
		
		private void OnButtonSave(object sender, EventArgs e) {
			OnSave?.Invoke();
		}

		private void OnButtonDelete(object sender, EventArgs e) {
			OnDelete?.Invoke();
		}
	}
}