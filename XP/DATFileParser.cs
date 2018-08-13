using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XP2AFSAirportConverter.XP
{
    public class DATFileParser
    {
        //https://developer.x-plane.com/article/airport-data-apt-dat-file-format-specification
        //http://developer.x-plane.com/wp-content/uploads/2015/11/XP-APT1000-Spec.pdf
        //http://developer.x-plane.com/wp-content/uploads/2015/11/XP-APT850-Spec.pdf

        /*
        1	    Land airport header	
        16	    Seaplane base header	
        17	    Heliport header	
        100	    Runway	
        101	    Water runway	
        102	    Helipad	
        110	    Pavement (taxiway or ramp) header	 Must form a closed loop.
        120	    Linear feature (painted line or light string) header	Can form closed loop or simple string
        130	    Airport boundary header	Must form a closed loop
        111	    Node	All nodes can also include a “style” (line or lights)
        112	    Node with Bezier control point	Bezier control points define smooth curves
        113	    Node with implicit close of loop	Implied join to first node in chain
        114	    Node with Bezier control point, with implicit close of loop	Implied join to first node in chain
        115	    Node terminating a string (no close loop)	No “styles” used
        116	    Node with Bezier control point, terminating a string (no close loop)	No “styles” used
        14	    Airport viewpoint	One or none for each airport
        15	    Aeroplane startup location	*** Convert these to new row code 1300 ***
        18	    Airport light beacon	One or none for each airport
        19	    Windsock	Zero, one or many for each airport
        20	    Taxiway sign (inc. runway distance-remaining signs)	Zero, one or many for each airport
        21	    Lighting object (VASI, PAPI, Wig-Wag, etc.)	Zero, one or many for each airport
        1000	Airport traffic flow	Zero, one or many for an airport. Used if following rules met (rules of same type use ‘or’ logic, rules of a different type use ‘and’ logic). First flow to pass all rules is used.
        1001	Traffic flow wind rule	Zero, one or many for a flow. Multiple rules use ‘or’ logic.
        1002	Traffic flow minimum ceiling rule	Zero or one rule for each flow
        1003	Traffic flow minimum visibility rule	Zero or one rule for each flow
        1004	Traffic flow time rule	Zero, one or many for a flow. Multiple rules use ‘or’ logic.
        1100	Runway-in-use arrival/departure constraints	First constraint met is used. Sequence matters!
        1101	VFR traffic pattern	Zero or one pattern for each traffic flow
        1200	Header indicating that taxi route network data follows
        1201	Taxi route network node	Sequencing must be 0 based, ascending by ID. Must be part of one or more edges.
        1202	Taxi route network edge	Must connect two nodes. Also takes one of 6 sizes (A-F).
        1204	Taxi route edge active zone	Can refer to up to 4 runway ends
        1300	Airport location (deprecates code 15)	Not explicitly connected to taxi route network
        1301	Ramp start metadata.	Includes width, operations type, equipment type, & airlines.
        1302	Metadata records	Zero or many for each airport.
        1400	Truck Parking Location	Not explicitly connected to taxi route network.
        1401	Truck Destination Location	Not explicitly connected to taxi route network.
        50 – 56	Communication frequencies	Zero, one or many for each airport
        */

        public DATFile Parse(string data)
        {
            DATFile datFile = new DATFile();

            StringReader strReader = new StringReader(data);
            while (true)
            {
                var line = strReader.ReadLine();
                if (line != null)
                {
                    int i = 0;
                }
                else
                {
                    break;

                }
            }

            return datFile;
        }

    }
}
