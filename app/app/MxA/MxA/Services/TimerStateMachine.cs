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

      public int? NextCounter {
         get {
            if (_prepare) {
               return GetStateTime(_state);
            } else {
               return GetStateTime(_nextState);
            }
         }
      }

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

         Counter = (int) (_activity.PrepTime == 0 ? 9 : _activity.PrepTime - 1);
         SubCounter = 9;
         _prepare = true;
         _state = TimerInternalState.Work;
         _nextState = GetNextState();
      }

      public void PlayPause() {
         _running = !_running;
         if (_running) {
            _timer.Start();
         } else {
            _timer.Stop();
            _prepare = true;
            Counter = (int) (_activity.PrepTime == 0 ? 9 : _activity.PrepTime - 1);
         }
         StateChanged?.Invoke(this, EventArgs.Empty);
      }

      public void NextRepetition() {
         _running = false;

         _timer.Stop();
         _prepare = true;
         Counter = (int) (_activity.PrepTime == 0 ? 9 : _activity.PrepTime - 1);

         AdvanceState();
         _nextState = GetNextState();

         StateChanged?.Invoke(this, EventArgs.Empty);
      }

      public void PreviousRepetition() {
         _running = false;

         _timer.Stop();
         _prepare = true;
         Counter = (int) (_activity.PrepTime == 0 ? 9 : _activity.PrepTime - 1);

         RevertState();
         _nextState = GetNextState();

         StateChanged?.Invoke(this, EventArgs.Empty);
      }

      public void NextSet() {
         _running = false;

         _timer.Stop();
         _prepare = true;
         Counter = (int) (_activity.PrepTime == 0 ? 9 : _activity.PrepTime - 1);

         CurrentSet++;
         CurrentRepetition = 0;
         _state = TimerInternalState.Work;
         Counter = GetStateTime(_state) - 1;
         _nextState = GetNextState();

         StateChanged?.Invoke(this, EventArgs.Empty);
      }

      public void PreviousSet() {
         _running = false;

         _timer.Stop();
         _prepare = true;
         Counter = (int) (_activity.PrepTime == 0 ? 9 : _activity.PrepTime - 1);

         CurrentSet = CurrentSet == 0 ? 0 : CurrentSet - 1;
         CurrentRepetition = 0;
         _state = TimerInternalState.Work;
         Counter = GetStateTime(_state) - 1;
         _nextState = GetNextState();

         StateChanged?.Invoke(this, EventArgs.Empty);
      }

      private void Timer_Elapsed(object sender, ElapsedEventArgs e) {
         if (_running) {
            if (--SubCounter == -1) {
               SubCounter = 9;
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
            Counter = GetStateTime(_state) - 1;
         } else {
            // Advanced to next state
            AdvanceState();
            Counter = GetStateTime(_state) - 1;
            _nextState = GetNextState();
         }
      }

      private void RevertState() {
         if (_state == TimerInternalState.Work) {
            if (CurrentRepetition > 0) {
               _state = TimerInternalState.RepetitionRest;
               CurrentRepetition--;
            } else {
               _state = TimerInternalState.SetRest;
               CurrentSet--;
               CurrentRepetition = TotalRepetitions - 1;
            }
         } else if (_state == TimerInternalState.RepetitionRest) {
            _state = TimerInternalState.Work;
         } else if (_state == TimerInternalState.SetRest) {
            _state = TimerInternalState.RepetitionRest;

            if (IsLastRep() && _activity.SkipLastRepRest) {
               _state = TimerInternalState.Work;
            }
         }
      }

      private void AdvanceState() {
         if (_state == TimerInternalState.Work) {
            // Move to repetition rest
            _state = TimerInternalState.RepetitionRest;


            if (IsLastRep() && _activity.SkipLastRepRest) {
               // if is last rep and skip last rep => move to set rest
               _state = TimerInternalState.SetRest;

               if (IsLastSet() && _activity.SkipLastSetRest) {
                  // TODO: Time done
               }
            }
         } else if (_state == TimerInternalState.RepetitionRest) {
            if (IsLastRep()) {
               _state = TimerInternalState.SetRest;

               if (IsLastSet() && _activity.SkipLastSetRest) {
                  // TODO: Time done
               }
            } else {
               _state = TimerInternalState.Work;
               CurrentRepetition++;
            }
         } else if (_state == TimerInternalState.SetRest) {
            if (IsLastSet()) {
               // TODO: Time done
            } else {
               _state = TimerInternalState.Work;
               CurrentRepetition = 0;
               CurrentSet++;
            }
         }
      }

      private TimerInternalState? GetNextState() {
         TimerInternalState? result = null;
         if (_state == TimerInternalState.Work) {
            // Move to repetition rest
            result = TimerInternalState.RepetitionRest;

            if (IsLastRep() && _activity.SkipLastRepRest) {
               // if is last rep and skip last rep => move to set rest
               result = TimerInternalState.SetRest;

               if (IsLastSet() && _activity.SkipLastSetRest) {
                  result = null;
               }
            }
         } else if (_state == TimerInternalState.RepetitionRest) {
            if (IsLastRep()) {
               result = TimerInternalState.SetRest;

               if (IsLastSet() && _activity.SkipLastSetRest) {
                  // TODO: Time done
                  result = null;
               }
            } else {
               result = TimerInternalState.Work;
            }
         } else if (_state == TimerInternalState.SetRest) {
            if (IsLastSet()) {
               // TODO: Time done
               result = null;
            } else {
               result = TimerInternalState.Work;
            }
         }

         return result;
      }

      private int GetStateTime(TimerInternalState? state) {
         switch (state) {
            case null:
               return 0;
            case TimerInternalState.Work:
               return (int) _activity.WorkTime ;
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
