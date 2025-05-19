using System;

namespace OrchestratorCore
{
    public interface IOrchestratorFileBased
    {
        string FilePath { get; set; }

        bool LoadFile();

        event EventHandler FileLoaded;
    }
}