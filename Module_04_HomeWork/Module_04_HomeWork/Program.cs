using System;
using System.Collections.Generic;
using System.Linq;

// --------------------------------------------------------------------
// ЧАСТЬ 1: Паттерн ФАБРИЧНЫЙ МЕТОД (Factory Method)
// --------------------------------------------------------------------

// --------------------------------------------------------------------
// Интерфейс Транспортного Средства (Product)
// --------------------------------------------------------------------

public interface IVehicle
{
    void Drive();
    void Refuel();
}

// --------------------------------------------------------------------
// Конкретные Транспортные Средства (Concrete Products)
// --------------------------------------------------------------------

public class Car : IVehicle
{
    private string marka;
    private string model;
    private string fuelType;

    public Car(string marka, string model, string fuelType)
    {
        this.marka = marka;
        this.model = model;
        this.fuelType = fuelType;
    }

    public void Drive()
    {
        Console.WriteLine($"[Автомобиль] {marka} {model} едет. Тип топлива: {fuelType}.");
    }

    public void Refuel()
    {
        Console.WriteLine($"[Автомобиль] {marka} заправляется топливом типа {fuelType}.");
    }
}

public class Motorcycle : IVehicle
{
    private string type;
    private int engineVolume;

    public Motorcycle(string type, int engineVolume)
    {
        this.type = type;
        this.engineVolume = engineVolume;
    }

    public void Drive()
    {
        Console.WriteLine($"[Мотоцикл] {type} мчится. Объем двигателя: {engineVolume}cc.");
    }

    public void Refuel()
    {
        Console.WriteLine($"[Мотоцикл] Проверяется уровень бензина.");
    }
}

public class Truck : IVehicle
{
    private double capacity;
    private int axles;

    public Truck(double capacity, int axles)
    {
        this.capacity = capacity;
        this.axles = axles;
    }

    public void Drive()
    {
        Console.WriteLine($"[Грузовик] Тяжеловес с грузоподъемностью {capacity} тонн и {axles} осями движется по маршруту.");
    }

    public void Refuel()
    {
        Console.WriteLine($"[Грузовик] Заправка дизельным топливом в большой бак.");
    }
}

public class Bus : IVehicle
{
    private int seats;
    private string route;

    public Bus(int seats, string route)
    {
        this.seats = seats;
        this.route = route;
    }

    public void Drive()
    {
        Console.WriteLine($"[Автобус] Едет по маршруту '{route}'. Мест: {seats}.");
    }

    public void Refuel()
    {
        Console.WriteLine($"[Автобус] Заправка на конечной остановке маршрута {route}.");
    }
}

// --------------------------------------------------------------------
// Абстрактный Создатель (Abstract Creator)
// --------------------------------------------------------------------

public abstract class VehicleFactory
{
    public abstract IVehicle CreateVehicle(Dictionary<string, string> parameters);

    public void CheckVehicle(Dictionary<string, string> parameters)
    {
        IVehicle vehicle = CreateVehicle(parameters);
        Console.WriteLine($"\n--- Тест транспорта типа: {vehicle.GetType().Name} ---");
        vehicle.Drive();
        vehicle.Refuel();
    }
}

// --------------------------------------------------------------------
// Конкретные Создатели (Concrete Creators)
// --------------------------------------------------------------------

public class CarFactory : VehicleFactory
{
    public override IVehicle CreateVehicle(Dictionary<string, string> parameters)
    {
        string marka = parameters.ContainsKey("Marka") ? parameters["Marka"] : "DefaultMarka";
        string model = parameters.ContainsKey("Model") ? parameters["Model"] : "DefaultModel";
        string fuel = parameters.ContainsKey("FuelType") ? parameters["FuelType"] : "Petrol";
        return new Car(marka, model, fuel);
    }
}

public class MotorcycleFactory : VehicleFactory
{
    public override IVehicle CreateVehicle(Dictionary<string, string> parameters)
    {
        string type = parameters.ContainsKey("Type") ? parameters["Type"] : "Sport";
        int volume = parameters.ContainsKey("EngineVolume") ? int.Parse(parameters["EngineVolume"]) : 600;
        return new Motorcycle(type, volume);
    }
}

public class TruckFactory : VehicleFactory
{
    public override IVehicle CreateVehicle(Dictionary<string, string> parameters)
    {
        double capacity = parameters.ContainsKey("Capacity") ? double.Parse(parameters["Capacity"]) : 20.0;
        int axles = parameters.ContainsKey("Axles") ? int.Parse(parameters["Axles"]) : 4;
        return new Truck(capacity, axles);
    }
}

public class BusFactory : VehicleFactory
{
    public override IVehicle CreateVehicle(Dictionary<string, string> parameters)
    {
        int seats = parameters.ContainsKey("Seats") ? int.Parse(parameters["Seats"]) : 40;
        string route = parameters.ContainsKey("Route") ? parameters["Route"] : "City Route 1";
        return new Bus(seats, route);
    }
}

// --------------------------------------------------------------------
// КЛИЕНТСКИЙ КОД (Демонстрация)
// --------------------------------------------------------------------

public class Client
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("---------------------------------------------------------");
        Console.WriteLine("Демонстрация Паттерна ФАБРИЧНЫЙ МЕТОД (Система Транспорта)");
        Console.WriteLine("---------------------------------------------------------");

        Dictionary<string, VehicleFactory> factories = new Dictionary<string, VehicleFactory>
        {
            {"CAR", new CarFactory()},
            {"MOTORCYCLE", new MotorcycleFactory()},
            {"TRUCK", new TruckFactory()},
            {"BUS", new BusFactory()}
        };

        DemonstrateVehicleCreation(factories);
    }

    public static void DemonstrateVehicleCreation(Dictionary<string, VehicleFactory> factories)
    {
        Console.WriteLine("Доступные типы транспорта для создания:");
        foreach (var key in factories.Keys)
        {
            Console.WriteLine($"- {key}");
        }

        while (true)
        {
            Console.WriteLine("\nВведите тип транспорта для создания (CAR, MOTORCYCLE, TRUCK, BUS) или 'EXIT' для выхода:");
            string typeInput = Console.ReadLine()?.ToUpper();

            if (typeInput == "EXIT") break;

            if (factories.TryGetValue(typeInput, out VehicleFactory factory))
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                try
                {
                    Console.WriteLine($"\n--- Ввод параметров для {typeInput} ---");

                    switch (typeInput)
                    {
                        case "CAR":
                            parameters = GetCarParameters();
                            break;
                        case "MOTORCYCLE":
                            parameters = GetMotorcycleParameters();
                            break;
                        case "TRUCK":
                            parameters = GetTruckParameters();
                            break;
                        case "BUS":
                            parameters = GetBusParameters();
                            break;
                    }

                    factory.CheckVehicle(parameters);
                }
                catch (FormatException)
                {
                    Console.WriteLine("[ОШИБКА] Неверный формат ввода. Пожалуйста, введите корректные данные.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[КРИТИЧЕСКАЯ ОШИБКА] Произошла ошибка: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"[ОШИБКА] Неизвестный тип транспорта: {typeInput}.");
            }
        }
        Console.WriteLine("\nДемонстрация завершена.");
    }

    private static Dictionary<string, string> GetCarParameters()
    {
        Console.Write("Марка (например, Toyota): ");
        string marka = Console.ReadLine();
        Console.Write("Модель (например, Camry): ");
        string model = Console.ReadLine();
        Console.Write("Тип топлива (например, Petrol/Diesel/Electric): ");
        string fuelType = Console.ReadLine();

        return new Dictionary<string, string>
        {
            { "Marka", marka },
            { "Model", model },
            { "FuelType", fuelType }
        };
    }

    private static Dictionary<string, string> GetMotorcycleParameters()
    {
        Console.Write("Тип мотоцикла (например, Sport/Touring): ");
        string type = Console.ReadLine();
        Console.Write("Объем двигателя в куб.см (например, 1000): ");
        string volume = Console.ReadLine();

        return new Dictionary<string, string>
        {
            { "Type", type },
            { "EngineVolume", volume }
        };
    }

    private static Dictionary<string, string> GetTruckParameters()
    {
        Console.Write("Грузоподъемность в тоннах (например, 35.5): ");
        string capacity = Console.ReadLine();
        Console.Write("Количество осей (например, 6): ");
        string axles = Console.ReadLine();

        return new Dictionary<string, string>
        {
            { "Capacity", capacity },
            { "Axles", axles }
        };
    }

    private static Dictionary<string, string> GetBusParameters()
    {
        Console.Write("Количество сидячих мест (например, 55): ");
        string seats = Console.ReadLine();
        Console.Write("Маршрут (например, 10A): ");
        string route = Console.ReadLine();

        return new Dictionary<string, string>
        {
            { "Seats", seats },
            { "Route", route }
        };
    }
}
