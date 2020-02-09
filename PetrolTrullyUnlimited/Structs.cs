using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace PetrolTrullyUnlimited
{
    /// <summary>
    /// Basic definition of vehicle.
    /// Type: Type of vehicle
    /// Capacity: Capacity of fuel
    /// Fuel: Acceptable Fuels
    /// </summary>
    struct Vehicle
    {
        public string type { get; set; }
        public float capacity { get; set; }
        public Fuel[] fuel { get; set; }
    }

    /// <summary>
    /// Definition of created vehicle.
    /// ID: Unique ID of vehicle
    /// Type: Type of vehicle
    /// Litres: Litres remaining in the tank
    /// Fuel: Acceptable Fuel
    /// </summary>
    struct VehicleState
    {
        public int id { get; set; }
        public string type { get; set; }
        public float litres { get; set; }
        public Fuel fuel { get; set; }
    }

    /// <summary>
    /// Kinds of fuel available.
    /// Type: Type of fuel
    /// Price: Price per litre.
    /// </summary>
    struct Fuel
    {
        public string type { get; set; }
        public float price { get; set; }
    }

    /// <summary>
    /// Definition of pump.
    /// ID: Unique ID of pump
    /// Fuel: Fuel/s available in the pump
    /// Velocity: Rate of litres per second
    /// </summary>
    struct Pump
    {
        public byte id { get; set; }
        public Fuel[] fuel { get; set; }
        public float velocity { get; set; }
        public bool available { get; set; }
        public byte priority { get; set; }
    }
}
