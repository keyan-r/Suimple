using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OrchestratorCore
{
    public abstract class OrchestratorWorkNode : INotifyPropertyChanged
    {
        public event EventHandler SolutionStart;
        
        public event EventHandler SolutionEnd;

        public event EventHandler SolutionRequested;

        public bool IsRunning;

        private bool m_enabled = true;

        public bool Enabled
        {
            get => m_enabled;
            set => SetField(ref m_enabled, value);
        }

        public void OnSolutionStart()
        {
            IsRunning = true;
            SolutionStart?.Invoke(this, EventArgs.Empty);
        }        

        public void OnSolutionEnd()
        {
            IsRunning = false;
            SolutionEnd?.Invoke(this, EventArgs.Empty);
        }

        public void RequestSolution()
        {
            SolutionRequested.Invoke(this, EventArgs.Empty);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}