using System;
using Eto.Drawing;
using Eto.Forms;
using RedditImageScheduler.Data;
using RedditImageScheduler.Utils;

namespace RedditImageScheduler.UI.Editor {
	public class ReddUIEntryEdit : DynamicLayout {
		private readonly DynamicLayout etoButtons = new DynamicLayout();
		private readonly Button etoSave = new Button();
		private readonly Label etoConfirm = new Label();
		private readonly Button etoDelete = new Button();
		private readonly ReddUITitle uiTitle = new ReddUITitle();
		private readonly ReddUISource uiSource = new ReddUISource();
		private readonly ReddUIImage uiImage = new ReddUIImage();
		private readonly ReddUIStatus uiStatus = new ReddUIStatus();
		
		private readonly ReddUIDate uiDate;

		private readonly ReddUtilDropHandler utilDrop = new ReddUtilDropHandler();
		private ReddDataEntry dataEntry;

		public ReddUIEntryEdit(ReddDataOptions options) {
			Spacing = new Size(2, 2);

			uiDate = new ReddUIDate(options);
			
			BeginVertical();
			
			etoButtons.BeginHorizontal();
			etoSave.Text = ReddLanguage.BUTTON_SAVE;
			etoButtons.Add(etoSave);
			etoButtons.AddSpace();
			etoConfirm.Visible = false;
			etoConfirm.VerticalAlignment = VerticalAlignment.Center;
			etoConfirm.Text = ReddLanguage.TEXT_CONFIRM_DELETE;
			etoButtons.Add(etoConfirm);
			etoDelete.Text = ReddLanguage.BUTTON_REMOVE;
			etoButtons.Add(etoDelete);
			etoButtons.EndHorizontal();
			
			Add(etoButtons);
			Add(uiTitle);
			Add(uiSource);
			Add(uiDate);
			AddCentered( uiImage, new Padding(2), new Size(2,2), true, true );
			Add(uiStatus);
			
			EndVertical();

			AllowDrop = true;
			Visible = false;
		}
		
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			MouseUp += OnMousePress;
			OnAlign();
		}

		protected override void OnUnLoad(EventArgs e) {
			MouseUp -= OnMousePress;
			base.OnUnLoad(e);
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
			
			uiStatus.Date = entry.Date;
			OnValidate();
			
			etoSave.Click += OnButtonSave;
			etoDelete.Click += OnButtonDelete;
			
			uiTitle.OnTitleChanged += OnModify;
			uiDate.EventChanged += OnModify;
			uiImage.OnImageChanged += OnModify;
			uiSource.OnSourceChanged += OnModify;
			OnModify();

			utilDrop.OnDropFile += OnDropFile;
			utilDrop.OnDropURL += OnDropURL;
			utilDrop.OnDropImage += OnDropImage;
			
			Visible = true;
		}

		public void Unset() {
			uiStatus.HasChanges = false;
			
			uiTitle.OnTitleChanged -= OnModify;
			uiDate.EventChanged -= OnModify;
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
			uiImage.Set(dataEntry.Bitmap);
			uiStatus.HasChanges = false;
			return dataEntry;
		}

		public void Refresh() => OnValidate();

		// ===============================================
		// EVENTS
		public delegate void UIEntryEvent();
		public event UIEntryEvent EventSave;
		public event UIEntryEvent EventDelete;

		// ===============================================
		// CALLBACKS
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			OnAlign();
			OnClearConfirm();
		}

		protected override void OnDragEnter(DragEventArgs e) {
			base.OnDragEnter(e);
			utilDrop.HandleStart(e);
		}

		protected override void OnDragDrop(DragEventArgs e) {
			base.OnDragDrop(e);
			utilDrop.HandleDrop(e);
			OnClearConfirm();
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
			int height = Height - (uiTitle.Height + uiSource.Height + uiDate.Height + uiStatus.Height + etoButtons.Height + (Spacing.GetValueOrDefault().Height * 2));
			int size = Width < height ? Width : height;
			uiImage.Size = new Size(size, size);
		}

		protected void OnValidate() {
			uiStatus.Date = uiDate.Date;
			uiTitle.Refresh();
			uiStatus.HasValidTitle = uiTitle.IsValid;
			uiSource.Refresh();
			uiStatus.HasValidSource = uiSource.IsValid;
			uiDate.Refresh();
			uiStatus.HasValidDate = uiDate.IsValid;
			uiImage.Refresh();
			uiStatus.HasValidImage = uiImage.IsValid;
		}
		
		private void OnModify() {
			uiStatus.HasChanges = uiTitle.Text != dataEntry.Title ||
								  uiSource.Text != dataEntry.Source ||
								  uiDate.Date != dataEntry.Date ||
								  uiImage.HasChanged;
			etoSave.Visible = uiStatus.HasChanges;
			OnValidate();
		}
		
		private void OnButtonSave(object sender, EventArgs e) {
			EventSave?.Invoke();
		}

		private void OnButtonDelete(object sender, EventArgs e) {
			if( etoConfirm.Visible ) {
				OnClearConfirm();
				EventDelete?.Invoke();
			}
			else {
				etoConfirm.Visible = true;
				etoDelete.TextColor = ReddConfig.UI_TEXT_IMPORTANT;
				etoDelete.LostFocus += OnDeleteUnfocus;
			}
		}

		private void OnDeleteUnfocus(object sender, EventArgs e) {
			OnClearConfirm();
		}

		private void OnMousePress(object sender, MouseEventArgs e) {
			OnClearConfirm();
		}

		private void OnClearConfirm() {
			etoConfirm.Visible = false;
			etoDelete.TextColor = SystemColors.ControlText;
			etoDelete.LostFocus -= OnDeleteUnfocus;
		}
	}
}