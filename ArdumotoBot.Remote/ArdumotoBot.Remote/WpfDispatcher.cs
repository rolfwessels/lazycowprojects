using System;
using System.Windows.Threading;

namespace ArdumotoBot.Remote
{
    public class WpfDispatcher : IDispatcher
    {
        private readonly Dispatcher _dispatcher;

        public WpfDispatcher(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public void Invoke(Action action)
        {
            if (!_dispatcher.CheckAccess())
                _dispatcher.BeginInvoke(action);
            else
                action();
        }

        public bool CheckAccess()
        {
            return _dispatcher.CheckAccess();
        }
    }
}