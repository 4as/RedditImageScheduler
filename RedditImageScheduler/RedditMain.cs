using System;
using System.ComponentModel;
using Eto.Drawing;
using Eto.Forms;
using Eto.IO;
using RedditImageScheduler.UI;

namespace RedditImageScheduler {
	public partial class RedditMain : Form {
		private static RedditMain INSTANCE;
		public static RedditMain Instance => INSTANCE;
		
		private readonly RedditUIMenu uiMenu = new RedditUIMenu();
		
		public RedditMain() {
			INSTANCE = this;
			
			Title = "Reddit Image Scheduler";
			MinimumSize = new Size(500, 300);

			Content = new StackLayout {
				Padding = 10,
				Items = {
					"Hello World!",
					// add more controls here
				}
			};
			
			Menu = uiMenu.UI;
		}
		
		protected override void OnShown(EventArgs e) {
			base.OnShown(e);
			uiMenu.Initialize();
		}

		protected override void OnClosing(CancelEventArgs e) {
			INSTANCE = null;
			uiMenu.Dispose();
			base.OnClosing(e);
		}
	}
}