﻿using System.Collections.ObjectModel;
using Microsoft.Maui.Graphics;
using Syncfusion.Maui.Toolkit.Graphics.Internals;

namespace Syncfusion.Maui.Toolkit.Charts
{
	internal class ChartAxisView : SfDrawableView
    {
        #region Properties

        internal CartesianChartArea Area { get; set; }

        #endregion

        #region Constructor

        public ChartAxisView(CartesianChartArea area)
        {
            Area = area;
        }

        #endregion

        #region Methods

        #region Protected Methods

        protected override void OnDraw(ICanvas canvas, RectF dirtyRect)
        {
            var axisLayout = Area.AxisLayout;
            OnDrawAxis(canvas, axisLayout.HorizontalAxes);
            OnDrawAxis(canvas, axisLayout.VerticalAxes);
        }

        #endregion

        #region Private Methods

        void OnDrawAxis(ICanvas canvas, ObservableCollection<ChartAxis>? axes)
        {
            if (axes == null) return;

            foreach (ChartAxis chartAxis in axes)
            {
                Rect arrangeRect = chartAxis.ArrangeRect;
                if (arrangeRect != Rect.Zero)
                {
                    canvas.CanvasSaveState();
                    chartAxis.DrawAxis(canvas, arrangeRect);
                    canvas.CanvasRestoreState();
                }
            }
        }

        #endregion

        #endregion
    }
}
