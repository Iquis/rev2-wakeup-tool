using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace GGXrdReversalTool.Controls;

//Do not use directly!!!
public class NotifiedUserControl : UserControl, INotifyPropertyChanged
{
    public NotifiedUserControl(): base()
    {
        
    }
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}