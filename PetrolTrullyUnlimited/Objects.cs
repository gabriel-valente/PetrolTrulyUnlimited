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
            fuel = new Fuel[]
            {
                diesel,
                lpg,
                gasoline
            }
        };

        static public Vehicle van = new Vehicle()
        {
            type = "Van",
            capacity = 80.0f,
            fuel = new Fuel[]
            {
                diesel,
                lpg
            }
        };

        static public Vehicle lorry = new Vehicle()
        {
            type = "Lorry",
            capacity = 150.0f,
            fuel = new Fuel[]
            {
                diesel
            }
        };

        //Definitions of pumps
        static public Pump[] pumps = new Pump[]
        {
            new Pump()
            {
                id = 1,
                fuel =
                new Fuel[]
                {
                    diesel,
                    gasoline,
                },
                velocity = PUMP_VELOCITY,
                available = true,
                priority = 3
            },
            new Pump()
            {
                id = 2,
                fuel =
                new Fuel[]
                {
                    diesel,
                    gasoline,
                },
                velocity = PUMP_VELOCITY,
                available = true,
                priority = 6
            },
            new Pump()
            {
                id = 3,
                fuel =
                new Fuel[]
                {
                    diesel,
                    gasoline,
                },
                velocity = PUMP_VELOCITY,
                available = true,
                priority = 9
            },
            new Pump()
            {
                id = 4,
                fuel =
                new Fuel[]
                {
                    diesel,
                    lpg
                },
                velocity = PUMP_VELOCITY,
                available = true,
                priority = 2
            },
            new Pump()
            {
                id = 5,
                fuel =
                new Fuel[]
                {
                    diesel,
                    lpg
                },
                velocity = PUMP_VELOCITY,
                available = true,
                priority = 5
            },
            new Pump()
            {
                id = 6,
                fuel =
                new Fuel[]
                {
                    diesel,
                    lpg
                },
                velocity = PUMP_VELOCITY,
                available = true,
                priority = 8
            },
            new Pump()
            {
                id = 7,
                fuel =
                new Fuel[]
                {
                    diesel,
                    lpg
                },
                velocity = PUMP_VELOCITY,
                available = true,
                priority = 1
            },
            new Pump()
            {
                id = 8,
                fuel =
                new Fuel[]
                {
                    diesel,
                    lpg
                },
                velocity = PUMP_VELOCITY,
                available = true,
                priority = 4
            },
            new Pump()
            {
                id = 9,
                fuel =
                new Fuel[]
                {
                    diesel,
                    lpg
                },
                velocity = PUMP_VELOCITY,
                available = true,
                priority = 7
            },
        };

        //Constants
        public const int MIN_SPAWN_TIME = 1500;
        public const int MAX_SPAWN_TIME = 2200;

        public const int MIN_SERVICE_TIME = 1000;
        public const int MAX_SERVICE_TIME = 2000;

        private const float PUMP_VELOCITY = 1.5f;

        public const byte LOWEST_PRIORITY_PUMP = 6;
    }
}
