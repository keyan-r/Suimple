using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OrchestratorCore.Params
{
    public abstract class OrchestratorParam<T> : IOrchestratorParam, INotifyPropertyChanged
    {
        public string Name { get; set; }
        public string Description { get; set; }                

        private T m_value;
        public T Value
        {
            get => m_value;
            set => SetField(ref m_value, value);
        }

        public OrchestratorParam(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public OrchestratorParam()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }


    }
}