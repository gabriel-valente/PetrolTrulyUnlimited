namespace PetrolTrulyUnlimited.Entity
{
    public class Station : Pump
    {
        //Basic information of a pump
        private static readonly Pump basePump = new Pump()
        {
            Velocity = Global.PUMP_VELOCITY,
            Available = true
        };

        //First combination of fuels
        private static Fuel[] fuelCombo1 = new Fuel[]
        {
                new Diesel(),
                new Gasoline()
        };

        //Second combination of fuels
        private static Fuel[] fuelCombo2 = new Fuel[]
        {
                new Diesel(),
                new Lpg()
        };
        /* Multiple pump definitions
         * First 3 pumps has the first fuel combo
         * Rest of pumps have the second fuel combo
         */
        private static Pump _pump0 = new Pump()
        {
            Id = 1,
            Fuel = fuelCombo1,
            Velocity = basePump.Velocity,
            Available = basePump.Available,
            Priority = 3
        };

        private static Pump _pump1 = new Pump()
        {
            Id = 2,
            Fuel = fuelCombo1,
            Velocity = basePump.Velocity,
            Available = basePump.Available,
            Priority = 6
        };

        private static Pump _pump2 = new Pump()
        {
            Id = 3,
            Fuel = fuelCombo1,
            Velocity = basePump.Velocity,
            Available = basePump.Available,
            Priority = 9
        };

        private static Pump _pump3 = new Pump()
        {
            Id = 4,
            Fuel = fuelCombo2,
            Velocity = basePump.Velocity,
            Available = basePump.Available,
            Priority = 2
        };

        private static Pump _pump4 = new Pump()
        {
            Id = 5,
            Fuel = fuelCombo2,
            Velocity = basePump.Velocity,
            Available = basePump.Available,
            Priority = 5
        };

        private static Pump _pump5 = new Pump()
        {
            Id = 6,
            Fuel = fuelCombo2,
            Velocity = basePump.Velocity,
            Available = basePump.Available,
            Priority = 8
        };

        private static Pump _pump6 = new Pump()
        {
            Id = 7,
            Fuel = fuelCombo2,
            Velocity = basePump.Velocity,
            Available = basePump.Available,
            Priority = 1
        };

        private static Pump _pump7 = new Pump()
        {
            Id = 8,
            Fuel = fuelCombo2,
            Velocity = basePump.Velocity,
            Available = basePump.Available,
            Priority = 4
        };

        private static Pump _pump8 = new Pump()
        {
            Id = 9,
            Fuel = fuelCombo2,
            Velocity = basePump.Velocity,
            Available = basePump.Available,
            Priority = 7
        };

        //Seting up the pumps array
        public static Pump[] Pumps = new Pump[]
        {
            _pump0,
            _pump1,
            _pump2,
            _pump3,
            _pump4,
            _pump5,
            _pump6,
            _pump7,
            _pump8
        };
    }
}