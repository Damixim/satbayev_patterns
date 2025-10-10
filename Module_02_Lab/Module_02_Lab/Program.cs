

// --------------------------------------- [ 1 ] ---------------------------------------

//public class OrderService
//{
//    public double GetTotalPrice(int item_quantity, double item_price)
//    {
//        double totalPrice = item_quantity * item_price;
//        return totalPrice;
//    }
//    public void CreateOrder(string productName, int quantity, double price)
//    {
//        double totalPrice = GetTotalPrice(quantity, price);
//        Console.WriteLine($"Order for {productName} created. Total: {totalPrice}");
//    }
//    public void UpdateOrder(string productName, int quantity, double price)
//    {
//        double totalPrice = GetTotalPrice(quantity, price);
//        Console.WriteLine($"Order for {productName} updated. New total: {totalPrice}");
//    }
//}



// --------------------------------------- [ 2 ] ---------------------------------------

//public class Transport
//{
//    public string transportName = "transport";

//    public void Start()
//    {
//        Console.WriteLine($"{this.transportName} is starting");
//    }
//    public void Stop()
//    {
//        Console.WriteLine($"{this.transportName} is stopping");
//    }
//}

//public class Program
//{
//    public static void Main(string[] args)
//    {
//        Transport car = new Transport();
//        car.transportName = "Car";
//        car.Start();
//        car.Stop();
//    }
//}



// --------------------------------------- [ 3 ] ---------------------------------------

//public class Calculator
//{
//    public double Add(double a, double b)
//    {
//        return a + b;
//    }
//}

//public class Program
//{
//    public static void Main(string[] args)
//    {
//        Calculator calc = new Calculator();
//        double result = calc.Add(1, 2);
//        Console.WriteLine(result);
//    }
//}



// --------------------------------------- [ 4 ] ---------------------------------------

//public static class Singleton
//{
//    public static void DoSomething()
//    {
//        Console.WriteLine("Doing something...");
//    }
//}

//public class Program
//{
//    public static void Main(string[] args)
//    {
//        Singleton.DoSomething();
//    }
//}



// --------------------------------------- [ 5 ] ---------------------------------------

//public class Circle
//{
//    private double _radius;

//    public Circle(double radius)
//    {
//        _radius = radius;
//    }

//    public double CalculateArea()
//    {
//        return Math.PI * (_radius * _radius);
//    }
//}

//public class Square
//{
//    private double _side;

//    public Square(double side)
//    {
//        _side = side;
//    }

//    public double CalculateArea()
//    {
//        return _side * _side;
//    }
//}

//public class Program
//{
//    public static void Main(string[] args)
//    {
//        Circle circle = new Circle(5);
//        Square square = new Square(4);

//        Console.WriteLine($"Circle area: {circle.CalculateArea()}");
//        Console.WriteLine($"Square area: {square.CalculateArea()}");
//    }
//}



// --------------------------------------- [ 6 ] ---------------------------------------

//public class MathOperations
//{
//    public double Add(double a, double b)
//    {
//        return a + b;
//    }
//    public void LogFunction(Func<double> function)
//    {
//        Console.WriteLine(function());
//    }
//}

//public class Program
//{
//    public static void Main(string[] args)
//    {
//        MathOperations operations = new MathOperations();
//        operations.Add(1, 2);
//        operations.LogFunction(() => operations.Add(1,2));
//    }
//}