﻿using Newtonsoft.Json;
using PixelStacker.IO.Config;
using PixelStacker.Logic.IO.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelStacker.IO
{
    public class LocalDataOptionsProvider : IOptionsProvider
    {
        public Options Load()
        {
            Properties.Settings.Default.Upgrade();
            string json = Properties.Settings.Default.JSON;
            var rt = JsonConvert.DeserializeObject<Options>(json) ?? new Options(this);
            rt.StorageProvider = this;
            return rt;
        }

        public void Save(Options t)
        {
            string json = JsonConvert.SerializeObject(t);
            Properties.Settings.Default.JSON = json;
            Properties.Settings.Default.Save();
        }
    }
}
