using System;
using Eto.Forms;
using Eto.IO;
using RedditImageScheduler.UI.Core;
using RedditImageScheduler.Utils;

namespace RedditImageScheduler {
	public class ReddTray {
		private readonly TrayIndicator formTray;
		private readonly ButtonMenuItem menuDebug = new ButtonMenuItem();
		private readonly ReddUIMenuItem commandOpen;
		private readonly ReddUIMenuItem commandQuit;
		public ReddTray() {
			commandOpen = new ReddUIMenuItem() { Text = ReddLanguage.MENU_EDIT };
			commandQuit = new ReddUIMenuItem() { Text = ReddLanguage.MENU_QUIT };
			
			formTray = new TrayIndicator() {
				Image = SystemIcons.GetStaticIcon(StaticIconType.OpenDirectory, IconSize.Large),
				Title = ReddLanguage.NAME_APP,
				Menu = new ContextMenu()
			};
			
#if DEBUG
			menuDebug.Enabled = false;
			formTray.Menu.Items.Add(menuDebug);
#endif
			
			formTray.Menu.Items.Add(commandOpen);
			formTray.Menu.Items.Add(commandQuit);
		}

		public bool IsInitialized => !formTray.IsDisposed && formTray.Visible;

		public void Initialize() {
			Deinitialize();
			formTray.Visible = true;
			commandOpen.Click += OnActivation;
			formTray.Activated += OnActivation;
			formTray.Menu.Opening += OnMenu;
			commandQuit.Click += OnQuitting;
		}

		public void Deinitialize() {
			if( !formTray.IsDisposed ) {
				formTray.Visible = false;
				formTray.Activated -= OnActivation;
				formTray.Menu.Opening -= OnMenu;
			}

			commandOpen.Click -= OnActivation;
			commandQuit.Click -= OnQuitting;
		}

		public void Dispose() {
			Deinitialize();
			formTray.Hide();
			formTray.Dispose();
		}
		
		public event Action EventOpen;
		public event Action EventQuit;

		private void OnActivation(object sender, EventArgs e) {
			EventOpen?.Invoke();
		}

		private void OnQuitting(object sender, EventArgs e) {
			EventQuit?.Invoke();
		}
		
		private void OnMenu(object sender, EventArgs e) {
			menuDebug.Text = $"Entries: {ReddUtilBitmaps.NumEntries}, Files: {ReddUtilBitmaps.NumFiles}";
		}
	}
}