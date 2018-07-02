using System;
using System.Windows;

// https://stackoverflow.com/a/6486469/3960200
// License: Attribution-ShareAlike 3.0 Unported https://creativecommons.org/licenses/by-sa/3.0/
// Moved code into a stand-alone class.

namespace EmergenceGuardian.WpfExtensions {
    /// <summary>
    /// Allows showing message boxes from MVVM ViewModel. Declare an instance of this class as a field in your ViewModel, 
    /// and listen to the MessageBoxRequest event in your View to call e.Show().
    /// </summary>
    public class MessageBoxManager {
        public event EventHandler<MessageBoxEventArgs> MessageBoxRequest;

        public void Show(Action<MessageBoxResult> resultAction, string messageBoxText, string caption = "", MessageBoxButton button = MessageBoxButton.OK, MessageBoxImage icon = MessageBoxImage.None, MessageBoxResult defaultResult = MessageBoxResult.None, MessageBoxOptions options = MessageBoxOptions.None) {
            MessageBoxRequest?.Invoke(this, new MessageBoxEventArgs(resultAction, messageBoxText, caption, button, icon, defaultResult, options));
        }
    }

    public class MessageBoxEventArgs : EventArgs {
        public MessageBoxEventArgs(Action<MessageBoxResult> resultAction, string messageBoxText, string caption = "", MessageBoxButton button = MessageBoxButton.OK, MessageBoxImage icon = MessageBoxImage.None, MessageBoxResult defaultResult = MessageBoxResult.None, MessageBoxOptions options = MessageBoxOptions.None) {
            this.ResultAction = resultAction;
            this.MessageBoxText = messageBoxText;
            this.Caption = caption;
            this.Button = button;
            this.Icon = icon;
            this.DefaultResult = defaultResult;
            this.Options = options;
        }

        public Action<MessageBoxResult> ResultAction { get; private set; }
        public string MessageBoxText { get; private set; }
        public string Caption { get; private set; }
        public MessageBoxButton Button { get; private set; }
        public MessageBoxImage Icon { get; private set; }
        public MessageBoxResult DefaultResult { get; private set; }
        public MessageBoxOptions Options { get; private set; }

        public void Show(Window owner) {
            MessageBoxResult messageBoxResult = MessageBox.Show(owner, MessageBoxText, Caption, Button, Icon, DefaultResult, Options);
            ResultAction?.Invoke(messageBoxResult);
        }

        public void Show() {
            MessageBoxResult messageBoxResult = MessageBox.Show(MessageBoxText, Caption, Button, Icon, DefaultResult, Options);
            ResultAction?.Invoke(messageBoxResult);
        }
    }
}
