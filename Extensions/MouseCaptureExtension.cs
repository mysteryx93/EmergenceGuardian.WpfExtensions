using System.Windows;
using System.Windows.Controls;

// Author: Emergence Guardian
// License: Attribution-ShareAlike 3.0 Unported https://creativecommons.org/licenses/by-sa/3.0/

namespace EmergenceGuardian.WpfExtensions {
    /// <summary>
    /// Allows setting and releasing mouse capture.
    /// </summary>
    public static class MouseCaptureExtension {
        // HasCapture
        public static readonly DependencyProperty CaptureProperty = DependencyProperty.RegisterAttached("Capture", typeof(MouseCaptureState), 
            typeof(MouseCaptureExtension),  new UIPropertyMetadata(MouseCaptureState.None, OnCaptureChanged));
        public static MouseCaptureState GetCapture(DependencyObject obj) => (MouseCaptureState)obj.GetValue(CaptureProperty);
        public static void SetCapture(DependencyObject obj, MouseCaptureState value) => obj.SetValue(CaptureProperty, value);
        private static void OnCaptureChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d is Control P) {
                switch ((MouseCaptureState)e.NewValue) {
                    case MouseCaptureState.Mouse:
                        P.CaptureMouse();
                        P.LostFocus += Control_LostFocus;
                        break;
                    case MouseCaptureState.Stylus:
                        P.CaptureStylus();
                        P.LostFocus += Control_LostFocus;
                        break;
                    case MouseCaptureState.None:
                        if ((MouseCaptureState)e.OldValue == MouseCaptureState.Mouse)
                            P.ReleaseMouseCapture();
                        else 
                            P.ReleaseStylusCapture();
                        P.LostFocus -= Control_LostFocus;
                        break;
                }
            }
        }

        private static void Control_LostFocus(object sender, RoutedEventArgs e) {
            SetCapture(sender as Control, MouseCaptureState.None);
        }
    }

    public enum MouseCaptureState {
        None,
        Mouse,
        Stylus
    }
}
