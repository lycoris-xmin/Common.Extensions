namespace Lycoris.Common.Utils.Throttler
{
    /// <summary>
    /// 节流
    /// </summary>
    public class ThrottlerUtils
    {
        private Action _action;
        private System.Timers.Timer _timer;
        private int _intervalMilliseconds;
        private bool _isRunning = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="intervalMilliseconds"></param>
        public ThrottlerUtils(Action action, int intervalMilliseconds)
        {
            _action = action;
            _intervalMilliseconds = intervalMilliseconds;
            _timer = new System.Timers.Timer(intervalMilliseconds);
            _timer.Elapsed += (sender, e) =>
            {
                _isRunning = false;
                _action?.Invoke();
            };
        }

        /// <summary>
        /// 
        /// </summary>
        public void Trigger()
        {
            if (!_isRunning)
            {
                _isRunning = true;
                _timer.Start();
            }
        }
    }
}
