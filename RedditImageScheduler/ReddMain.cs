using System;
using System.ComponentModel;
using Eto.Drawing;
using Eto.Forms;
using RedditImageScheduler.Data;
using RedditImageScheduler.Scheduler;
using RedditImageScheduler.UI;

namespace RedditImageScheduler {
	public partial class ReddMain : Form {
		private static ReddMain INSTANCE;
		public static ReddMain Instance => INSTANCE;
		
		private readonly ReddUIMenu uiMenu = new ReddUIMenu();
		private readonly ReddUIEditor uiEditor;
		private readonly ReddUITimetable uiTimetable;
		
		private readonly ReddScheduler reddScheduler;
		
		public ReddMain(ReddScheduler scheduler, ReddDataOptions options) {
			reddScheduler = scheduler;
			
			INSTANCE = this;
			
			uiEditor = new ReddUIEditor(reddScheduler, options);
			uiTimetable = new ReddUITimetable(reddScheduler);
			
			Title = ReddLanguage.APP_NAME;
			MinimumSize = new Size(ReddConfig.WIDTH, ReddConfig.HEIGHT);
			uiEditor.Size = MinimumSize;
			uiTimetable.Size = MinimumSize;
			
			Menu = uiMenu;
			OnEdit();
		}
		
		protected override void OnLoad(EventArgs e) {
			INSTANCE = this;
			base.OnLoad(e);
		}

		protected override void OnUnLoad(EventArgs e) {
			base.OnUnLoad(e);
			INSTANCE = null;
		}
		
		protected override void OnShown(EventArgs e) {
			base.OnShown(e);
			uiEditor.EventTimetable += OnTimetable;
			uiTimetable.EventEdit += OnEdit;
			uiMenu.EventQuit += OnQuit;
			
		}

		protected override void OnClosing(CancelEventArgs e) {
			if( HasChanges && !ShowSaveWarning() ) return;
			
			uiEditor.EventTimetable -= OnTimetable;
			uiTimetable.EventEdit -= OnEdit;
			uiMenu.EventQuit -= OnQuit;
			base.OnClosing(e);
		}

		// ===============================================
		// GETTERS / SETTERS
		public bool HasChanges => Content == uiEditor && uiEditor.HasChanges;

		// ===============================================
		// PUBLIC METHODS
		public bool ShowSaveWarning() {
			DialogResult result = MessageBox.Show(ReddLanguage.MESSAGE_UNSAVED_CHANGES, MessageBoxButtons.YesNoCancel, MessageBoxType.Question, MessageBoxDefaultButton.Yes);
			switch( result ) {
				case DialogResult.Yes:
					uiEditor.Save();
					return true;
				case DialogResult.No:
					uiEditor.Unset();
					return true;
				default:
				case DialogResult.Cancel:
					return false;
			}
		}

		// ===============================================
		// CALLBACKS
		private void OnTimetable() {
			reddScheduler.Paused = false;
			Content = uiTimetable;
		}

		private void OnEdit() {
			reddScheduler.Paused = true;
			Content = uiEditor;
		}

		private void OnQuit() {
			if( HasChanges && !ShowSaveWarning() ) return;
			Application.Instance.Quit();
		}
	}
}