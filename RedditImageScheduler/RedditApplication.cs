using System;
using System.ComponentModel;
using Eto.Forms;

namespace RedditImageScheduler {
	public class RedditApplication : Application {
		private readonly RedditTray redditTray;
		private RedditMain redditMain;

		private bool isRunning;
		
		public RedditApplication(string platform)
			: base(platform) {
			redditTray = new RedditTray();
			isRunning = true;
		}

		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
			redditTray.OnOpen += OnOpen;
			redditTray.Initialize();
		}

		protected override void OnTerminating(CancelEventArgs e) {
			if( isRunning ) return;
			
			redditTray.OnOpen -= OnOpen;
			redditTray.Dispose();
			base.OnTerminating(e);
		}

		private void OnOpen() {
			if( redditMain != null ) return;

			redditMain = new RedditMain();
			redditMain.Show();
			redditMain.Closed += OnClose;
		}

		private void OnClose(object sender, EventArgs e) {
			redditMain.Closed -= OnClose;
			if( !redditMain.IsDisposed ) redditMain.Dispose();
			redditMain = null;
		}
	}
}