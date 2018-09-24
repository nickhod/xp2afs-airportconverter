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
        public override void GenerateScripts(string icao, DATFile datFile, DSFFile dsfFile, TSCFile tscFile, string outputFolder, string texturesFolder)
        {
            this.icao = icao;
            this.datFile = datFile;
            this.dsfFile = dsfFile;
            this.tscFile = tscFile;

            var maxScriptFilePath = AppDomain.CurrentDomain.BaseDirectory + "\\ScriptGenerators\\ScriptTemplates\\MaxScript.liquid";
            var maxScript = File.ReadAllText(maxScriptFilePath);

            var scriptModel = new ScriptModel();
            this.CalculateRunways(scriptModel);
            this.CalculateDATFilePavements(scriptModel);
            this.CalculateDSFFileBuildings(scriptModel);
            this.CalculateAirportBoundary(scriptModel);

            scriptModel.AirportName = datFile.AirportHeader.Name;
            scriptModel.AirportICAO = datFile.AirportHeader.ICAOCode;
            scriptModel.GeneratedOn = DateTime.UtcNow;
            scriptModel.SaveFilePath = (outputFolder + @"\Input\" + icao + ".max").Replace(@"\", @"\\"); ;
            scriptModel.TexturesPath = (texturesFolder + @"\").Replace(@"\", @"\\");

            Template template = Template.Parse(maxScript);
            Template.NamingConvention = new CSharpNamingConvention();
            var maxScriptFinal = template.Render(Hash.FromAnonymousObject(scriptModel));

            var outputFilePath = outputFolder + @"\Input\" + icao  + ".py";

            File.WriteAllText(outputFilePath, maxScriptFinal);

        }


    }
}
