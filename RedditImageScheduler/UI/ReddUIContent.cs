using System;
using System.Collections;
using System.Collections.Generic;
using Eto.Drawing;
using Eto.Forms;
using RedditImageScheduler.Data;

namespace RedditImageScheduler.UI {
	public class ReddUIContent {
		private readonly ReddUIContentEntry uiEntry = new ReddUIContentEntry();
		private readonly ReddUIContentEdit uiEdit = new ReddUIContentEdit();
		
		private readonly DynamicLayout etoLayout = new DynamicLayout();
		private readonly ListBox etoList = new ListBox();
		
		private readonly ReddDataEntries dataEntries;
		
		public ReddUIContent(ReddDataEntries entries) {
			dataEntries = entries;
			
			etoList.Size = new Size(ReddConfig.WIDTH/2, ReddConfig.HEIGHT);
			
			etoLayout.Padding = new Padding(2);
			etoLayout.Spacing = new Size(2, 2);
			etoLayout.BeginHorizontal();
			etoLayout.Add(etoList);
			etoLayout.Add(uiEntry.UI);
			etoLayout.EndHorizontal();
			etoLayout.BeginHorizontal();
			etoLayout.Add(uiEdit.UI);
			etoLayout.EndHorizontal();
		}

		public Container UI => etoLayout;

		public void Initialize() {
			Deinitialize();
			
			uiEdit.OnAdd += OnAdd;
			uiEdit.OnRemove += OnRemove;
			uiEdit.Initialize();

			etoList.SelectedIndexChanged += OnSelect;
			
			dataEntries.OnUpdate += OnUpdate;
			OnUpdate();
		}

		public void Deinitialize() {
			uiEdit.OnAdd -= OnAdd;
			uiEdit.OnRemove -= OnRemove;
			uiEdit.Deinitialize();
			
			dataEntries.OnUpdate -= OnUpdate;
		}
		
		private void OnUpdate() {
			etoList.DataStore = dataEntries;
			if( uiEntry.Data != null ) {
				int idx = dataEntries.IndexOf(uiEntry.Data);
				etoList.SelectedIndex = idx;
			}

			uiEdit.AllowRemove = uiEntry.Data != null;
		}
		
		private void OnSelect(object sender, EventArgs e) {
			int idx = etoList.SelectedIndex;
			if( idx < 0 || idx >= dataEntries.Count ) {
				uiEntry.Unset();
				return;
			}
			
			var entry = dataEntries[idx];
			uiEntry.Set(entry);
		}
		
		private void OnAdd() {
			dataEntries.Add("", "", 0, null);
		}

		private void OnRemove() {
			
		}
	}
}