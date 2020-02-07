using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetrolTrullyUnlimited
{
    class Objects
    {
        //Definition of fuels

        static Fuel diesel = new Fuel()
        {
            type = "Diesel",
            price = 1.329f
        };

        static Fuel lpg = new Fuel()
        {
            type = "LPG",
            price = 0.639f
        };

        static Fuel gasoline = new Fuel()
        {
            type = "Gasoline",
            price = 1.273f
        };


        //Definitions of vehicles

        static public Vehicle car = new Vehicle()
        {
            type = "Car",
            capacity = 40.0f,
            fuel = new Fuel[] { diesel, lpg, gasoline }
        };

        static public Vehicle van = new Vehicle()
        {
            type = "Van",
            capacity = 80.0f,
            fuel = new Fuel[] { diesel, lpg }
        };

        static public Vehicle lorry = new Vehicle()
        {
            type = "Lorry",
            capacity = 150.0f,
            fuel = new Fuel[] { diesel }
        };

        //Definition of Global Variable

        static public List<VehicleState> vehiclesQueue = new List<VehicleState>();
    }
}
