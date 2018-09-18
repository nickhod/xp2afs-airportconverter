using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XP2AFSAirportConverter.Models;

namespace XP2AFSAirportConverter.XP
{
    public class DSFFileLoader
    {
        private readonly ILog log = LogManager.GetLogger("XP2AFSAirportConverter");

        public DSFFile GetDSFFileFromXPZip(string icaoCode, string xpZipFilename)
        {
            // Fun and games, the DSF file is in the hierarchy
            // ICAO.zip
            //   KORD.txt (this file might be present, but may not be)
            //   ICAO_Scenery_Pack.zip
            //     ICAO_Scenery_Pack (dir)
            //       Earth nav data
            //         -60-080 (this is not fixed)
            //           -52-073.dsf (this is not fixed)

            byte[] dsfFileData = null;
            var dsfAsText = "";

            using (var fileStream = new FileStream(xpZipFilename, FileMode.Open, FileAccess.Read))
            {
                using (var zipFile = new ZipFile(fileStream))
                {
                    var dsfTextFile = icaoCode + ".txt";
                    var dsfTextFileZipEntry = zipFile.GetEntry(dsfTextFile);

                    if (dsfTextFileZipEntry != null)
                    {
                        using (Stream dsfTextFileStream = zipFile.GetInputStream(dsfTextFileZipEntry))
                        {
                            using (MemoryStream dsfTextFileMemoryStream = new MemoryStream())
                            {
                                byte[] buffer = new byte[4096];
                                StreamUtils.Copy(dsfTextFileStream, dsfTextFileMemoryStream, buffer);
                                dsfFileData = dsfTextFileMemoryStream.ToArray();
                                dsfAsText = Encoding.UTF8.GetString(dsfFileData, 0, dsfFileData.Length);
                            }
                        }
                    }
                    else
                    {
                        var innerZipName = String.Format("{0}_Scenery_Pack.zip", icaoCode);
                        var zipEntry = zipFile.GetEntry(innerZipName);
                        if (zipEntry == null)
                        {
                            log.ErrorFormat("{0} not found in zip", innerZipName);
                        }


                        using (var stream = zipFile.GetInputStream(zipEntry))
                        {
                            // The stream above is not seekable. We need to copy it to a memory
                            // stream first, then we can instantiate another ZipFile object from
                            // the memory stream
                            using (var memoryStream = new MemoryStream())
                            {
                                stream.CopyTo(memoryStream);
                                memoryStream.Position = 0;

                                using (var innerZipFile = new ZipFile(memoryStream))
                                {
                                    foreach (ZipEntry innerZipEntry in innerZipFile)
                                    {
                                        if (zipEntry.IsDirectory)
                                            continue;

                                        //log.Debug(innerZipEntry.Name);

                                        if (innerZipEntry.Name.Contains(".dsf"))
                                        {
                                            using (Stream dsfFileStream = innerZipFile.GetInputStream(innerZipEntry))
                                            {
                                                // Get the bytes of the dsf file
                                                using (MemoryStream dsfFileMemoryStream = new MemoryStream())
                                                {
                                                    byte[] buffer = new byte[4096];
                                                    StreamUtils.Copy(dsfFileStream, dsfFileMemoryStream, buffer);
                                                    dsfFileData = dsfFileMemoryStream.ToArray();

                                                    var dsf2TextManager = new DSF2TextManager();
                                                    dsfAsText = dsf2TextManager.GetTextDSFFile(dsfFileData, XP2AFSConverterManager.Settings);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }


            var dsfTextFileParser = new DSFTextFileParser();
            var dsfFile = dsfTextFileParser.ParseFromString(dsfAsText);

            dsfAsText = null;
            dsfFileData = null;
            return dsfFile;
        }

    }
}
