﻿using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Syncfusion.Maui.Toolkit.Graphics.Internals;

namespace Syncfusion.Maui.Toolkit
{
    internal class TextStyle : BindableObject, ITextElement
    {
        #region Bindable properties

        /// <summary>
        /// Identifies the <see cref="TextColor"/> bindable property.
        /// </summary>
        /// <value>
        /// The identifier for <see cref="TextColor"/> bindable property.
        /// </value>
        public static readonly BindableProperty TextColorProperty =
           BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(TextStyle), Colors.Black);

        /// <summary>
        /// Identifies the <see cref="FontSize"/> bindable property.
        /// </summary>
        /// <value>
        /// The identifier for <see cref="FontSize"/> bindable property.
        /// </value>
        public static readonly BindableProperty FontSizeProperty = FontElement.FontSizeProperty;

        /// <summary>
        /// Identifies the <see cref="FontFamily"/> bindable property.
        /// </summary>
        /// <value>
        /// The identifier for <see cref="FontFamily"/> bindable property.
        /// </value>
        public static readonly BindableProperty FontFamilyProperty = FontElement.FontFamilyProperty;

        /// <summary>
        /// Identifies the <see cref="FontAttributes"/> bindable property.
        /// </summary>
        /// <value>
        /// The identifier for <see cref="FontAttributes"/> bindable property.
        /// </value>
        public static readonly BindableProperty FontAttributesProperty = FontElement.FontAttributesProperty;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the text color for the icon text.
        /// </summary>
        public Color TextColor
        {
            get { return (Color)this.GetValue(TextColorProperty); }
            set { this.SetValue(TextColorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the double value that represents size of the icon text.
        /// </summary>
        public double FontSize
        {
            get { return (double)this.GetValue(FontSizeProperty); }
            set { this.SetValue(FontSizeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the string, that represents font family of the icon text.
        /// </summary>
        public string FontFamily
        {
            get { return (string)this.GetValue(FontFamilyProperty); }
            set { this.SetValue(FontFamilyProperty, value); }
        }

        /// <summary>
        /// Gets or sets the FontAttributes of the icon text.
        /// </summary>
        public FontAttributes FontAttributes
        {
            get { return (FontAttributes)this.GetValue(FontAttributesProperty); }
            set { this.SetValue(FontAttributesProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool FontAutoScalingEnabled
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the font of the icon text.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1033:Interface methods should be callable by child types", Justification = "We require this.")]
        Microsoft.Maui.Font ITextElement.Font => (Microsoft.Maui.Font)this.GetValue(FontElement.FontProperty);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        void ITextElement.OnFontAutoScalingEnabledChanged(bool oldValue, bool newValue)
        {
            //throw new NotImplementedException();
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1033:Interface methods should be callable by child types", Justification = "We require this.")]
        double ITextElement.FontSizeDefaultValueCreator()
        {
            return 12d;
        }

        /// <inheritdoc/>
        void ITextElement.OnFontAttributesChanged(FontAttributes oldValue, FontAttributes newValue)
        {
        }

        /// <inheritdoc/>
        void ITextElement.OnFontChanged(Microsoft.Maui.Font oldValue, Microsoft.Maui.Font newValue)
        {
        }

        /// <inheritdoc/>
        void ITextElement.OnFontFamilyChanged(string oldValue, string newValue)
        {
        }

        /// <inheritdoc/>
        void ITextElement.OnFontSizeChanged(double oldValue, double newValue)
        {
        }

        #endregion
    }
}
