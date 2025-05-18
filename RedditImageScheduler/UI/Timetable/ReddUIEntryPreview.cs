using System;
using Eto.Drawing;
using Eto.Forms;
using RedditImageScheduler.Data;

namespace RedditImageScheduler.UI.Timetable {
	public class ReddUIEntryPreview : DynamicLayout {
		private readonly Label etoTitleLabel = new Label();
		private readonly TextBox etoTitle = new TextBox();
		private readonly ImageView etoImage = new ImageView();
		private readonly Label etoSourceLabel = new Label();
		private readonly TextBox etoSource = new TextBox();
		private readonly TextBox etoState = new TextBox();

		private readonly ReddScheduler reddScheduler;
		private ReddDataEntry dataEntry;
		
		public ReddUIEntryPreview(ReddScheduler scheduler) {
			reddScheduler = scheduler;
			
			Spacing = new Size(2, 2);
			Padding = new Padding(2);
			
			BeginVertical();

			BeginHorizontal();
			DynamicLayout title = new DynamicLayout();
			title.BeginHorizontal();
			etoTitleLabel.VerticalAlignment = VerticalAlignment.Center;
			etoTitleLabel.Text = ReddLanguage.LABEL_TITLE;
			title.Add(etoTitleLabel);
			etoTitle.ReadOnly = true;
			title.Add(etoTitle);
			title.EndHorizontal();
			Add(title);
			EndHorizontal();
			
			BeginHorizontal();
			DynamicLayout source = new DynamicLayout();
			source.BeginHorizontal();
			etoSourceLabel.VerticalAlignment = VerticalAlignment.Center;
			etoSourceLabel.Text = ReddLanguage.LABEL_SOURCE;
			source.Add(etoSourceLabel);
			etoSource.ReadOnly = true;
			source.Add(etoSource);
			source.EndHorizontal();
			Add(source);
			EndHorizontal();
			
			BeginHorizontal(true);
			etoImage.Size = new Size(ReddConfig.WIDTH/2, ReddConfig.HEIGHT/2);
			Add(etoImage);
			EndHorizontal();

			BeginHorizontal();
			etoState.ShowBorder = false;
			etoState.ReadOnly = true;
			etoState.Enabled = false;
			Add(etoState);
			EndHorizontal();

			EndVertical();

			Unset();
		}

		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			reddScheduler.OnUpdate += OnUpdate;
		}

		protected override void OnUnLoad(EventArgs e) {
			reddScheduler.OnUpdate -= OnUpdate;
			base.OnUnLoad(e);
		}

		// ===============================================
		// PUBLIC METHODS
		public void Unset() {
			dataEntry = null;
			Visible = false;
		}
		
		public void Set(ReddDataEntry entry) {
			if( entry == null ) {
				Unset();
				return;
			}

			dataEntry = entry;
			
			etoTitle.Text = entry.Title;
			etoSource.Text = entry.Source;
			etoImage.Image = entry.Bitmap;

			OnUpdate();

			Visible = true;
		}

		public void Refresh() => OnUpdate();

		// ===============================================
		// CALLBACKS

		private void OnUpdate() {
			if( dataEntry == null ) {
				etoState.Text = ReddLanguage.STATE_INVALID;
			}
			else {
				ReddDateState state = dataEntry.State;
				switch( state.Status ) {
					case EReddDataStatus.INVALID: etoState.Text = ReddLanguage.STATE_INVALID; break;
					case EReddDataStatus.POSTED: etoState.Text = string.Format(ReddLanguage.STATE_POSTED, dataEntry.Date); break;
					case EReddDataStatus.VALID: etoState.Text = string.Format(ReddLanguage.STATE_VALID, dataEntry.Date); break;
					case EReddDataStatus.VALID_HOURS: etoState.Text = string.Format(ReddLanguage.STATE_VALID_HOURS, state.Time.Hours); break;
					case EReddDataStatus.VALID_MINUTES: etoState.Text = string.Format(ReddLanguage.STATE_VALID_MINUTES, state.Time.Minutes); break;
					case EReddDataStatus.VALID_SECONDS: etoState.Text = string.Format(ReddLanguage.STATE_VALID_SECONDS, state.Time.Seconds); break;
					default: etoState.Text = "Invalid state."; break;
				}
			}
		} 
	}
}