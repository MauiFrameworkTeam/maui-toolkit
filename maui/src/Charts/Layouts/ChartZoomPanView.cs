﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Syncfusion.Maui.Toolkit.Graphics.Internals;

namespace Syncfusion.Maui.Toolkit.Charts.Chart.Layouts
{
	internal class ChartZoomPanView : SfView
    {
        #region Fields

        int _dimension = 2;
        double _yValue = 0, _xValue = 0;
        List<TrackballAxisInfo> _axisPointInfos = new List<TrackballAxisInfo>();

        #endregion

        #region Properties

        internal ChartZoomPanBehavior? Behavior { get; set; }

        #endregion

        #region Constructor

        public ChartZoomPanView()
        {
            DrawingOrder = DrawingOrder.AboveContent;
        }

        #endregion

        #region Methods

        #region Protected Methods

        protected override void OnDraw(ICanvas canvas, RectF dirtyRect)
        {
            if (Behavior != null && Behavior.Chart is SfCartesianChart chart && Behavior.SelectionRect != Rect.Zero)
            {
                canvas.CanvasSaveState();
                DrawSelectionRect(canvas, Behavior);
                DrawSelectionElements(canvas, chart, Behavior);
                canvas.CanvasRestoreState();
            }
        }

        #endregion

        #region Private Methods

        void Drawable()
        {

        }

        void DrawSelectionRect(ICanvas canvas, ChartZoomPanBehavior behavior)
        {
            var selectionRectStrokeDashArray = behavior.SelectionRectStrokeDashArray;
            var selectionRectStrokeWidth = behavior.SelectionRectStrokeWidth;
            var selectionRect = behavior.SelectionRect;
            bool hasBorder = selectionRectStrokeWidth > 0;

            canvas.CanvasSaveState();

            if (hasBorder)
            {
                canvas.StrokeColor = behavior.SelectionRectStroke.ToColor();
                canvas.StrokeSize = (float)selectionRectStrokeWidth;

                if (selectionRectStrokeDashArray != null && selectionRectStrokeDashArray.Count > 0)
                {
                    canvas.StrokeDashPattern = selectionRectStrokeDashArray.ToFloatArray();
                }
            }

            canvas.SetFillPaint(behavior.SelectionRectFill, selectionRect);
            canvas.DrawShape(selectionRect, ShapeType.Rectangle, hasBorder, false);
            canvas.CanvasRestoreState();
        }

        void DrawSelectionElements(ICanvas canvas, SfCartesianChart chart, ChartZoomPanBehavior behavior)
        {
            Point startPoint = Point.Zero, endPoint = Point.Zero;
            TooltipPosition tooltipPosition = TooltipPosition.Auto;

            canvas.StrokeColor = chart.TrackLineStroke.ToColor();
            canvas.StrokeSize = 1;

            if (chart != null && (Math.Abs(behavior.SelectionRect.Width) > 20 || Math.Abs(behavior.SelectionRect.Height) > 20))
            {
                var area = chart.ChartArea;
                var axisLayout = area.AxisLayout;
                var xAxes = axisLayout.HorizontalAxes;
                GenerateSelectionElements(canvas, xAxes, ref startPoint, ref endPoint, ref tooltipPosition);
                var yAxes = axisLayout.VerticalAxes;
                GenerateSelectionElements(canvas, yAxes, ref startPoint, ref endPoint, ref tooltipPosition);
            }
        }

        void GenerateSelectionElements(ICanvas canvas, ObservableCollection<ChartAxis> axes, ref Point startPoint, ref Point endPoint, ref TooltipPosition tooltipPosition)
        {
            if (Behavior == null)
            {
                return;
            }

            var selectedRect = Behavior.SelectionRect;

            foreach (var axis in axes)
            {
                bool isOpposed = axis.IsOpposed();
                Rect actualArrangeRect = axis.ArrangeRect;

                if (axis.IsVisible && axis.ShowTrackballLabel && Behavior.IsSelectionZoomingActivated)
                {
                    if (axis.IsVertical)
                    {
                        startPoint = new Point(isOpposed ? actualArrangeRect.X : actualArrangeRect.X + actualArrangeRect.Width, selectedRect.Top);
                        endPoint = new Point(isOpposed ? actualArrangeRect.X : actualArrangeRect.X + actualArrangeRect.Width, selectedRect.Bottom);
                        tooltipPosition = isOpposed ? TooltipPosition.Right : TooltipPosition.Left;
                    }
                    else
                    {
                        startPoint = new Point(selectedRect.Left, isOpposed ? actualArrangeRect.Y + actualArrangeRect.Height : actualArrangeRect.Y);
                        endPoint = new Point(selectedRect.Right, isOpposed ? actualArrangeRect.Y + actualArrangeRect.Height : actualArrangeRect.Y);
                        tooltipPosition = isOpposed ? TooltipPosition.Top : TooltipPosition.Bottom;
                    }

                    canvas.DrawLine((float)startPoint.X, (float)startPoint.Y, (float)endPoint.X, (float)endPoint.Y);

                    GenerateAxisTrackballInfos(startPoint, endPoint, tooltipPosition, axis);

                    foreach (var item in _axisPointInfos)
                    {
                        item.Helper.Draw(canvas);
                    }
                }
            }
        }

        void GenerateAxisTrackballInfos(PointF startPoint, PointF endPoint, TooltipPosition tooltipPosition, ChartAxis axis)
        {
            if (Behavior != null && Behavior.Chart is SfCartesianChart chart && chart is IChart iChart)
            {
                Rect axisRect = axis.ArrangeRect;
                var clipRect = iChart.ActualSeriesClipRect;
                _axisPointInfos.Clear();
                string labelFormat = "##.#";

                if (!string.IsNullOrEmpty(axis.TrackballLabelStyle.LabelFormat))
                {
                    labelFormat = axis.TrackballLabelStyle.LabelFormat;
                }
                else if (axis is DateTimeAxis)
                {
                    labelFormat = "MM-dd-yyyy";
                }

                if (axis.IsVertical)
                {
                    _yValue = (float)clipRect.Top - axisRect.Top;
                }
                else
                {
                    _xValue = (float)clipRect.Left - axisRect.Left;
                }

                TrackballAxisInfo axisPointInfo1 = new TrackballAxisInfo(axis, new TooltipHelper(Drawable) { Duration = int.MaxValue }, GetAxisLabel(axis, chart.PointToValue(axis, startPoint.X + _xValue, startPoint.Y + _yValue), labelFormat), startPoint.X, startPoint.Y + (float)clipRect.Top);
                TrackballAxisInfo axisPointInfo2 = new TrackballAxisInfo(axis, new TooltipHelper(Drawable) { Duration = int.MaxValue }, GetAxisLabel(axis, chart.PointToValue(axis, endPoint.X + _xValue, endPoint.Y + _yValue), labelFormat), endPoint.X, endPoint.Y + (float)clipRect.Top);

                axisPointInfo1.Helper.Position = tooltipPosition;
                axisPointInfo2.Helper.Position = tooltipPosition;

                if (axis.TrackballLabelStyle != null)
                {
                    MapChartLabelStyle(chart, axisPointInfo1.Helper, axis.TrackballLabelStyle);
                    MapChartLabelStyle(chart, axisPointInfo2.Helper, axis.TrackballLabelStyle);
                }

                Rect actualArrangeRect = new Rect(axis.ArrangeRect.X, axis.ArrangeRect.Y, axis.ArrangeRect.X + axis.ArrangeRect.Width, axis.ArrangeRect.Y + axis.ArrangeRect.Height);

                axisPointInfo1.Helper.Show(actualArrangeRect, new Rect(startPoint.X - 1, startPoint.Y - 1, _dimension, _dimension), false);
                axisPointInfo2.Helper.Show(actualArrangeRect, new Rect(endPoint.X - 1, endPoint.Y - 1, _dimension, _dimension), false);

                _axisPointInfos.Add(axisPointInfo1);
                _axisPointInfos.Add(axisPointInfo2);
            }
        }

        static string GetAxisLabel(ChartAxis axis, double value, string labelFormat)
        {
            if (axis is CategoryAxis categoryAxis)
            {
                var currSeries = categoryAxis.GetActualSeries();
                if (currSeries != null)
                {
                    int roundedValue = Math.Max(0, (int)Math.Round(value));
                    return categoryAxis.GetLabelContent(currSeries, roundedValue, labelFormat);
                }
            }
            else if (axis is NumericalAxis)
            {
                return value.ToString(labelFormat);
            }
            else if (axis is LogarithmicAxis)
            {
                return ChartAxis.GetActualLabelContent(value, labelFormat).ToString();
            }
            else if (axis is DateTimeAxis datetimeAxis)
            {
                string format = labelFormat ?? datetimeAxis.GetSpecificFormattedLabel(datetimeAxis.ActualIntervalType);
                return ChartAxis.GetFormattedAxisLabel(format, value);
            }

            return ChartAxis.GetActualLabelContent(value, labelFormat);
        }

        void MapChartLabelStyle(SfCartesianChart cartesianChart, TooltipHelper helper, ChartLabelStyle chartLabelStyle)
        {
            var background = chartLabelStyle.Background;
            helper.FontAttributes = chartLabelStyle.FontAttributes;
            helper.FontFamily = chartLabelStyle.FontFamily;
            helper.FontSize = chartLabelStyle.FontSize;
            helper.Padding = chartLabelStyle.Margin;
            helper.Stroke = chartLabelStyle.Stroke;
            helper.StrokeWidth = (float)chartLabelStyle.StrokeWidth;
            helper.Background = chartLabelStyle.Background;

            if (!chartLabelStyle.IsTextColorUpdated)
            {
                var fontColor = background == default(Brush) || background.ToColor() == Colors.Transparent ?
                        cartesianChart.GetTextColorBasedOnChartBackground() :
                        ChartUtils.GetContrastColor((background as SolidColorBrush).ToColor());
                helper.TextColor = fontColor;
            }
            else
            {
                helper.TextColor = chartLabelStyle.TextColor;
            }
        }

        #endregion

        #endregion
    }
}
