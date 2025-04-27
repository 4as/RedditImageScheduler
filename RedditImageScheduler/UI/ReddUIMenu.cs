using System;
using System.Diagnostics;
using Eto.Forms;

namespace RedditImageScheduler.UI {
	public class ReddUIMenu : MenuBar{
		private readonly Command commandHome;
		private readonly Command commandAbout;
		private readonly Command commandQuit;
		
		public ReddUIMenu() {
			commandHome = new Command { MenuText = "&Home", Shortcut = Application.Instance.CommonModifier | Keys.H };
			commandAbout = new Command { MenuText = "&About", Shortcut = Application.Instance.CommonModifier | Keys.A };
			commandQuit = new Command { MenuText = "&Quit", Shortcut = Application.Instance.CommonModifier | Keys.Q };

			QuitItem = commandQuit;
			HelpItems.Add(commandHome);
			HelpItems.Add(commandAbout);
		}

		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			
			commandHome.Executed += OnHomePage;
			commandQuit.Executed += OnQuit;
			commandAbout.Executed += OnAbout;
		}

		protected override void OnUnLoad(EventArgs e) {
			commandHome.Executed -= OnHomePage;
			commandQuit.Executed -= OnQuit;
			commandAbout.Executed -= OnAbout;
			base.OnUnLoad(e);
		}
		
		// ===============================================
		// CALLBACKS
		protected void OnHomePage(object sender, EventArgs e) {
			Process.Start(new ProcessStartInfo("https://github.com/4as/RedditImageScheduler") { UseShellExecute = true });
		}
		
		protected void OnAbout(object sender, EventArgs e) {
			//TODO: replace with custom dedicated About dialog
			new AboutDialog().ShowDialog(ReddMain.Instance);
		}
		
		protected void OnQuit(object sender, EventArgs e) {
			Application.Instance.Quit();
		}
	}
}