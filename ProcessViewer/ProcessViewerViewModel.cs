using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ProcessViewer.Commands;

namespace ProcessViewer;

public class ProcessViewerViewModel: INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private DispatcherTimer refreshTimer = new DispatcherTimer();
    private void OnPropertyChanged(string propName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }

    private ProcessViewerModel _model;
    public ProcessViewerViewModel()
    {
        _model = new ProcessViewerModel();
        RefreshProcessesCommand = new(this);
        KillSelectedProcessCommand = new(this);
        SortProcessesCommand = new(this);
        SetPriorityCommand = new(this);
        
        
        PropertyChanged += UpdateRefreshInterval;

        refreshTimer.Tick += ConditionalRefreshList;
        refreshTimer.Interval = TimeSpan.FromMilliseconds(RefreshInterval);
        refreshTimer.Start();
    }

    public RefreshProcessesCommand RefreshProcessesCommand { get; }
    public KillSelectedProcessCommand KillSelectedProcessCommand { get; }
    public SortProcessesCommand SortProcessesCommand { get; }
    public SetPriorityCommand SetPriorityCommand { get; }

    private void UpdateRefreshInterval(object? sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName == nameof(RefreshInterval))
        {
            refreshTimer.Interval = TimeSpan.FromMilliseconds(RefreshInterval);
        }
    }
    

    private void ConditionalRefreshList(object? param, EventArgs eventArgs)
    {
        if(AutoRefreshEnabled)
            RefreshList();
    }
    
    public void RefreshList()
    {
        int tmpPID = SelectedPID;
        
        _visibleProcesses = new(_model.GetCurrentProcessList());
        OnPropertyChanged(nameof(ChildrenOfSelected));
        OnPropertyChanged(nameof(VisibleProcesses));

        SelectedProcInfo = Enumerable.FirstOrDefault<ProcessInfo>(_visibleProcesses, (info => ((ProcessInfo)info).PID == tmpPID), null);
    }
    
    public void KillSelectedProcess()
    {
        if (SelectedPID == -1)
            return;
        _model.KillProcess(SelectedPID);
        RefreshList();
    }

    public void SetSelectedProcessPriority(ProcessPriorityClass priorityClass)
    {
        if(SelectedPID == -1)
            return;
        _model.ChangePriority(SelectedPID, priorityClass);
        RefreshList();
    }
    
    public void UpdateSortDescription(string columnName)
    {
        if (_sortDescription.PropertyName == columnName)
        {
            _sortDescription.Direction =
                _sortDescription.Direction == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
        }

        _sortDescription.PropertyName = columnName;
        OnPropertyChanged(nameof(VisibleProcesses));
    }

    private ObservableCollection<ProcessInfo> _visibleProcesses = new();
    public ICollectionView VisibleProcesses
    {
        get
        {
            ICollectionView view = new CollectionViewSource { Source = _visibleProcesses }.View;
            view.Filter = o =>
            {
                if (String.IsNullOrEmpty(FilterString))
                    return true;
                return ((ProcessInfo)o).Name.Contains(FilterString);
            };
            view.SortDescriptions.Clear();
            view.SortDescriptions.Add(_sortDescription);
            return view;
        }
    }
    
    public ICollectionView ChildrenOfSelected
    {
        get
        {
            ICollectionView view = new CollectionViewSource { Source = _visibleProcesses }.View;
            view.Filter = o => ((ProcessInfo)o).ParentPID == SelectedPID;
            
            return view;
        }
    }

    private bool _autoRefreshEnabled = false;
    public bool AutoRefreshEnabled
    {
        get { return _autoRefreshEnabled; }
        set { _autoRefreshEnabled = value; OnPropertyChanged(nameof(AutoRefreshEnabled)); }
    }

    private int _refreshInterval = 400;
    public int RefreshInterval
    {
        get { return _refreshInterval; }
        set { _refreshInterval = value; OnPropertyChanged(nameof(RefreshInterval)); }
    }

    private ProcessInfo? _selectedProcInfo = null;

    private void UpdateSelectedProc()
    {
        OnPropertyChanged(nameof(SelectedProcInfo));
        OnPropertyChanged(nameof(SelectedPID));
        OnPropertyChanged(nameof(IsProcessSelected));
        OnPropertyChanged(nameof(ChildrenOfSelected));
    }
    
    public ProcessInfo? SelectedProcInfo
    {
        get { return _selectedProcInfo; }
        set { _selectedProcInfo = value; UpdateSelectedProc();}
    }

    public int SelectedPID => SelectedProcInfo?.PID ?? -1;

    public bool IsProcessSelected => SelectedPID != -1;

    
    private string _filterString = "";
    public string FilterString
    {
        get { return _filterString; }
        set { _filterString = value; OnPropertyChanged(nameof(FilterString)); OnPropertyChanged(nameof(VisibleProcesses)); }
    }

    private SortDescription _sortDescription = new SortDescription("PID", ListSortDirection.Ascending);
}