using System;
using System.Timers;

namespace MxA.Services {
   internal class TimerStateMachine : IDisposable {

      public enum TimerState {
         Preparation = 0,
         Work = 1,
         RepetitionRest = 2,
         SetRest = 3
      }

      private enum TimerInternalState {
         Work = 1,
         RepetitionRest = 2,
         SetRest = 3
      }

      private struct Activity {
         public uint PrepTime;
         public uint WorkTime;
         public uint RestBetweenReps;
         public uint RestBetweenSets;

         public uint Reps;
         public uint Sets;

         public bool SkipLastRepRest;
         public bool SkipLastSetRest;
      }

      #region members
      private Timer _timer;
      private bool _running;

      private TimerInternalState _state;
      private TimerInternalState? _nextState;

      private bool _prepare;
      private Activity _activity;
      public event EventHandler StateChanged;
      #endregion

      #region properties
      public uint CurrentRepetition { get; private set; }
      public uint TotalRepetitions => _activity.Reps;
      public uint CurrentSet { get; private set; }
      public uint TotalSets => _activity.Sets;

      public int? NextCounter { get; private set; }
      public int Counter { get; private set; }
      public int SubCounter { get; private set; }

      public bool IsRunning => _running;

      public TimerState State {
         get {
            if (_prepare) {
               return TimerState.Preparation;
            } else {
               return (TimerState) _state;
            }
         }
      }

      public TimerState NextState {
         get {
            if (_prepare) {
               return (TimerState) _state;
            } else {
               return (TimerState) _nextState;
            }
         }
      }
      #endregion

      private TimerStateMachine() {
         _timer = new Timer(100);
         _timer.Elapsed += Timer_Elapsed;
         _timer.Start();
      }

      public TimerStateMachine(
         uint prepTime,
         uint workTime,
         uint restBetweenReps,
         uint restBetweenSets,

         uint reps,
         uint sets,

         bool skipLastRepRest,
         bool skipLastSetRest) : this() {
         _activity.PrepTime = prepTime;
         _activity.WorkTime = workTime;
         _activity.RestBetweenReps = restBetweenReps;
         _activity.RestBetweenSets = restBetweenSets;
         _activity.Reps = reps;
         _activity.Sets = sets;
         _activity.SkipLastRepRest = skipLastRepRest;
         _activity.SkipLastSetRest = skipLastSetRest;
      }

      public void PlayPause() {
         _running = !_running;
         if (_running) {
            _timer.Start();
         } else {
            _timer.Stop();
            _prepare = true;
            Counter = (int) _activity.PrepTime;
         }
      }

      private void Timer_Elapsed(object sender, ElapsedEventArgs e) {
         if (_running) {
            if (--SubCounter == -1) {
               SubCounter = 10;
               if (--Counter == -1) {
                  UpdateState();
               }
            }
            StateChanged?.Invoke(this, EventArgs.Empty);
         }
      }

      private void UpdateState() {
         if (_prepare) {
            // Prepare period had finished
            // We start or continue where we left off
            _prepare = false;
            Counter = GetPeriodTime(_state);
         } else {
            // Advanced to next period
            _state = GetNextState(_state);
            Counter = GetPeriodTime(_state);

            _nextState = GetNextState(_state);
            NextCounter = GetPeriodTime(_nextState);
         }
      }

      private TimerInternalState GetNextState(TimerInternalState state) {
         if (state == TimerInternalState.Work && IsLastRep() && _activity.SkipLastRepRest) {
            state = TimerInternalState.RepetitionRest;
         }

         if (state == TimerInternalState.RepetitionRest) {
            if (IsLastSet() && _activity.SkipLastSetRest) {
               state = TimerInternalState.SetRest;
            }
            CurrentRepetition = (++CurrentRepetition) % TotalRepetitions;
         }

         if (state == TimerInternalState.SetRest) {
            CurrentSet = (++CurrentSet) % TotalSets;
         }
         return ++state;
      }

      private int GetPeriodTime(TimerInternalState? state) {
         switch (state) {
            case null:
               return 0;
            case TimerInternalState.Work:
               return (int) _activity.WorkTime;
            case TimerInternalState.RepetitionRest:
               return (int) _activity.RestBetweenReps;
            case TimerInternalState.SetRest:
               return (int) _activity.RestBetweenSets;
            default:
               return 0;
         }
      }

      private bool IsLastSet() {
         return CurrentSet == TotalSets - 1;
      }

      private bool IsLastRep() {
         return CurrentRepetition == TotalRepetitions - 1;
      }

      private bool disposedValue;

      protected virtual void Dispose(bool disposing) {
         if (!disposedValue) {
            if (disposing) {
               _timer.Stop();
               _timer.Dispose();
            }

            disposedValue = true;
         }
      }

      public void Dispose() {
         // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
         Dispose(disposing: true);
         GC.SuppressFinalize(this);
      }
   }
}
