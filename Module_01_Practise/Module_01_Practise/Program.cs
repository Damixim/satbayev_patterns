using System;
using System.Collections.Generic;



// --------------------------------------- [ 1 ] ---------------------------------------

public class Vehicle
{
    public string Brand;
    public string Model;
    public int ReleaseYear;

    public Vehicle(string brand, string model, int releaseYear)
    {
        Brand = brand;
        Model = model;
        ReleaseYear = releaseYear;
    }

    public virtual void StartEngine()
    {
        Console.WriteLine($"{Brand} {Model}: двигатель успешно запущен.");
    }

    public virtual void StopEngine()
    {
        Console.WriteLine($"{Brand} {Model}: двигатель успешно остановлен.");
    }
}



// --------------------------------------- [ 2 ] ---------------------------------------

public class Car : Vehicle
{
    public int DoorsCount;
    public string TransmissionType;

    public Car(string brand, string model, int releaseYear, int doorsCount, string transmissionType) : base(brand, model, releaseYear)
    {
        DoorsCount = doorsCount;
        TransmissionType = transmissionType;
    }
}



public class Motorcycle : Vehicle
{
    public string BodyType;
    public bool HasBox;

    public Motorcycle(string brand, string model, int releaseYear, string bodyType, bool hasBox) : base(brand, model, releaseYear)
    {
        BodyType = bodyType;
        HasBox = hasBox;
    }
}



// --------------------------------------- [ 3 ] ---------------------------------------

public class Garage
{
    public string GarageName;
    private List<Vehicle> vehicles = new List<Vehicle>();

    public Garage(string garageName)
    {
        GarageName = garageName;
    }

    public void AddVehicle(Vehicle vehicle)
    {
        vehicles.Add(vehicle);
        Console.WriteLine($"В гараж {GarageName} добавлено: {vehicle}");
    }

    public void RemoveVehicle(Vehicle vehicle)
    {
        vehicles.Remove(vehicle);
        Console.WriteLine($"Из гаража {GarageName} удалёно: {vehicle}");
    }

    public List<Vehicle> GetVehiclesList()
    {
        return vehicles;
    }
}



public class Fleet
{
    private List<Garage> garages = new List<Garage>();

    public void AddGarage(Garage garage)
    {
        garages.Add(garage);
        Console.WriteLine($"Добавлен гараж: {garage.GarageName}");
    }

    public void RemoveGarage(Garage garage)
    {
        garages.Remove(garage);
        Console.WriteLine($"Удалён гараж: {garage.GarageName}");
    }

    public Vehicle FindVehicle(string brand, string model)
    {
        foreach (var garage in garages)
        {
            foreach (var vehicle in garage.GetVehiclesList())
            {
                if (vehicle.Brand == brand && vehicle.Model == model)
                {
                    return vehicle;
                }
            }
        }

        return null;
    }
}



// --------------------------------------- [ 4 ] ---------------------------------------

class Program
{
    static void Main(string[] args)
    {
        Car car1 = new Car("Toyota", "Camry", 2020, 4, "Автомат");
        Car car2 = new Car("Москвич", "2136", 1964, 4, "Автомат");
        Motorcycle moto1 = new Motorcycle("Yamaha", "R1", 2019, "Спортбайк", false);

        Garage garage1 = new Garage("Гараж №1");
        Garage garage2 = new Garage("Гараж №2");

        garage1.AddVehicle(car1);
        garage1.AddVehicle(moto1);

        garage2.AddVehicle(car2);

        Fleet fleet1 = new Fleet();
        fleet1.AddGarage(garage1);
        fleet1.AddGarage(garage2);

        var found = fleet1.FindVehicle("Москвич", "2136");
        Console.WriteLine(found != null ? $"Найдено ТС: {found}" : "ТС не найдено.");

        garage1.RemoveVehicle(moto1);

        fleet1.RemoveGarage(garage2);

        Console.ReadLine();
    }
}