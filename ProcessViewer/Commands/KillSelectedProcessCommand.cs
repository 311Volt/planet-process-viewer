namespace ProcessViewer.Commands;

public class KillSelectedProcessCommand: BaseCommand
{
    public KillSelectedProcessCommand(ProcessViewerViewModel model) : base(model){}

    public override void Execute(object? param)
    {
        _model.KillSelectedProcess();
    }
}