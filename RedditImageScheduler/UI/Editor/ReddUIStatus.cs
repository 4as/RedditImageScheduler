using System;
using System.Collections.Generic;
using System.Text;
using Eto.Forms;

namespace RedditImageScheduler.UI.Editor {
	public class ReddUIStatus : TextBox {
		private readonly StringBuilder stringBuilder = new StringBuilder();
		private readonly List<string> listStates = new List<string>();
		private DateTime dateCurrent;
		private bool hasChanges;
		private bool hasValidTitle;
		private bool hasValidSource;
		private bool hasValidDate;
		private bool hasValidImage;

		public ReddUIStatus() {
			Enabled = false;
			ShowBorder = false;
			ReadOnly = true;
		}

		public bool HasChanges {
			get => hasChanges;
			set {
				hasChanges = value;
				OnValidate();
			}
		}

		public DateTime Date {
			get => dateCurrent;
			set {
				dateCurrent = value;
				OnValidate();
			}
		}

		public bool HasValidTitle {
			get => hasValidTitle;
			set {
				hasValidTitle = value;
				OnValidate();
			}
		}

		public bool HasValidSource {
			get => hasValidSource;
			set {
				hasValidSource = value;
				OnValidate();
			}
		}

		public bool HasValidDate {
			get => hasValidDate;
			set {
				hasValidDate = value;
				OnValidate();
			}
		}

		public bool HasValidImage {
			get => hasValidImage;
			set {
				hasValidImage = value;
				OnValidate();
			}
		}

		private void OnValidate() {
			bool valid = true;
			listStates.Clear();

			if( !hasValidTitle ) {
				listStates.Add(ReddLanguage.STATUS_INVALID_TITLE);
				valid = false;
			}

			if( !hasValidSource ) {
				listStates.Add(ReddLanguage.STATUS_INVALID_SOURCE);
				valid = false;
			}

			if( !hasValidDate ) {
				listStates.Add(ReddLanguage.STATUS_INVALID_DATE);
				valid = false;
			}

			if( !hasValidImage ) {
				listStates.Add(ReddLanguage.STATUS_INVALID_IMAGE);
				valid = false;
			}

			if( valid ) {
				if( !hasChanges ) {
					Text = string.Format(ReddLanguage.STATUS_VALID_SAVED, Date);
				}
				else {
					Text = string.Format(ReddLanguage.STATUS_VALID_UNSAVED, Date);
				}
			}
			else {
				stringBuilder.Clear();
				string delim = string.Empty;
				foreach( string state in listStates ) {
					stringBuilder.Append(delim).Append(state);
					delim = ", ";
				}

				Text = string.Format(ReddLanguage.STATUS_INVALID, stringBuilder);
			}
		}
	}
}