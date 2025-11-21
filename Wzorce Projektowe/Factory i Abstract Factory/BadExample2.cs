using System;

namespace DesignPatterns.Factory.Bad2
{
    // ❌ BAD: Switch w wielu miejscach

    public enum VehicleType { Car, Bike, Truck }

    public class VehicleService
    {
        public void Process(VehicleType type)
        {
            // ❌ Duplikacja switch logic
            switch (type)
            {
                case VehicleType.Car:
                    var car = new Car();
                    break;
                case VehicleType.Bike:
                    var bike = new Bike();
                    break;
            }
        }

        public void Another Method(VehicleType type)
        {
            // ❌ Ten sam switch znowu
            switch (type)
            {
                case VehicleType.Car:
                    Console.WriteLine("Car");
                    break;
            }
        }
    }

    class Car { }
    class Bike { }
}
