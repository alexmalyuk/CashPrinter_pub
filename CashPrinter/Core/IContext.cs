using System;
using System.Windows.Threading;

namespace CashPrinter.Core
{
    public interface IContext
    {
        void Invoke(Action action);
        void BeginInvoke(Action action);
    }

    public sealed class WpfDispatcherContext : IContext
    {
        private readonly Dispatcher _dispatcher;

        public WpfDispatcherContext()
            : this(Dispatcher.CurrentDispatcher) { }

        public WpfDispatcherContext(Dispatcher dispatcher)
        {
            this._dispatcher = dispatcher;
        }

        public void Invoke(Action action)
        {
            this._dispatcher.Invoke(action);
        }

        public void BeginInvoke(Action action)
        {
            this._dispatcher.BeginInvoke(action);
        }
    }
}
