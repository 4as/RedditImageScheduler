using Eto.Drawing;
using Eto.Forms;
using RedditImageScheduler.Data;

namespace RedditImageScheduler.UI {
	public class ReddUIContent {
		private readonly TableLayout etoLayout;
		private readonly ListBox etoList;
		
		private ReddDataEntries dataEntries;
		
		public ReddUIContent() {
			etoList = new ListBox();
			etoList.Size = new Size(ReddConfig.WIDTH/2, ReddConfig.HEIGHT);
			
			etoLayout = new TableLayout();
			etoLayout.Spacing = new Size(5, 5);
			etoLayout.Padding = new Padding(4);
			etoLayout.Size = new Size(ReddConfig.WIDTH, ReddConfig.HEIGHT);
			etoLayout.Rows.Add(new TableRow(etoList));
		}

		public TableLayout UI => etoLayout;

		public void Initialize(ReddDataEntries entries) {
			etoList.Items.Clear();

			dataEntries = entries;
			for( int i = 0, iLen = dataEntries.Count; i < iLen; i++ ) {
				ReddDataEntry entry = dataEntries[i];
				etoList.Items.Add(entry.Title);
			}
		}

		public void Deinitialize() {
			etoList.Items.Clear();
		}
	}
}