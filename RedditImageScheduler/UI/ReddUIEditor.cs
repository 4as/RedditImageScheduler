using System;
using System.Collections.Generic;
using Eto.Drawing;
using Eto.Forms;
using RedditImageScheduler.Data;
using RedditImageScheduler.UI.Editor;

namespace RedditImageScheduler.UI {
	public class ReddUIEditor : DynamicLayout {
		private readonly Button etoTimetable = new Button();
		private readonly Button etoAdd = new Button();
		private readonly ListBox etoList = new ListBox();
		
		private readonly ReddUIEntryEdit uiEntry;

		private readonly List<ReddDataEntry> listItems = new List<ReddDataEntry>();
		private readonly ReddScheduler reddScheduler;
		private readonly ReddDataOptions dataOptions;
		private readonly ReddDataEntries dataEntries;
		
		public ReddUIEditor(ReddScheduler scheduler, ReddDataOptions options) {
			reddScheduler = scheduler;
			dataOptions = options;
			dataEntries = reddScheduler.Entries;

			uiEntry = new ReddUIEntryEdit(options);
			
			etoList.Size = new Size(ReddConfig.WIDTH/2, ReddConfig.HEIGHT);
			etoList.DataStore = listItems;
			
			Padding = new Padding(2);
			Spacing = new Size(2, 2);
			BeginHorizontal();
			BeginVertical();
			DynamicLayout header = new DynamicLayout();
			header.BeginHorizontal();
			etoTimetable.Text = ReddLanguage.BUTTON_TIMETABLE;
			header.Add(etoTimetable);
			header.AddSpace();
			etoAdd.Text = ReddLanguage.BUTTON_ADD;
			header.Add(etoAdd);
			header.EndHorizontal();
			Add(header);
			etoList.Width = 300;
			Add(etoList);
			EndVertical();
			Add(uiEntry);
			EndHorizontal();
		}
		
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);

			uiEntry.EventSave += OnSave;
			uiEntry.EventDelete += OnRemove;
			etoAdd.Click += OnAdd;
			etoTimetable.Click += OnClickTimetable;
			etoList.SelectedIndexChanged += OnSelect;
			dataEntries.OnUpdate += OnUpdate;
			reddScheduler.EventUpdate += OnTick;
			OnUpdate();
		}

		protected override void OnUnLoad(EventArgs e) {
			etoAdd.Click -= OnAdd;
			uiEntry.EventSave -= OnSave;
			uiEntry.EventDelete -= OnRemove;
			etoList.SelectedIndexChanged -= OnSelect;
			dataEntries.OnUpdate -= OnUpdate;
			reddScheduler.EventUpdate -= OnTick;
			base.OnUnLoad(e);
		}

		// ===============================================
		// GETTERS / SETTERS
		public bool HasChanges => uiEntry.HasChanges;

		// ===============================================
		// PUBLIC METHODS
		public void Save() {
			ReddDataEntry entry = uiEntry.Commit();
			dataEntries.Update(entry);
		}

		public void Unset() {
			uiEntry.Unset();
		}

		// ===============================================
		// EVENTS
		public delegate void UIContentEvent();
		public event UIContentEvent EventTimetable;
		
		// ===============================================
		// CALLBACKS
		private void OnTick() {
			uiEntry.Refresh();
		}
		
		private void OnUpdate() {
			listItems.Clear();
			foreach( var entry in dataEntries ) {
#if DEBUG
				listItems.Add(entry);
#else
				if( !entry.IsPosted ) listItems.Add(entry);
#endif
			}
			
			etoList.DataStore = listItems;
			if( uiEntry.Data != null ) {
				int idx = listItems.IndexOf(uiEntry.Data);
				if( idx == -1 ) uiEntry.Unset();
				if( etoList.SelectedIndex != idx ) {
					etoList.SelectedIndex = idx;
				}
			}
		}
		
		private void OnSelect(object sender, EventArgs e) {
			int idx = etoList.SelectedIndex;
			if( idx < 0 || idx >= listItems.Count ) {
				uiEntry.Unset();
				return;
			}

			ReddDataEntry saved = null;
			int current;
			if( uiEntry.HasChanges && (current = listItems.IndexOf(uiEntry.Data)) != idx ) {
				DialogResult result = MessageBox.Show(ReddLanguage.MESSAGE_UNSAVED_CHANGES, MessageBoxButtons.YesNoCancel, MessageBoxType.Question, MessageBoxDefaultButton.Yes);
				switch( result ) {
					case DialogResult.Yes:
						// saving is done like this to avoid recursive updates (OnSave()->OnSelect())
						saved = uiEntry.Commit();
						break;
					case DialogResult.No:
						break;
					case DialogResult.Cancel:
					default:
						etoList.SelectedIndex = current;
						return;
				}
			}
			
			ReddDataEntry entry = listItems[idx];
			uiEntry.Set(entry);

			if( saved != null ) {
				dataEntries.Update(saved);
			}
		}
		
		private void OnAdd(object sender, EventArgs eventArgs) {
			dataEntries.Create(dataOptions.EntrySpacingHours);
		}
		
		private void OnClickTimetable(object sender, EventArgs e) {
			if( uiEntry.HasChanges ) {
				DialogResult result = MessageBox.Show(ReddLanguage.MESSAGE_UNSAVED_CHANGES, MessageBoxButtons.YesNoCancel, MessageBoxType.Question, MessageBoxDefaultButton.Yes);
				switch( result ) {
					case DialogResult.Yes:
						ReddDataEntry saved = uiEntry.Commit();
						dataEntries.Update(saved);
						break;
					case DialogResult.No:
						break;
					case DialogResult.Cancel:
					default:
						return;
				}
			}
			
			EventTimetable?.Invoke();
		}
		
		private void OnSave() {
			ReddDataEntry entry = uiEntry.Commit();
			dataEntries.Update(entry);
		}

		private void OnRemove() {
			dataEntries.Remove(uiEntry.Data);
		}
	}
}