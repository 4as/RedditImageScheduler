using System;
using System.ComponentModel;
using System.IO;
using Eto;
using Eto.Drawing;
using Eto.Forms;
using RedditImageScheduler.Data;
using RedditImageScheduler.IO;
using RedditImageScheduler.UI;

namespace RedditImageScheduler {
	public partial class ReddMain : Form {
		private static ReddMain INSTANCE;
		public static ReddMain Instance => INSTANCE;
		
		private readonly OpenFileDialog dialogOpenFile = new OpenFileDialog();
		private readonly SaveFileDialog dialogSaveFile = new SaveFileDialog();
		private readonly ReddUIMenu uiMenu = new ReddUIMenu();
		private readonly ReddUIEditor uiEditor;
		private readonly ReddUITimetable uiTimetable;

		private readonly ReddIODatabase ioDatabase;
		private readonly ReddScheduler reddScheduler;
		private readonly ReddDataOptions dataOptions;
		
		public ReddMain(ReddIODatabase database, ReddScheduler scheduler, ReddDataOptions options) {
			ioDatabase = database;
			reddScheduler = scheduler;
			dataOptions = options;
			
			INSTANCE = this;
			
			uiEditor = new ReddUIEditor(reddScheduler, options);
			uiTimetable = new ReddUITimetable(reddScheduler);

			dialogOpenFile.MultiSelect = false;
			dialogOpenFile.Filters.Add(ReddConfig.DB_FILTER);

			dialogSaveFile.FileName = ReddConfig.FILE_DATABASE;
			dialogSaveFile.Directory = new Uri(EtoEnvironment.GetFolderPath(EtoSpecialFolder.EntryExecutable));
			dialogSaveFile.Filters.Add(ReddConfig.DB_FILTER);
			
			Title = ReddLanguage.NAME_APP;
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
			uiMenu.EventLogout += OnLogout;
			uiMenu.EventQuit += OnQuit;
			uiMenu.EventOptions += OnOptions;
			uiMenu.EventSave += OnSave;
			uiMenu.EventLoad += OnLoad;
		}

		protected override void OnClosing(CancelEventArgs e) {
			if( HasChanges && !ShowSaveWarning() ) return;
			
			uiEditor.EventTimetable -= OnTimetable;
			uiTimetable.EventEdit -= OnEdit;
			uiMenu.EventLogout -= OnLogout;
			uiMenu.EventSave -= OnSave;
			uiMenu.EventLoad -= OnLoad;
			uiMenu.EventQuit -= OnQuit;
			uiMenu.EventOptions -= OnOptions;
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
		// EVENTS
		public delegate void MainEvent();
		public event MainEvent EventLogout;

		// ===============================================
		// CALLBACKS
		private void OnLoad() {
			DialogResult result = dialogOpenFile.ShowDialog(ParentWindow);
			if( dialogOpenFile.FileName != null && (result == DialogResult.Yes || result == DialogResult.Ok) ) {
				ioDatabase.Open(dialogOpenFile.FileName);
				if( ioDatabase.IsOpen ) {
					dataOptions.SetDatabase(ioDatabase.FilePath);
				}
			}
		}

		private void OnSave() {
			DialogResult result = dialogSaveFile.ShowDialog(ParentWindow);
			if( dialogSaveFile.FileName != null && (result == DialogResult.Yes || result == DialogResult.Ok) ) {
				ioDatabase.Close();
				try {
					File.Move(dataOptions.DatabasePath, dialogSaveFile.FileName);
					dataOptions.SetDatabase(dialogSaveFile.FileName);
				}
				catch( Exception ) {
					MessageBox.Show(string.Format(ReddLanguage.ERROR_FILE_SAVE_FAILED, dialogSaveFile.FileName), MessageBoxType.Error);
				}

				ioDatabase.Open(dataOptions.DatabasePath);
			}
		}
		
		private void OnTimetable() {
			reddScheduler.Paused = false;
			Content = uiTimetable;
		}

		private void OnEdit() {
			reddScheduler.Paused = true;
			Content = uiEditor;
		}
		
		private void OnOptions() {
			ReddUIOptions options = new ReddUIOptions();
			options.EntrySpacingHours = dataOptions.EntrySpacingHours;
			options.PostSpacingMinutes = dataOptions.PostingSpacingMinutes;
			options.OldTrimDays = dataOptions.TrimmingOldDays;
			
			DialogResult result = options.ShowModal();
			switch( result ) {
				case DialogResult.Yes:
				case DialogResult.Ok:
					dataOptions.SetOptions(options.EntrySpacingHours, options.PostSpacingMinutes, options.OldTrimDays);
					break;
				default:
					return;
			}
		}
		
		private void OnLogout() {
			if( HasChanges && !ShowSaveWarning() ) return;
			EventLogout?.Invoke();
		}

		private void OnQuit() {
			if( HasChanges && !ShowSaveWarning() ) return;
			Application.Instance.Quit();
		}
	}
}