using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;

namespace ProcessViewer;

public class ProcessInfo
{
    private static class ParentPIDMemoization
    {
        private static Dictionary<ProcessInfo, int> _data = new();

        public static int ParentPIDOf(ProcessInfo processInfo)
        {
            if (_data.ContainsKey(processInfo))
                return _data[processInfo];
            int parentPID = FindParentPID(processInfo.PID);
            _data[processInfo] = parentPID;
            return parentPID;
        }
        
        private static int FindParentPID(int pid)
        {
            using (ManagementObject mo = new ManagementObject($"win32_process.handle='{pid}'"))
            {
                mo.Get();
                return Convert.ToInt32(mo["ParentProcessId"]);
            }
        }
    }
    
    protected bool Equals(ProcessInfo other)
    {
        return PID == other.PID && Name == other.Name && StartTime == other.StartTime;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((ProcessInfo)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(PID, Name, StartTime);
    }

    public int PID { get; }
    public string Name { get; }
    public DateTime StartTime { get; }
    public TimeSpan CPUTime { get; }
    public long MemoryKB { get; }
    public string Priority { get; }
    public int ParentPID
    {
        get { return ParentPIDMemoization.ParentPIDOf(this); }
    }

    public ProcessInfo(Process process)
    {
        StartTime = GetStartTime(process);
        PID = process.Id;
        Name = process.ProcessName;
        CPUTime = GetCPUTime(process);
        Priority = GetPriority(process);
        MemoryKB = process.PrivateMemorySize64 / 1024;
    }
    
    
    private static DateTime GetStartTime(Process process)
    {
        try
        {
            return process.StartTime;
        }
        catch
        {
            return DateTime.UnixEpoch;
        }
    }
    private static TimeSpan GetCPUTime(Process process)
    {
        try
        {
            return process.TotalProcessorTime;
        }
        catch
        {
            return TimeSpan.Zero;
        }
    }

    private static string GetPriority(Process process)
    {
        try
        {
            return process.PriorityClass.ToString();
        }
        catch
        {
            return "Unknown";
        }
    }

}