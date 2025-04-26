using System;
using System.ComponentModel;
using Eto.Forms;
using RedditImageScheduler.IO;

namespace RedditImageScheduler {
	public class ReddApplication : Application {
		private readonly ReddIODatabase ioDatabase;
		private readonly RedditTray reddTray;
			
		private ReddMain reddMain;
		private bool isRunning;

		public ReddApplication(string platform)
			: base(platform) {
			ioDatabase = new ReddIODatabase(ReddConfig.FILE);
			reddTray = new RedditTray();
		}

		~ReddApplication() {
			isRunning = false;
		}

		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
			
			ioDatabase.OnErrorInitialize += OnDatabaseErrorInitialize;
			ioDatabase.OnErrorChange += OnDatabaseErrorChange;
			ioDatabase.Open();
			
			reddTray.OnOpen += OnOpen;
			reddTray.Initialize();
			
			isRunning = true;
#if DEBUG
			OnOpen();
#endif
		}

		protected override void OnTerminating(CancelEventArgs e) {
#if DEBUG
			if( !isRunning ) return;
			isRunning = false;
#else
			if( isRunning ) return;
#endif
			
			ioDatabase.OnErrorInitialize -= OnDatabaseErrorInitialize;
			ioDatabase.OnErrorChange -= OnDatabaseErrorChange;
			ioDatabase.Close();
			
			reddTray.OnOpen -= OnOpen;
			reddTray.Dispose();
			
			base.OnTerminating(e);

			Quit();
		}

		private void OnOpen() {
			if( reddMain != null ) return;
			if( !ioDatabase.IsOpen ) {
				ioDatabase.Open();
				if(!ioDatabase.IsOpen) return;
			}

			reddMain = new ReddMain(ioDatabase);
			reddMain.Show();
			reddMain.Closed += OnClose;
		}

		private void OnClose(object sender, EventArgs e) {
			reddMain.Closed -= OnClose;
			if( !reddMain.IsDisposed ) reddMain.Dispose();
			reddMain = null;
		}
		
		private void OnDatabaseErrorInitialize() {
			MessageBox.Show("Failed to access '" + ioDatabase.FilePath + ".' Make sure the current folder can be read from and written to.", MessageBoxType.Error); 
		}
		
		private void OnDatabaseErrorChange() {
			MessageBox.Show("Failed to modify the database. Is '"+ioDatabase.FilePath+"' still accessible?", MessageBoxType.Error); 
		}
	}
}