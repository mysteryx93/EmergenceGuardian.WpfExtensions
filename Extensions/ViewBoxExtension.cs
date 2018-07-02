using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LinqToVisualTree;

// https://stackoverflow.com/a/40069088/3960200
// License: Attribution-ShareAlike 3.0 Unported https://creativecommons.org/licenses/by-sa/3.0/

namespace EmergenceGuardian.WpfExtensions {
    /// <summary>
    /// Adds a MaxZoomFactor attached property to the Viewbox.
    /// </summary>
	public static class ViewboxExtensions {
		public static readonly DependencyProperty MaxZoomFactorProperty =
			DependencyProperty.RegisterAttached("MaxZoomFactor", typeof(double), typeof(ViewboxExtensions), new PropertyMetadata(1.0, OnMaxZoomFactorChanged));

		private static void OnMaxZoomFactorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (!(d is Viewbox Obj) || e.NewValue is bool == false)
                return;

            if ((bool)e.NewValue)
                Attach(Obj);
            else
                Detach(Obj);
        }

        private static void Attach(Viewbox viewbox) {
            if (!(viewbox?.Child is FrameworkElement child))
                return;

            child.SizeChanged += Child_SizeChanged;
			CalculateMaxSize(viewbox);
		}

        private static void Detach(Viewbox viewbox) {
            if (!(viewbox?.Child is FrameworkElement child))
                return;

            child.SizeChanged -= Child_SizeChanged;
        }

        private static void Child_SizeChanged(object sender, SizeChangedEventArgs e) {
            Viewbox Parent = (sender as DependencyObject).Ancestors<Viewbox>().FirstOrDefault() as Viewbox;
            CalculateMaxSize(Parent);
        }

        private static void CalculateMaxSize(Viewbox viewbox) {
            if (!(viewbox.Child is FrameworkElement child))
                return;
            viewbox.MaxWidth = child.ActualWidth * GetMaxZoomFactor(viewbox);
			viewbox.MaxHeight = child.ActualHeight * GetMaxZoomFactor(viewbox);
		}

		public static void SetMaxZoomFactor(DependencyObject d, double value) {
			d.SetValue(MaxZoomFactorProperty, value);
		}

		public static double GetMaxZoomFactor(DependencyObject d) {
			return (double)d.GetValue(MaxZoomFactorProperty);
		}
	}
}
