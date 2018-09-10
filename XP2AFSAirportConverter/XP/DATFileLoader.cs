using ICSharpCode.SharpZipLib.Zip;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XP2AFSAirportConverter.XP
{
    public class DATFileLoader
    {
        private readonly ILog log = LogManager.GetLogger("XP2AFSAirportConverter");

        public DATFile GetDATFileFromXPZip(string icaoCode, string xpZipFilename)
        {
            string datFileData = "";

            using (var fileStream = new FileStream(xpZipFilename, FileMode.Open, FileAccess.Read))
            {
                using (var zipFile = new ZipFile(fileStream))
                {
                    var zipEntry = zipFile.GetEntry(icaoCode + ".dat");
                    if (zipEntry == null)
                    {
                        log.ErrorFormat("{0}.dat not found in zip", icaoCode);
                    }

                    using (var stream = zipFile.GetInputStream(zipEntry))
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            datFileData = reader.ReadToEnd();
                        }
                    }
                }
            }

            var datFileParser = new DATFileParser();
            var datFile = datFileParser.ParseFromString(datFileData);
            return datFile;
        }

    }
}
