using PortableLoadCell.Models;
using System;
using System.Linq;
using System.Timers;

namespace PortableLoadCell.Services {
   class TrainingWorker {
      private Timer _timer;
      private readonly Training _training;
      private Period[] _periods;
      private int _currentPeriodIndex;

      private Period _preparePeriod;
      private Period _coolDownPeriod;

      public Period CurrentPeriod { get; private set; }
      public Period NextPeriod { get; private set; }
      public bool IsRunning { get; private set; }
      public int Counter { get; private set; }

      public bool CanPrevRep => _currentPeriodIndex > 0;
      public bool CanNextRep => _currentPeriodIndex < _periods.Length - 1;
      public bool CanPrevSet => _currentPeriodIndex > 0;
      public bool CanNextSet => _currentPeriodIndex < _periods.Length - 1;

      public event EventHandler WorkerChanged;

      public TrainingWorker(Training training) {
         if (training == null)
            throw new ArgumentNullException(nameof(training));

         _training = training;
         _timer = new Timer(1000);
         _timer.Elapsed += Timer_Elapsed;
         _timer.Start();

         var periods = training.Expand();

         _periods = periods.
            Where(p =>
               p.PeriodType != PeriodType.Prepare &&
               p.PeriodType != PeriodType.CoolDown).
            ToArray();
         _preparePeriod = periods.FirstOrDefault(p => p.PeriodType == PeriodType.Prepare);
         _coolDownPeriod = periods.FirstOrDefault(p => p.PeriodType == PeriodType.CoolDown);

         SetPeriod(_preparePeriod);
      }

      public void PlayPause() {
         IsRunning = !IsRunning;
         if (!IsRunning) {
            SetPeriod(_preparePeriod);
         }
      }

      public void PrevRep() {
         IsRunning = false;
         _currentPeriodIndex = Math.Max(0, _currentPeriodIndex - 1);
         SetPeriod(_preparePeriod);
      }

      public void NextRep() {
         IsRunning = false;
         _currentPeriodIndex = Math.Min(_currentPeriodIndex + 1, _periods.Length - 1);
         SetPeriod(_preparePeriod);
      }

      public void PrevSet() {
         IsRunning = false;

         for (int i = _currentPeriodIndex; i >= 0; i--) {
            if (_periods[_currentPeriodIndex].Set != _periods[i].Set &&
               _periods[i].Rep == 1 &&
               _periods[i].PeriodType == PeriodType.Work) {
               _currentPeriodIndex = i;
               break;
            }
         }
         SetPeriod(_preparePeriod);
      }

      public void NextSet() {
         IsRunning = false;
         SetPeriod(_preparePeriod);

         for (int i = _currentPeriodIndex; i < _periods.Length; i++) {
            if (_periods[_currentPeriodIndex].Set != _periods[i].Set) {
               _currentPeriodIndex = i;
               break;
            }
         }
      }

      private void Timer_Elapsed(object sender, ElapsedEventArgs e) {
         if (IsRunning && CurrentPeriod != null) {
            Counter--;

            if (Counter == -1) {
               AdvancePeriod();
            }
         }

         WorkerChanged?.Invoke(this, new EventArgs());
      }

      private void AdvancePeriod() {
         if (CurrentPeriod == _preparePeriod) {
            if (_currentPeriodIndex < _periods.Length) {
               SetPeriod(_periods[_currentPeriodIndex]);
            } else {
               SetPeriod(_coolDownPeriod);
            }
         } else if (CurrentPeriod == _coolDownPeriod) {
            SetPeriod(null);
         } else if (_currentPeriodIndex + 1 < _periods.Length) {
            SetPeriod(_periods[++_currentPeriodIndex]);
         } else {
            SetPeriod(_coolDownPeriod);
         }
      }

      private void SetPeriod(Period period) {
         CurrentPeriod = period;

         Counter = (int) period?.Time;

         if (CurrentPeriod == _preparePeriod) {
            NextPeriod = _periods[_currentPeriodIndex];
         } else if (_currentPeriodIndex + 1 < _periods.Length) {
            NextPeriod = _periods[_currentPeriodIndex + 1];
         } else if (_currentPeriodIndex + 1 == _periods.Length) {
            NextPeriod = _coolDownPeriod;
         } else {
            NextPeriod = null;
         }
      }
   }
}
