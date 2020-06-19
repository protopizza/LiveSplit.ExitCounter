using LiveSplit.Model;
using System;

namespace LiveSplit.UI.Components
{
    public class ExitCounterComponentFactory : IComponentFactory
    {
        public string ComponentName => "Exit Counter";

        public string Description => "Exit Counter for SMW Romhacks.";

        public ComponentCategory Category => ComponentCategory.Other;

        public IComponent Create(LiveSplitState state) => new ExitCounterComponent();

        public string UpdateName => ComponentName;

        public string XMLURL => "";

        public string UpdateURL => "";

        public Version Version => Version.Parse("1.8.0");
    }
}
