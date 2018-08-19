﻿using DotLiquid;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XP2AFSAirportConverter.Common;
using XP2AFSAirportConverter.Models;
using XP2AFSAirportConverter.ScriptGenerators.Models;
using XP2AFSAirportConverter.XP;

namespace XP2AFSAirportConverter.ScriptGenerators
{
    public class MaxScriptGenerator : ScriptGenerator
    {
        public override void GenerateScripts(string icao, DATFile datFile, DSFFile dsfFile, TSCFile tscFile, string outputFolder)
        {
            this.icao = icao;
            this.datFile = datFile;
            this.dsfFile = dsfFile;
            this.tscFile = tscFile;

            var maxScriptFilePath = AppDomain.CurrentDomain.BaseDirectory + "\\ScriptGenerators\\ScriptTemplates\\MaxScript.liquid";
            var maxScript = File.ReadAllText(maxScriptFilePath);

            this.scriptModel = new ScriptModel();
            this.CalculateRunways();

            Template template = Template.Parse(maxScript); 
            var maxScriptFinal = template.Render(Hash.FromAnonymousObject(scriptModel));

            var outputFilePath = outputFolder + "\\" + icao  + ".ms";

            File.WriteAllText(outputFilePath, maxScriptFinal);

        }


    }
}