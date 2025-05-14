using System;
using Eto.Drawing;
using Eto.Forms;

namespace RedditImageScheduler.UI {
	public class ReddUIOptions : Dialog<DialogResult> {
		private readonly DynamicLayout etoLayout = new DynamicLayout();
		private readonly NumericStepper etoSpacing = new NumericStepper();
		private readonly Label etoSpacingInfo = new Label();
		private readonly NumericStepper etoOverlap = new NumericStepper();
		private readonly Label etoOverlapInfo = new Label();
		private readonly NumericStepper etoTrim = new NumericStepper();
		private readonly Label etoTrimInfo = new Label();
		private readonly Button etoSave = new Button();
		private readonly Button etoCancel = new Button();
		public ReddUIOptions() {
			Title = ReddLanguage.NAME_OPTIONS;
			Resizable = false;
			Size = new Size(400, 300);
			Content = etoLayout;
			Padding = new Padding(2);
			
			etoLayout.BeginVertical(null, new Size(2,4), true);

			etoLayout.BeginGroup(ReddLanguage.LABEL_ENTRY_SPACING);
			etoSpacing.MinValue = ReddConfig.OPTION_ENTRY_SPACING_HOURS.Min;
			etoSpacing.MaxValue = ReddConfig.OPTION_ENTRY_SPACING_HOURS.Max;
			etoSpacing.Increment = 1;
			etoSpacing.DecimalPlaces = 0;
			etoSpacing.MaximumDecimalPlaces = 0;
			//etoSpacing.FormatString = ReddLanguage.FORMAT_HOURS;
			etoSpacing.ToolTip = ReddLanguage.INFO_ENTRY_SPACING;
			etoLayout.Add(etoSpacing);

			etoSpacingInfo.Text = ReddLanguage.INFO_ENTRY_SPACING;
			etoSpacingInfo.Enabled = false;
			etoSpacingInfo.TextColor = SystemColors.DisabledText;
			etoLayout.Add(etoSpacingInfo);
			etoLayout.EndGroup();
			
			etoLayout.BeginGroup(ReddLanguage.LABEL_POSTING_SPACING);
			etoOverlap.MinValue = ReddConfig.OPTION_POSTING_SPACING_MINUTES.Min;
			etoOverlap.MaxValue = ReddConfig.OPTION_POSTING_SPACING_MINUTES.Max;
			etoOverlap.Increment = 1;
			etoOverlap.DecimalPlaces = 0;
			etoOverlap.MaximumDecimalPlaces = 0;
			//etoOverlap.FormatString = ReddLanguage.FORMAT_MINUTES;
			etoOverlap.ToolTip = ReddLanguage.INFO_POSTING_SPACING;
			etoLayout.Add(etoOverlap);

			etoOverlapInfo.Text = ReddLanguage.INFO_POSTING_SPACING;
			etoOverlapInfo.Enabled = false;
			etoOverlapInfo.TextColor = SystemColors.DisabledText;
			etoLayout.Add(etoOverlapInfo);
			etoLayout.EndGroup();
			
			etoLayout.BeginGroup(ReddLanguage.LABEL_TRIM_OLD);
			etoTrim.MinValue = ReddConfig.OPTION_OLD_TRIMMING_DAYS.Min;
			etoTrim.MaxValue = ReddConfig.OPTION_OLD_TRIMMING_DAYS.Max;
			etoTrim.Increment = 1;
			etoTrim.DecimalPlaces = 0;
			etoTrim.MaximumDecimalPlaces = 0;
			//etoTrim.FormatString = ReddLanguage.FORMAT_DAYS;
			etoTrim.ToolTip = ReddLanguage.INFO_OLD_TRIMMING;
			etoLayout.Add(etoTrim);

			etoTrimInfo.Text = ReddLanguage.INFO_OLD_TRIMMING;
			etoTrimInfo.Enabled = false;
			etoTrimInfo.TextColor = SystemColors.DisabledText;
			etoLayout.Add(etoTrimInfo);
			etoLayout.EndGroup();

			DynamicLayout layout = new DynamicLayout();
			layout.BeginHorizontal();
			etoSave.Text = ReddLanguage.BUTTON_SAVE;
			layout.Add(etoSave);
			layout.AddSpace();
			etoCancel.Text = ReddLanguage.BUTTON_CANCEL;
			layout.Add(etoCancel);
			layout.EndHorizontal();

			etoLayout.Add(layout);

			etoLayout.EndVertical();
		}

		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			etoSave.Click += OnSave;
			etoCancel.Click += OnCancel;
		}

		protected override void OnUnLoad(EventArgs e) {
			etoSave.Click -= OnSave;
			etoCancel.Click -= OnCancel;
			base.OnUnLoad(e);
		}

		// ===============================================
		// GETTERS / SETTERS
		public uint EntrySpacingHours {
			get => (uint)etoSpacing.Value;
			set => etoSpacing.Value = value;
		}

		public uint PostSpacingMinutes {
			get => (uint)etoOverlap.Value;
			set => etoOverlap.Value = value;
		}

		public uint OldTrimDays {
			get => (uint)etoTrim.Value;
			set => etoTrim.Value = value;
		}

		// ===============================================
		// CALLBACKS
		protected override void OnShown(EventArgs e) {
			base.OnShown(e);
			Result = DialogResult.None;
		}

		private void OnSave(object sender, EventArgs e) {
			Close(DialogResult.Ok);
			Close();
		}
		
		private void OnCancel(object sender, EventArgs e) {
			Close(DialogResult.Cancel);
			Close();
		}
	}
}