using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

// https://stackoverflow.com/a/2674291/3960200
// License: Attribution-ShareAlike 3.0 Unported https://creativecommons.org/licenses/by-sa/3.0/

namespace EmergenceGuardian.WpfExtensions {
    /// <summary>
    /// Automatically selects text when a TextBox receives focus.
    /// </summary>
    public class SelectTextOnFocus : DependencyObject {
        public static readonly DependencyProperty ActiveProperty = DependencyProperty.RegisterAttached(
            "Active",
            typeof(bool),
            typeof(SelectTextOnFocus),
            new PropertyMetadata(false, ActivePropertyChanged));

        private static void ActivePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d is TextBox) {
                TextBox textBox = d as TextBox;
                if ((e.NewValue as bool?).GetValueOrDefault(false)) {
                    textBox.GotKeyboardFocus += OnKeyboardFocusSelectText;
                    textBox.PreviewMouseLeftButtonDown += OnMouseLeftButtonDown;
                } else {
                    textBox.GotKeyboardFocus -= OnKeyboardFocusSelectText;
                    textBox.PreviewMouseLeftButtonDown -= OnMouseLeftButtonDown;
                }
            }
        }

        private static void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            DependencyObject dependencyObject = GetParentFromVisualTree(e.OriginalSource);

            if (dependencyObject == null) {
                return;
            }

            var textBox = (TextBox)dependencyObject;
            if (!textBox.IsKeyboardFocusWithin) {
                textBox.Focus();
                e.Handled = true;
            }
        }

        private static DependencyObject GetParentFromVisualTree(object source) {
            DependencyObject parent = source as UIElement;
            while (parent != null && !(parent is TextBox)) {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return parent;
        }

        private static void OnKeyboardFocusSelectText(object sender, KeyboardFocusChangedEventArgs e) {
            TextBox textBox = e.OriginalSource as TextBox;
            if (textBox != null) {
                textBox.SelectAll();
            }
        }

        [AttachedPropertyBrowsableForChildrenAttribute(IncludeDescendants = false)]
        [AttachedPropertyBrowsableForType(typeof(TextBox))]
        public static bool GetActive(DependencyObject @object) {
            return (bool)@object.GetValue(ActiveProperty);
        }

        public static void SetActive(DependencyObject @object, bool value) {
            @object.SetValue(ActiveProperty, value);
        }
    }
}