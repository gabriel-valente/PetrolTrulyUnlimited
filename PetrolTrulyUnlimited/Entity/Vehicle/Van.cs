namespace PetrolTrulyUnlimited.Entity
{
    class Van : Vehicle
    {
        public Van() : base() {
            Type = "Van";
            Capacity = 80.0f;
            Fuel = new Fuel[]
            {
                new Diesel(),
                new Lpg()
            };
        }
    }
}
