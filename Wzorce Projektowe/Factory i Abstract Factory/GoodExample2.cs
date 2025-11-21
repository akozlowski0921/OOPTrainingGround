using System;

namespace DesignPatterns.Factory.Good2
{
    // ✅ GOOD: Factory Method pattern

    public interface IVehicle
    {
        void Drive();
    }

    public abstract class VehicleFactory
    {
        public abstract IVehicle CreateVehicle();

        public void ProcessVehicle()
        {
            var vehicle = CreateVehicle();
            vehicle.Drive();
        }
    }

    public class CarFactory : VehicleFactory
    {
        public override IVehicle CreateVehicle() => new Car();
    }

    public class BikeFactory : VehicleFactory
    {
        public override IVehicle CreateVehicle() => new Bike();
    }

    class Car : IVehicle
    {
        public void Drive() => Console.WriteLine("Driving car");
    }

    class Bike : IVehicle
    {
        public void Drive() => Console.WriteLine("Riding bike");
    }
}
