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
		private readonly ReddUITimetable uiTimetable;
		
		public ReddMain(ReddIODatabase database) {
			ioDatabase = database;
			
			uiContent = new ReddUIContent(ioDatabase.Entries);
			uiTimetable = new ReddUITimetable(ioDatabase.Entries);
			
			Title = ReddLanguage.APP_NAME;
			MinimumSize = new Size(ReddConfig.WIDTH, ReddConfig.HEIGHT);
			uiContent.Size = MinimumSize;
			uiTimetable.Size = MinimumSize;

			Content = uiTimetable;
			Menu = uiMenu;
		}

		public bool EditMode {
			get => Content == uiContent;
			set {
				if( value ) Content = uiContent;
				else Content = uiTimetable;
			}
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