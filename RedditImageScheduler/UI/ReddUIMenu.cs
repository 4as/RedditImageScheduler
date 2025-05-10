using System;
using System.Diagnostics;
using Eto.Forms;
using RedditImageScheduler.UI.Core;

namespace RedditImageScheduler.UI {
	public class ReddUIMenu : MenuBar {
		private readonly ReddUIMenuItem commandHome;
		private readonly ReddUIMenuItem commandAbout;
		private readonly ReddUIMenuItem commandQuit;
		private readonly ReddUIMenuItem commandOptions;
		
		public ReddUIMenu() {
			commandHome = new ReddUIMenuItem() { Text = ReddLanguage.MENU_HOME };
			commandAbout = new ReddUIMenuItem() { Text = ReddLanguage.MENU_ABOUT };
			commandQuit = new ReddUIMenuItem() { Text = ReddLanguage.MENU_QUIT };
			commandOptions = new ReddUIMenuItem() { Text = ReddLanguage.MENU_OPTIONS };

			IncludeSystemItems = MenuBarSystemItems.None;

			ApplicationItems.Add(commandOptions);
			QuitItem = commandQuit;
			HelpItems.Add(commandHome);
			HelpItems.Add(commandAbout);
		}

		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			
			commandHome.Click += OnHomePage;
			commandQuit.Click += OnQuit;
			commandAbout.Click += OnAbout;
			commandOptions.Click += OnOptions;
		}

		protected override void OnUnLoad(EventArgs e) {
			commandHome.Click -= OnHomePage;
			commandQuit.Click -= OnQuit;
			commandAbout.Click -= OnAbout;
			commandOptions.Click -= OnOptions;
			base.OnUnLoad(e);
		}

		// ===============================================
		// EVENTS
		public delegate void UIMenuEvent();
		public event UIMenuEvent EventQuit;

		// ===============================================
		// CALLBACKS
		protected void OnOptions(object sender, EventArgs e) {
			//TODO: open options dialog
		}
		
		protected void OnHomePage(object sender, EventArgs e) {
			Process.Start(new ProcessStartInfo(ReddConfig.HOMEPAGE) { UseShellExecute = true });
		}
		
		protected void OnAbout(object sender, EventArgs e) {
			//TODO: replace with custom dedicated About dialog
			new AboutDialog().ShowDialog(ReddMain.Instance);
		}
		
		protected void OnQuit(object sender, EventArgs e) {
			EventQuit?.Invoke();
		}
	}
}