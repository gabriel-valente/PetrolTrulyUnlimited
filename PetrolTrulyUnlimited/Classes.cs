using System.Timers;

namespace PetrolTrulyUnlimited
{
    /// <summary>
    /// Basic definition of vehicle.
    /// Type: Type of vehicle
    /// Capacity: Capacity of fuel
    /// Fuel: Acceptable Fuels
    /// </summary>
    public class Vehicle
    {
        private static int autoId;

        private int _id;
        private string _type;
        private float _capacity;
        private float _litres;
        private Fuel[] _fuel;
        private Fuel _fuelType;

        public int Id {
            get
            {
                return _id;
            }

            set
            {
                _id = value;
            }
        }
        public string Type {
            get
            {
                return _type;
            }

            set
            {
                _type = value;
            }
        }
        public float Capacity {
            get
            {
                return _capacity;
            }

            set
            {
                _capacity = value;
            }
        }
        public float Litres {
            get
            {
                return _litres;
            }

            set
            {
                _litres = value;
            }
        }
        public Fuel[] Fuel {
            get
            {
                return _fuel;
            }

            set
            {
                _fuel = value;
            }
        }
        public Fuel FuelType {
            get
            {
                return _fuelType;
            }

            set
            {
                _fuelType = value;
            }
        }

        public Vehicle() { }

        public static Vehicle SetVehicle(Vehicle vehicle, float litres, Fuel fuelType)
        {
            Vehicle thisVehicle = new Vehicle
            {
                _id = autoId++,
                _type = vehicle.Type,
                _capacity = vehicle.Capacity,
                _litres = litres,
                _fuel = vehicle.Fuel,
                _fuelType = fuelType
            };

            return thisVehicle;
        }
    }

    /// <summary>
    /// Kinds of fuel available.
    /// Type: Type of fuel
    /// Price: Price per litre.
    /// </summary>
    public class Fuel
    {
        private string _type;
        private float _price;

        public string Type {
            get
            {
                return _type;
            }

            set
            {
                _type = value;
            }
        }
        public float Price {
            get
            {
                return _price;
            }

            set
            {
                _price = value;
            }
        }

        public Fuel() { }

        public Fuel(string type, float price)
        {
            this._type = type;
            this._price = price;
        }
    }

    /// <summary>
    /// Definition of pump.
    /// ID: Unique ID of pump
    /// Fuel: Fuel/s available in the pump
    /// Velocity: Rate of litres per second
    /// </summary>
    public class Pump
    {
        private byte _id;
        private Fuel[] _fuel;
        private float _velocity;
        private bool _available;
        private byte _priority;
        private Timer _fuelingTimer = new Timer();

        public byte Id {
            get
            {
                return _id;
            }

            set
            {
                _id = value;
            }
        }
        public Fuel[] Fuel {
            get
            {
                return _fuel;
            }

            set
            {
                _fuel = value;
            }
        }
        public float Velocity {
            get
            {
                return _velocity;
            }

            set
            {
                _velocity = value;
            }
        }
        public bool Available {
            get
            {
                return _available;
            }

            set
            {
                _available = value;
            }
        }
        public byte Priority {
            get
            {
                return _priority;
            }

            set
            {
                _priority = value;
            }
        }
        public Timer FuelingTimer {
            get
            {
                return _fuelingTimer;
            }

            set
            {
                _fuelingTimer = value;
            }
        }

        public Pump() { }
    }
}
