using System;
using System.ComponentModel;
using Eto.Forms;
using RedditImageScheduler.IO;

namespace RedditImageScheduler {
	public class ReddApplication : Application {
		private readonly ReddIOWeb ioWeb;
		private readonly ReddIOOptions ioOptions;
		private readonly ReddIODatabase ioDatabase;
		private readonly ReddLogin reddLogin;
		private readonly ReddScheduler reddScheduler;
		private readonly ReddTray reddTray;
		
		private ReddMain reddMain;
		private bool isRunning;

		public ReddApplication(string platform)
			: base(platform) {
			ioWeb = new ReddIOWeb();
			ioOptions = new ReddIOOptions(ReddConfig.FILE_SETTINGS);
			ioDatabase = new ReddIODatabase();
			reddLogin = new ReddLogin(ioWeb);
			reddScheduler = new ReddScheduler(ioOptions.Data);
			reddTray = new ReddTray();
		}

		~ReddApplication() {
			isRunning = false;
		}

		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
			
			ioOptions.EventError += OnOptionsError;
			ioOptions.Load();

			reddLogin.EventLogin += OnLogin;
			reddLogin.Initialize(ioOptions.Data);
			
			isRunning = true;
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

		private void OnLogin() {
			reddLogin.EventLogin -= OnLogin;
			reddLogin.Deinitialize();
			
			ioDatabase.EventErrorInitialize += OnDatabaseErrorInitialize;
			ioDatabase.EventErrorChange += OnDatabaseErrorChange;
			ioDatabase.Open(ioOptions.DatabasePath);
			
			reddTray.EventOpen += OnOpen;
			reddTray.EventQuit += OnQuit;
			reddTray.Initialize();

			reddScheduler.Initialize(ioDatabase.Entries);
		}

		private void OnOpen() {
			if( reddMain != null ) {
				return;
			}
			
			if( !ioDatabase.IsOpen ) {
				ioDatabase.Open(ioOptions.DatabasePath);
				if(!ioDatabase.IsOpen) return;
			}

			reddMain = new ReddMain(ioDatabase, reddScheduler, ioOptions.Data);
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