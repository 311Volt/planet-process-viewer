using System;
using System.Windows;
using System.Windows.Controls;

namespace ProcessViewer.Commands;

public class SortProcessesCommand: BaseCommand
{
    public SortProcessesCommand(ProcessViewerViewModel model) : base(model){}

    public override void Execute(object? param)
    {
        if (param is string columnName)
        {
            _model.UpdateSortDescription(columnName);
        }
    }
}