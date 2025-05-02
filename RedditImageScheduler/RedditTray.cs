using System;
using Eto.Forms;
using Eto.IO;
using RedditImageScheduler.UI.Core;
using RedditImageScheduler.Utils;

namespace RedditImageScheduler {
	public class RedditTray {
		private readonly TrayIndicator formTray;
		private readonly ButtonMenuItem menuDebug = new ButtonMenuItem();
		private readonly ReddUIMenuItem commandOpen;
		private readonly ReddUIMenuItem commandTimetable;
		private readonly ReddUIMenuItem commandQuit;
		public RedditTray() {
			commandOpen = new ReddUIMenuItem() { Text = ReddLanguage.MENU_EDIT };
			commandTimetable = new ReddUIMenuItem() { Text = ReddLanguage.MENU_TIMETABLE };
			commandQuit = new ReddUIMenuItem() { Text = ReddLanguage.MENU_QUIT };
			
			formTray = new TrayIndicator() {
				Image = SystemIcons.GetStaticIcon(StaticIconType.OpenDirectory, IconSize.Large),
				Title = ReddLanguage.APP_NAME,
				Visible = true,
				Menu = new ContextMenu() {
					Items = { commandOpen, commandTimetable, commandQuit }
				}
			};
			
#if DEBUG
			menuDebug.Enabled = false;
			formTray.Menu.Items.Add(menuDebug);
#endif
		}

		public TrayIndicator UI => formTray;

		public void Initialize() {
			Deinitialize();
			commandOpen.Click += OnActivation;
			formTray.Activated += OnActivation;
			formTray.Menu.Opening += OnMenu;
			commandQuit.Click += OnQuitting;
			commandTimetable.Click += OnTimetable;
		}

		public void Deinitialize() {
			commandOpen.Click -= OnActivation;
			formTray.Activated -= OnActivation;
			formTray.Menu.Opening -= OnMenu;
			commandQuit.Click -= OnQuitting;
			commandTimetable.Click -= OnTimetable;
		}

		public void Dispose() {
			Deinitialize();
			formTray.Hide();
			formTray.Dispose();
		}

		

		public event Action OnOpen;
		public event Action OnPreview;

		private void OnActivation(object sender, EventArgs e) {
			OnOpen?.Invoke();
		}
		
		private void OnTimetable(object sender, EventArgs e) {
			OnPreview?.Invoke();
		}

		private void OnQuitting(object sender, EventArgs e) {
			Application.Instance.Quit();
		}
		
		private void OnMenu(object sender, EventArgs e) {
			menuDebug.Text = $"Entries: {ReddUtilBitmaps.NumEntries}, Files: {ReddUtilBitmaps.NumFiles}";
		}
	}
}