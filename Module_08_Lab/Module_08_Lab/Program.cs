
// ------------------------------------------- 1 ---------------------------------------------


using System;

public interface IBeverage
{
    double GetCost();
    string GetDescription();
}

public class Coffee : IBeverage
{
    public double GetCost()
    {
        return 50.0;
    }

    public string GetDescription()
    {
        return "Coffee";
    }
}

public abstract class BeverageDecorator : IBeverage
{
    protected IBeverage _beverage;

    public BeverageDecorator(IBeverage beverage)
    {
        _beverage = beverage;
    }

    public virtual double GetCost()
    {
        return _beverage.GetCost();
    }

    public virtual string GetDescription()
    {
        return _beverage.GetDescription();
    }
}


public class MilkDecorator : BeverageDecorator
{
    public MilkDecorator(IBeverage beverage) : base(beverage) { }

    public override double GetCost()
    {
        return base.GetCost() + 10.0;
    }

    public override string GetDescription()
    {
        return base.GetDescription() + ", Milk";
    }
}

public class SugarDecorator : BeverageDecorator
{
    public SugarDecorator(IBeverage beverage) : base(beverage) { }

    public override double GetCost()
    {
        return base.GetCost() + 5.0;
    }

    public override string GetDescription()
    {
        return base.GetDescription() + ", Sugar";
    }
}

public class ChocolateDecorator : BeverageDecorator
{
    public ChocolateDecorator(IBeverage beverage) : base(beverage) { }

    public override double GetCost()
    {
        return base.GetCost() + 15.0;
    }

    public override string GetDescription()
    {
        return base.GetDescription() + ", Chocolate";
    }
}


public class VanillaDecorator : BeverageDecorator
{
    public VanillaDecorator(IBeverage beverage) : base(beverage) { }

    public override double GetCost()
    {
        return base.GetCost() + 12.0;
    }

    public override string GetDescription()
    {
        return base.GetDescription() + ", Vanilla";
    }
}

public class CinnamonDecorator : BeverageDecorator
{
    public CinnamonDecorator(IBeverage beverage) : base(beverage) { }

    public override double GetCost()
    {
        return base.GetCost() + 8.0;
    }

    public override string GetDescription()
    {
        return base.GetDescription() + ", Cinnamon";
    }
}


class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("=== Декоратор: оформление напитков ===\n");

        IBeverage beverage = new Coffee();
        Console.WriteLine($"{beverage.GetDescription()} : {beverage.GetCost()} тг");

        beverage = new MilkDecorator(beverage);
        Console.WriteLine($"{beverage.GetDescription()} : {beverage.GetCost()} тг");

        beverage = new SugarDecorator(beverage);
        Console.WriteLine($"{beverage.GetDescription()} : {beverage.GetCost()} тг");

        beverage = new ChocolateDecorator(beverage);
        Console.WriteLine($"{beverage.GetDescription()} : {beverage.GetCost()} тг");

        beverage = new VanillaDecorator(beverage);
        beverage = new CinnamonDecorator(beverage);
        Console.WriteLine($"{beverage.GetDescription()} : {beverage.GetCost()} тг");
    }
}






// ------------------------------------------- 2 ---------------------------------------------








using System;

public interface IPaymentProcessor
{
    void ProcessPayment(double amount);
    void RefundPayment(double amount);
}

public class InternalPaymentProcessor : IPaymentProcessor
{
    public void ProcessPayment(double amount)
    {
        Console.WriteLine($"[Internal] Processing payment of {amount} via internal system.");
    }

    public void RefundPayment(double amount)
    {
        Console.WriteLine($"[Internal] Refunding payment of {amount} via internal system.");
    }
}

public class ExternalPaymentSystemA
{
    public void MakePayment(double amount)
    {
        Console.WriteLine($"[External A] Making payment of {amount}.");
    }

    public void MakeRefund(double amount)
    {
        Console.WriteLine($"[External A] Making refund of {amount}.");
    }
}

public class ExternalPaymentSystemB
{
    public void SendPayment(double amount)
    {
        Console.WriteLine($"[External B] Sending payment of {amount}.");
    }

    public void ProcessRefund(double amount)
    {
        Console.WriteLine($"[External B] Processing refund of {amount}.");
    }
}

public class PaymentAdapterA : IPaymentProcessor
{
    private ExternalPaymentSystemA _systemA;

    public PaymentAdapterA(ExternalPaymentSystemA systemA)
    {
        _systemA = systemA;
    }

    public void ProcessPayment(double amount)
    {
        _systemA.MakePayment(amount);
    }

    public void RefundPayment(double amount)
    {
        _systemA.MakeRefund(amount);
    }
}

public class PaymentAdapterB : IPaymentProcessor
{
    private ExternalPaymentSystemB _systemB;

    public PaymentAdapterB(ExternalPaymentSystemB systemB)
    {
        _systemB = systemB;
    }

    public void ProcessPayment(double amount)
    {
        _systemB.SendPayment(amount);
    }

    public void RefundPayment(double amount)
    {
        _systemB.ProcessRefund(amount);
    }
}

public class PaymentProgram
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("\n=== Адаптер: интеграция платёжных систем ===\n");

        IPaymentProcessor internalProcessor = new InternalPaymentProcessor();
        internalProcessor.ProcessPayment(100);
        internalProcessor.RefundPayment(50);
        Console.WriteLine();

        IPaymentProcessor adapterA = new PaymentAdapterA(new ExternalPaymentSystemA());
        adapterA.ProcessPayment(200);
        adapterA.RefundPayment(100);
        Console.WriteLine();

        IPaymentProcessor adapterB = new PaymentAdapterB(new ExternalPaymentSystemB());
        adapterB.ProcessPayment(300);
        adapterB.RefundPayment(150);
        Console.WriteLine();

        string region = "EU";
        IPaymentProcessor selectedProcessor;

        if (region == "EU")
            selectedProcessor = adapterA;
        else
            selectedProcessor = adapterB;

        selectedProcessor.ProcessPayment(500);
        selectedProcessor.RefundPayment(200);
    }
}
