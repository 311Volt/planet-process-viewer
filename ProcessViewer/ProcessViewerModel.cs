using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace ProcessViewer;

public class ProcessViewerModel
{
    public List<ProcessInfo> GetCurrentProcessList()
    {
        return Process.GetProcesses().Select(proc => new ProcessInfo(proc)).ToList();
    }

    public void KillProcess(int pid)
    {
        try
        {
            Process.GetProcessById(pid).Kill();
        }
        catch (Exception e)
        {
            MessageBox.Show($"Cannot kill PID {pid}: {e.Message}");
        }
    }

    public void ChangePriority(int pid, ProcessPriorityClass priorityClass)
    {
        try
        {
            MessageBox.Show($"{pid} {priorityClass}");
            Process.GetProcessById(pid).PriorityClass = priorityClass;
        }
        catch (Exception e)
        {
            MessageBox.Show($"Cannot set PID {pid} priority to {priorityClass}: {e.Message}");
        }
    }
}