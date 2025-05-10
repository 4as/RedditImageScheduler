using System;
using System.ComponentModel;
using Eto.Forms;
using RedditImageScheduler.IO;
using RedditImageScheduler.Scheduler;

namespace RedditImageScheduler {
	public class ReddApplication : Application {
		private readonly ReddIOOptions ioOptions;
		private readonly ReddIODatabase ioDatabase;
		private readonly ReddScheduler reddScheduler;
		private readonly RedditTray reddTray;
			
		private ReddMain reddMain;
		private bool isRunning;

		public ReddApplication(string platform)
			: base(platform) {
			ioOptions = new ReddIOOptions(ReddConfig.FILE_SETTINGS);
			ioDatabase = new ReddIODatabase();
			reddScheduler = new ReddScheduler(ioOptions.Data);
			reddTray = new RedditTray();
		}

		~ReddApplication() {
			isRunning = false;
		}

		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);

			ioOptions.EventError += OnOptionsError;
			ioOptions.Load();
			
			ioDatabase.EventErrorInitialize += OnDatabaseErrorInitialize;
			ioDatabase.EventErrorChange += OnDatabaseErrorChange;
			ioDatabase.Open(ioOptions.DatabasePath);
			
			reddTray.EventOpen += OnOpen;
			reddTray.EventQuit += OnQuit;
			reddTray.Initialize();

			reddScheduler.Initialize(ioDatabase.Entries);
			
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
			
			ioOptions.EventError -= OnOptionsError;
			
			ioDatabase.EventErrorInitialize -= OnDatabaseErrorInitialize;
			ioDatabase.EventErrorChange -= OnDatabaseErrorChange;
			ioDatabase.Close();
			
			reddTray.EventOpen -= OnOpen;
			reddTray.EventQuit -= OnQuit;
			reddTray.Dispose();

			reddScheduler.Deinitialize();
			
			base.OnTerminating(e);

			Quit();
		}

		private void OnOpen() {
			if( reddMain != null ) {
				return;
			}
			
			if( !ioDatabase.IsOpen ) {
				ioDatabase.Open(ioOptions.DatabasePath);
				if(!ioDatabase.IsOpen) return;
			}

			reddMain = new ReddMain(reddScheduler, ioOptions.Data);
			reddMain.Show();
			reddMain.Closed += OnClose;
		}

		private void OnClose(object sender, EventArgs e) {
			reddMain.Closed -= OnClose;
			if( !reddMain.IsDisposed ) reddMain.Dispose();
			reddMain = null;
		}

		private void OnQuit() {
			if( reddMain != null && reddMain.HasChanges && !reddMain.ShowSaveWarning() ) return;
			Application.Instance.Quit();
		}
		
		private void OnOptionsError() {
			MessageBox.Show(string.Format(ReddLanguage.ERROR_SETTINGS_LOAD_FAILED, ioOptions.FilePath), MessageBoxType.Error); 
		}
		
		private void OnDatabaseErrorInitialize() {
			MessageBox.Show(string.Format(ReddLanguage.ERROR_DATABASE_INITIALIZATION_FAILED, ioDatabase.FilePath), MessageBoxType.Error); 
		}
		
		private void OnDatabaseErrorChange() {
			MessageBox.Show(string.Format(ReddLanguage.ERROR_DATABASE_MODIFY_FAILED, ioDatabase.FilePath), MessageBoxType.Error); 
		}
	}
}