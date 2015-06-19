﻿namespace LASI.Content.InteropBindings
{
    public static class Configuration
    {
        public static void Initialize(Utilities.Configuration.IConfig settings)
        {
            TaggerInterop.SharpNLPTagger.Settings = settings;
            DocToDocXConverter.Config = settings;
        }
    }
}