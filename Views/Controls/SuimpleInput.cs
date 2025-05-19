using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using OrchestratorRhinoCore.Components;

namespace Suimple.Views.Controls
{
    internal abstract class SuimpleInput : SuimpleFormComponent, ISuimpleInput
    {
        protected SuimpleInput(IOrchestratorGHComponent OrchestratorGHComponent) : base(OrchestratorGHComponent)
        {
        }

        public abstract void PushDataToOrchestrator();

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