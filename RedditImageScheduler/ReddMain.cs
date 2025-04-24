using System;
using System.ComponentModel;
using Eto.Drawing;
using Eto.Forms;
using RedditImageScheduler.IO;
using RedditImageScheduler.UI;

namespace RedditImageScheduler {
	public partial class ReddMain : Form {
		private static ReddMain INSTANCE;
		public static ReddMain Instance => INSTANCE;

		private readonly ReddIODatabase ioDatabase;
		private readonly ReddUIMenu uiMenu = new ReddUIMenu();
		private readonly ReddUIContent uiContent = new ReddUIContent();
		
		public ReddMain(ReddIODatabase database) {
			ioDatabase = database;
			
			Title = "Reddit Image Scheduler";
			MinimumSize = new Size(ReddConfig.WIDTH, ReddConfig.HEIGHT);

			Content = uiContent.UI;
			Menu = uiMenu.UI;
		}
		
		protected override void OnShown(EventArgs e) {
			base.OnShown(e);
			INSTANCE = this;
			uiMenu.Initialize();
			uiContent.Initialize(ioDatabase.Entries);
		}

		protected override void OnClosing(CancelEventArgs e) {
			INSTANCE = null;
			uiMenu.Deinitialize();
			uiContent.Deinitialize();
			base.OnClosing(e);
		}

		
	}
}