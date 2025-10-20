using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

// --------------------------------------------------------------------
// ЧАСТЬ 1: Паттерн КОМАНДА (Command)
// --------------------------------------------------------------------

public interface ICommand
{
    void Execute();
    void Undo();
}

public class NoCommand : ICommand
{
    public void Execute()
    {
        Console.WriteLine(" > Слот пульта не назначен. Действие не выполняется.");
    }
    public void Undo()
    {
        Console.WriteLine(" > Нечего отменять, команда в слоте не была назначена.");
    }
}

// --------------------------------------------------------------------
// Устройства (Receivers)
// --------------------------------------------------------------------

public class Light
{
    private string location;
    public bool IsOn { get; private set; }

    public Light(string location)
    {
        this.location = location;
        IsOn = false;
    }

    public void On()
    {
        IsOn = true;
        Console.WriteLine($"[СВЕТ] {location} - включен.");
    }

    public void Off()
    {
        IsOn = false;
        Console.WriteLine($"[СВЕТ] {location} - выключен.");
    }
}

public class AirConditioner
{
    private string location;
    public int Temperature { get; private set; }
    public bool IsOn { get; private set; }

    public AirConditioner(string location)
    {
        this.location = location;
        Temperature = 25;
        IsOn = false;
    }

    public void SetTemperature(int temp)
    {
        IsOn = true;
        int oldTemp = Temperature;
        Temperature = temp;
        Console.WriteLine($"[КОНДИЦИОНЕР] {location} - установлена температура: {Temperature}°C (было {oldTemp}°C).");
    }

    public void Off()
    {
        IsOn = false;
        Console.WriteLine($"[КОНДИЦИОНЕР] {location} - выключен.");
    }
}

public class SmartCurtains
{
    private string location;
    public string State { get; private set; }

    public SmartCurtains(string location)
    {
        this.location = location;
        State = "Closed";
    }

    public void Open()
    {
        State = "Open";
        Console.WriteLine($"[ШТОРЫ] {location} - открыты.");
    }

    public void Close()
    {
        State = "Closed";
        Console.WriteLine($"[ШТОРЫ] {location} - закрыты.");
    }
}

// --------------------------------------------------------------------
// Конкретные команды
// --------------------------------------------------------------------

public class LightOnCommand : ICommand
{
    private Light light;
    public LightOnCommand(Light light) => this.light = light;
    public void Execute() => light.On();
    public void Undo() => light.Off();
}

public class ACTemperatureCommand : ICommand
{
    private AirConditioner ac;
    private int newTemp;
    private int oldTemp;

    public ACTemperatureCommand(AirConditioner ac, int newTemp)
    {
        this.ac = ac;
        this.newTemp = newTemp;
        this.oldTemp = ac.Temperature;
    }

    public void Execute()
    {
        oldTemp = ac.Temperature;
        ac.SetTemperature(newTemp);
    }

    public void Undo()
    {
        ac.SetTemperature(oldTemp);
    }
}

public class CurtainsOpenCommand : ICommand
{
    private SmartCurtains curtains;
    public CurtainsOpenCommand(SmartCurtains curtains) => this.curtains = curtains;
    public void Execute() => curtains.Open();
    public void Undo() => curtains.Close();
}

// --------------------------------------------------------------------
// Макрокоманда
// --------------------------------------------------------------------

public class MacroCommand : ICommand
{
    private List<ICommand> commands;

    public MacroCommand(List<ICommand> commands)
    {
        this.commands = commands;
    }

    public void Execute()
    {
        Console.WriteLine("\n=== ЗАПУСК МАКРОКОМАНДЫ ===");
        foreach (var command in commands)
        {
            command.Execute();
        }
        Console.WriteLine("=== МАКРОКОМАНДА ЗАВЕРШЕНА ===\n");
    }

    public void Undo()
    {
        Console.WriteLine("\n=== ОТМЕНА МАКРОКОМАНДЫ (в обратном порядке) ===");
        for (int i = commands.Count - 1; i >= 0; i--)
        {
            commands[i].Undo();
        }
        Console.WriteLine("=== ОТМЕНА МАКРОКОМАНДЫ ЗАВЕРШЕНА ===\n");
    }
}

// --------------------------------------------------------------------
// Invoker (RemoteControl)
// --------------------------------------------------------------------

public class RemoteControl
{
    private ICommand[] commandSlots;
    private Stack<ICommand> undoStack;
    private Stack<ICommand> redoStack;
    private const int SLOTS = 7;

    public RemoteControl()
    {
        commandSlots = new ICommand[SLOTS];
        ICommand noCommand = new NoCommand();
        for (int i = 0; i < SLOTS; i++)
        {
            commandSlots[i] = noCommand;
        }

        undoStack = new Stack<ICommand>();
        redoStack = new Stack<ICommand>();
    }

    public void SetCommand(int slot, ICommand command)
    {
        if (slot >= 0 && slot < SLOTS)
        {
            commandSlots[slot] = command;
            Console.WriteLine($"[ПУЛЬТ] На слот {slot} назначена команда: {command.GetType().Name}");
        }
        else
        {
            Console.WriteLine("[ПУЛЬТ ОШИБКА] Неверный номер слота.");
        }
    }

    public void PressButton(int slot)
    {
        Console.WriteLine($"\n[ПУЛЬТ] Нажата кнопка {slot}.");
        ICommand command = commandSlots[slot];
        command.Execute();

        if (command.GetType() != typeof(NoCommand))
        {
            undoStack.Push(command);
            redoStack.Clear();
        }
    }

    public void PressUndo()
    {
        Console.WriteLine("\n[ПУЛЬТ] Нажата кнопка ОТМЕНА (Undo).");
        if (undoStack.Count > 0)
        {
            ICommand undoneCommand = undoStack.Pop();
            undoneCommand.Undo();
            redoStack.Push(undoneCommand);
        }
        else
        {
            Console.WriteLine("[ПУЛЬТ] История команд пуста. Нечего отменять.");
        }
    }

    public void PressRedo()
    {
        Console.WriteLine("\n[ПУЛЬТ] Нажата кнопка ПОВТОР (Redo).");
        if (redoStack.Count > 0)
        {
            ICommand redoneCommand = redoStack.Pop();
            redoneCommand.Execute();
            undoStack.Push(redoneCommand);
        }
        else
        {
            Console.WriteLine("[ПУЛЬТ] Нечего повторять. Стек повтора пуст.");
        }
    }

    public MacroCommand RecordLastCommands(int count)
    {
        Console.WriteLine($"\n[ПУЛЬТ] Запись макрокоманды из последних {count} команд.");

        var commandsToRecord = undoStack.Take(count).Reverse().ToList();

        if (commandsToRecord.Count == 0)
        {
            Console.WriteLine("[ПУЛЬТ] В истории недостаточно команд для записи.");
            return null;
        }

        return new MacroCommand(commandsToRecord);
    }
}

// --------------------------------------------------------------------
// ЧАСТЬ 2: Паттерн ШАБЛОННЫЙ МЕТОД (Template Method)
// --------------------------------------------------------------------

public abstract class ReportGenerator
{
    public void GenerateReport(string reportName)
    {
        LogStep($"--- Запуск генерации отчета: {reportName} ({this.GetType().Name}) ---");

        object data = CollectData();

        GenerateHeader();

        string formattedContent = FormatContent(data);

        if (CustomerWantsSave())
        {
            SaveReport(reportName, formattedContent);
        }

        if (CustomerWantsEmail())
        {
            SendReport(reportName);
        }

        LogStep($"--- Отчет {reportName} успешно сгенерирован. ---");
    }

    protected abstract void GenerateHeader();
    protected abstract string FormatContent(object data);

    protected object CollectData()
    {
        LogStep("Сбор данных из источника (имитация).");
        return new List<Dictionary<string, object>>()
        {
            new Dictionary<string, object> { {"ID", 1}, {"Description", "Покупка A"}, {"Amount", 100.50} },
            new Dictionary<string, object> { {"ID", 2}, {"Description", "Покупка B"}, {"Amount", 250.00} },
            new Dictionary<string, object> { {"ID", 3}, {"Description", "Возврат C"}, {"Amount", -50.75} }
        };
    }

    protected virtual bool CustomerWantsSave()
    {
        LogStep("Проверка: Требуется ли сохранение?");
        return false;
    }

    protected virtual bool CustomerWantsEmail()
    {
        LogStep("Проверка: Требуется ли отправка по Email?");
        return false;
    }

    protected virtual void SaveReport(string name, string content)
    {
        LogStep($"[ОБЩЕЕ] Сохранение отчета '{name}' в файл. Размер контента: {content.Length} символов.");
    }

    protected virtual void SendReport(string name)
    {
        LogStep($"[ОБЩЕЕ] Отправка отчета '{name}' по Email.");
    }

    protected void LogStep(string message)
    {
        Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}] [ЛОГ] {message}");
    }
}

public class PdfReport : ReportGenerator
{
    protected override void GenerateHeader()
    {
        LogStep("Генерация стилизованного PDF-заголовка.");
    }

    protected override string FormatContent(object data)
    {
        LogStep("Форматирование данных в структуру PDF (имитация).");
        return "PDF-Контент: Заголовок, Таблица, Итоги.";
    }
}

public class ExcelReport : ReportGenerator
{
    protected override void GenerateHeader()
    {
        LogStep("Создание рабочего листа Excel и заголовков столбцов.");
    }

    protected override string FormatContent(object data)
    {
        LogStep("Заполнение ячеек Excel данными.");
        return "Excel-Контент: Таблица с формулами и стилями.";
    }

    protected override bool CustomerWantsSave()
    {
        LogStep("Excel Отчет: Сохранение ТРЕБУЕТСЯ по умолчанию.");
        return true;
    }
}

public class HtmlReport : ReportGenerator
{
    protected override void GenerateHeader()
    {
        LogStep("Вставка тегов <html>, <head> и <h1>.");
    }

    protected override string FormatContent(object data)
    {
        LogStep("Форматирование данных в HTML-таблицу.");
        return "HTML-Контент: <table><tr>...</tr></table>.";
    }

    protected override void SaveReport(string name, string content)
    {
        LogStep($"[HTML СПЕЦИФИКА] Сохранение {name} как файла .html (Content-Type: text/html).");
    }

    protected override bool CustomerWantsEmail()
    {
        LogStep("HTML Отчет: Отправка по Email ТРЕБУЕТСЯ.");
        return true;
    }
}

public class CsvReport : ReportGenerator
{
    protected override void GenerateHeader()
    {
        LogStep("Создание строки заголовков CSV (названия столбцов).");
    }

    protected override string FormatContent(object data)
    {
        LogStep("Форматирование данных в формат CSV (разделение запятыми).");
        return "CSV-Контент: ID,Description,Amount\n1,Покупка A,100.50...";
    }

    protected override bool CustomerWantsSave()
    {
        return true;
    }
}

// --------------------------------------------------------------------
// ЧАСТЬ 3: Паттерн ПОСРЕДНИК (Mediator)
// --------------------------------------------------------------------

public interface IMediator
{
    void SendMessage(string senderName, string channelName, string message);
    void SendPrivateMessage(string senderName, string recipientName, string message);
    void JoinChannel(IChatUser user, string channelName);
    void LeaveChannel(IChatUser user, string channelName);
}

public interface IChatUser
{
    string GetName();
    void ReceiveMessage(string sender, string channel, string message);
    void ReceiveSystemNotification(string notification);
    void SetMediator(IMediator mediator);
}

public class ChatUser : IChatUser
{
    private string name;
    private IMediator mediator;
    private List<string> channels;

    public ChatUser(string name)
    {
        this.name = name;
        this.channels = new List<string>();
    }

    public string GetName() => name;

    public void SetMediator(IMediator mediator) => this.mediator = mediator;

    public void SendMessage(string channelName, string message)
    {
        if (mediator == null)
        {
            Console.WriteLine($"[ЧАТ ОШИБКА] {name} не подключен к посреднику.");
            return;
        }

        if (!channels.Contains(channelName))
        {
            Console.WriteLine($"[ЧАТ ОШИБКА] {name} не состоит в канале '{channelName}'. Сообщение не отправлено.");
            return;
        }

        mediator.SendMessage(name, channelName, message);
    }

    public void SendPrivateMessage(string recipientName, string message)
    {
        if (mediator == null) return;
        mediator.SendPrivateMessage(name, recipientName, message);
    }

    public void ReceiveMessage(string sender, string channel, string message)
    {
        Console.WriteLine($"[ЧАТ] {channel} | {sender} -> {name}: {message}");
    }

    public void ReceiveSystemNotification(string notification)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"[СИСТЕМА] {name}: {notification}");
        Console.ResetColor();
    }

    public void JoinChannel(string channelName)
    {
        if (mediator == null) return;
        mediator.JoinChannel(this, channelName);
        channels.Add(channelName);
    }

    public void LeaveChannel(string channelName)
    {
        if (mediator == null) return;
        mediator.LeaveChannel(this, channelName);
        channels.Remove(channelName);
    }
}

public class ChatMediator : IMediator
{
    private Dictionary<string, List<IChatUser>> channels = new Dictionary<string, List<IChatUser>>();
    private Dictionary<string, IChatUser> allUsers = new Dictionary<string, IChatUser>();

    public void RegisterUser(IChatUser user)
    {
        if (!allUsers.ContainsKey(user.GetName()))
        {
            allUsers.Add(user.GetName(), user);
            user.SetMediator(this);
        }
    }

    public void JoinChannel(IChatUser user, string channelName)
    {
        if (!channels.ContainsKey(channelName))
        {
            channels.Add(channelName, new List<IChatUser>());
            BroadcastSystemNotification(channelName, $"Канал '{channelName}' был создан.");
        }

        if (!channels[channelName].Contains(user))
        {
            channels[channelName].Add(user);
            BroadcastSystemNotification(channelName, $"{user.GetName()} присоединился к каналу.");
        }
    }

    public void LeaveChannel(IChatUser user, string channelName)
    {
        if (channels.ContainsKey(channelName) && channels[channelName].Contains(user))
        {
            channels[channelName].Remove(user);
            BroadcastSystemNotification(channelName, $"{user.GetName()} покинул канал.");
        }
    }

    public void SendMessage(string senderName, string channelName, string message)
    {
        if (!channels.ContainsKey(channelName))
        {
            Console.WriteLine($"[ЧАТ ОШИБКА] Канал '{channelName}' не существует.");
            return;
        }

        var usersInChannel = channels[channelName];
        foreach (var user in usersInChannel)
        {
            if (user.GetName() != senderName)
            {
                user.ReceiveMessage(senderName, channelName, message);
            }
        }
    }

    public void SendPrivateMessage(string senderName, string recipientName, string message)
    {
        if (allUsers.TryGetValue(recipientName, out IChatUser recipient))
        {
            recipient.ReceiveMessage(senderName, "[ПРИВАТ]", $"[Приватное] {message}");
            allUsers[senderName].ReceiveMessage("Вы", "[ПРИВАТ]", $"[Приватное отправлено] {message}");
        }
        else
        {
            Console.WriteLine($"[ЧАТ ОШИБКА] Пользователь '{recipientName}' не найден в системе.");
        }
    }

    private void BroadcastSystemNotification(string channelName, string notification)
    {
        if (channels.ContainsKey(channelName))
        {
            foreach (var user in channels[channelName])
            {
                user.ReceiveSystemNotification(notification);
            }
        }
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
        Console.WriteLine("Демонстрация Паттерна КОМАНДА (Smart Home)");
        Console.WriteLine("---------------------------------------------------------");
        DemonstrateCommandPattern();

        Console.WriteLine("\n\n---------------------------------------------------------");
        Console.WriteLine("Демонстрация Паттерна ШАБЛОННЫЙ МЕТОД (Report Generator)");
        Console.WriteLine("---------------------------------------------------------");
        DemonstrateTemplateMethodPattern();

        Console.WriteLine("\n\n---------------------------------------------------------");
        Console.WriteLine("Демонстрация Паттерна ПОСРЕДНИК (Chat System)");
        Console.WriteLine("---------------------------------------------------------");
        DemonstrateMediatorPattern();
    }

    public static void DemonstrateCommandPattern()
    {
        Light kitchenLight = new Light("Кухня");
        AirConditioner livingAC = new AirConditioner("Гостиная");
        SmartCurtains bedroomCurtains = new SmartCurtains("Спальня");

        ICommand kitchenLightOn = new LightOnCommand(kitchenLight);
        ICommand acSetCool = new ACTemperatureCommand(livingAC, 20);
        ICommand acSetWarm = new ACTemperatureCommand(livingAC, 28);
        ICommand curtainsOpen = new CurtainsOpenCommand(bedroomCurtains);

        RemoteControl remote = new RemoteControl();

        remote.SetCommand(0, kitchenLightOn);
        remote.SetCommand(1, acSetCool);
        remote.SetCommand(2, curtainsOpen);
        remote.SetCommand(6, acSetWarm);

        Console.WriteLine("\n*** Тест: Выполнение отдельных команд ***");
        remote.PressButton(0);
        remote.PressButton(1);
        remote.PressButton(6);
        remote.PressButton(2);

        Console.WriteLine("\n*** Тест: Отмена (Undo) ***");
        remote.PressUndo();
        remote.PressUndo();

        Console.WriteLine("\n*** Тест: Повтор (Redo) ***");
        remote.PressRedo();
        remote.PressRedo();

        Console.WriteLine("\n*** Тест: Макрокоманда (MacroCommand) ***");
        List<ICommand> eveningCommands = new List<ICommand>
        {
            new ACTemperatureCommand(livingAC, 23),
            new CurtainsOpenCommand(bedroomCurtains),
            new LightOnCommand(kitchenLight)
        };
        MacroCommand eveningMode = new MacroCommand(eveningCommands);
        remote.SetCommand(3, eveningMode);

        remote.PressButton(3);

        remote.PressUndo();

        Console.WriteLine("\n*** Тест: Обработка пустого слота (NoCommand) ***");
        remote.PressButton(5);
        remote.PressUndo();

        Console.WriteLine("\n*** Расширение: Запись макрокоманды из истории (Record) ***");
        remote.PressButton(0);
        remote.PressButton(1);

        MacroCommand recordedMacro = remote.RecordLastCommands(2);
        if (recordedMacro != null)
        {
            remote.SetCommand(4, recordedMacro);
            Console.WriteLine("\n[ПУЛЬТ] Записанная макрокоманда назначена на слот 4.");
            remote.PressButton(4);
        }
    }

    public static void DemonstrateTemplateMethodPattern()
    {
        ReportGenerator pdfGenerator = new PdfReport();
        ReportGenerator excelGenerator = new ExcelReport();
        ReportGenerator htmlGenerator = new HtmlReport();
        ReportGenerator csvGenerator = new CsvReport();

        Console.WriteLine("\n*** Генерация PDF Отчета (не сохраняется, не отправляется) ***");
        pdfGenerator.GenerateReport("Продажи_Q3_PDF");

        Console.WriteLine("\n*** Генерация Excel Отчета (принудительно сохраняется) ***");
        excelGenerator.GenerateReport("Финансы_2024_Excel");

        Console.WriteLine("\n*** Генерация HTML Отчета (сохраняется и отправляется Email) ***");
        htmlGenerator.GenerateReport("Статистика_Сайта_HTML");

        Console.WriteLine("\n*** Генерация CSV Отчета (Расширение) ***");
        csvGenerator.GenerateReport("Данные_Для_Анализа_CSV");
    }

    public static void DemonstrateMediatorPattern()
    {
        ChatMediator mediator = new ChatMediator();

        ChatUser ivan = new ChatUser("Иван");
        ChatUser elena = new ChatUser("Елена");
        ChatUser sergey = new ChatUser("Сергей");

        mediator.RegisterUser(ivan);
        mediator.RegisterUser(elena);
        mediator.RegisterUser(sergey);

        Console.WriteLine("\n*** Тест: Подключение к каналам ***");
        ivan.JoinChannel("Общий");
        elena.JoinChannel("Общий");
        sergey.JoinChannel("Проект_А");
        elena.JoinChannel("Проект_А");

        Console.WriteLine("\n*** Тест: Отправка сообщений в каналы ***");
        ivan.SendMessage("Общий", "Привет всем! Как дела?");
        elena.SendMessage("Проект_А", "Сергей, ты видел мой отчет по Проекту А?");

        Console.WriteLine("\n*** Тест: Отправка сообщения в чужой канал (Ошибка) ***");
        ivan.SendMessage("Проект_А", "Это сообщение не должно пройти.");

        Console.WriteLine("\n*** Тест: Приватные сообщения ***");
        elena.SendPrivateMessage("Иван", "Хочу обсудить Проект А лично.");

        Console.WriteLine("\n*** Тест: Выход из канала ***");
        elena.LeaveChannel("Общий");
        ivan.SendMessage("Общий", "Елена вышла из чата?");

        Console.WriteLine("\n*** Тест: Создание нового канала (при необходимости) ***");
        ChatUser pavel = new ChatUser("Павел");
        mediator.RegisterUser(pavel);
        pavel.JoinChannel("Маркетинг");
        elena.JoinChannel("Маркетинг");
        pavel.SendMessage("Маркетинг", "Запускаем новую кампанию!");
    }
}
