using System.ComponentModel;

namespace Suimple.Views.Controls
{
    internal interface ISuimpleInput : INotifyPropertyChanged
    {
        void PushDataToOrchestrator();
    }
}