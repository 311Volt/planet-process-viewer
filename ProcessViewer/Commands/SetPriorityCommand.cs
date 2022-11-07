using System.Diagnostics;
using System.Windows;

namespace ProcessViewer.Commands;

public class SetPriorityCommand: BaseCommand
{
    public SetPriorityCommand(ProcessViewerViewModel model) : base(model){}

    public override void Execute(object? param)
    {
        if (param is string priorityLevel)
        {
            ProcessPriorityClass priorityClass = priorityLevel switch
            {
                "idle" => ProcessPriorityClass.Idle,
                "belownormal" => ProcessPriorityClass.BelowNormal,
                "normal" => ProcessPriorityClass.Normal,
                "abovenormal" => ProcessPriorityClass.AboveNormal,
                "high" => ProcessPriorityClass.High,
                "realtime" => ProcessPriorityClass.RealTime
            };
            _model.SetSelectedProcessPriority(priorityClass);
        }
    }
}