using System;

namespace QManager.View
{
    public sealed class NavigationRequestEventArgs : EventArgs
    {
        public NavigationRequestEventArgs(string target)
        {
            Target = target;
        }

        public string Target { get; }
    }
}
