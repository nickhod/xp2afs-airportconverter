﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XP2AFSAirportConverter.Common
{
    public enum ConverterAction
    {
        GetAirportList,
        DownloadAirports,
        AirportCsvList,
        ImportAirportCsvList,
        Clean,
        GenerateAssetList,
        GetMissingLocationData,

        // Convert steps
        GenerateRenderScripts,
        RunRenderScripts,
        BuildAirports,
        UploadAirports,



    }
}
