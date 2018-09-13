using DotLiquid;
using DotLiquid.NamingConventions;
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
            this.CalculateDATFilePavements();

            this.scriptModel.AirportName = datFile.AirportHeader.Name;
            this.scriptModel.AirportICAO = datFile.AirportHeader.ICAOCode;
            this.scriptModel.GeneratedOn = DateTime.UtcNow;
            this.scriptModel.SaveFilePath = outputFolder + @"\Input\" + icao + ".max";
            this.scriptModel.SaveFilePath = this.scriptModel.SaveFilePath.Replace(@"\", @"\\");

            Template template = Template.Parse(maxScript);
            Template.NamingConvention = new CSharpNamingConvention();
            var maxScriptFinal = template.Render(Hash.FromAnonymousObject(scriptModel));

            var outputFilePath = outputFolder + @"\Input\" + icao  + ".py";

            File.WriteAllText(outputFilePath, maxScriptFinal);

        }


    }
}
