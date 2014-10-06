﻿using System;
using System.Linq;

namespace BouvetCodeCamp.SpillOppretter
{
    class Program
    {
        static void Main(string[] args)
        {
            var mapdataConverter = new JSONKartdataConverter();

            Console.WriteLine("Initializing Document Db");
            var kartdataLagring = new KartdataLagring();
            var lagoppretter = new LagOppretter(15);

            Console.WriteLine("Converting data and saving to database");

            var mapdata = mapdataConverter.KonverterKartdata().ToList();

            kartdataLagring.SlettAlleKartdata();
            var poster = kartdataLagring.LagreKartdata(mapdata);

            Console.WriteLine("Done processing {0} map data points", mapdata.Count);

            Console.WriteLine("Oppretter lag");
            lagoppretter.OpprettLag(poster);

            Console.WriteLine("\r\nPress any key to exit...");
            Console.ReadLine();
        }
    }
}