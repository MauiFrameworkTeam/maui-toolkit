﻿using Microsoft.Maui.Controls;
using Microsoft.Maui.Platform;
using Syncfusion.Maui.Toolkit.Internals;
using PointerEventArgs = Syncfusion.Maui.Toolkit.Internals.PointerEventArgs;
using UIKit;
using Syncfusion.Maui.Toolkit.Platform;
using System.Linq;
using System;

namespace Syncfusion.Maui.Toolkit.TabView;
internal partial class SfHorizontalContent
{
    #region Fields

    internal bool _canProcessTouch = true;
    bool _isTapGestureRemoved;
    UIPanGestureRecognizer? _panGesture;
    LayoutViewExt? _nativeView;

    #endregion

    #region Interface Methods

    /// <summary>
    /// This method is used to handle the tap gesture.
    ///  If `canProcessTouch` is `false`, it sets it to `true`.
    /// </summary>
    /// <param name="e">Tap event arguments</param>
    void ITapGestureListener.OnTap(TapEventArgs e)
    {
        if (!_canProcessTouch)
        {
            _canProcessTouch = true;
            return;
        }
    }

    /// <summary>
    /// Handles the gesture based on which child view is tapped.
    /// </summary>
    /// <param name="view">view.</param>
    void ITapGestureListener.ShouldHandleTap(object view)
    {
#if IOS || MACCATALYST
        UIKit.UIView? touchView = (view as UIKit.UITouch)?.View;
        _canProcessTouch = true;
        if (IsSpecialView(touchView))
        {
            _canProcessTouch = false;
        }
        else if (touchView is Microsoft.Maui.Platform.MauiImageView)
        {
            HandleMauiImageViewOnTap(view);
        }
        else
        {
            // Provide the touch to the list view(framework) by removing the TapGesture.
            if (touchView is not Syncfusion.Maui.Toolkit.Platform.LayoutViewExt &&
                touchView is not Syncfusion.Maui.Toolkit.Platform.NativePlatformGraphicsView)
            {
                HandleOtherViewsOnTap(touchView, view);
            }
        }
#endif
    }

    /// <summary>
    /// This method triggers on any touch interaction on <see cref="SfHorizontalContent"/>.
    /// </summary>
    /// <param name="e">The pointer event args.</param>
    void ITouchListener.OnTouch(PointerEventArgs e)
    {
        if (!_canProcessTouch)
        {
            return;
        }

        switch (e.Action)
        {
            case PointerActions.Pressed:
                OnHandleTouchInteraction(PointerActions.Pressed, e.TouchPoint);
                break;

            case PointerActions.Moved:
                OnHandleTouchInteraction(PointerActions.Moved, e.TouchPoint);
                break;

            case PointerActions.Released:
                if (_isTapGestureRemoved)
                {
                    this.AddGestureListener(this);
                    _isTapGestureRemoved = false;
                }
                OnHandleTouchInteraction(PointerActions.Released, e.TouchPoint);
                break;
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Check if the touch view is one of the special views we don't want to process.
    /// </summary>
    /// <param name="touchView">The touched view</param>
    /// <returns>True if it's a special view, otherwise false</returns>
    bool IsSpecialView(UIKit.UIView? touchView)
    {
        return touchView is Syncfusion.Maui.Toolkit.Platform.PlatformGraphicsViewExt ||
            touchView is Syncfusion.Maui.Toolkit.Carousel.PlatformCarousel ||
            touchView is Syncfusion.Maui.Toolkit.Carousel.PlatformCarouselItem ||
            touchView is Syncfusion.Maui.Toolkit.Platform.NativePlatformGraphicsView;
    }

    void HandleMauiImageViewOnTap(object view)
    {
        if (view is UIKit.UITouch uiTouch && 
            uiTouch?.GestureRecognizers != null && 
            uiTouch.GestureRecognizers.Length >= 2)
        {
            for (int i = 0; i < uiTouch.GestureRecognizers.Length - 1; i++)
            {
                if (uiTouch.GestureRecognizers[i] is UIPanGestureRecognizer)
                {
                    _canProcessTouch = false;
                    return;
                }
            }
        }
    }

    void HandleOtherViewsOnTap(UIKit.UIView? touchView, object view)
    {
        // Disable touch processing for specific input views.
        if (touchView is MauiTextField || touchView is MauiTextView || touchView is UIKit.UITextField)
        {
            _canProcessTouch = false;
        }
        else if (view is UIKit.UITouch uiTouch)
        {
            if (uiTouch != null && uiTouch.GestureRecognizers != null)
            {
                foreach (var gesture in uiTouch.GestureRecognizers)
                {
                    if (gesture is UILongPressGestureRecognizer)
                    {
                        this.RemoveGestureListener(this);
                        _isTapGestureRemoved = true;
                    }
                }
            }
        }
    }

    /// <summary>
    /// This method is used to initialize the gesture.
    /// </summary>
    void InitializeGesture()
    {
        if (Handler != null && Handler.PlatformView != null)
        {
            _nativeView = Handler.PlatformView as LayoutViewExt;
            if (_nativeView != null && 
                _nativeView.GestureRecognizers != null && 
                _nativeView.GestureRecognizers.Length > 0)
            {
                _panGesture = _nativeView.GestureRecognizers.FirstOrDefault(x => x is UIPanGestureRecognizer) as UIPanGestureRecognizer;
                if (_panGesture != null)
                {
                    _panGesture.ShouldBegin += _proxy.GestureShouldBegin;
                }
            }
        }
        else
        {
            Dispose();
        }
    }

    /// <summary>
    /// Raises when <see cref="_panGesture"/> begins.
    /// </summary>
    /// <param name="uIGestureRecognizer">Instance of <see cref="_panGesture"/>.</param>
    bool GestureShouldBegin(UIGestureRecognizer uIGestureRecognizer)
    {
        if (!_canProcessTouch)
        {
            // Return false if the CanProcessTouch value is false, preventing the control's gesture from starting.
            return false;
        }

        return true;
    }

    /// <summary>
    /// Unwires wired events and disposes used objects.
    /// </summary>
    void Dispose()
    {
        if (_panGesture != null)
        {
            _panGesture.ShouldBegin -= _proxy.GestureShouldBegin;
        }

        _nativeView = null;
        _panGesture = null;
    }

    void ConfigureTouch()
    {
        InitializeGesture();
    }
    #endregion

    #region Override Methods

    /// <summary>
    /// Raises on handler changing event to dispose old resources.
    /// </summary>
    /// <param name="args">Relevant <see cref="HandlerChangingEventArgs"/>.</param>
    protected override void OnHandlerChanging(HandlerChangingEventArgs args)
    {
        if (args.OldHandler != null)
        {
            Dispose();
        }

        base.OnHandlerChanging(args);
    }

    #endregion
}

#if IOS || MACCATALYST

internal class SfHorizontalContentProxy
{
	readonly WeakReference<SfHorizontalContent> _view;
	public SfHorizontalContentProxy(SfHorizontalContent horizontalContent) => _view = new(horizontalContent);

	internal bool GestureShouldBegin(UIGestureRecognizer uIGestureRecognizer)
	{
		_view.TryGetTarget(out var view);
		if (view != null)
		{
			bool? isPressed = view?._canProcessTouch;
			if (isPressed == false)
			{
				// Return false if the CanProcessTouch value is false, preventing the control's gesture from starting.
				return false;
			}
		}

		return true;
	}

}

#endif