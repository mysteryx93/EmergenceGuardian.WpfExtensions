using System.Windows;
using System.Windows.Controls;

// https://stackoverflow.com/a/31272370/3960200
// License: Attribution-ShareAlike 3.0 Unported https://creativecommons.org/licenses/by-sa/3.0/
// Did some code layout refactoring.

namespace EmergenceGuardian.WpfExtensions {
    /// <summary>
    /// Allows setting focus via binding.
    /// </summary>
    public static class FocusExtension {
        // IsFocused
        public static readonly DependencyProperty IsFocusedProperty = DependencyProperty.RegisterAttached( "IsFocused", typeof(bool), 
            typeof(FocusExtension), new UIPropertyMetadata(false, OnIsFocusedPropertyChanged));
        public static bool GetIsFocused(DependencyObject obj) => (bool)obj.GetValue(IsFocusedProperty);
        public static void SetIsFocused(DependencyObject obj, bool value) => obj.SetValue(IsFocusedProperty, value);
        private static void OnIsFocusedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d is Control P) {
                if ((bool)e.NewValue) {
                    // To set false value to get focus on control. if we don't set value to False then we have to set all binding
                    //property to first False then True to set focus on control.
                    SetIsFocused(P, false);
                    P.Focus(); // Don't care about false values.
                }
            }
        }
    }
}
