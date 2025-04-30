using System;
using Eto.Drawing;
using Eto.Forms;
using RedditImageScheduler.Data;

namespace RedditImageScheduler.UI {
	public class ReddUIContent : DynamicLayout {
		private readonly ReddUIContentEntry uiEntry = new ReddUIContentEntry();
		private readonly Button etoAdd = new Button();
		private readonly ListBox etoList = new ListBox();
		
		private readonly ReddDataEntries dataEntries;
		
		public ReddUIContent(ReddDataEntries entries) {
			dataEntries = entries;
			
			etoList.Size = new Size(ReddConfig.WIDTH/2, ReddConfig.HEIGHT);
			
			Padding = new Padding(2);
			Spacing = new Size(2, 2);
			BeginHorizontal();
			BeginVertical();
			etoAdd.Text = ReddLanguage.ADD;
			Add(etoAdd);
			etoList.Width = 300;
			Add(etoList);
			EndVertical();
			Add(uiEntry);
			EndHorizontal();
		}
		
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);

			uiEntry.OnSave += OnSave;
			uiEntry.OnDelete += OnRemove;
			etoAdd.Click += OnAdd;
			etoList.SelectedIndexChanged += OnSelect;
			dataEntries.OnUpdate += OnUpdate;
			OnUpdate();
		}

		protected override void OnUnLoad(EventArgs e) {
			etoAdd.Click -= OnAdd;
			uiEntry.OnSave -= OnSave;
			uiEntry.OnDelete -= OnRemove;
			etoList.SelectedIndexChanged -= OnSelect;
			dataEntries.OnUpdate -= OnUpdate;
			base.OnUnLoad(e);
		}

		private void OnUpdate() {
			etoList.DataStore = dataEntries;
			if( uiEntry.Data != null ) {
				int idx = dataEntries.IndexOf(uiEntry.Data);
				if( idx == -1 ) uiEntry.Unset();
				if( etoList.SelectedIndex != idx ) {
					etoList.SelectedIndex = idx;
				}
			}
		}
		
		private void OnSelect(object sender, EventArgs e) {
			int idx = etoList.SelectedIndex;
			if( idx < 0 || idx >= dataEntries.Count ) {
				uiEntry.Unset();
				return;
			}

			ReddDataEntry saved = null;
			int current;
			if( uiEntry.HasChanges && (current = dataEntries.IndexOf(uiEntry.Data)) != idx ) {
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
			
			ReddDataEntry entry = dataEntries[idx];
			uiEntry.Set(entry);

			if( saved != null ) {
				dataEntries.Update(saved);
			}
		}
		
		private void OnAdd(object sender, EventArgs eventArgs) {
			dataEntries.Create();
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