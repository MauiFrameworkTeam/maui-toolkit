﻿namespace Syncfusion.Maui.Toolkit.TabView
{
    using Microsoft.Maui;
    using Microsoft.Maui.Controls;
    using Syncfusion.Maui.Toolkit.Helper;

    /// <summary>
    /// Represents the material design visual style for a <see cref="SfTabView"/> control.
    /// </summary>
    internal class TabViewMaterialVisualStyle : SfGrid
    {
        #region Fields

        SfImage? _image;
        SfLabel? _header;
        const double _tabHeaderImageSize = 24d;
        SfHorizontalStackLayout? _horizontalLayout;
        SfVerticalStackLayout? _verticalLayout;

        #endregion

        #region Bindable Properties

        /// <summary>
        /// Identifies the <see cref="HeaderDisplayMode"/> bindable property.
        /// </summary>
        public static readonly BindableProperty HeaderDisplayModeProperty =
           BindableProperty.Create(
               nameof(HeaderDisplayMode),
               typeof(TabBarDisplayMode),
               typeof(TabViewMaterialVisualStyle),
               TabBarDisplayMode.Default,
               propertyChanged: OnHeaderDisplayModePropertyChanged);

        /// <summary>
        /// Identifies the <see cref="ImagePosition"/> bindable property.
        /// </summary>
        public static readonly BindableProperty ImagePositionProperty =
           BindableProperty.Create(
               nameof(ImagePosition),
               typeof(TabImagePosition),
               typeof(TabViewMaterialVisualStyle),
               TabImagePosition.Top,
               propertyChanged: OnImagePositionPropertyChanged);

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the value that defines the header display mode.
        /// </summary>
        public TabBarDisplayMode HeaderDisplayMode
        {
            get => (TabBarDisplayMode)GetValue(HeaderDisplayModeProperty);
            set => SetValue(HeaderDisplayModeProperty, value);
        }

        /// <summary>
        /// Gets or sets the value that defines the image position.
        /// </summary>
        public TabImagePosition ImagePosition
        {
            get => (TabImagePosition)GetValue(ImagePositionProperty);
            set => SetValue(ImagePositionProperty, value);
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TabViewMaterialVisualStyle"/> class.
        /// </summary>
        public TabViewMaterialVisualStyle()
        {
            IsClippedToBounds = true;

            VerticalOptions = LayoutOptions.Fill;
            HorizontalOptions = LayoutOptions.Fill;

            Margin = new Thickness(0, 0, 0, 0);

            InitializeLayout();
            InitializeIcon();
            InitializeHeader();
            UpdateLayout();
            ResetStyle();
        }

        #endregion

        #region Override Methods

        /// <summary>
        /// Invoked whenever the parent of an element is set.
        /// </summary>
        protected override void OnParentSet()
        {
            if (Parent != null)
            {
                Parent.PropertyChanged -= OnParentPropertyChanged;
            }

            base.OnParentSet();
            SetBinding();

            if (Parent != null)
            {
                Parent.PropertyChanged += OnParentPropertyChanged;
            }
        }
        #endregion

        #region Property Changed Implementation

        /// <summary>
        /// Handles property changes of the parent control, updating the current control accordingly.
        /// </summary>
        void OnParentPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is SfTabItem item && item != null)
            {
                if (e.PropertyName == nameof(SfTabItem.HeaderDisplayMode))
                {
                    HeaderDisplayMode = item.HeaderDisplayMode;
                }
                if (e.PropertyName == nameof(SfTabItem.TabWidthMode) ||
                    e.PropertyName == nameof(SfTabItem.HeaderDisplayMode) ||
                    e.PropertyName == nameof(SfTabItem.HeaderDisplayMode) ||
                    e.PropertyName == nameof(SfTabItem.ImagePosition) ||
                    e.PropertyName == nameof(SfTabItem.HeaderHorizontalTextAlignment))
                {
                    UpdateHorizontalOptions(item);
                    UpdateHeaderHorizontalOptions(item);
                }
            }
        }

        static void OnHeaderDisplayModePropertyChanged(BindableObject bindable, object oldValue, object newValue) => (bindable as TabViewMaterialVisualStyle)?.UpdateHeaderDisplayMode();

        static void OnImagePositionPropertyChanged(BindableObject bindable, object oldValue, object newValue) => (bindable as TabViewMaterialVisualStyle)?.UpdateImagePosition();

        #endregion

        #region Private Methods

        /// <summary>
        /// Resets the styles of the control and its child elements to their default values.
        /// </summary>
        void ResetStyle()
        {
            Style = null;

            if (_horizontalLayout != null)
            {
                _horizontalLayout.Style = null;
            }

            if (_verticalLayout != null)
            {
                _verticalLayout.Style = null;
            }

            if (_image != null)
            {
                _image.Style = null;
            }

            if (_header != null)
            {
                _header.Style = null;
            }
        }

        /// <summary>
        /// Initializes the horizontal and vertical layouts for the control.
        /// </summary>
        void InitializeLayout()
        {
            _horizontalLayout = new SfHorizontalStackLayout()
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Fill,
            };

            _verticalLayout = new SfVerticalStackLayout()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Center,
            };

            Children.Add(_verticalLayout);
        }

        /// <summary>
        /// Initializes the image icon for the tab and adds it to the vertical layout.
        /// </summary>
        void InitializeIcon()
        {
            _image = new SfImage()
            {
                Aspect = Aspect.AspectFit,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = _tabHeaderImageSize,
                HeightRequest = _tabHeaderImageSize,
            };

            _verticalLayout?.Children.Add(_image);
        }

        /// <summary>
        /// Initializes the header label for the tab and adds it to the vertical layout.
        /// </summary>
        void InitializeHeader()
        {
            _header = new SfLabel()
            {
                LineBreakMode = LineBreakMode.TailTruncation,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                FontAutoScalingEnabled = false,
            };

            _verticalLayout?.Children.Add(_header);
        }

        /// <summary>
        /// Updates the layout based on the current <see cref="HeaderDisplayMode"/>  and <see cref="ImagePosition"/>.
        /// </summary>
        void UpdateLayout()
        {
            ClearAllChildren();

            if (HeaderDisplayMode == TabBarDisplayMode.Text)
            {
                if (_header != null)
                {
                    Children.Add(_header);
                }
            }
            else if (HeaderDisplayMode == TabBarDisplayMode.Image)
            {
                if (_image != null)
                {
                    Children.Add(_image);
                }
            }
            else
            {
                if (ImagePosition == TabImagePosition.Top || ImagePosition == TabImagePosition.Bottom)
                {
                    UpdateVerticalLayout();
                }
                else
                {
                    UpdateHorizontalLayout();
                }
            }
        }

        /// <summary>
        /// Clears all child elements from the main control and its layouts.
        /// </summary>
        void ClearAllChildren()
        {
            Children?.Clear();
            _horizontalLayout?.Clear();
            _verticalLayout?.Clear();
        }

        /// <summary>
        /// Updates the vertical layout based on the current <see cref="ImagePosition"/>.
        /// </summary>
        void UpdateVerticalLayout()
        {
            if (_verticalLayout == null)
                return;

            if (ImagePosition == TabImagePosition.Top)
            {
                _verticalLayout.Add(_image);
                _verticalLayout.Add(_header);
            }
            else
            {
                _verticalLayout.Add(_header);
                _verticalLayout.Add(_image);
            }
            Children.Add(_verticalLayout);
        }

        /// <summary>
        /// Updates the horizontal layout based on the current <see cref="ImagePosition"/>.
        /// </summary>
        void UpdateHorizontalLayout()
        {
            if (_horizontalLayout == null)
                return;

            if (ImagePosition == TabImagePosition.Left)
            {
                _horizontalLayout.Add(_image);
                _horizontalLayout.Add(_header);
            }
            else
            {
                _horizontalLayout.Add(_header);
                _horizontalLayout.Add(_image);
            }
            Children.Add(_horizontalLayout);
        }

        /// <summary>
        /// Sets up data bindings for the control and its child elements.
        /// </summary>
        void SetBinding()
        {
            BindingContext = Parent;
            _image?.SetBinding(SfImage.SourceProperty, nameof(SfTabItem.ImageSource));

            SetHeaderBinding();

            this.SetBinding(TabViewMaterialVisualStyle.ImagePositionProperty, nameof(SfTabItem.ImagePosition), BindingMode.TwoWay);
            _horizontalLayout?.SetBinding(StackBase.SpacingProperty, nameof(SfTabItem.ImageTextSpacing));
            _verticalLayout?.SetBinding(StackBase.SpacingProperty, nameof(SfTabItem.ImageTextSpacing));
        }

        void SetHeaderBinding()
        {
            if (_header != null)
            {
                _header.SetBinding(SfLabel.TextProperty, nameof(SfTabItem.Header), BindingMode.OneWay);
                _header.SetBinding(SfLabel.TextColorProperty, nameof(SfTabItem.TextColor));
                _header.SetBinding(SfLabel.FontSizeProperty, nameof(SfTabItem.FontSize));
                _header.SetBinding(SfLabel.FontAttributesProperty, nameof(SfTabItem.FontAttributes));
                _header.SetBinding(SfLabel.FontFamilyProperty, nameof(SfTabItem.FontFamily));
                _header.SetBinding(SfLabel.FontAutoScalingEnabledProperty, nameof(SfTabItem.FontAutoScalingEnabled));
            }
        }

        /// <summary>
        /// Updates the layout in response to a change in the image position.
        /// </summary>
        void UpdateImagePosition()
        {
            UpdateLayout();
        }

        /// <summary>
        /// Updates the layout in response to a change in the header display mode.
        /// </summary>
        void UpdateHeaderDisplayMode()
        {
            UpdateLayout();
        }

        /// <summary>
        /// Updates the horizontal options of the control based on the current <see cref="SfTabItem"/>.
        /// </summary>
        void UpdateHorizontalOptions(SfTabItem item)
        {
            if (_horizontalLayout != null && _image != null)
            {
                var options = GetLayoutOptions(item.HeaderHorizontalTextAlignment);
                _horizontalLayout.HorizontalOptions = options;
                _image.HorizontalOptions = options;
            }
        }

        LayoutOptions GetLayoutOptions(TextAlignment alignment)
        {
            switch (alignment)
            {
                case TextAlignment.Center:
                    return LayoutOptions.Center;
                case TextAlignment.End:
                    return LayoutOptions.End;
                default:
                    return LayoutOptions.Start;
            }
        }

        /// <summary>
        /// Updates the horizontal options of the header label based on the current <see cref="SfTabItem"/>.
        /// </summary>
        void UpdateHeaderHorizontalOptions(SfTabItem item)
        {
            if (_header != null)
            {
                if (item.HeaderHorizontalTextAlignment == TextAlignment.Start)
                {
                    _header.HorizontalOptions = LayoutOptions.Start;
                }
                else if (item.HeaderHorizontalTextAlignment == TextAlignment.Center)
                {
                    _header.HorizontalOptions = LayoutOptions.Center;
                }
                else if (item.HeaderHorizontalTextAlignment == TextAlignment.End)
                {
                    _header.HorizontalOptions = LayoutOptions.End;
                }
            }
        }

        #endregion
    }
}