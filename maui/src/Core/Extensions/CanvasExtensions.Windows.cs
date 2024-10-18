﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Win2D;
using Microsoft.Maui.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.ViewManagement;

namespace Syncfusion.Maui.Toolkit.Graphics.Internals
{
	/// <summary>
	/// Provides extension methods for the <see cref="ICanvas"/> interface on the windows platfrom.
	/// </summary>
	public static partial class CanvasExtensions
    {

		/// <summary>
		/// Draws text on the specified canvas at the given coordinates using the provided text element.
		/// </summary>
		/// <param name="canvas">The canvas to draw on.</param>
		/// <param name="value">The text to draw.</param>
		/// <param name="x">The x-coordinate where the text should be drawn.</param>
		/// <param name="y">The y-coordinate where the text should be drawn.</param>
		/// <param name="textElement">The text element that defines the text's appearance.</param>
		public static void DrawText(this ICanvas canvas, string value, float x, float y, ITextElement textElement)
        {
            if (canvas is W2DCanvas w2DCanvas)
            {
                using (var format = new CanvasTextFormat())
                {
                    IFontManager? fontManager = textElement.FontManager;
                    var font = textElement.Font;
                    if (fontManager != null)
                    {
                        var fontFamily = fontManager.GetFontFamily(font);
                        format.FontFamily = fontFamily.Source;
                        UpdateFontSize(textElement, format);
                        format.FontStyle = font.ToFontStyle();
                        format.FontWeight = font.ToFontWeight();
                        w2DCanvas.Session.DrawText(value, new Vector2(x, y), textElement.TextColor.AsColor(), format);
                    }
                }
            }
        }

        /// <summary>
        /// Draw the text with in specified rectangle area.
        /// </summary>
        /// <param name="canvas">The canvas value.</param>
        /// <param name="value">The text value.</param>
        /// <param name="rect">The rectangle area that specifies the text bound.</param>
        /// <param name="horizontalAlignment">Text horizontal alignment option.</param>
        /// <param name="verticalAlignment">Text vertical alignment option.</param>
        /// <param name="textElement">The text style.</param>
        public static void DrawText(this ICanvas canvas, string value, Rect rect, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment, ITextElement textElement)
        {
            if (canvas is W2DCanvas w2DCanvas)
            {
                using (var format = new CanvasTextFormat())
                {
                    IFontManager? fontManager = textElement.FontManager;
                    var font = textElement.Font;
                    if (fontManager != null)
                    {
                        var fontFamily = fontManager.GetFontFamily(font);
                        format.FontFamily = fontFamily.Source;
                        if (!double.IsNaN(textElement.FontSize))
                        {
                            UpdateFontSize(textElement, format);
                        }
                        format.FontStyle = font.ToFontStyle();
                        format.FontWeight = font.ToFontWeight();

                        CanvasVerticalAlignment canvasVerticallAlignment = CanvasVerticalAlignment.Top;
                        if (verticalAlignment == VerticalAlignment.Center)
                        {
                            canvasVerticallAlignment = CanvasVerticalAlignment.Center;
                        }
                        else if (verticalAlignment == VerticalAlignment.Bottom)
                        {
                            canvasVerticallAlignment = CanvasVerticalAlignment.Bottom;
                        }

                        format.VerticalAlignment = canvasVerticallAlignment;
                        CanvasHorizontalAlignment canvasHorizontalAlignment = CanvasHorizontalAlignment.Left;
                        if (horizontalAlignment == HorizontalAlignment.Center)
                        {
                            canvasHorizontalAlignment = CanvasHorizontalAlignment.Center;
                        }
                        else if (horizontalAlignment == HorizontalAlignment.Right)
                        {
                            canvasHorizontalAlignment = CanvasHorizontalAlignment.Right;
                        }

                        format.HorizontalAlignment = canvasHorizontalAlignment;
                        format.Options = CanvasDrawTextOptions.Clip;
                        w2DCanvas.Session.DrawText(value, new Windows.Foundation.Rect(rect.X, rect.Y, rect.Width, rect.Height), textElement.TextColor.AsColor(), format);
                    }
                }
            }
        }

        private static void UpdateFontSize(ITextElement textElement, CanvasTextFormat format)
        {
            var uiSettings = new UISettings();
            float fontScale = (float)uiSettings.TextScaleFactor;
            double fontSize = textElement.FontSize > 0 ? textElement.FontSize : 12;
            if (textElement.FontAutoScalingEnabled)
                format.FontSize = (float)fontSize * fontScale;
            else
                format.FontSize = (float)fontSize;
        }

		/// <summary>
		/// Draws lines connecting a series of points on the specified canvas using the provided line drawing settings.
		/// </summary>
		/// <param name="canvas">The canvas to draw on.</param>
		/// <param name="points">An array of points defining the lines to be drawn.</param>
		/// <param name="lineDrawing">The line drawing settings to use.</param>
		public static void DrawLines(this ICanvas canvas, float[] points, ILineDrawing lineDrawing)
        {
            if (canvas is W2DCanvas w2DCanvas)
            {
                int j = 0;

                w2DCanvas.StrokeSize = (float)lineDrawing.StrokeWidth;
                w2DCanvas.StrokeColor = lineDrawing.Stroke;
                w2DCanvas.Antialias = lineDrawing.EnableAntiAliasing;
                w2DCanvas.Alpha = lineDrawing.Opacity;

                if (lineDrawing.StrokeDashArray != null)
                    w2DCanvas.StrokeDashPattern = lineDrawing.StrokeDashArray.ToFloatArray();
                //Draw path.

                PathF pathF = new PathF();
                while (j + 1 < points.Length)
                {
                    pathF.LineTo(points[j++], points[j++]);
                }

                //Rendering performance was improved while created as new path.
                pathF = new PathF(pathF);
                w2DCanvas.DrawPath(pathF);
            }
        }
    }
}
