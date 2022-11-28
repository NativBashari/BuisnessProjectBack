using System.Threading;

namespace BuisnessProjectAPI.TimerFeatures
{
    public class TimerManager
    {

        private Timer _timer;
        private AutoResetEvent _autoResetEvent;
        private Action _action;

        public TimerManager(Action action)
        {
            _action = action;
            _autoResetEvent= new AutoResetEvent(false);
            _timer = new Timer(Execute, _autoResetEvent, 0, 3000);
        }

        private void Execute(object? state)
        {
           Task.Run(() => _action());
        }
    }
}
