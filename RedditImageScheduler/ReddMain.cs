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
		private readonly ReddUIContent uiContent;
		
		public ReddMain(ReddIODatabase database) {
			ioDatabase = database;
			
			uiContent = new ReddUIContent(ioDatabase.Entries);
			
			Title = ReddLanguage.NAME;
			MinimumSize = new Size(ReddConfig.WIDTH, ReddConfig.HEIGHT);

			Content = uiContent;
			Menu = uiMenu;
		}
		
		protected override void OnShown(EventArgs e) {
			base.OnShown(e);
			INSTANCE = this;
		}

		protected override void OnClosing(CancelEventArgs e) {
			INSTANCE = null;
			base.OnClosing(e);
		}

		
	}
}