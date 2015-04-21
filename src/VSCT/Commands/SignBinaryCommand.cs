﻿using System;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace MadsKristensen.ExtensibilityTools.VSCT.Commands
{
    sealed class SignBinaryCommand : BaseCommand
    {
        private const string ExtensibilityProjectGuid = "{82b43b9b-a64c-4715-b499-d71e9ca2bd60}";
        private Project _project;

        public SignBinaryCommand(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        public static SignBinaryCommand Instance
        {
            get;
            private set;
        }

        public static void Initialize(IServiceProvider provider)
        {
            Instance = new SignBinaryCommand(provider);
        }

        /// <summary>
        /// Overriden by child class to setup own menu commands and bind with invocation handlers.
        /// </summary>
        protected override void SetupCommands()
        {
            AddCommand(GuidList.guidExtensibilityToolsCmdSet, PackageCommands.cmdSignBinary, ShowSignBinaryUI, CheckForExtensibilityPackageFlavorBeforeQueryStatus);
        }

        private void ShowSignBinaryUI(object sender, EventArgs e)
        {
        }

        private void CheckForExtensibilityPackageFlavorBeforeQueryStatus(object sender, EventArgs e)
        {
            OleMenuCommand button = (OleMenuCommand) sender;
            button.Visible = false;

            UIHierarchyItem uiItem = GetSelectedItem();

            if (uiItem == null)
                return;

            _project = uiItem.Object as Project;
            if (_project == null)
                return;

            var solution = GetService<IVsSolution>();
            if (solution == null)
                return;

            IVsHierarchy projectHierarchy;
            ErrorHandler.ThrowOnFailure(solution.GetProjectOfUniqueName(_project.UniqueName, out projectHierarchy));

            var aggregatableProject = projectHierarchy as IVsAggregatableProject;
            if (aggregatableProject == null)
                return;

            string projectTypeGuids;
            ErrorHandler.ThrowOnFailure(aggregatableProject.GetAggregateProjectTypeGuids(out projectTypeGuids));

            button.Visible = projectTypeGuids != null && projectTypeGuids.IndexOf(ExtensibilityProjectGuid, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
