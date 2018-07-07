using System;
using System.Collections.Generic;
using System.Reflection;

namespace EmergenceGuardian.WpfExtensions {
    public interface IEnvironmentService {
        IEnumerable<string> CommandLineArguments { get; }
        Version AppVersion { get; }
    }

    public class EnvironmentService : IEnvironmentService {
        public IEnumerable<string> CommandLineArguments => Environment.GetCommandLineArgs();
        public Version AppVersion => Assembly.GetEntryAssembly().GetName().Version;
    }
}
