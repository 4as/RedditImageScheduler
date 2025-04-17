using System;
using System.Diagnostics;
using Eto.Forms;

namespace RedditImageScheduler.UI {
	public class RedditUIMenu {

		private readonly MenuBar menuBar;
		private readonly Command commandHome;
		private readonly Command commandAbout;
		private readonly Command commandQuit;
		
		public RedditUIMenu() {
			commandHome = new Command { MenuText = "&Home", Shortcut = Application.Instance.CommonModifier | Keys.H };
			commandAbout = new Command { MenuText = "&About", Shortcut = Application.Instance.CommonModifier | Keys.A };
			commandQuit = new Command { MenuText = "&Quit", Shortcut = Application.Instance.CommonModifier | Keys.Q };

			menuBar = new MenuBar() {
				QuitItem = commandQuit,
				HelpItems = { commandHome, commandAbout }
			};
		}

		// ===============================================
		// GETTERS / SETTERS
		public MenuBar UI => menuBar;

		// ===============================================
		// PUBLIC METHODS
		public void Initialize() {
			Deinitialize();
			commandHome.Executed += OnHomePage;
			commandQuit.Executed += OnQuit;
			commandAbout.Executed += OnAbout;
		}

		public void Deinitialize() {
			commandHome.Executed -= OnHomePage;
			commandQuit.Executed -= OnQuit;
			commandAbout.Executed -= OnAbout;
		}

		public void Dispose() {
			Deinitialize();
			menuBar.Dispose();
		}

		// ===============================================
		// EVENTS
		
		// ===============================================
		// CALLBACKS
		protected void OnHomePage(object sender, EventArgs e) {
			Process.Start(new ProcessStartInfo("https://github.com/4as/RedditImageScheduler") { UseShellExecute = true });
		}
		
		protected void OnAbout(object sender, EventArgs e) {
			//TODO: replace with custom dedicated About dialog
			new AboutDialog().ShowDialog(RedditMain.Instance);
		}
		
		protected void OnQuit(object sender, EventArgs e) {
			Application.Instance.Quit();
		}
	}
}