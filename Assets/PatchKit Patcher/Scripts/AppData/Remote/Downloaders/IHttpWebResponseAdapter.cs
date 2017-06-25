﻿using System;
using System.IO;
using System.Net;

namespace PatchKit.Patcher.AppData.Remote.Downloaders
{
    public interface IHttpWebResponseAdapter : IDisposable
    {
        HttpStatusCode StatusCode { get; }

        Stream GetResponseStream();
    }
}