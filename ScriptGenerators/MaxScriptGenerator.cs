using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XP2AFSAirportConverter.Models;
using XP2AFSAirportConverter.XP;

namespace XP2AFSAirportConverter.ScriptGenerators
{
    public class MaxScriptGenerator : ScriptGenerator
    {
        public override void GenerateScripts(DATFile datFile, DSFFile dsfFile)
        {
            StringBuilder sb = new StringBuilder();

            // Move this to a template - nightmare
            sb.AppendLine("--Set units to meters. 1 unit = 1 meter");
            sb.AppendLine("units.DisplayType = #Metric");
            sb.AppendLine("units.MetricType = #meters");
            sb.AppendLine("units.SystemType = #meters");
            sb.AppendLine("units.SystemScale = 1");


            sb.AppendLine("-- Draw runway");
            sb.AppendLine("macros.run \"Objects Shapes\" \"Rectangle\"");
            sb.AppendLine("Rectangle length:111.555 width:769.232 cornerRadius:0 pos:[0,0,0] isSelected:on");
            sb.AppendLine("$.name = \"Runway1\"");
            sb.AppendLine("select $Runway1");
            sb.AppendLine("toolMode.coordsys #view");
            sb.AppendLine("rotate $ (angleaxis 44.4805 [0,0,1])");

            sb.AppendLine("-- Quadify Runway");
            sb.AppendLine("select $Runway1");
            sb.AppendLine("modPanel.addModToSelection (Quadify_Mesh ()) ui:on");
            sb.AppendLine("$.modifiers[#Quadify_Mesh].quadsize = 16");

        }
    }
}
