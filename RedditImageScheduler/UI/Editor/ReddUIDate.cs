using System;
using System.Reflection;
using Eto.Drawing;
using Eto.Forms;
using RedditImageScheduler.Data;
using RedditImageScheduler.Utils;

namespace RedditImageScheduler.UI.Editor {
	public class ReddUIDate : DynamicLayout {
		private readonly Label etoLabelDate = new Label();
		private readonly DateTimePicker etoPicker = new DateTimePicker();
		private readonly Label etoLabelHours = new Label();
		private readonly NumericStepper etoHours = new NumericStepper();
		private readonly Button etoSuggest = new Button();
		private readonly Color colorDefault;
		private readonly ReddDataOptions dataOptions;
		public ReddUIDate(ReddDataOptions options) {
			colorDefault = BackgroundColor;
			dataOptions = options;
			
			PropertyInfo prop = etoPicker.ControlObject.GetType().GetProperty("ShowCheckBox");
			if( prop != null && prop.CanWrite ) {
				prop.SetValue(etoPicker.ControlObject, false, null);
			}
			
			BeginHorizontal();
			
			etoLabelDate.VerticalAlignment = VerticalAlignment.Center;
			etoLabelDate.Text = ReddLanguage.LABEL_DATE;
			Add(etoLabelDate);
			
			etoPicker.Mode = DateTimePickerMode.Date;
			Add(etoPicker);
			
			etoLabelHours.VerticalAlignment = VerticalAlignment.Center;
			etoLabelHours.Text = ReddLanguage.LABEL_HOUR;
			Add(etoLabelHours);
			
			etoHours.Increment = 1;
			etoHours.Wrap = true;
			etoHours.DecimalPlaces = 0;
			etoHours.MaximumDecimalPlaces = 0;
			etoHours.MinValue = 0;
			etoHours.MaxValue = 23;
			Add(etoHours);

			etoSuggest.Text = ReddLanguage.BUTTON_SUGGEST;
			Add(etoSuggest);
			
			EndHorizontal();
		}

		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			etoHours.ValueChanged += OnModify;
			etoPicker.ValueChanged += OnModify;
			etoSuggest.Click += OnSuggest;
			OnValidate();
		}

		protected override void OnUnLoad(EventArgs e) {
			etoHours.ValueChanged -= OnModify;
			etoPicker.ValueChanged -= OnModify;
			etoSuggest.Click -= OnSuggest;
			base.OnUnLoad(e);
		}

		// ===============================================
		// GETTERS / SETTERS
		public DateTime Date {
			get {
#if DEBUG
				return etoPicker.Value.GetValueOrDefault();
#else
				DateTime date = etoPicker.Value.GetValueOrDefault();
				return new DateTime(date.Year, date.Month, date.Day, (int)etoHours.Value, date.Minute, 0, date.Kind);
#endif
			}
			set {
				etoHours.Value = value.Hour;
				etoPicker.Value = value;
			}
		}

		public bool IsValid => ReddValidator.IsValidDate(Date);

		// ===============================================
		// PUBLIC METHODS
		public void Refresh() => OnValidate();

		// ===============================================
		// EVENTS
		public delegate void UIDateEvent();
		public event UIDateEvent EventChanged;
		
		private void OnModify(object sender, EventArgs e) {
			OnValidate();
			EventChanged?.Invoke();
		}
		
		private void OnSuggest(object sender, EventArgs e) {
#if DEBUG
			DateTime date = DateTime.Now;
			Date = date.AddSeconds(10);
#else
			DateTime date = DateTime.Now;
			date = new DateTime(date.Year, date.Month, date.Day, date.Hour, 0, 0);
			Date = date.AddHours(dataOptions.EntrySpacingHours);
#endif
		}

		private void OnValidate() {
			BackgroundColor = IsValid ? colorDefault : ReddConfig.UI_BG_INVALID;
		}
	}
}