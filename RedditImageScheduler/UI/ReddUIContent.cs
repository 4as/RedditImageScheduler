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
			//TODO: if entry has pending changes show the "save changes?" dialog
			
			int idx = etoList.SelectedIndex;
			if( idx < 0 || idx >= dataEntries.Count ) {
				uiEntry.Unset();
				return;
			}
			
			ReddDataEntry entry = dataEntries[idx];
			uiEntry.Set(entry);
		}
		
		private void OnAdd(object sender, EventArgs eventArgs) {
			dataEntries.Create();
		}
		
		private void OnSave() {
			dataEntries.Update(uiEntry.Data);
		}

		private void OnRemove() {
			dataEntries.Remove(uiEntry.Data);
		}
	}
}