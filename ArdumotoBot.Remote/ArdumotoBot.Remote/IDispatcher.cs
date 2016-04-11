using System;

namespace ArdumotoBot.Remote
{
    public interface IDispatcher
    {
        void Invoke(Action action);
        bool CheckAccess();
    }
}