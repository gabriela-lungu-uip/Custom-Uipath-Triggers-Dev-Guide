using System;
using System.Activities;
using System.Threading;
using UiPath.Platform.Triggers;

namespace TimerTrigger
{
    public class TimerTrigger : TriggerBase<TimerTriggerArgs>
    {
        //it is recommended to use Variable to store fields in order for  
        //activities like Parallel For Each to work correctly 
        private readonly Variable<Timer> _timer = new Variable<Timer>();

        public InArgument<TimeSpan> Period { get; set; }

        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            metadata.AddImplementationVariable(_timer);
            base.CacheMetadata(metadata);
        }

        //in this method you can subscribe to events. It is called when the trigger starts execution 
        protected override void StartMonitor(NativeActivityContext context, Action<TimerTriggerArgs> sendTrigger)
        {
            var eventIndex = 0;
            var period = Period.Get(context);
            _timer.Set(context, new Timer(OnTick, state: null, dueTime: period, period: period));
            return;

            void OnTick(object state) => sendTrigger(new TimerTriggerArgs(eventIndex++));
        }

        //this is used for cleanup. It is called when the trigger is Cancelled or Aborted 
        protected override void StopMonitor(ActivityContext context) => _timer.Get(context).Dispose();

    }

    //Each trigger may declare a type that sub-classes TriggerArgs 
    //that corresponds to the “args” item in Trigger Scope activity. If no extra info 
    //needs to be passed along, TriggerArgs can be used directly 
    public class TimerTriggerArgs : TriggerArgs
    {
        public int EventIndex { get; }

        public TimerTriggerArgs(int eventIndex) => EventIndex = eventIndex;
    }
}
