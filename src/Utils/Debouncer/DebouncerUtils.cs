namespace Lycoris.Common.Utils.Debouncer
{
    /// <summary>
    /// 防抖
    /// </summary>
    public class DebouncerUtils
    {
        private Action _action;
        private System.Timers.Timer _timer;
        private int _delayMilliseconds;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="delayMilliseconds"></param>
        public DebouncerUtils(Action action, int delayMilliseconds)
        {
            _action = action;
            _delayMilliseconds = delayMilliseconds;
            _timer = new System.Timers.Timer(delayMilliseconds);
            _timer.Elapsed += (sender, e) =>
            {
                _timer.Stop();
                _action?.Invoke();
            };
        }

        /// <summary>
        /// 
        /// </summary>
        public void Trigger()
        {
            if (_timer.Enabled)
            {
                _timer.Stop();
                _timer.Start();
            }
            else
            {
                _timer.Start();
            }
        }
    }
}
