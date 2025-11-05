using System;
using System.Collections.Generic;
using System.Linq;

// ----------------------------------------------- 1 -----------------------------------
public interface IReport
{
    string Generate();
}

public class SalesReport : IReport
{
    private List<(DateTime Date, double Amount)> sales = new()
    {
        (new DateTime(2025, 1, 10), 1000),
        (new DateTime(2025, 2, 5), 1500),
        (new DateTime(2025, 3, 20), 2000),
        (new DateTime(2025, 4, 1), 1800)
    };

    public string Generate()
    {
        return string.Join("\n", sales.Select(s => $"Date: {s.Date.ToShortDateString()}, Amount: {s.Amount}"));
    }

    public List<(DateTime, double)> GetSales() => sales;
}

public class UserReport : IReport
{
    private List<(string Name, int Orders)> users = new()
    {
        ("Alice", 10),
        ("Bob", 5),
        ("Charlie", 8)
    };

    public string Generate()
    {
        return string.Join("\n", users.Select(u => $"User: {u.Name}, Orders: {u.Orders}"));
    }

    public List<(string, int)> GetUsers() => users;
}

public abstract class ReportDecorator : IReport
{
    protected IReport report;
    public ReportDecorator(IReport report) => this.report = report;
    public abstract string Generate();
}

public class DateFilterDecorator : ReportDecorator
{
    private DateTime from, to;
    public DateFilterDecorator(IReport report, DateTime from, DateTime to) : base(report)
    {
        this.from = from;
        this.to = to;
    }

    public override string Generate()
    {
        if (report is SalesReport sr)
        {
            var filtered = sr.GetSales().Where(s => s.Item1 >= from && s.Item1 <= to)
                .Select(s => $"Date: {s.Item1.ToShortDateString()}, Amount: {s.Item2}");
            return string.Join("\n", filtered);
        }
        return report.Generate();
    }
}

public class SortingDecorator : ReportDecorator
{
    private string criterion;
    public SortingDecorator(IReport report, string criterion) : base(report)
    {
        this.criterion = criterion;
    }

    public override string Generate()
    {
        if (report is SalesReport sr)
        {
            var sorted = criterion == "amount"
                ? sr.GetSales().OrderBy(s => s.Item2)
                : sr.GetSales().OrderBy(s => s.Item1);
            return string.Join("\n", sorted.Select(s => $"Date: {s.Item1.ToShortDateString()}, Amount: {s.Item2}"));
        }
        else if (report is UserReport ur)
        {
            var sorted = criterion == "orders"
                ? ur.GetUsers().OrderBy(u => u.Item2)
                : ur.GetUsers().OrderBy(u => u.Item1);
            return string.Join("\n", sorted.Select(u => $"User: {u.Item1}, Orders: {u.Item2}"));
        }
        return report.Generate();
    }
}

public class CsvExportDecorator : ReportDecorator
{
    public CsvExportDecorator(IReport report) : base(report) { }
    public override string Generate()
    {
        var data = report.Generate().Replace("\n", "\n;");
        return "CSV Export:\n" + data;
    }
}

public class PdfExportDecorator : ReportDecorator
{
    public PdfExportDecorator(IReport report) : base(report) { }
    public override string Generate()
    {
        return "PDF Export:\n" + report.Generate();
    }
}

public class AmountFilterDecorator : ReportDecorator
{
    private double minAmount;
    public AmountFilterDecorator(IReport report, double minAmount) : base(report)
    {
        this.minAmount = minAmount;
    }

    public override string Generate()
    {
        if (report is SalesReport sr)
        {
            var filtered = sr.GetSales().Where(s => s.Item2 >= minAmount)
                .Select(s => $"Date: {s.Item1.ToShortDateString()}, Amount: {s.Item2}");
            return string.Join("\n", filtered);
        }
        return report.Generate();
    }
}

// ----------------------------------------------- 2 -----------------------------------

public interface IInternalDeliveryService
{
    void DeliverOrder(string orderId);
    string GetDeliveryStatus(string orderId);
    double CalculateCost(double weight);
}

public class InternalDeliveryService : IInternalDeliveryService
{
    public void DeliverOrder(string orderId) => Console.WriteLine($"Internal: Delivering order {orderId}");
    public string GetDeliveryStatus(string orderId) => $"Internal: Order {orderId} delivered";
    public double CalculateCost(double weight) => 10 + weight * 2;
}

public class ExternalLogisticsServiceA
{
    public void ShipItem(int itemId) => Console.WriteLine($"ServiceA: Shipping item {itemId}");
    public string TrackShipment(int shipmentId) => $"ServiceA: Shipment {shipmentId} on the way";
    public double GetCost(double weight) => weight * 3;
}

public class ExternalLogisticsServiceB
{
    public void SendPackage(string packageInfo) => Console.WriteLine($"ServiceB: Sending {packageInfo}");
    public string CheckPackageStatus(string trackingCode) => $"ServiceB: {trackingCode} in transit";
    public double ComputeCost(double weight) => 5 + weight * 1.5;
}

public class ExternalLogisticsServiceC
{
    public void Dispatch(string code) => Console.WriteLine($"ServiceC: Dispatching code {code}");
    public string Status(string code) => $"ServiceC: Status for {code}";
    public double DeliveryCharge(double weight) => weight * 4.2;
}

public class LogisticsAdapterA : IInternalDeliveryService
{
    private ExternalLogisticsServiceA service;
    public LogisticsAdapterA(ExternalLogisticsServiceA service) => this.service = service;

    public void DeliverOrder(string orderId)
    {
        try
        {
            int id = int.Parse(orderId);
            service.ShipItem(id);
        }
        catch { Console.WriteLine("Error: Invalid orderId for ServiceA"); }
    }

    public string GetDeliveryStatus(string orderId)
    {
        try { return service.TrackShipment(int.Parse(orderId)); }
        catch { return "Error tracking shipment in ServiceA"; }
    }

    public double CalculateCost(double weight) => service.GetCost(weight);
}

public class LogisticsAdapterB : IInternalDeliveryService
{
    private ExternalLogisticsServiceB service;
    public LogisticsAdapterB(ExternalLogisticsServiceB service) => this.service = service;

    public void DeliverOrder(string orderId) => service.SendPackage(orderId);
    public string GetDeliveryStatus(string orderId) => service.CheckPackageStatus(orderId);
    public double CalculateCost(double weight) => service.ComputeCost(weight);
}

public class LogisticsAdapterC : IInternalDeliveryService
{
    private ExternalLogisticsServiceC service;
    public LogisticsAdapterC(ExternalLogisticsServiceC service) => this.service = service;

    public void DeliverOrder(string orderId) => service.Dispatch(orderId);
    public string GetDeliveryStatus(string orderId) => service.Status(orderId);
    public double CalculateCost(double weight) => service.DeliveryCharge(weight);
}

public static class DeliveryServiceFactory
{
    public static IInternalDeliveryService GetService(string type)
    {
        return type switch
        {
            "internal" => new InternalDeliveryService(),
            "A" => new LogisticsAdapterA(new ExternalLogisticsServiceA()),
            "B" => new LogisticsAdapterB(new ExternalLogisticsServiceB()),
            "C" => new LogisticsAdapterC(new ExternalLogisticsServiceC()),
            _ => throw new ArgumentException("Unknown service type")
        };
    }
}





public class Program
{
    public static void Main()
    {
        IReport report = new SalesReport();
        report = new DateFilterDecorator(report, new DateTime(2025, 2, 1), new DateTime(2025, 3, 31));
        report = new AmountFilterDecorator(report, 1200);
        report = new SortingDecorator(report, "amount");
        report = new CsvExportDecorator(report);
        Console.WriteLine(report.Generate());

        Console.WriteLine("\n--- Logistics Demo ---");
        var service = DeliveryServiceFactory.GetService("B");
        service.DeliverOrder("PKG-101");
        Console.WriteLine(service.GetDeliveryStatus("PKG-101"));
        Console.WriteLine($"Cost: {service.CalculateCost(12.5)}");
    }
}
