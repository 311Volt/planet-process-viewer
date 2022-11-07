using System;
using System.Windows.Input;

namespace ProcessViewer.Commands;

public abstract class BaseCommand: ICommand
{
    protected ProcessViewerViewModel _model;
    
    public BaseCommand(ProcessViewerViewModel model)
    {
        this._model = model;
    }
    
    public bool CanExecute(object? param)
    {
        return true;
    }
    public event EventHandler? CanExecuteChanged;

    public abstract void Execute(object? args);
}