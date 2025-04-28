using System;
using System.Reflection;
using Eto.Forms;

namespace RedditImageScheduler.UI.Core {
	public class ReddUIDate : DateTimePicker {
		public ReddUIDate() {
			PropertyInfo prop = ControlObject.GetType().GetProperty("ShowCheckBox");
			if( prop != null && prop.CanWrite ) {
				prop.SetValue(ControlObject, false, null);
			}
		}

		public DateTime GetDateAtHour(int hour) {
			DateTime date = Value.GetValueOrDefault();
			return new DateTime(date.Year, date.Month, date.Day, hour, date.Minute, 0, date.Kind);
		}
	}
}