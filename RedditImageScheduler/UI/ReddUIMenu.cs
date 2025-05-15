using System;
using System.Diagnostics;
using Eto.Forms;
using RedditImageScheduler.UI.Core;

namespace RedditImageScheduler.UI {
	public class ReddUIMenu : MenuBar {
		private readonly ReddUIMenuItem commandHome;
		private readonly ReddUIMenuItem commandLoad;
		private readonly ReddUIMenuItem commandSave;
		private readonly ReddUIMenuItem commandAbout;
		private readonly ReddUIMenuItem commandQuit;
		private readonly ReddUIMenuItem commandOptions;
		
		public ReddUIMenu() {
			commandHome = new ReddUIMenuItem() { Text = ReddLanguage.MENU_HOME };
			commandLoad = new ReddUIMenuItem() { Text = ReddLanguage.MENU_LOAD };
			commandSave = new ReddUIMenuItem() { Text = ReddLanguage.MENU_SAVE };
			commandAbout = new ReddUIMenuItem() { Text = ReddLanguage.MENU_ABOUT };
			commandQuit = new ReddUIMenuItem() { Text = ReddLanguage.MENU_QUIT };
			commandOptions = new ReddUIMenuItem() { Text = ReddLanguage.MENU_OPTIONS };

			IncludeSystemItems = MenuBarSystemItems.None;

			ApplicationItems.Add(commandLoad);
			ApplicationItems.Add(commandSave);
			ApplicationItems.Add(commandOptions);
			QuitItem = commandQuit;
			HelpItems.Add(commandHome);
			HelpItems.Add(commandAbout);
		}

		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			
			commandHome.Click += OnHomePage;
			commandLoad.Click += OnLoad;
			commandSave.Click += OnSave;
			commandQuit.Click += OnQuit;
			commandAbout.Click += OnAbout;
			commandOptions.Click += OnOptions;
		}

		protected override void OnUnLoad(EventArgs e) {
			commandHome.Click -= OnHomePage;
			commandLoad.Click -= OnLoad;
			commandSave.Click -= OnSave;
			commandQuit.Click -= OnQuit;
			commandAbout.Click -= OnAbout;
			commandOptions.Click -= OnOptions;
			base.OnUnLoad(e);
		}

		// ===============================================
		// EVENTS
		public delegate void UIMenuEvent();
		public event UIMenuEvent EventQuit;
		public event UIMenuEvent EventLoad;
		public event UIMenuEvent EventSave;
		public event UIMenuEvent EventOptions;

		// ===============================================
		// CALLBACKS
		private void OnLoad(object sender, EventArgs e) {
			EventLoad?.Invoke();
		}
		
		private void OnSave(object sender, EventArgs e) {
			EventSave?.Invoke();
		}
		
		protected void OnOptions(object sender, EventArgs e) {
			EventOptions?.Invoke();
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