
//// --------------------------------- 1 ---------------------------------

//using System;

//public abstract class Beverage
//{
//    public abstract string GetDescription();
//    public abstract double Cost();
//}

//public class Espresso : Beverage
//{
//    public override string GetDescription() => "Эспрессо";
//    public override double Cost() => 200.0;
//}

//public class Tea : Beverage
//{
//    public override string GetDescription() => "Чай";
//    public override double Cost() => 150.0;
//}

//public class Latte : Beverage
//{
//    public override string GetDescription() => "Латте";
//    public override double Cost() => 250.0;
//}

//public class Mocha : Beverage
//{
//    public override string GetDescription() => "Мокка";
//    public override double Cost() => 270.0;
//}

//public abstract class BeverageDecorator : Beverage
//{
//    protected Beverage _beverage;

//    public BeverageDecorator(Beverage beverage)
//    {
//        _beverage = beverage;
//    }

//    public override string GetDescription()
//    {
//        return _beverage.GetDescription();
//    }

//    public override double Cost()
//    {
//        return _beverage.Cost();
//    }
//}

//public class Milk : BeverageDecorator
//{
//    public Milk(Beverage beverage) : base(beverage) { }

//    public override string GetDescription() => _beverage.GetDescription() + ", молоко";
//    public override double Cost() => _beverage.Cost() + 50.0;
//}

//public class Sugar : BeverageDecorator
//{
//    public Sugar(Beverage beverage) : base(beverage) { }

//    public override string GetDescription() => _beverage.GetDescription() + ", сахар";
//    public override double Cost() => _beverage.Cost() + 20.0;
//}

//public class WhippedCream : BeverageDecorator
//{
//    public WhippedCream(Beverage beverage) : base(beverage) { }

//    public override string GetDescription() => _beverage.GetDescription() + ", взбитые сливки";
//    public override double Cost() => _beverage.Cost() + 70.0;
//}

//public class Caramel : BeverageDecorator
//{
//    public Caramel(Beverage beverage) : base(beverage) { }

//    public override string GetDescription() => _beverage.GetDescription() + ", карамель";
//    public override double Cost() => _beverage.Cost() + 60.0;
//}

//public class Vanilla : BeverageDecorator
//{
//    public Vanilla(Beverage beverage) : base(beverage) { }

//    public override string GetDescription() => _beverage.GetDescription() + ", ваниль";
//    public override double Cost() => _beverage.Cost() + 40.0;
//}

//class Program
//{
//    static void Main()
//    {
//        Console.OutputEncoding = System.Text.Encoding.UTF8;

//        Beverage beverage1 = new Espresso();
//        beverage1 = new Milk(beverage1);
//        beverage1 = new Sugar(beverage1);

//        Console.WriteLine($"Ваш напиток: {beverage1.GetDescription()}");
//        Console.WriteLine($"Стоимость: {beverage1.Cost()} тг\n");

//        Beverage beverage2 = new Latte();
//        beverage2 = new Caramel(beverage2);
//        beverage2 = new WhippedCream(beverage2);

//        Console.WriteLine($"Ваш напиток: {beverage2.GetDescription()}");
//        Console.WriteLine($"Стоимость: {beverage2.Cost()} тг\n");

//        Beverage beverage3 = new Tea();
//        beverage3 = new Vanilla(beverage3);
//        beverage3 = new Milk(beverage3);
//        beverage3 = new Sugar(beverage3);

//        Console.WriteLine($"Ваш напиток: {beverage3.GetDescription()}");
//        Console.WriteLine($"Стоимость: {beverage3.Cost()} тг\n");
//    }
//}






// --------------------------------- 2 ---------------------------------






using System;

public interface IPaymentProcessor
{
    void ProcessPayment(double amount);
}

public class PayPalPaymentProcessor : IPaymentProcessor
{
    public void ProcessPayment(double amount)
    {
        Console.WriteLine($"[PayPal] Оплата {amount:F2} тг успешно проведена через PayPal.");
    }
}

public class StripePaymentService
{
    public void MakeTransaction(double totalAmount)
    {
        Console.WriteLine($"[Stripe] Транзакция на сумму {totalAmount:F2} тг выполнена через Stripe.");
    }
}

public class StripePaymentAdapter : IPaymentProcessor
{
    private readonly StripePaymentService _stripeService;

    public StripePaymentAdapter(StripePaymentService stripeService)
    {
        _stripeService = stripeService;
    }

    public void ProcessPayment(double amount)
    {
        _stripeService.MakeTransaction(amount);
    }
}

public class YooMoneyService
{
    public void Pay(double value)
    {
        Console.WriteLine($"[YooMoney] Платёж на сумму {value:F2} тг успешно проведён через YooMoney.");
    }
}

public class YooMoneyAdapter : IPaymentProcessor
{
    private readonly YooMoneyService _service;

    public YooMoneyAdapter(YooMoneyService service)
    {
        _service = service;
    }

    public void ProcessPayment(double amount)
    {
        _service.Pay(amount);
    }
}

class PaymentTest
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("=== Тестирование платёжных систем ===\n");

        IPaymentProcessor paypal = new PayPalPaymentProcessor();
        IPaymentProcessor stripe = new StripePaymentAdapter(new StripePaymentService());
        IPaymentProcessor yoomoney = new YooMoneyAdapter(new YooMoneyService());

        IPaymentProcessor[] processors = { paypal, stripe, yoomoney };

        foreach (var processor in processors)
        {
            processor.ProcessPayment(999.99);
        }
    }
}
