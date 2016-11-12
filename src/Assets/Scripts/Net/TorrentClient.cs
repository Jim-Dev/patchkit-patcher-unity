﻿using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace PatchKit.Unity.Patcher.Net
{
    internal class TorrentClient : IDisposable
    {
        private readonly string _streamingAssetsPath;

        private readonly Process _process;

        private readonly StreamReader _stdOutput;

        private readonly StreamWriter _stdInput;

        public TorrentClient(string streamingAssetsPath)
        {
            _streamingAssetsPath = streamingAssetsPath;
            _process = StartProcess();
            _stdOutput = CreateStdOutputStream();
            _stdInput = CreateStdInputStream();
        }

        /// <summary>
        /// Executes the command and returns the result.
        /// </summary>
        public JToken ExecuteCommand(string command)
        {
            WriteCommand(command);
            string resultStr = ReadCommandResult();
            return ParseCommandResult(resultStr);
        }

        private void WriteCommand(string command)
        {
            _stdInput.WriteLine(command);
            _stdInput.Flush();
        }

        private JToken ParseCommandResult(string resultStr)
        {
            return JToken.Parse(resultStr);
        }

        private string ReadCommandResult()
        {
            var str = new StringBuilder();

            while (!str.ToString().EndsWith("#=end"))
            {
                str.Append((char)_stdOutput.Read());
            }

            return str.ToString().Substring(0, str.Length - 5);
        }

        private StreamReader CreateStdOutputStream()
        {
            return new StreamReader(_process.StandardOutput.BaseStream, CreateStdEncoding());
        }

        private StreamWriter CreateStdInputStream()
        {
            return new StreamWriter(_process.StandardInput.BaseStream, CreateStdEncoding());
        }

        private Encoding CreateStdEncoding()
        {
            return new UTF8Encoding(false);
        }

        private Process StartProcess()
        {
            return Process.Start(new ProcessStartInfo()
            {
                CreateNoWindow = true,
                FileName = GetExecutablePath(),
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                UseShellExecute = false
            });
        }

        private string GetExecutablePath()
        {
            if (Application.platform == RuntimePlatform.WindowsPlayer ||
                Application.platform == RuntimePlatform.WindowsEditor)
            {
                return Path.Combine(_streamingAssetsPath, "torrent-client/win/torrent-client.exe");
            }

            throw new InvalidOperationException("Unsupported platform by torrent-client.");
        }

        void IDisposable.Dispose()
        {
            _stdOutput.Dispose();
            _stdInput.Dispose();

            if (!_process.HasExited)
            {
                _process.Kill();
            }
        }
    }
}