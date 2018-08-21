using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XP2AFSAirportConverter.AFS
{
    public class TMCFile
    {
        public string BaseOutputFolder { get; set; }
        public string OutputFolder { get; set; }
        public string InputFolder { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<[file][][]");
            sb.AppendLine("  <[tm_config][][]");
            sb.AppendLine("");
            sb.AppendLine("    <[string8][base_output_folder][]>");
            sb.AppendLine("    <[string8][texture_base_type][ttx_dxt]>");
            sb.AppendLine("");
            sb.AppendLine("    <[list_tm_config_folderpair][folder_pairs][]");
            sb.AppendLine("      <[tm_config_folderpair][element][1]");
            sb.AppendLine("        <[string8][input_folder][./]>");
            sb.AppendLine("        <[string8][output_folder][scenery/places/kigm_kingman/]>");
            sb.AppendLine("        <[string8][type][place]>");
            sb.AppendLine("        <[uint32][recurse_level][0]>");
            sb.AppendLine("        <[list_string8][file_types][tsc tgi jpg bmp tif png ]>");
            sb.AppendLine("        <[list_tm_texture_settings][texture_settings][]");
            sb.AppendLine("          <[tm_config_folderpair][element][0]");
            sb.AppendLine("            <[list_string8][regex][.*]>");
            sb.AppendLine("            <[bool][compressed][true]>");
            sb.AppendLine("            <[bool][compress_file][true]>");
            sb.AppendLine("            <[bool][flip_vertical][false]>");
            sb.AppendLine("            <[bool][mipmaps][true]>");
            sb.AppendLine("            <[uint][max_size][2048]>");
            sb.AppendLine("            <[bool][make_square][true]>");
            sb.AppendLine("          >");
            sb.AppendLine("        >");
            sb.AppendLine("        <[tm_config_geometry_settings][geometry_settings][]");
            sb.AppendLine("          <[float32][collision_mesh_quality][0]>");
            sb.AppendLine("        >");
            sb.AppendLine("      >");
            sb.AppendLine("    >");
            sb.AppendLine("  >");
            sb.AppendLine(">");


            return sb.ToString();
        }
    }
}
