using System;
using Eto.Drawing;
using Eto.Forms;
using RedditImageScheduler.Data;
using RedditImageScheduler.UI.Timetable;

namespace RedditImageScheduler.UI {
	public class ReddUITimetable : DynamicLayout {
		private readonly ReddUITimeGrid uiTimeGrid = new ReddUITimeGrid();
		private readonly ReddUIEntryPreview uiPreview;

		private readonly DynamicLayout etoContainer = new DynamicLayout();
		private readonly Button etoEdit = new Button();
		private readonly Label etoNextLabel = new Label();
		private readonly TextBox etoNext = new TextBox();

		private readonly DataStoreCollection<ReddDataEntry> collectionEntries = new DataStoreCollection<ReddDataEntry>();
		private readonly ReddScheduler reddScheduler;
		private readonly ReddDataEntries dataEntries;
		private ReddDataEntry dataSelected;
		
		public ReddUITimetable(ReddScheduler scheduler) {
			reddScheduler = scheduler;
			dataEntries = reddScheduler.Entries;

			uiPreview = new ReddUIEntryPreview(reddScheduler);
			
			Padding = new Padding(2);
			Spacing = new Size(2, 2);
			
			BeginHorizontal(true);
			
			etoContainer.BeginHorizontal();
			etoContainer.BeginVertical();
			
			DynamicLayout header = new DynamicLayout();
			header.BeginHorizontal();
			etoEdit.Text = ReddLanguage.BUTTON_EDIT;
			header.Add(etoEdit);
			header.AddSpace();
			header.EndHorizontal();
			etoContainer.Add(header);
			
			uiTimeGrid.Size = new Size(300, ReddConfig.HEIGHT);
			uiTimeGrid.RowHeight = 20;
			uiTimeGrid.CanDeleteItem = o => false;
			uiTimeGrid.DataStore = collectionEntries;
			etoContainer.Add(uiTimeGrid);
			
			etoContainer.EndVertical();
			
			etoContainer.BeginVertical();
			etoContainer.Add(uiPreview);
			etoContainer.EndVertical();
			etoContainer.EndHorizontal();
			
			Add(etoContainer);
			EndHorizontal();
			
			BeginHorizontal();
			DynamicLayout next = new DynamicLayout();
			next.BeginHorizontal();
			etoNextLabel.VerticalAlignment = VerticalAlignment.Center;
			etoNextLabel.Text = ReddLanguage.LABEL_NEXT;
			next.Add(etoNextLabel);
			etoNext.ReadOnly = true;
			next.Add(etoNext);
			next.EndHorizontal();
			Add(next);
			EndHorizontal();
		}

		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			reddScheduler.OnUpdate += OnTick;
			dataEntries.OnUpdate += OnUpdate;
			etoEdit.Click += OnClickEdit;
			uiTimeGrid.SelectedItemsChanged += OnSelection;
			etoNext.MouseUp += OnNext;
			OnUpdate();
		}

		protected override void OnUnLoad(EventArgs e) {
			reddScheduler.OnUpdate -= OnTick;
			dataEntries.OnUpdate -= OnUpdate;
			etoEdit.Click -= OnClickEdit;
			uiTimeGrid.SelectedItemsChanged -= OnSelection;
			etoNext.MouseUp -= OnNext;
			base.OnUnLoad(e);
		}

		protected override void OnShown(EventArgs e) {
			base.OnShown(e);
			OnUpdate();
			OnTick();
			OnResize();
		}

		// ===============================================
		// EVENTS
		public delegate void UITimetableEvent();
		public event UITimetableEvent EventEdit;
		
		// ===============================================
		// CALLBACKS
		protected override void OnSizeChanged(EventArgs e) {
			OnResize();
			base.OnSizeChanged(e);
		}

		private void OnResize() {
			int sum = etoEdit.Height + etoNext.Height + (Spacing.GetValueOrDefault().Height * 2);
			Size size = new Size(300, etoContainer.Height - sum);
			uiTimeGrid.Size = size;
		}

		private void OnClickEdit(object sender, EventArgs e) {
			EventEdit?.Invoke();
		}
		
		private void OnSelection(object sender, EventArgs e) {
			dataSelected = uiTimeGrid.SelectedItem as ReddDataEntry;
			uiPreview.Set(dataSelected);
		}
		
		private void OnTick() {
			uiPreview.Refresh();
			etoNext.Text = reddScheduler.GetNextState();
		}

		private void OnNext(object sender, MouseEventArgs e) {
			if( reddScheduler.NextTarget == null ) return;
			
			uiTimeGrid.CurrentNext = reddScheduler.NextTarget;
			int idx = collectionEntries.IndexOf(reddScheduler.NextTarget);
			if( idx != -1 ) {
				uiTimeGrid.SelectRow(idx);
			}
		}

		private void OnUpdate() {
			collectionEntries.Clear();
			foreach( var entry in dataEntries ) {
				collectionEntries.Add(entry);
			}

			collectionEntries.Sort(OnSort);
			
			uiTimeGrid.CurrentNext = reddScheduler.NextTarget;
			uiTimeGrid.Visible = dataEntries.Count > 0;
			uiTimeGrid.Invalidate(true);
		}

		private int OnSort(ReddDataEntry x, ReddDataEntry y) {
			return x.Timestamp.CompareTo(y.Timestamp);
		}
	}
}