using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

// ReSharper disable CheckNamespace

namespace Walterlv.Themes
{
    internal class UniversalWindowStyle
    {
        #region TitleBar
        public static readonly DependencyProperty TitleBarProperty = DependencyProperty.RegisterAttached(
            "TitleBar", typeof(UniversalTitleBar), typeof(UniversalWindowStyle),
            new PropertyMetadata(new UniversalTitleBar(), OnTitleBarChanged));

        public static UniversalTitleBar GetTitleBar(DependencyObject element)
            => (UniversalTitleBar)element.GetValue(TitleBarProperty);

        public static void SetTitleBar(DependencyObject element, UniversalTitleBar value)
            => element.SetValue(TitleBarProperty, value);
        #endregion
        #region FrameColor
        public static SolidColorBrush GetFrameColor(DependencyObject obj)
        {
            return (SolidColorBrush)obj.GetValue(FrameColorProperty);
        }

        public static void SetFrameColor(DependencyObject obj, SolidColorBrush value)
        {
            obj.SetValue(FrameColorProperty, value);
        }

        // Using a DependencyProperty as the backing store for FrameColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FrameColorProperty =
            DependencyProperty.RegisterAttached("FrameColor", typeof(SolidColorBrush), typeof(UniversalWindowStyle), new PropertyMetadata(new SolidColorBrush(Colors.DimGray)));
        #endregion

        #region InactiveFrameColor


        public static SolidColorBrush GetInactiveFrameColor(DependencyObject obj)
        {
            return (SolidColorBrush)obj.GetValue(InactiveFrameColorProperty);
        }

        public static void SetInactiveFrameColor(DependencyObject obj, SolidColorBrush value)
        {
            obj.SetValue(InactiveFrameColorProperty, value);
        }

        public static readonly DependencyProperty InactiveFrameColorProperty =
            DependencyProperty.RegisterAttached("InactiveFrameColor", typeof(SolidColorBrush), typeof(UniversalWindowStyle), new PropertyMetadata(new SolidColorBrush(SystemParameters.WindowGlassColor)));


        #endregion

        #region TitleBarButtonState
        public static readonly DependencyProperty TitleBarButtonStateProperty = DependencyProperty.RegisterAttached(
            "TitleBarButtonState", typeof(WindowState?), typeof(UniversalWindowStyle),
            new PropertyMetadata(null, OnButtonStateChanged));

        public static WindowState? GetTitleBarButtonState(DependencyObject element)
            => (WindowState?)element.GetValue(TitleBarButtonStateProperty);

        public static void SetTitleBarButtonState(DependencyObject element, WindowState? value)
            => element.SetValue(TitleBarButtonStateProperty, value);
        #endregion
        #region IsTitleBarCloseButton
        public static readonly DependencyProperty IsTitleBarCloseButtonProperty = DependencyProperty.RegisterAttached(
            "IsTitleBarCloseButton", typeof(bool), typeof(UniversalWindowStyle),
            new PropertyMetadata(false, OnIsCloseButtonChanged));

        public static bool GetIsTitleBarCloseButton(DependencyObject element)
            => (bool)element.GetValue(IsTitleBarCloseButtonProperty);

        public static void SetIsTitleBarCloseButton(DependencyObject element, bool value)
            => element.SetValue(IsTitleBarCloseButtonProperty, value);
        #endregion

        private static void OnTitleBarChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is null) throw new NotSupportedException("TitleBar property should not be null.");
        }

        private static void OnWindowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is null) throw new NotSupportedException("TitleBar property should not be null.");
        }

        private static void OnButtonStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = (Button)d;

            if (e.OldValue is WindowState)
            {
                button.Click -= StateButton_Click;
            }

            if (e.NewValue is WindowState)
            {
                button.Click -= StateButton_Click;
                button.Click += StateButton_Click;
            }
        }

        private static void OnIsCloseButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = (Button)d;

            if (e.OldValue is true)
            {
                button.Click -= CloseButton_Click;
            }

            if (e.NewValue is true)
            {
                button.Click -= CloseButton_Click;
                button.Click += CloseButton_Click;
            }
        }

        private static void StateButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (DependencyObject)sender;
            var window = Window.GetWindow(button);
            var state = GetTitleBarButtonState(button);
            if (window != null && state != null)
            {
                window.WindowState = state.Value;
            }
        }

        private static void CloseButton_Click(object sender, RoutedEventArgs e)
            => Window.GetWindow((DependencyObject)sender)?.Close();
    }

    internal class UniversalTitleBar
    {
        public Color ForegroundColor { get; set; } = Colors.Black;
        public Color InactiveForegroundColor { get; set; } = Color.FromRgb(0x99, 0x99, 0x99);
        public Color ButtonForegroundColor { get; set; } = Colors.Black;
        public Color ButtonInactiveForegroundColor { get; set; } = Color.FromRgb(0x99, 0x99, 0x99);
        public Color ButtonHoverForeground { get; set; } = Colors.Black;
        public Color ButtonHoverBackground { get; set; } = Color.FromRgb(0xE6, 0xE6, 0xE6);
        public Color ButtonPressedForeground { get; set; } = Colors.Black;
        public Color ButtonPressedBackground { get; set; } = Color.FromRgb(0xCC, 0xCC, 0xCC);
    }

    internal class UniversalWindow
    {

        public Color FrameColor { get; set; } = SystemParameters.WindowGlassColor;
        public Color InactiveFrameColor { get; set; } = Colors.DimGray;
    }

    internal class UniversalWindowParameters
    {
        public static double DefaultWindowWidth { get; } = (int)SystemParameters.PrimaryScreenHeight;
        public static double DefaultWindowHeight { get; } = (int)(SystemParameters.PrimaryScreenHeight * 0.75);
        public static double DefaultMinWindowWidth { get; } = 500;
        public static double DefaultMinWindowHeight { get; } = 500;
    }
}
