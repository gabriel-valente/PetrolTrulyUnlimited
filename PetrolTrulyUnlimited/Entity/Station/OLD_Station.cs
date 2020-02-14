namespace PetrolTrulyUnlimited.Entity
{
    public class Station() : Pump
    {
        public Station()
        {

        public Pump[] Pumps = new Pump[]
        {
            pump0,
            pump1,
            pump2,
            pump3,
            pump4,
            pump5,
            pump6,
            pump7,
            pump8
        };
    }

    private static Pump basePump = new Pump()
    {
        Velocity = Global.PUMP_VELOCITY,
        Available = true
    };

    private static Fuel[] fuelCombo1 = new Fuel[]
    {
        new Diesel(),
        new Gasoline()
    };

    private static Fuel[] fuelCombo2 = new Fuel[]
    {
        new Diesel(),
        new Lpg()
    };

    private Pump pump0 = new Pump()
    {
        Id = 1,
        Fuel = fuelCombo1,
        Velocity = basePump.Velocity,
        Available = basePump.Available,
        Priority = 3
    };

    private Pump pump1 = new Pump()
    {
        Id = 2,
        Fuel = fuelCombo1,
        Velocity = basePump.Velocity,
        Available = basePump.Available,
        Priority = 6
    };

    private Pump pump2 = new Pump()
    {
        Id = 3,
        Fuel = fuelCombo1,
        Velocity = basePump.Velocity,
        Available = basePump.Available,
        Priority = 9
    };

    private Pump pump3 = new Pump()
    {
        Id = 4,
        Fuel = fuelCombo2,
        Velocity = basePump.Velocity,
        Available = basePump.Available,
        Priority = 2
    };

    private Pump pump4 = new Pump()
    {
        Id = 5,
        Fuel = fuelCombo2,
        Velocity = basePump.Velocity,
        Available = basePump.Available,
        Priority = 5
    };

    private Pump pump5 = new Pump()
    {
        Id = 6,
        Fuel = fuelCombo2,
        Velocity = basePump.Velocity,
        Available = basePump.Available,
        Priority = 8
    };

    private Pump pump6 = new Pump()
    {
        Id = 7,
        Fuel = fuelCombo2,
        Velocity = basePump.Velocity,
        Available = basePump.Available,
        Priority = 1
    };

    private Pump pump7 = new Pump()
    {
        Id = 8,
        Fuel = fuelCombo2,
        Velocity = basePump.Velocity,
        Available = basePump.Available,
        Priority = 4
    };

    private Pump pump8 = new Pump()
    {
        Id = 9,
        Fuel = fuelCombo2,
        Velocity = basePump.Velocity,
        Available = basePump.Available,
        Priority = 7
    };
}