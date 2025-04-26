using Eto.Drawing;
using Eto.Forms;
using RedditImageScheduler.Data;

namespace RedditImageScheduler.UI {
	public class ReddUIContentEntry {
		private readonly StackLayout etoLayout = new StackLayout();
		private readonly TextBox etoTitle = new TextBox();
		private readonly TextBox etoSource = new TextBox();
		private readonly ImageView etoImage = new ImageView();
		
		private readonly Bitmap bmpPlaceholder = new Bitmap(128,128,PixelFormat.Format24bppRgb);
		private ReddDataEntry dataEntry;

		public ReddUIContentEntry() {
			etoLayout.Width = -1;
			etoTitle.Width = -1;
			etoLayout.Items.Add(etoTitle);
			etoSource.Width = -1;
			etoLayout.Items.Add(etoSource);
			etoLayout.Items.Add(etoImage);
			etoLayout.Visible = false;
		}
		
		public StackLayout UI => etoLayout;
		public ReddDataEntry Data => dataEntry;
		
		public void Set(ReddDataEntry entry) {
			Unset();

			if( entry == null ) return;
			
			dataEntry = entry;
			
			etoLayout.Visible = true;
			etoTitle.Text = entry.Title;
			etoSource.Text = entry.Source;
			etoImage.Image = dataEntry.Image == null ? bmpPlaceholder : new Bitmap(entry.Image);
		}

		public void Unset() {
			etoLayout.Visible = false;
			dataEntry = null;
		}
	}
}