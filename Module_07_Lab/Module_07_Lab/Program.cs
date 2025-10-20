using System;
using System.Collections.Generic;

// --------------------------- 1 --------------------

public interface ICommand
{
    void Execute();
    void Undo();
}

// --------------------------- 2 --------------------

public class Light
{
    public void On() => Console.WriteLine("Свет включен.");
    public void Off() => Console.WriteLine("Свет выключен.");
}

public class Television
{
    public void On() => Console.WriteLine("Телевизор включен.");
    public void Off() => Console.WriteLine("Телевизор выключен.");
}

public class AirConditioner
{
    public void On() => Console.WriteLine("Кондиционер включен.");
    public void Off() => Console.WriteLine("Кондиционер выключен.");
}

// --------------------------- 3 --------------------

public class LightOnCommand : ICommand
{
    private Light _light;
    public LightOnCommand(Light light) { _light = light; }
    public void Execute() { _light.On(); }
    public void Undo() { _light.Off(); }
}

public class LightOffCommand : ICommand
{
    private Light _light;
    public LightOffCommand(Light light) { _light = light; }
    public void Execute() { _light.Off(); }
    public void Undo() { _light.On(); }
}

public class TelevisionOnCommand : ICommand
{
    private Television _tv;
    public TelevisionOnCommand(Television tv) { _tv = tv; }
    public void Execute() { _tv.On(); }
    public void Undo() { _tv.Off(); }
}

public class TelevisionOffCommand : ICommand
{
    private Television _tv;
    public TelevisionOffCommand(Television tv) { _tv = tv; }
    public void Execute() { _tv.Off(); }
    public void Undo() { _tv.On(); }
}

public class AirConditionerOnCommand : ICommand
{
    private AirConditioner _ac;
    public AirConditionerOnCommand(AirConditioner ac) { _ac = ac; }
    public void Execute() { _ac.On(); }
    public void Undo() { _ac.Off(); }
}

public class AirConditionerOffCommand : ICommand
{
    private AirConditioner _ac;
    public AirConditionerOffCommand(AirConditioner ac) { _ac = ac; }
    public void Execute() { _ac.Off(); }
    public void Undo() { _ac.On(); }
}

public class MacroCommand : ICommand
{
    private List<ICommand> _commands;
    public MacroCommand(List<ICommand> commands) { _commands = commands; }
    public void Execute() { foreach (var c in _commands) c.Execute(); }
    public void Undo() { for (int i = _commands.Count - 1; i >= 0; i--) _commands[i].Undo(); }
}

// --------------------------- 4 --------------------

public class RemoteControl
{
    private ICommand _onCommand;
    private ICommand _offCommand;
    private Stack<ICommand> _history = new Stack<ICommand>();

    public void SetCommands(ICommand onCommand, ICommand offCommand)
    {
        _onCommand = onCommand;
        _offCommand = offCommand;
    }

    public void PressOnButton()
    {
        if (_onCommand == null) { Console.WriteLine("Команда не назначена."); return; }
        _onCommand.Execute();
        _history.Push(_onCommand);
    }

    public void PressOffButton()
    {
        if (_offCommand == null) { Console.WriteLine("Команда не назначена."); return; }
        _offCommand.Execute();
        _history.Push(_offCommand);
    }

    public void PressUndoButton()
    {
        if (_history.Count > 0) _history.Pop().Undo();
        else Console.WriteLine("Нет команд для отмены.");
    }
}

// --------------------------- 5 --------------------

public abstract class Beverage
{
    public void PrepareRecipe()
    {
        BoilWater();
        Brew();
        PourInCup();
        if (CustomerWantsCondiments()) AddCondiments();
    }

    private void BoilWater() => Console.WriteLine("Кипячение воды...");
    private void PourInCup() => Console.WriteLine("Наливание в чашку...");
    protected abstract void Brew();
    protected abstract void AddCondiments();
    protected virtual bool CustomerWantsCondiments() => true;
}

public class Tea : Beverage
{
    protected override void Brew() => Console.WriteLine("Заваривание чая...");
    protected override void AddCondiments() => Console.WriteLine("Добавление лимона...");
}

public class Coffee : Beverage
{
    protected override void Brew() => Console.WriteLine("Заваривание кофе...");
    protected override void AddCondiments() => Console.WriteLine("Добавление сахара и молока...");
    protected override bool CustomerWantsCondiments()
    {
        Console.Write("Добавить сахар и молоко? (y/n): ");
        string input = Console.ReadLine();
        return input?.ToLower() == "y";
    }
}

public class HotChocolate : Beverage
{
    protected override void Brew() => Console.WriteLine("Приготовление горячего шоколада...");
    protected override void AddCondiments() => Console.WriteLine("Добавление маршмеллоу...");
}

// --------------------------- 6 --------------------

public interface IMediator
{
    void SendMessage(string message, Colleague colleague);
}

public abstract class Colleague
{
    protected IMediator _mediator;
    public Colleague(IMediator mediator) { _mediator = mediator; }
    public abstract void ReceiveMessage(string message);
}

public class ChatMediator : IMediator
{
    private List<Colleague> _colleagues = new List<Colleague>();
    public void RegisterColleague(Colleague colleague) => _colleagues.Add(colleague);
    public void SendMessage(string message, Colleague sender)
    {
        foreach (var colleague in _colleagues)
        {
            if (colleague != sender) colleague.ReceiveMessage(message);
        }
    }
}

public class User : Colleague
{
    private string _name;
    public User(IMediator mediator, string name) : base(mediator) { _name = name; }
    public void Send(string message)
    {
        Console.WriteLine($"{_name} отправляет сообщение: {message}");
        _mediator.SendMessage(message, this);
    }
    public override void ReceiveMessage(string message) => Console.WriteLine($"{_name} получил сообщение: {message}");
}

// --------------------------- 7 --------------------

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Паттерн Команда ===");
        Light light = new Light();
        Television tv = new Television();
        AirConditioner ac = new AirConditioner();

        ICommand lightOn = new LightOnCommand(light);
        ICommand lightOff = new LightOffCommand(light);
        ICommand tvOn = new TelevisionOnCommand(tv);
        ICommand tvOff = new TelevisionOffCommand(tv);
        ICommand acOn = new AirConditionerOnCommand(ac);
        ICommand acOff = new AirConditionerOffCommand(ac);

        RemoteControl remote = new RemoteControl();
        remote.SetCommands(lightOn, lightOff);
        remote.PressOnButton();
        remote.PressOffButton();
        remote.PressUndoButton();

        remote.SetCommands(tvOn, tvOff);
        remote.PressOnButton();
        remote.PressOffButton();

        var macro = new MacroCommand(new List<ICommand> { lightOn, tvOn, acOn });
        remote.SetCommands(macro, null);
        remote.PressOnButton();
        remote.PressUndoButton();

        Console.WriteLine("\n=== Паттерн Шаблонный метод ===");
        Beverage tea = new Tea();
        tea.PrepareRecipe();
        Console.WriteLine();
        Beverage coffee = new Coffee();
        coffee.PrepareRecipe();
        Console.WriteLine();
        Beverage chocolate = new HotChocolate();
        chocolate.PrepareRecipe();

        Console.WriteLine("\n=== Паттерн Посредник ===");
        ChatMediator chat = new ChatMediator();
        User user1 = new User(chat, "Алиса");
        User user2 = new User(chat, "Боб");
        User user3 = new User(chat, "Чарли");

        chat.RegisterColleague(user1);
        chat.RegisterColleague(user2);
        chat.RegisterColleague(user3);

        user1.Send("Привет всем!");
        user2.Send("Привет, Алиса!");
        user3.Send("Всем привет!");


        Console.WriteLine("----------------------------- Ответы на вопросы ------------------------------");

        Console.WriteLine("Вопрос 1: В чем преимущество использования паттерна 'Посредник'?");
        Console.WriteLine("- Меньше связей между участниками — все общаются через одного посредника.");
        Console.WriteLine("- Вся логика общения в одном месте.");
        Console.WriteLine("- Легко добавлять новые правила и функции.");
        Console.WriteLine("- Можно менять посредника, не меняя участников.");

        Console.WriteLine();

        Console.WriteLine("Вопрос 2: Как изменить посредника, чтобы он поддерживал отправку сообщений группе?");
        Console.WriteLine("- Добавить хранение групп (например, словарь: группа -> список пользователей).");
        Console.WriteLine("- Добавить методы для создания и изменения групп.");
        Console.WriteLine("- При отправке проверять группу и отправлять сообщение только её участникам.");

        Console.WriteLine();

        Console.WriteLine("Вопрос 3: Где можно применять паттерн 'Посредник' в жизни?");
        Console.WriteLine("- Чаты и мессенджеры.");
        Console.WriteLine("- Интерфейсы с большим количеством элементов (GUI).");
        Console.WriteLine("- Обмен сообщениями между сервисами.");
        Console.WriteLine("- Координация подсистем в больших программах.");
        Console.WriteLine("- Умные дома и IoT — устройства общаются через хаб.");
    }
}
