using PetrolTrulyUnlimited.Entity;
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

        public static Fuel[] SetFuelType(string fuel)
        {
            char[] combo = fuel.ToCharArray();

            Fuel[] fuels = new Fuel[combo.Length];

            for (int i = 0; i < combo.Length; i++)
            {
                if (combo[i] == '0')
                {
                    fuels[i] = new Diesel();
                }
                else if (combo[i] == '1')
                {
                    fuels[i] = new Gasoline();
                }
                else if (combo[i] == '2')
                {
                    fuels[i] = new Lpg();
                }
            }

            return fuels;
        }

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

        /// <summary>
        /// Id of the pump.
        /// </summary>
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
        /// <summary>
        /// Types of fuel the pump has.
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
        /// Velocity that the pump dispenses the fuel.
        /// </summary>
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
        /// <summary>
        /// If the pump is available or occupied.
        /// </summary>
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
        /// <summary>
        /// Higher priority means a better choice.
        /// </summary>
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
        /// <summary>
        /// Fuelling Timer.
        /// </summary>
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

    /// <summary>
    /// Basic definition of the QueueInformation popup.
    /// </summary>
    public class QueueInformation
    {
        private int _totalVehicles; //Total number of vehicles that went through
        private int _vehiclesEntered; //Vehicles that went to fuelling
        private int _vehiclesRejected; //Vehicles that went out without fuelling

        /// <summary>
        /// Total number od vehicles that went through.
        /// </summary>
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
        /// <summary>
        /// Vehicles that went to fuelling.
        /// </summary>
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
        /// <summary>
        /// Vehicles that went out without fuelling.
        /// </summary>
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
    
    /// <summary>
    /// Basic definition of the receipt.
    /// </summary>
    public class Receipt
    {
        private string _vehicleType; //Type of the vehicle fuelled
        private float _litres; //Litres fuelled
        private int _pumpId; //Pump the vehicle was in
        private Fuel _fuel = new Fuel("", 0f); //Type of fuel used
        private float _cost; //Cost of litres

        /// <summary>
        /// Type of the vehicle fuelled.
        /// </summary>
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
        /// <summary>
        /// Litres fuelled.
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
        /// Pump the vehicle was in.
        /// </summary>
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
        /// <summary>
        /// Type of fuel.
        /// </summary>
        public Fuel Fuel
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
        /// Cost of litres.
        /// </summary>
        public float Cost
        {
            get
            {
                return _cost;
            }

            set
            {
                _cost = value;
            }
        }

        public Receipt() { }
    }

    /// <summary>
    /// Basic information for the counters of the pump.
    /// </summary>
    public class PumpInformation
    {
        private float[] _litresDispensed = { 0f, 0f, 0f }; //Litres dispensed for each fuel
        private float[] _amountWon = { 0f, 0f, 0f }; //Amout of money won for each fuel
        private float _commission = 0f; //Commission of that money
        private int[] _vehicleCounter = { 0, 0, 0 }; //Vehicles that went through the pump
        private int _notFullVehicle = 0; //Vehicles that did not totally filled the tank
        private Receipt _receipt = new Receipt(); //Current Receipt

        /// <summary>
        /// Litres dispensed for each type of fuel
        /// </summary>
        public float[] LitresDispensed
        {
            get
            {
                return _litresDispensed;
            }

            set
            {
                _litresDispensed = value;
            }
        }
        /// <summary>
        /// Amount of money won for each type of fuel
        /// </summary>
        public float[] AmountWon
        {
            get
            {
                return _amountWon;
            }

            set
            {
                _amountWon = value;
            }
        }
        /// <summary>
        /// Commission amount of that money
        /// </summary>
        public float Commission
        {
            get
            {
                return _commission;
            }

            set
            {
                _commission = value;
            }
        }
        /// <summary>
        /// Vehicles that went through the pump
        /// </summary>
        public int[] VehiclesCounter
        {
            get
            {
                return _vehicleCounter;
            }

            set
            {
                _vehicleCounter = value;
            }
        }
        /// <summary>
        /// Vehicles that did not totally filled the tank
        /// </summary>
        public int NotFullVehicle
        {
            get
            {
                return _notFullVehicle;
            }

            set
            {
                _notFullVehicle = value;
            }
        }
        /// <summary>
        /// Current Receipt
        /// </summary>
        public Receipt Receipt
        {
            get
            {
                return _receipt;
            }

            set
            {
                _receipt = value;
            }
        }

        public PumpInformation() { }
    }
}
