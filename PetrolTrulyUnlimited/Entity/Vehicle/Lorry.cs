namespace PetrolTrulyUnlimited.Entity
{
    class Lorry : Vehicle
    {
        public Lorry() : base() {
            Type = "Lorry";
            Capacity = 150.0f;
            Fuel = new Fuel[]
            {
                new Diesel()
            };
        }
    }
}
