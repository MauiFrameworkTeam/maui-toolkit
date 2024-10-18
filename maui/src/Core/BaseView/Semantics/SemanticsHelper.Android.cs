﻿namespace Syncfusion.Maui.Toolkit.Graphics.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Maui.Platform;
    using Android.Views.Accessibility;
    using AndroidX.Core.View.Accessibility;
    using Android.OS;
    using Microsoft.Maui.Controls;
    using Syncfusion.Maui.Toolkit.Semantics;
    using AndroidX.Core.View;
    using Rectangle = Android.Graphics.Rect;

    internal class CustomAccessibilityDelegate : AccessibilityDelegateCompat
    {
        private CustomAccessibilityProvider accessibilityNodeProvider;

        internal CustomAccessibilityDelegate(Android.Views.View host, View virtualView, bool isChildrenImportant)
        {
            this.accessibilityNodeProvider = new CustomAccessibilityProvider(host, virtualView, isChildrenImportant);
        }
        public override AccessibilityNodeProviderCompat GetAccessibilityNodeProvider(Android.Views.View host)
        {
            return this.accessibilityNodeProvider;
        }

        /// <summary>
        /// Check the hovering triggered for the drawn UI.
        /// </summary>
        internal bool DispatchHoverEvent(Android.Views.MotionEvent? e)
        {
            if (e == null)
            {
                return false;
            }

            return this.accessibilityNodeProvider.DispatchHoverEvent(e);
        }

        /// <summary>
        /// Update the drawing order(like above children or below children).
        /// </summary>
        internal void UpdateChildOrder(bool isChildrenImportant)
        {
            this.accessibilityNodeProvider.UpdateChildOrder(isChildrenImportant);
        }

        /// <summary>
        /// Clear and create the children of accessibility node.
        /// </summary>
        internal void InvalidateSemantics()
        {
            this.accessibilityNodeProvider.InvalidateSemantics();
        }

        /// <summary>
        /// Dispatch the accessibility event to process its children for adding their text for the event.
        /// </summary>
        public override bool DispatchPopulateAccessibilityEvent(Android.Views.View host, AccessibilityEvent e)
        {
            //// Check the host view have accessibility.
            if (host.IsImportantForAccessibility)
            {
                bool handled = base.DispatchPopulateAccessibilityEvent(host, e);
                if (handled)
                {
                    return true;
                }
            }

            Android.Views.ViewGroup? viewGroup = (Android.Views.ViewGroup?)host;
            //// Check the children handles the accessibility.
            if (viewGroup != null)
            {
                for (int i = 0; i < viewGroup.ChildCount; i++)
                {
                    Android.Views.View? child = viewGroup.GetChildAt(i);
                    if (child != null && child.Visibility == Android.Views.ViewStates.Visible)
                    {
                        if (child.DispatchPopulateAccessibilityEvent(e))
                        {
                            return true;
                        }
                    }
                }
            }

            return base.DispatchPopulateAccessibilityEvent(host, e);
        }
    }

    /// <summary>
    /// Accessibility provider handles to create and manage the accessibility node and its children.
    /// </summary>
    internal class CustomAccessibilityProvider : AccessibilityNodeProviderCompat
    {
        /// <summary>
        /// The id value denotes the android view .
        /// </summary>
        const int Root_ID = Android.Views.View.NoId;

        /// <summary>
        /// The id value denotes the accessibility node between the android view and its children.
        /// It holds the drawn UI nodes as children.
        /// host node is needed because provider cannot differentiate the root node and its children(not drawn view)
        /// All android view(root and its children) id is -1.
        /// </summary>
        const int Host_ID = 2222;

        /// <summary>
        /// Current focused(highlighted) node id.
        /// </summary>
        private int focusedVirtualID = Root_ID;

        /// <summary>
        /// Hovered accessibility node id.
        /// </summary>
        private int hoveredVirtualID = Root_ID;

        /// <summary>
        /// Native android view.
        /// </summary>
        private Android.Views.View host;

        /// <summary>
        /// Maui view instance.
        /// </summary>
        private View virtualView;

        /// <summary>
        /// Children is important means the children gets the accessibility event
        /// while the drawn UI and children are overlapped.
        /// </summary>
        private bool isChildNode;

        internal CustomAccessibilityProvider(Android.Views.View host, View virtualView, bool isChildNode)
        {
            this.host = host;
            this.virtualView = virtualView;
            this.isChildNode = isChildNode;
        }

        /// <summary>
        /// Invalidate while the child drawing order changed.
        /// </summary>
        /// <param name="isChildNode">Is children important</param>
        internal void UpdateChildOrder(bool isChildNode)
        {
            if (this.isChildNode == isChildNode)
            {
                return;
            }

            this.isChildNode = isChildNode;
            this.InvalidateSemantics();
        }

        /// <summary>
        /// Clear and create the children of accessibility node.
        /// </summary>
        internal void InvalidateSemantics()
        {
            this.SendEventForVirtualViewId(Host_ID, EventTypes.WindowContentChanged, null);
        }

        /// <summary>
        /// Create the accessibility node of the virtual id.
        /// </summary>
        /// <param name="virtualViewId">View id value.</param>
        /// <returns>Accessibility node.</returns>
        public override AccessibilityNodeInfoCompat? CreateAccessibilityNodeInfo(int virtualViewId)
        {
            //// Create the accessibility node for the root view(equivalent to android view).
            if (virtualViewId == Root_ID)
            {
                //// Since we don't want the parent to be focusable, but we can't remove
                //// actions from a node, copy over the necessary fields.
                AccessibilityNodeInfoCompat? result = AccessibilityNodeInfoCompat.Obtain(this.host);
                AccessibilityNodeInfoCompat? source = AccessibilityNodeInfoCompat.Obtain(this.host);
                //// If the result and source return null then the host view is not valid.
                if (result == null || source == null)
                {
                    return null;
                }

                ViewCompat.OnInitializeAccessibilityNodeInfo(this.host, source);


                Rectangle parentRect = new Rectangle();
                Rectangle screenRect = new Rectangle();
                //// Copy over parent and screen bounds.
#pragma warning disable CS0618 // Type or member is obsolete
                source.GetBoundsInParent(parentRect);
                source.GetBoundsInScreen(screenRect);
                result.SetBoundsInParent(parentRect);
                result.SetBoundsInScreen(screenRect);
#pragma warning restore CS0618 // Type or member is obsolete

                Android.Views.IViewParent? parent = ViewCompat.GetParentForAccessibility(this.host);
                if (parent != null && parent is Android.Views.View)
                {
                    result.SetParent((Android.Views.View)parent);
                }

                result.VisibleToUser = source.VisibleToUser;
                result.PackageName = source.PackageName;
                result.ClassName = source.ClassName;
                //// Add the root layout with ID.
                result.AddChild(this.host, Host_ID);
                return result;
            }
            else if (virtualViewId == Host_ID)
            {
                //// The host node is identical to the root node, except that it is a
                //// child of the root view and is populated with virtual descendants.
                AccessibilityNodeInfoCompat? node = AccessibilityNodeInfoCompat.Obtain(this.host, virtualViewId);
                //// If the node return null then the host view is not valid.
                if (node == null)
                {
                    return null;
                }

                ViewCompat.OnInitializeAccessibilityNodeInfo(this.host, node);
                List<SemanticsNode>? semanticsNodes = null;
                if (this.virtualView is ISemanticsProvider)
                {
                    semanticsNodes = ((ISemanticsProvider)this.virtualView).GetSemanticsNodes(this.virtualView.Width, this.virtualView.Height);
                }

                List<Android.Views.View> children = new List<Android.Views.View>();
                this.AddAccessibilityChildren(host, children);

                //// Children is important means the children gets the accessibility event
                //// while the drawn UI and children are overlapped.
                if (this.isChildNode)
                {
                    foreach (var child in children)
                    {
                        node.AddChild(child);
                    }
                }

                if (semanticsNodes != null)
                {
                    foreach (SemanticsNode info in semanticsNodes)
                    {
                        node.AddChild(this.host, info.Id);
                    }
                }

                //// Children is not important means the drawn UI gets the accessibility event
                //// while the drawn UI and children are overlapped.
                if (!this.isChildNode)
                {
                    foreach (var child in children)
                    {
                        node.AddChild(child);
                    }
                }

                node.SetParent(this.host);
                node.SetSource(this.host, Host_ID);

                return node;
            }
            else
            {
                List<SemanticsNode>? semanticsNodes = null;
                if (this.virtualView is ISemanticsProvider)
                {
                    semanticsNodes = ((ISemanticsProvider)this.virtualView).GetSemanticsNodes(this.virtualView.Width, this.virtualView.Height);
                }

                SemanticsNode? semanticsNode = null;
                if (semanticsNodes != null)
                {
                    semanticsNode = semanticsNodes.FirstOrDefault(info => info.Id == virtualViewId);
                }

                if (semanticsNode != null)
                {
                    AccessibilityNodeInfoCompat? node = AccessibilityNodeInfoCompat.Obtain();
                    //// If the node return null then the host view is not valid.
                    if (node == null)
                    {
                        return null;
                    }

                    node.Enabled = true;
                    node.ClassName = this.host.Class.Name;

                    node.ContentDescription = semanticsNode.Text;

                    Func<double, float> toPixels = Android.App.Application.Context.ToPixels;
                    //// Parent bounds consider the right and bottom position from rectangle.
                    Rectangle rect = new Rectangle();
                    rect.Left = (int)toPixels(semanticsNode.Bounds.X);
                    rect.Top = (int)toPixels(semanticsNode.Bounds.Y);
                    rect.Right = (int)toPixels(semanticsNode.Bounds.X + semanticsNode.Bounds.Width);
                    rect.Bottom = (int)toPixels(semanticsNode.Bounds.Y + semanticsNode.Bounds.Height);

#pragma warning disable CS0618 // Type or member is obsolete
                    node.SetBoundsInParent(rect);
#pragma warning restore CS0618 // Type or member is obsolete
                    //// Add click action only for touch enabled semantics node.
                    if (semanticsNode.IsTouchEnabled)
                    {
                        node.AddAction(AccessibilityNodeInfoCompat.AccessibilityActionCompat.ActionClick);
                    }

                    node.PackageName = this.host.Context?.PackageName;
                    node.SetParent(this.host, Host_ID);
                    node.SetSource(this.host, virtualViewId);
                    //// add accessibility focus clear action for focused node.
                    if (focusedVirtualID == virtualViewId)
                    {
                        node.AccessibilityFocused = true;
                        node.AddAction(AccessibilityNodeInfoCompat.AccessibilityActionCompat.ActionClearAccessibilityFocus);
                    }
                    else
                    {
                        //// add accessibility focus action for non focused node.
                        node.AccessibilityFocused = false;
                        node.AddAction(AccessibilityNodeInfoCompat.AccessibilityActionCompat.ActionAccessibilityFocus);
                    }

                    Rectangle tempRect = new Rectangle();

#pragma warning disable CS0618 // Type or member is obsolete
                    node.GetBoundsInParent(tempRect);

                    if (this.IntersectVisibleToUser(tempRect))
                    {
                        node.VisibleToUser = true;
                        node.SetBoundsInParent(tempRect);
                    }
#pragma warning restore CS0618 // Type or member is obsolete

                    node.VisibleToUser = true;
                    node.Focusable = true;
                    int[] position = new int[2];
                    //// Calculate screen-relative bound.
                    this.host.GetLocationOnScreen(position);
                    Rectangle screenBounds = new Rectangle();
                    screenBounds.Set(tempRect);
                    screenBounds.Offset(position[0], position[1]);

                    node.SetBoundsInScreen(screenBounds);
                    return node;
                }

                return base.CreateAccessibilityNodeInfo(virtualViewId);
            }
        }

        private bool IntersectVisibleToUser(Rectangle localRect)
        {
            // Missing or empty bounds mean this view is not visible.
            if (localRect.IsEmpty)
            {
                return false;
            }

            // Attached to invisible window means this view is not visible.
            if (this.host.WindowVisibility != Android.Views.ViewStates.Visible)
            {
                return false;
            }

            Rectangle rect = new Rectangle();
            // If no portion of the parent is visible, this view is not visible.
            if (!this.host.GetLocalVisibleRect(rect))
            {
                return false;
            }

            // Check if the view intersects the visible portion of the parent.
            return localRect.Intersect(rect);
        }

        /// <summary>
        /// Check the native view children have hovering.
        /// </summary>
        /// <param name="p0">x position value.</param>
        /// <param name="p1">y position value</param>
        /// <param name="view">Android view</param>
        /// <returns>the view have hovering point.</returns>
        private bool HandleChildHover(float p0, float p1, Android.Views.View view)
        {
            Android.Views.ViewGroup? viewGroup = (Android.Views.ViewGroup?)view;
            if (viewGroup == null)
            {
                return false;
            }

            for (int i = 0; i < viewGroup.ChildCount; i++)
            {
                Android.Views.View? child = viewGroup.GetChildAt(i);
                if (child == null || child.Visibility != Android.Views.ViewStates.Visible)
                {
                    continue;
                }

                if (child.IsImportantForAccessibility)
                {
                    int[] position = new int[2];
                    //// Calculate screen-relative bound.
                    child.GetLocationOnScreen(position);
                    Rectangle rect = new Rectangle(position[0], position[1], position[0] + child.Width, position[1] + child.Height);
                    if (rect.Contains((int)p0, (int)p1))
                    {
                        return true;
                    }
                }
                else if (child is Android.Views.ViewGroup && this.HandleChildHover(p0, p1, child))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Handle the hovering children.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        internal bool DispatchHoverEvent(Android.Views.MotionEvent e)
        {
            if (this.HandleChildHover(e.GetX(), e.GetY(), this.host))
            {
                return false;
            }

            int virtualViewId = this.GetVirtualViewAt(e.GetX(), e.GetY());

            switch (e.Action)
            {
                case Android.Views.MotionEventActions.HoverEnter:
                case Android.Views.MotionEventActions.HoverMove:
                    UpdateHoveredVirtualViewId(virtualViewId);
                    return virtualViewId != Root_ID;
                case Android.Views.MotionEventActions.HoverExit:
                    if (hoveredVirtualID != Root_ID)
                    {
                        UpdateHoveredVirtualViewId(Root_ID);
                        return true;
                    }
                    return false;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Return the virtual id for the virtual view placed on the point.
        /// </summary>
        /// <param name="p0">x position.</param>
        /// <param name="p1">Y position.</param>
        /// <returns>Virtual view id.</returns>
        private int GetVirtualViewAt(float p0, float p1)
        {
            List<SemanticsNode>? semanticsNodes = null;
            if (this.virtualView is ISemanticsProvider)
            {
                semanticsNodes = ((ISemanticsProvider)this.virtualView).GetSemanticsNodes(this.virtualView.Width, this.virtualView.Height);
            }

            if (semanticsNodes != null)
            {
                for (int i = 0; i < semanticsNodes.Count; i++)
                {
                    SemanticsNode semanticsNode = semanticsNodes[i];
                    Func<double, float> toPixels = Android.App.Application.Context.ToPixels;
                    Rectangle rect = new Rectangle();
                    rect.Left = (int)toPixels(semanticsNode.Bounds.X);
                    rect.Top = (int)toPixels(semanticsNode.Bounds.Y);
                    rect.Right = (int)toPixels(semanticsNode.Bounds.X + semanticsNode.Bounds.Width);
                    rect.Bottom = (int)toPixels(semanticsNode.Bounds.Y + semanticsNode.Bounds.Height);
                    if (rect.Contains((int)p0, (int)p1))
                    {
                        return semanticsNode.Id;
                    }
                }
            }

            return Root_ID;
        }

        /// <summary>
        /// Update and trigger hover event for virtual view id.
        /// </summary>
        /// <param name="virtualViewId">Virtual view id value.</param>
        private void UpdateHoveredVirtualViewId(int virtualViewId)
        {
            if (this.hoveredVirtualID == virtualViewId)
            {
                return;
            }

            int previousVirtualViewId = this.hoveredVirtualID;
            this.hoveredVirtualID = virtualViewId;
            List<SemanticsNode>? semanticsNodes = null;
            if (this.virtualView is ISemanticsProvider)
            {
                semanticsNodes = ((ISemanticsProvider)this.virtualView).GetSemanticsNodes(this.virtualView.Width, this.virtualView.Height);
            }

            if (virtualViewId != Root_ID)
            {
                SemanticsNode? semanticsNode = semanticsNodes?.FirstOrDefault(info => info.Id == virtualViewId);
                this.SendEventForVirtualViewId(virtualViewId, EventTypes.ViewHoverEnter, semanticsNode);
            }

            if (previousVirtualViewId != Root_ID)
            {
                SemanticsNode? previousNode = semanticsNodes?.FirstOrDefault(info => info.Id == previousVirtualViewId);
                this.SendEventForVirtualViewId(previousVirtualViewId, EventTypes.ViewHoverExit, previousNode);
            }
        }

        /// <summary>
        /// Add accessibility for children of the android view.
        /// </summary>
        /// <param name="host"></param>
        /// <param name="children"></param>
        private void AddAccessibilityChildren(Android.Views.View host, List<Android.Views.View> children)
        {
            Android.Views.ViewGroup? viewGroup = (Android.Views.ViewGroup?)host;
            if (viewGroup == null)
            {
                return;
            }

            for (int i = 0; i < viewGroup.ChildCount; i++)
            {
                Android.Views.View? child = viewGroup.GetChildAt(i);
                if (child == null || child.Visibility != Android.Views.ViewStates.Visible)
                {
                    continue;
                }

                if (child.IsImportantForAccessibility)
                {
                    children.Add(child);
                }
                else if (child is Android.Views.ViewGroup)
                {
                    this.AddAccessibilityChildren(child, children);
                }
            }
        }

        /// <summary>
        /// Performs the specified accessibility action on the view
        /// </summary>
        /// <param name="virtualViewId">Virtual view id value.</param>
        /// <param name="action">Action performed.</param>
        /// <param name="arguments">Action arguments.</param>
        /// <returns></returns>
        public override bool PerformAction(int virtualViewId, int action, Bundle? arguments)
        {
            List<SemanticsNode>? semanticsNodes = null;
            if (this.virtualView is ISemanticsProvider)
            {
                semanticsNodes = ((ISemanticsProvider)this.virtualView).GetSemanticsNodes(this.virtualView.Width, this.virtualView.Height);
            }

            SemanticsNode? semanticsNode = semanticsNodes?.FirstOrDefault(info => info.Id == virtualViewId);
            if (semanticsNode != null || virtualViewId == Host_ID)
            {
                switch (action)
                {
                    case AccessibilityNodeInfoCompat.ActionAccessibilityFocus:
                        {
                            bool handled = false;
                            if (focusedVirtualID != virtualViewId)
                            {
                                focusedVirtualID = virtualViewId;
                                this.host.Invalidate();
                                SendEventForVirtualViewId(virtualViewId, EventTypes.ViewAccessibilityFocused, semanticsNode);
                                handled = true;
                            }

                            return handled;
                        }
                    case AccessibilityNodeInfoCompat.ActionClearAccessibilityFocus:
                        {
                            if (focusedVirtualID == virtualViewId)
                            {
                                focusedVirtualID = Root_ID;
                            }

                            this.host.Invalidate();
                            SendEventForVirtualViewId(virtualViewId, EventTypes.ViewAccessibilityFocusCleared, semanticsNode);

                            return true;
                        }
                    default:
                        {
                            if ((action == AccessibilityNodeInfoCompat.ActionClick && semanticsNode != null && semanticsNode.IsTouchEnabled) || virtualViewId == Host_ID)
                            {
                                return ViewCompat.PerformAccessibilityAction(this.host, action, arguments);
                            }
                            else if (virtualViewId != Host_ID)
                            {
                                return false;
                            }

                            return base.PerformAction(virtualViewId, action, arguments);
                        }
                }
            }

            return base.PerformAction(virtualViewId, action, arguments);
        }

        /// <summary>
        ///  Populates an event of the specified type with information about an item 
        ///  and attempts to send it up through the view hierarchy.
        ///  Should call this method after performing a user action that normally fires an accessibility event
        /// </summary>
        /// <param name="virtualViewId"></param>
        /// <param name="eventType"></param>
        /// <param name="semanticsNode"></param>
        /// <returns></returns>
        internal void SendEventForVirtualViewId(int virtualViewId, EventTypes eventType, SemanticsNode? semanticsNode)
        {
            if (virtualViewId == Root_ID)
            {
                return;
            }

            Android.Views.ViewGroup? parent = (Android.Views.ViewGroup?)this.host.Parent;
            if (parent == null)
            {
                return;
            }

            AccessibilityEvent? accessibilityEvent;
            if (virtualViewId == Host_ID)
            {
                accessibilityEvent = this.GetAccessibilityEvent(eventType);
                this.host.OnInitializeAccessibilityEvent(accessibilityEvent);
            }
            else
            {
                if (semanticsNode == null)
                {
                    return;
                }

                accessibilityEvent = this.GetAccessibilityEvent(eventType);
                if (accessibilityEvent != null)
                {
                    accessibilityEvent.Enabled = true;
                    accessibilityEvent.ClassName = this.host.Class.Name;
                    accessibilityEvent.ContentDescription = semanticsNode.Text;
                    accessibilityEvent.PackageName = this.host.Context?.PackageName;
                    accessibilityEvent.SetSource(this.host, virtualViewId);
                }
            }

            parent.RequestSendAccessibilityEvent(this.host, accessibilityEvent);
        }

        /// <summary>
        /// Create the accessibility event based on android API version.
        /// </summary>
        /// <param name="eventTypes">Event type.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
        private AccessibilityEvent? GetAccessibilityEvent(EventTypes eventTypes)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
            {
                AccessibilityEvent accessibilityEvent = new AccessibilityEvent();
                accessibilityEvent.EventType = eventTypes;
                return accessibilityEvent;
            }
            else
            {
#pragma warning disable CA1422 // Validate platform compatibility
                AccessibilityEvent? accessibilityEvent = AccessibilityEvent.Obtain();
#pragma warning restore CA1422 // Validate platform compatibility
                if (accessibilityEvent != null)
                {
                    accessibilityEvent.EventType = eventTypes;
                }

                return accessibilityEvent;
            }
        }
    }
}