namespace PetrolTrulyUnlimited.Entity
{
    class Car : Vehicle
    {
        public Car() : base() {
            Type = "Car";
            Capacity = 40.0f;
            Fuel = SetFuelType(Global.CAR_FUEL);
        }
    }
}
