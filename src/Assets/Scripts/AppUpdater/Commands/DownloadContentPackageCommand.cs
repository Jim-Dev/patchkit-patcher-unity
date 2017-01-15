﻿using PatchKit.Unity.Patcher.AppData.Remote;
using PatchKit.Unity.Patcher.Cancellation;
using PatchKit.Unity.Patcher.Debug;
using PatchKit.Unity.Patcher.Status;

namespace PatchKit.Unity.Patcher.AppUpdater.Commands
{
    public class DownloadContentPackageCommand : BaseAppUpdaterCommand, IDownloadContentPackageCommand
    {
        private static readonly DebugLogger DebugLogger = new DebugLogger(typeof(DownloadContentPackageCommand));

        private readonly int _versionId;
        private readonly AppUpdaterContext _context;

        private string _keySecret;
        private RemoteResource _resource;
        private IDownloadStatusReporter _statusReporter;

        public DownloadContentPackageCommand(int versionId, AppUpdaterContext context)
        {
            Checks.ArgumentValidVersionId(versionId, "versionId");
            AssertChecks.ArgumentNotNull(context, "context");

            DebugLogger.LogConstructor();
            DebugLogger.LogVariable(versionId, "versionId");

            _versionId = versionId;
            _context = context;
        }

        public override void Execute(CancellationToken cancellationToken)
        {
            base.Execute(cancellationToken);

            DebugLogger.Log("Downloading content package.");

            string contentPath = _context.App.LocalData.DownloadData.GetContentPackagePath(_versionId);

            var downloader = new RemoteResourceDownloader(contentPath, _resource, _context.Configuration.UseTorrents);

            downloader.DownloadProgressChanged += _statusReporter.OnDownloadProgressChanged;

            _statusReporter.OnDownloadStarted();

            downloader.Download(cancellationToken);

            _statusReporter.OnDownloadEnded();

            PackagePath = contentPath;
        }

        public override void Prepare(IStatusMonitor statusMonitor)
        {
            base.Prepare(statusMonitor);

            AssertChecks.ArgumentNotNull(statusMonitor, "statusMonitor");

            DebugLogger.Log("Preparing content package download.");

            _resource = _context.App.RemoteData.GetContentPackageResource(_versionId, _keySecret);

            double weight = StatusWeightHelper.GetResourceDownloadWeight(_resource);
            _statusReporter = statusMonitor.CreateDownloadStatusReporter(weight);
        }

        public void SetKeySecret(string keySecret)
        {
            _keySecret = keySecret;
        }

        public string PackagePath { get; private set; }
    }
}