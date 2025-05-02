using System;
using Eto.Forms;
using RedditImageScheduler.Data;
using RedditImageScheduler.UI.Timetable;

namespace RedditImageScheduler.UI {
	public class ReddUITimetable : DynamicLayout {
		private readonly ReddUISchedule uiSchedule = new ReddUISchedule();

		private readonly ReddDataEntries dataEntries;
		
		public ReddUITimetable(ReddDataEntries entries) {
			dataEntries = entries;
			
			BeginVertical();
			uiSchedule.RowHeight = 20;
			uiSchedule.CanDeleteItem = o => false;
			Add(uiSchedule);
			EndVertical();
		}

		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			dataEntries.OnUpdate += OnUpdate;
			OnUpdate();
		}

		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			uiSchedule.Size = Size;
			OnUpdate();
		}

		protected override void OnUnLoad(EventArgs e) {
			dataEntries.OnUpdate -= OnUpdate;
			base.OnUnLoad(e);
		}

		private void OnUpdate() {
			uiSchedule.DataStore = dataEntries;
			if( dataEntries.Count > 0 ) {
				uiSchedule.Visible = true;
				uiSchedule.SelectRow(dataEntries.Count - 1);
				var selected = uiSchedule.SelectedItem;
			}
			else {
				uiSchedule.Visible = false;
			}
		}
	}
}