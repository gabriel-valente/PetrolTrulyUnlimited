using System.Timers;

namespace PetrolTrulyUnlimited
{
    /// <summary>
    /// Basic definition of a vehicle.
    /// </summary>
    public class Vehicle
    {
        private static int autoId; //Auto increment Id

        private int _id; //Id of the vehicle
        private string _type; //Type of vehicle
        private float _capacity; //Total capacity of the tank
        private float _litres; //Current litres in the tank
        private Fuel[] _fuel; //Possible fuel types
        private Fuel _fuelType; //Fuel type of the current vehicle
        private Timer _serviceTimer = new Timer(); //Timer for vehicle to leave if is not in the pump

        /// <summary>
        /// Id of current vehicle.
        /// </summary>
        public int Id 
        {
            get
            {
                return _id;
            }

            set
            {
                _id = value;
            }
        }
        /// <summary>
        /// Type of vehicle.
        /// </summary>
        public string Type 
        {
            get
            {
                return _type;
            }

            set
            {
                _type = value;
            }
        }
        /// <summary>
        /// Total capacity of the tank.
        /// </summary>
        public float Capacity 
        {
            get
            {
                return _capacity;
            }

            set
            {
                _capacity = value;
            }
        }
        /// <summary>
        /// Litres remaining in the tank.
        /// </summary>
        public float Litres 
        {
            get
            {
                return _litres;
            }

            set
            {
                _litres = value;
            }
        }
        /// <summary>
        /// Possible types of fuel for the vehicle.
        /// </summary>
        public Fuel[] Fuel 
        {
            get
            {
                return _fuel;
            }

            set
            {
                _fuel = value;
            }
        }
        /// <summary>
        /// Type of fuel od the current vehicle.
        /// </summary>
        public Fuel FuelType 
        {
            get
            {
                return _fuelType;
            }

            set
            {
                _fuelType = value;
            }
        }
        /// <summary>
        /// Timer for the vehicle to leave if is not in the pump.
        /// </summary>
        public Timer ServiceTimer
        {
            get
            {
                return _serviceTimer;
            }

            set
            {
                _serviceTimer = value;
            }
        }

        public Vehicle() { }

        /// <summary>
        /// Get the next vehicle Id.
        /// </summary>
        /// <returns>Vehicle Id.</returns>
        public static int GetId()
        {
            return ++autoId;
        }

        /// <summary>
        /// Creates a new vehicle.
        /// </summary>
        /// <param name="Id">Id of the vehicle.</param>
        /// <param name="vehicle">Blueprint of vehicle.</param>
        /// <param name="litres">Remaining litres int the tank.</param>
        /// <param name="fuelType">Fuel that the vehicle uses.</param>
        /// <param name="serviceTimer">Timer to leave if is not in the pump.</param>
        /// <returns>Configured vehicle.</returns>
        public static Vehicle SetVehicle(int Id, Vehicle vehicle, float litres, Fuel fuelType, Timer serviceTimer)
        {
            Vehicle thisVehicle = new Vehicle
            {
                _id = Id,
                _type = vehicle.Type,
                _capacity = vehicle.Capacity,
                _litres = litres,
                _fuel = vehicle.Fuel,
                _fuelType = fuelType,
                _serviceTimer = serviceTimer
            };

            thisVehicle.ServiceTimer.Start();

            return thisVehicle;
        }
    }

    /// <summary>
    /// Basic definition of a fuel.
    /// </summary>
    public class Fuel
    {
        private string _type; //Name of the Fuel
        private float _price; //Current price of the fuel per litre

        /// <summary>
        /// Name of the fuel.
        /// </summary>
        public string Type 
        {
            get
            {
                return _type;
            }

            set
            {
                _type = value;
            }
        }
        /// <summary>
        /// Price of the fuel per litre.
        /// </summary>
        public float Price 
        {
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

        /// <summary>
        /// Sets the fuel definition.
        /// </summary>
        /// <param name="type">Name of the fuel.</param>
        /// <param name="price">Price of the fuel per litre.</param>
        public Fuel(string type, float price)
        {
            this._type = type;
            this._price = price;
        }
    }

    /// <summary>
    /// Basic definition of a pump.
    /// </summary>
    public class Pump
    {
        private byte _id; //Id of the pump
        private Fuel[] _fuel; //Fuel types that the pump has
        private float _velocity; //Velocity that the pump dispenses the fuel
        private bool _available; //If the pump is available or occupied
        private byte _priority; //Higher priority means a better choice
        private Timer _fuelingTimer = new Timer(); //Fuelling timer

        public byte Id 
        {
            get
            {
                return _id;
            }

            set
            {
                _id = value;
            }
        }
        public Fuel[] Fuel 
        {
            get
            {
                return _fuel;
            }

            set
            {
                _fuel = value;
            }
        }
        public float Velocity 
        {
            get
            {
                return _velocity;
            }

            set
            {
                _velocity = value;
            }
        }
        public bool Available 
        {
            get
            {
                return _available;
            }

            set
            {
                _available = value;
            }
        }
        public byte Priority 
        {
            get
            {
                return _priority;
            }

            set
            {
                _priority = value;
            }
        }
        public Timer FuelingTimer 
        {
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

    public class QueueInformation
    {
        private int _totalVehicles;
        private int _vehiclesEntered;
        private int _vehiclesRejected;

        public int TotalVehicles
        { 
            get
            {
                return _totalVehicles;
            }

            set
            {
                _totalVehicles = value;
            }
        }
        public int VehiclesEntered 
        {
            get
            {
                return _vehiclesEntered;
            }

            set
            {
                _vehiclesEntered = value;
            }
        }
        public int VehiclesRejected 
        {
            get
            {
                return _vehiclesRejected;
            }

            set
            {
                _vehiclesRejected = value;
            }
        }

        public QueueInformation() { }
    }

    public class Receipt
    {
        private string _vehicleType;
        private float _litres;
        private int _pumpId;

        public string VehicleType
        {
            get
            {
                return _vehicleType;
            }

            set
            {
                _vehicleType = value;
            }
        }
        public float Litres
        {
            get
            {
                return _litres;
            }

            set
            {
                _litres = value;
            }
        }
        public int PumpId
        {
            get
            {
                return _pumpId;
            }

            set
            {
                _pumpId = value;
            }
        }

        public Receipt() { }
    }

    public class PumpInformation
    {
        private float[] _litresDispensed;
        private float[] _amountWon;
        private float _commission;
        private int _vehicleCounter;
        private int _notFullVehicle;
        private Receipt _receipt;
    }
}
