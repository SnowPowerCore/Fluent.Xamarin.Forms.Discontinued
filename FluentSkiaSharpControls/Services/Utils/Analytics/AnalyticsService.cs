using Microsoft.AppCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using FluentSkiaSharpControls.Services.Utils.Settings;

namespace FluentSkiaSharpControls.Services.Utils.Analytics
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly ISettingsService _settings;

        public AnalyticsService(ISettingsService settings)
        {
            _settings = settings;

            Init();
        }

        private void Init() =>
            AppCenter.Start(_settings.AppCenterAndroidKey + _settings.AppCenteriOSKey,
                typeof(Microsoft.AppCenter.Crashes.Crashes),
                typeof(Microsoft.AppCenter.Analytics.Analytics));

        public void TrackError(Exception ex, Dictionary<string, string> keys = null, string additional = null)
        {
            var dataDict = new Dictionary<string, string>();
            dataDict.Add("exception message", ex.Message);
            if (keys != null)
                dataDict = dataDict.Concat(keys).ToDictionary(x => x.Key, x => x.Value);
            if (!string.IsNullOrEmpty(additional))
                dataDict.Add("Additional Info", additional);
            Microsoft.AppCenter.Crashes.Crashes.TrackError(ex, dataDict);
        }

        public void TrackEvent(string name, Dictionary<string, string> keys = null, string additional = null)
        {
            var dataDict = new Dictionary<string, string>();
            if (keys != null)
                dataDict = dataDict.Concat(keys).ToDictionary(x => x.Key, x => x.Value);
            if (!string.IsNullOrEmpty(additional))
                dataDict.Add("Additional Info", additional);
            Microsoft.AppCenter.Analytics.Analytics.TrackEvent(name, dataDict);
        }
    }
}