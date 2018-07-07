﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

// Based on https://stackoverflow.com/a/45101665/3960200
// License: Attribution-ShareAlike 3.0 Unported https://creativecommons.org/licenses/by-sa/3.0/
// Enhanced to support scrollbars.

namespace EmergenceGuardian.WpfExtensions {
    [TemplatePart(Name = ZoomViewer.PART_Scroll, Type = typeof(ScrollViewer))]
    [TemplatePart(Name = ZoomViewer.PART_Content, Type = typeof(FrameworkElement))]
    public class ZoomViewer : ContentControl {

        #region Declarations / Constructor

        static ZoomViewer() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ZoomViewer), new FrameworkPropertyMetadata(typeof(ZoomViewer)));
            FocusableProperty.OverrideMetadata(typeof(ZoomViewer), new FrameworkPropertyMetadata(false));
        }

        public const string PART_Scroll = "PART_Scroll";
        public ScrollViewer PartScroll => GetTemplateChild(PART_Scroll) as ScrollViewer;
        public const string PART_Content = "PART_Content";
        public FrameworkElement PartContent => GetTemplateChild(PART_Content) as FrameworkElement;

        /// <summary>
        ///     The origin of the transform at the start of capture
        /// </summary>
        private Point origin;

        /// <summary>
        ///     The mouse position at the start of capture
        /// </summary>
        private Point start;

        /// <summary>
        /// Tracks changes to ActualWidth to update ZoomWidth.
        /// </summary>
        private PropertyChangeNotifier ActualWidthObserver;
        /// <summary>
        /// Tracks changes to ActualHeight to update ZoomHeight.
        /// </summary>
        private PropertyChangeNotifier ActualHeightObserver;

        #endregion

        public override void OnApplyTemplate() {
            base.OnApplyTemplate();
            ActualWidthObserver = new PropertyChangeNotifier(PartContent, FrameworkElement.ActualWidthProperty);
            ActualWidthObserver.ValueChanged += delegate { UpdateZoomWidth(); };
            UpdateZoomWidth();
            ActualHeightObserver = new PropertyChangeNotifier(PartContent, FrameworkElement.ActualHeightProperty);
            ActualHeightObserver.ValueChanged += delegate { UpdateZoomHeight(); };
            UpdateZoomHeight();
        }

        private void UpdateZoomWidth() => ZoomWidth = PartContent != null ? PartContent.ActualWidth * Zoom : 0.0;
        private void UpdateZoomHeight() => ZoomHeight = PartContent != null ? PartContent.ActualHeight * Zoom : 0.0;


        #region Properties

        // AllowPan (left click)
        public static readonly DependencyProperty AllowPanProperty = DependencyProperty.Register("AllowPan", typeof(bool), typeof(ZoomViewer),
            new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
        public bool AllowPan { get => (bool)GetValue(AllowPanProperty); set => SetValue(AllowPanProperty, value); }

        // AllowZoom (wheel)
        public static readonly DependencyProperty AllowZoomProperty = DependencyProperty.Register("AllowZoom", typeof(bool), typeof(ZoomViewer),
            new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
        public bool AllowZoom { get => (bool)GetValue(AllowZoomProperty); set => SetValue(AllowZoomProperty, value); }

        // AllowReset (right click)
        public static readonly DependencyProperty AllowResetProperty = DependencyProperty.Register("AllowReset", typeof(bool), typeof(ZoomViewer),
            new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
        public bool AllowReset { get => (bool)GetValue(AllowResetProperty); set => SetValue(AllowResetProperty, value); }

        // ScrollVerticalOffset
        public static readonly DependencyProperty ScrollVerticalOffsetProperty = DependencyProperty.Register("ScrollVerticalOffset", typeof(double), typeof(ZoomViewer),
            new FrameworkPropertyMetadata(-1.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
        public double ScrollVerticalOffset { get => (double)GetValue(ScrollVerticalOffsetProperty); set => SetValue(ScrollVerticalOffsetProperty, value); }
        // internal static object CoerceVerticalOffset(DependencyObject d, object baseValue) => ScrollViewerBinding.CoerceVerticalOffset((d as ZoomViewer).PartScroll, baseValue);

        // ScrollHorizontalOffset
        public static readonly DependencyProperty ScrollHorizontalOffsetProperty = DependencyProperty.Register("ScrollHorizontalOffset", typeof(double), typeof(ZoomViewer),
            new FrameworkPropertyMetadata(-1.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
        public double ScrollHorizontalOffset { get => (double)GetValue(ScrollHorizontalOffsetProperty); set => SetValue(ScrollHorizontalOffsetProperty, value); }
        // internal static object CoerceHorizontalOffset(DependencyObject d, object baseValue) => ScrollViewerBinding.CoerceHorizontalOffset((d as ZoomViewer).PartScroll, baseValue);

        // ZoomWidth
        public static readonly DependencyPropertyKey ZoomWidthPropertyKey = DependencyProperty.RegisterReadOnly("ZoomWidth", typeof(double), typeof(ZoomViewer),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
        public static readonly DependencyProperty ZoomWidthProperty = ZoomWidthPropertyKey.DependencyProperty;
        public double ZoomWidth { get => (double)GetValue(ZoomWidthProperty); protected set => SetValue(ZoomWidthPropertyKey, value); }

        // ZoomHeight
        public static readonly DependencyPropertyKey ZoomHeightPropertyKey = DependencyProperty.RegisterReadOnly("ZoomHeight", typeof(double), typeof(ZoomViewer),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
        public static readonly DependencyProperty ZoomHeightProperty = ZoomHeightPropertyKey.DependencyProperty;
        public double ZoomHeight { get => (double)GetValue(ZoomHeightProperty); protected set => SetValue(ZoomHeightPropertyKey, value); }

        // ZoomIncrement
        public static readonly DependencyProperty ZoomIncrementProperty = DependencyProperty.Register("ZoomIncrement", typeof(double), typeof(ZoomViewer),
            new FrameworkPropertyMetadata(1.2, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure), ValidateZoom);
        public double ZoomIncrement { get => (double)GetValue(ZoomIncrementProperty); set => SetValue(ZoomIncrementProperty, value); }

        // MinZoom
        public static readonly DependencyProperty MinZoomProperty = DependencyProperty.Register("MinZoom", typeof(double), typeof(ZoomViewer),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure), ValidateMinZoom);
        public double MinZoom { get => (double)GetValue(MinZoomProperty); set => SetValue(MinZoomProperty, value); }
        private static bool ValidateMinZoom(object value) => (double)value >= 0;

        // MaxZoom
        public static readonly DependencyProperty MaxZoomProperty = DependencyProperty.Register("MaxZoom", typeof(double), typeof(ZoomViewer),
            new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure), ValidateMinZoom);
        public double MaxZoom { get => (double)GetValue(MaxZoomProperty); set => SetValue(MaxZoomProperty, value); }

        // Zoom
        public static readonly DependencyProperty ZoomProperty = DependencyProperty.Register("Zoom", typeof(double), typeof(ZoomViewer),
            new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure, ZoomChanged, CoerceZoom), ValidateZoom);
        private static bool ValidateZoom(object value) => (double)value > 0;
        public double Zoom { get => (double)GetValue(ZoomProperty); set => SetValue(ZoomProperty, value); }
        private static void ZoomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            ZoomViewer P = d as ZoomViewer;
            P.UpdateZoomWidth();
            P.UpdateZoomHeight();

            // Trying to find right equation to zoom from center, or zoom from mouse position. Haven't found the formula yet.
            //double Width = P.PartContent.ActualWidth;
            //double Height = P.PartContent.ActualHeight;
            //double ZoomDiff = (double)e.NewValue / (double)e.OldValue;
            //double DiffX = Width * (double)e.NewValue - Width * (double)e.OldValue;
            //double DiffY = Height * (double)e.NewValue - Height * (double)e.OldValue;
            //double RatioX = P.PartScroll.ViewportWidth / P.ZoomWidth * P.PartScroll.ViewportWidth;
            //double RatioY = P.PartScroll.ViewportHeight / P.ZoomHeight * P.PartScroll.ViewportHeight;
            //P.ScrollHorizontalOffset = P.ScrollHorizontalOffset * ZoomDiff + RatioX;
            //P.ScrollVerticalOffset = P.ScrollVerticalOffset * ZoomDiff + RatioY;

            // Adjust scrollbars to maintain position.
            P.ScrollHorizontalOffset = P.ScrollHorizontalOffset / (double)e.OldValue * (double)e.NewValue;
            P.ScrollVerticalOffset = P.ScrollVerticalOffset / (double)e.OldValue * (double)e.NewValue;
        }
        private static object CoerceZoom(DependencyObject d, object baseValue) {
            ZoomViewer P = d as ZoomViewer;
            double Value = (double)baseValue;
            if (P.MinZoom > 0)
                Value = Math.Max(Value, P.MinZoom);
            if (P.MaxZoom > 0)
                Value = Math.Min(Value, P.MaxZoom);
            return Value;
        }

        #endregion


        #region Event Handlers

        protected override void OnMouseLeave(MouseEventArgs e) {
            base.OnMouseLeave(e);
            Cursor = Cursors.Arrow;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
            base.OnMouseLeftButtonDown(e);
            if (AllowPan) {
                start = e.GetPosition(this);
                origin = new Point(ScrollHorizontalOffset, ScrollVerticalOffset);
                Cursor = Cursors.Hand;
                CaptureMouse();
                e.Handled = true;
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
            base.OnMouseLeftButtonUp(e);
            if (AllowPan) {
                Cursor = Cursors.Arrow;
                ReleaseMouseCapture();

                // Clamp overflow panning here.
                ScrollViewerBinding.CoerceOffset(PartScroll);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);

            if (IsMouseCaptured && AllowPan) {
                var position = e.GetPosition(this);
                var Pos = start - position;
                ScrollHorizontalOffset = origin.X + Pos.X;
                ScrollVerticalOffset = origin.Y + Pos.Y;
            }
        }

        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e) {
            base.OnMouseLeftButtonDown(e);
            if (AllowReset)
                Reset();
        }

        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e) {
            base.OnPreviewMouseWheel(e);
            if (AllowZoom) {
                // Set new zoom, enforcing MinZoom/MaxZoom.
                double NewZoom = e.Delta > 0 ? Zoom * ZoomIncrement : Zoom / ZoomIncrement;
                Zoom = (double)CoerceZoom(this, NewZoom);

                e.Handled = true;
            }
        }

        public void Reset() {
            Zoom = 1;
            ScrollHorizontalOffset = 0;
            ScrollVerticalOffset = 0;
        }
        #endregion

    }
}