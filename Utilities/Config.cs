﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LASI.Utilities
{
    public static class Config
    {
        public static IConfig FromJson(string filePath) => new JsonConfig(filePath);
        public static IConfig FromJson(Uri uri) => new JsonConfig(uri);
    }
}