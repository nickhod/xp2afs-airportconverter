﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XP2AFSAirportConverter.Models;
using XP2AFSAirportConverter.Common;
using log4net;
using System.IO;

namespace XP2AFSAirportConverter
{
    // https://developer.x-plane.com/article/dsf-file-format-specification/

    // Decided to use XPTools to convert the DSF into text first
    // May implement a binary DSF parser later
    public class DSFFileParser
    {
        private DSFFile dsfFile;
        private readonly ILog log = LogManager.GetLogger("XP2AFSAirportConverter");


        public DSFFile ParseFromBytes(byte[] dsfFileData)
        {
            this.dsfFile = new DSFFile();

            var header = dsfFileData.SubArray(0, 8);
            var headerStr = Encoding.ASCII.GetString(header);

            if (headerStr == "XPLNEDSF")
            {
                var versionBytes = dsfFileData.SubArray(8, 4);
                int version = BitConverter.ToInt32(versionBytes, 0);

                // Start reading atoms from position 12
                int position = 12;


                var firstAtom = dsfFileData.SubArray(12, 8);
                var atomTypeBytes = firstAtom.SubArray(0, 4);
                var atomType = Encoding.ASCII.GetString(EndianUtilities.Swap(atomTypeBytes));


                using (var stream = new MemoryStream(dsfFileData))
                {
                    stream.Position = position;

                    //stream.re
                }

            }
            else
            {
                log.Error("Invalid DSF File");
            }

            return this.dsfFile;
        }

        private void ParseDefinitionsAtom()
        {

        }

        private void ParseObjecctsAtom()
        {

        }

        private void ParsePolygonsAtom()
        {

        }


    }
}
