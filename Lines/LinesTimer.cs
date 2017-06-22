using System.Windows.Threading;

namespace Lines
{
    class LinesTimer : DispatcherTimer
    {
        private bool shallStop;

        public void ScheduleStop()
        {
            shallStop = true;
        }

        public new void Stop()
        {
            shallStop = false;

            base.Stop();
        }

        public bool ShallStop
        {
            get
            {
                return shallStop;
            }

            private set
            {
                shallStop = value;
            }
        }
    }
}
