using System;
using System.Reflection;
using Eto.Drawing;
using Eto.Forms;

namespace RedditImageScheduler.UI.Entry {
	public class ReddUIDate : DynamicLayout {
		private readonly Label etoLabelDate = new Label();
		private readonly DateTimePicker etoPicker = new DateTimePicker();
		private readonly Label etoLabelHours = new Label();
		private readonly NumericStepper etoHours = new NumericStepper();
		private readonly Color colorDefault;
		public ReddUIDate() {
			colorDefault = BackgroundColor;
			
			PropertyInfo prop = etoPicker.ControlObject.GetType().GetProperty("ShowCheckBox");
			if( prop != null && prop.CanWrite ) {
				prop.SetValue(etoPicker.ControlObject, false, null);
			}
			
			BeginHorizontal();
			
			etoLabelDate.VerticalAlignment = VerticalAlignment.Center;
			etoLabelDate.Text = ReddLanguage.DATE;
			Add(etoLabelDate);
			
			etoPicker.Mode = DateTimePickerMode.Date;
			etoPicker.MinDate = DateTime.Today;
			Add(etoPicker);
			
			etoLabelHours.VerticalAlignment = VerticalAlignment.Center;
			etoLabelHours.Text = ReddLanguage.HOUR;
			Add(etoLabelHours);
			
			etoHours.Increment = 1;
			etoHours.Wrap = true;
			etoHours.DecimalPlaces = 0;
			etoHours.MaximumDecimalPlaces = 0;
			etoHours.MinValue = 0;
			etoHours.MaxValue = 23;
			Add(etoHours);
			
			EndHorizontal();
		}

		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			etoHours.ValueChanged += OnModify;
			etoPicker.ValueChanged += OnModify;
			OnValidate();
		}

		protected override void OnUnLoad(EventArgs e) {
			etoHours.ValueChanged -= OnModify;
			etoPicker.ValueChanged -= OnModify;
			base.OnUnLoad(e);
		}

		// ===============================================
		// GETTERS / SETTERS
		public DateTime Date {
			get {
				DateTime date = etoPicker.Value.GetValueOrDefault();
				return new DateTime(date.Year, date.Month, date.Day, (int)etoHours.Value, date.Minute, 0, date.Kind);
			}
			set {
				etoPicker.Value = value;
				etoHours.Value = value.Hour;
			}
		}

		public bool IsValid => Date > DateTime.Now;

		// ===============================================
		// EVENTS
		public delegate void UIDateEvent();
		public event UIDateEvent OnDateChanged;
		
		private void OnModify(object sender, EventArgs e) {
			OnValidate();
			OnDateChanged?.Invoke();
		}

		private void OnValidate() {
			BackgroundColor = IsValid ? colorDefault : ReddConfig.UI_BG_INVALID;
		}
	}
}