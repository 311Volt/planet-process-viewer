using System;
using System.Windows.Input;

namespace ProcessViewer.Commands;

public class RefreshProcessesCommand: BaseCommand
{
    public RefreshProcessesCommand(ProcessViewerViewModel model) : base(model){}

    public override void Execute(object? param)
    {
        _model.RefreshList();
    }
}