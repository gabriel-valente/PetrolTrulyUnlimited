namespace PetrolTrulyUnlimited.Entity
{
    class Car : Vehicle
    {
        public Car() : base() {
            Type = "Car";
            Capacity = 40.0f;
            Fuel = new Fuel[]
            {
                new Diesel(),
                new Lpg(), 
                new Gasoline()
            };
        }
    }
}
