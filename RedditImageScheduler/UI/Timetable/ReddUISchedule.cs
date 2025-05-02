using System;
using Eto.Drawing;
using Eto.Forms;
using RedditImageScheduler.Data;

namespace RedditImageScheduler.UI.Timetable {
	public class ReddUISchedule : GridView {
		private readonly GridColumn etoColumn = new GridColumn();
		
		public ReddUISchedule() {
			ShowHeader = false;
			GridLines = GridLines.Horizontal;
			AllowColumnReordering = false;
			AllowEmptySelection = false;
			AllowMultipleSelection = false;
			etoColumn.DataCell = new GridEntry();
			etoColumn.AutoSize = true;
			etoColumn.Editable = false;
			etoColumn.MinWidth = 300;
			etoColumn.Expand = true;
			etoColumn.Resizable = false;
			etoColumn.Sortable = false;
			Columns.Add(etoColumn);
		}

		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
		}

		private class GridEntry : DrawableCell {
			private readonly FormattedText drawTextDefault = new FormattedText();
			private readonly SolidBrush drawBrushDefault = new SolidBrush(Colors.Black);
			private readonly FormattedText drawTextSelected = new FormattedText();
			private readonly SolidBrush drawBrushSelected = new SolidBrush(Colors.White);

			public GridEntry() {
				drawTextDefault.Alignment = FormattedTextAlignment.Left;
				drawTextDefault.Wrap = FormattedTextWrapMode.None;
				drawTextDefault.Font = SystemFonts.Default();
				drawTextDefault.ForegroundBrush = drawBrushDefault;
				
				drawTextSelected.Alignment = FormattedTextAlignment.Left;
				drawTextSelected.Wrap = FormattedTextWrapMode.None;
				drawTextSelected.Font = SystemFonts.Default();
				drawTextSelected.ForegroundBrush = drawBrushSelected;
			}
			
			protected override void OnPaint(CellPaintEventArgs e) {
				if( !(e.Item is ReddDataEntry entry) ) {
					base.OnPaint(e);
					return;
				}
				
				var g = e.Graphics;
				var bounds = e.ClipRectangle;
				PointF pos = bounds.Location;
				
				if( e.IsSelected ) {
					g.FillRectangle(new Color(0f, 0f, 1f, 1f), bounds);

					drawTextSelected.Text = entry.ToString();
					pos.Y += bounds.Height / 2 - drawTextSelected.Measure().Height / 2;
					g.DrawText(drawTextSelected, pos);
				}
				else {
					if( entry.IsValid ) {
						g.FillRectangle(new Color(1f, 1f, 1f, 1f), bounds);
					}
					else {
						g.FillRectangle(ReddConfig.UI_BG_INVALID, bounds);
					}

					drawTextDefault.Text = entry.ToString();
					
					float bh = bounds.Height / 2f;
					float th = drawTextDefault.Measure().Height / 2f;
					pos.Y += bh - th;
					g.DrawText(drawTextDefault, pos);
				}
			}
		}
	}
}