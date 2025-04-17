using System;
using Eto.Forms;
using Eto.IO;

namespace RedditImageScheduler {
	public class RedditTray {
		private readonly TrayIndicator formTray;
		private readonly Command commandOpen;
		private readonly Command commandQuit;
		public RedditTray() {
			commandOpen = new Command { MenuText = "&Open...", Shortcut = Application.Instance.CommonModifier | Keys.O };
			commandQuit = new Command { MenuText = "&Quit", Shortcut = Application.Instance.CommonModifier | Keys.Q };
			
			formTray = new TrayIndicator() {
				Image = SystemIcons.GetStaticIcon(StaticIconType.OpenDirectory, IconSize.Large),
				Title = "Reddit Image Scheduler",
				Visible = true,
				Menu = new ContextMenu() {
					Items = { commandOpen, commandQuit }
				}
			};
		}

		public TrayIndicator UI => formTray;

		public void Initialize() {
			Deinitialize();
			commandOpen.Executed += OnActivation;
			formTray.Activated += OnActivation;
			commandQuit.Executed += OnQuitting;
		}

		public void Deinitialize() {
			commandOpen.Executed -= OnActivation;
			formTray.Activated -= OnActivation;
			commandQuit.Executed -= OnQuitting;
		}

		public void Dispose() {
			Deinitialize();
			formTray.Hide();
			formTray.Dispose();
		}

		public event Action OnOpen;

		private void OnActivation(object sender, EventArgs e) {
			OnOpen?.Invoke();
		}

		private void OnQuitting(object sender, EventArgs e) {
			Application.Instance.Quit();
		}
	}
}