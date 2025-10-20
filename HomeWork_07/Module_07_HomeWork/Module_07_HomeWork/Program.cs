using System;
using System.Collections.Generic;

namespace DesignPatternsDemo
{

    public interface ICommand
    {
        void Execute();
        void Undo();
        bool WasExecuted { get; }
    }

    public class Light
    {
        public string Location { get; }
        public bool IsOn { get; private set; }
        public Light(string location) => Location = location;
        public void TurnOn()
        {
            IsOn = true;
            Console.WriteLine($"[Light] {Location} — ON");
        }
        public void TurnOff()
        {
            IsOn = false;
            Console.WriteLine($"[Light] {Location} — OFF");
        }
    }

    public class Door
    {
        public string Name { get; }
        public bool IsOpen { get; private set; }
        public Door(string name) => Name = name;
        public void Open()
        {
            IsOpen = true;
            Console.WriteLine($"[Door] {Name} — OPEN");
        }
        public void Close()
        {
            IsOpen = false;
            Console.WriteLine($"[Door] {Name} — CLOSED");
        }
    }

    public class Thermostat
    {
        public double Temperature { get; private set; }
        public Thermostat(double initial) => Temperature = initial;
        public void Increase(double delta)
        {
            Temperature += delta;
            Console.WriteLine($"[Thermostat] Temp increased by {delta:F1} → {Temperature:F1}°C");
        }
        public void Decrease(double delta)
        {
            Temperature -= delta;
            Console.WriteLine($"[Thermostat] Temp decreased by {delta:F1} → {Temperature:F1}°C");
        }
    }

    public class Television
    {
        public bool IsOn { get; private set; }
        public string Channel { get; private set; } = "None";
        public void TurnOn() { IsOn = true; Console.WriteLine("[TV] ON"); }
        public void TurnOff() { IsOn = false; Console.WriteLine("[TV] OFF"); }
        public void SetChannel(string channel) { Channel = channel; Console.WriteLine($"[TV] Channel set to {channel}"); }
    }

    public class LightOnCommand : ICommand
    {
        private readonly Light _light;
        public bool WasExecuted { get; private set; }
        public LightOnCommand(Light light) => _light = light;
        public void Execute()
        {
            _light.TurnOn();
            WasExecuted = true;
        }
        public void Undo()
        {
            if (!WasExecuted) throw new InvalidOperationException("Command wasn't executed.");
            _light.TurnOff();
            WasExecuted = false;
        }
    }

    public class LightOffCommand : ICommand
    {
        private readonly Light _light;
        public bool WasExecuted { get; private set; }
        public LightOffCommand(Light light) => _light = light;
        public void Execute()
        {
            _light.TurnOff();
            WasExecuted = true;
        }
        public void Undo()
        {
            if (!WasExecuted) throw new InvalidOperationException("Command wasn't executed.");
            _light.TurnOn();
            WasExecuted = false;
        }
    }

    public class DoorOpenCommand : ICommand
    {
        private readonly Door _door;
        public bool WasExecuted { get; private set; }
        public DoorOpenCommand(Door door) => _door = door;
        public void Execute()
        {
            _door.Open();
            WasExecuted = true;
        }
        public void Undo()
        {
            if (!WasExecuted) throw new InvalidOperationException("Command wasn't executed.");
            _door.Close();
            WasExecuted = false;
        }
    }

    public class DoorCloseCommand : ICommand
    {
        private readonly Door _door;
        public bool WasExecuted { get; private set; }
        public DoorCloseCommand(Door door) => _door = door;
        public void Execute()
        {
            _door.Close();
            WasExecuted = true;
        }
        public void Undo()
        {
            if (!WasExecuted) throw new InvalidOperationException("Command wasn't executed.");
            _door.Open();
            WasExecuted = false;
        }
    }

    public class IncreaseTempCommand : ICommand
    {
        private readonly Thermostat _thermostat;
        private readonly double _delta;
        private bool _executed = false;
        public bool WasExecuted => _executed;
        public IncreaseTempCommand(Thermostat thermostat, double delta)
        {
            _thermostat = thermostat;
            _delta = delta;
        }
        public void Execute()
        {
            _thermostat.Increase(_delta);
            _executed = true;
        }
        public void Undo()
        {
            if (!_executed) throw new InvalidOperationException("Command wasn't executed.");
            _thermostat.Decrease(_delta);
            _executed = false;
        }
    }

    public class DecreaseTempCommand : ICommand
    {
        private readonly Thermostat _thermostat;
        private readonly double _delta;
        private bool _executed = false;
        public bool WasExecuted => _executed;
        public DecreaseTempCommand(Thermostat thermostat, double delta)
        {
            _thermostat = thermostat;
            _delta = delta;
        }
        public void Execute()
        {
            _thermostat.Decrease(_delta);
            _executed = true;
        }
        public void Undo()
        {
            if (!_executed) throw new InvalidOperationException("Command wasn't executed.");
            _thermostat.Increase(_delta);
            _executed = false;
        }
    }

    public class TvToggleCommand : ICommand
    {
        private readonly Television _tv;
        public bool WasExecuted { get; private set; }
        public TvToggleCommand(Television tv) => _tv = tv;
        public void Execute()
        {
            if (_tv.IsOn) _tv.TurnOff();
            else _tv.TurnOn();
            WasExecuted = true;
        }
        public void Undo()
        {
            if (!WasExecuted) throw new InvalidOperationException("Command wasn't executed.");
            if (_tv.IsOn) _tv.TurnOff(); else _tv.TurnOn();
            WasExecuted = false;
        }
    }

    public class RemoteInvoker
    {
        private readonly Stack<ICommand> _history = new Stack<ICommand>();
        private readonly int _maxHistory;

        public RemoteInvoker(int maxHistory = 10)
        {
            _maxHistory = Math.Max(1, maxHistory);
        }

        public void ExecuteCommand(ICommand command)
        {
            try
            {
                command.Execute();
                if (command.WasExecuted)
                {
                    _history.Push(command);
                    // trim overflow
                    while (_history.Count > _maxHistory) _history.Pop();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Invoker] Ошибка при выполнении команды: {ex.Message}");
            }
        }

        public void UndoLast()
        {
            if (_history.Count == 0)
            {
                Console.WriteLine("[Invoker] Нечего отменять.");
                return;
            }
            var cmd = _history.Pop();
            try
            {
                cmd.Undo();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Invoker] Ошибка при отмене: {ex.Message}");
            }
        }

        public void UndoLastN(int n)
        {
            if (n <= 0) return;
            for (int i = 0; i < n; i++) UndoLast();
        }

        public void PrintHistory()
        {
            Console.WriteLine($"[Invoker] История ({_history.Count}):");
            foreach (var c in _history) Console.WriteLine($"  - {c.GetType().Name} (executed={c.WasExecuted})");
        }
    }

    public abstract class Beverage
    {
        public void PrepareRecipe()
        {
            BoilWater();
            BrewOrSteep();
            PourInCup();
            if (CustomerWantsCondiments())
            {
                AddCondiments();
            }
        }

        protected abstract void BrewOrSteep();
        protected abstract void AddCondiments();

        protected virtual void BoilWater() => Console.WriteLine("Boiling water");
        protected virtual void PourInCup() => Console.WriteLine("Pouring into cup");
        protected virtual bool CustomerWantsCondiments() => true;
    }

    public class Tea : Beverage
    {
        private readonly bool _wantsCondiments;
        public Tea(bool wantsCondiments = true) => _wantsCondiments = wantsCondiments;

        protected override void BrewOrSteep()
        {
            Console.WriteLine("Steeping the tea");
        }

        protected override void AddCondiments()
        {
            Console.WriteLine("Adding lemon");
        }

        protected override bool CustomerWantsCondiments()
        {
            return _wantsCondiments;
        }
    }

    public class Coffee : Beverage
    {
        private readonly Func<bool> _askUserHook;
        public Coffee(Func<bool> askUserHook = null) => _askUserHook = askUserHook ?? (() => true);

        protected override void BrewOrSteep()
        {
            Console.WriteLine("Brewing the coffee");
        }

        protected override void AddCondiments()
        {
            Console.WriteLine("Adding sugar and milk");
        }

        protected override bool CustomerWantsCondiments()
        {
            try
            {
                var res = _askUserHook();
                return res;
            }
            catch
            {
                Console.WriteLine("[Coffee] Ошибка при определении желания добавок — по умолчанию добавки НЕ будут добавлены.");
                return false;
            }
        }
    }

    public interface IMediator
    {
        void Register(User user);
        void Unregister(User user);
        void SendMessage(string from, string message, string to = null);
    }

    public class ChatRoom : IMediator
    {
        private readonly Dictionary<string, User> _users = new Dictionary<string, User>(StringComparer.OrdinalIgnoreCase);

        public void Register(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (_users.ContainsKey(user.Name))
            {
                Console.WriteLine($"[ChatRoom] Пользователь {user.Name} уже в чате.");
                return;
            }
            _users[user.Name] = user;
            user.SetMediator(this);
            BroadcastSystemMessage($"{user.Name} присоединился к чату.", exclude: user.Name);
            Console.WriteLine($"[ChatRoom] {user.Name} зарегистрирован(а).");
        }

        public void Unregister(User user)
        {
            if (user == null) return;
            if (_users.Remove(user.Name))
            {
                BroadcastSystemMessage($"{user.Name} покинул(а) чат.", exclude: user.Name);
            }
        }

        public void SendMessage(string from, string message, string to = null)
        {
            if (!_users.ContainsKey(from))
            {
                Console.WriteLine($"[ChatRoom] Ошибка: отправитель {from} не в чате.");
                return;
            }

            if (string.IsNullOrEmpty(to))
            {
                foreach (var kv in _users)
                {
                    if (!kv.Key.Equals(from, StringComparison.OrdinalIgnoreCase))
                    {
                        kv.Value.Receive(from, message, isPrivate: false);
                    }
                }
            }
            else
            {
                if (!_users.ContainsKey(to))
                {
                    Console.WriteLine($"[ChatRoom] Ошибка: получатель {to} не в чате.");
                    return;
                }
                _users[to].Receive(from, message, isPrivate: true);
            }
        }

        private void BroadcastSystemMessage(string text, string exclude = null)
        {
            foreach (var kv in _users)
            {
                if (kv.Key.Equals(exclude, StringComparison.OrdinalIgnoreCase)) continue;
                kv.Value.Receive("System", text, isPrivate: false);
            }
        }
    }

    public class User
    {
        public string Name { get; }
        private IMediator _mediator;

        public User(string name) => Name = name;

        internal void SetMediator(IMediator mediator) => _mediator = mediator;

        public void Send(string message, string to = null)
        {
            if (_mediator == null)
            {
                Console.WriteLine($"[User:{Name}] Ошибка: вы не в чате.");
                return;
            }
            _mediator.SendMessage(Name, message, to);
        }

        public void Receive(string from, string message, bool isPrivate)
        {
            if (isPrivate)
                Console.WriteLine($"[Private][{Name}] от {from}: {message}");
            else
                Console.WriteLine($"[{Name}] от {from}: {message}");
        }
    }

    class Program
    {
        static void Main()
        {
            Console.WriteLine("=== Command Pattern Demo ===");
            var livingLight = new Light("Living Room");
            var frontDoor = new Door("Front Door");
            var thermo = new Thermostat(22.0);
            var tv = new Television();

            var invoker = new RemoteInvoker(maxHistory: 5);

            var cmd1 = new LightOnCommand(livingLight);
            var cmd2 = new DoorOpenCommand(frontDoor);
            var cmd3 = new IncreaseTempCommand(thermo, 1.5);
            var cmd4 = new TvToggleCommand(tv);

            invoker.ExecuteCommand(cmd1);
            invoker.ExecuteCommand(cmd4);
            invoker.ExecuteCommand(cmd2);
            invoker.ExecuteCommand(cmd3);

            invoker.PrintHistory();

            Console.WriteLine("\nОтмена последней команды:");
            invoker.UndoLast();

            Console.WriteLine("\nОтмена 2 команд:");
            invoker.UndoLastN(2);

            Console.WriteLine("\nПопытка отменить при пустой истории:");
            invoker.UndoLastN(10);

            Console.WriteLine("\nТест: попытка отменить не выполненную команду напрямую:");
            var fake = new LightOffCommand(new Light("Ghost"));
            try
            {
                fake.Undo();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ожидаемая ошибка: {ex.Message}");
            }

            Console.WriteLine("\n=== Template Method Demo ===");
            Beverage tea = new Tea(wantsCondiments: true);
            Console.WriteLine("\n--- Making Tea ---");
            tea.PrepareRecipe();

            Beverage coffeeWithout = new Coffee(() => false); 
            Console.WriteLine("\n--- Making Coffee (без добавок) ---");
            coffeeWithout.PrepareRecipe();

            Beverage coffeeWith = new Coffee(() =>
            {
                return true;
            });
            Console.WriteLine("\n--- Making Coffee (с добавками) ---");
            coffeeWith.PrepareRecipe();

            Console.WriteLine("\n=== Mediator (ChatRoom) Demo ===");
            var room = new ChatRoom();
            var alice = new User("Alice");
            var bob = new User("Bob");
            var cathy = new User("Cathy");

            room.Register(alice);
            room.Register(bob);
            room.Register(cathy);

            alice.Send("Привет всем!");
            bob.Send("Привет, Alice!");
            cathy.Send("Привет!", to: "Alice");

            var stranger = new User("Stranger");
            stranger.Send("Можно войти?");

            room.Unregister(bob);
            alice.Send("Bob ушёл?");

            Console.WriteLine("\n=== Конец демонстрации ===");

            PrintSelfCheckAnswers();
        }

        static void PrintSelfCheckAnswers()
        {
            Console.WriteLine("\n=== Вопросы для самопроверки и ответы (кратко) ===\n");

            Console.WriteLine("1) Какие преимущества даёт паттерн 'Команда'?");
            Console.WriteLine("- Инкапсуляция запроса как объекта (удобно хранить/журналировать/переигрывать)");
            Console.WriteLine("- Позволяет реализовать отмену/повтор/отложенное выполнение");
            Console.WriteLine("- Упрощает добавление новых команд без изменения Invoker/Receiver");

            Console.WriteLine("\n2) Как добавить новые команды, не меняя существующий код?");
            Console.WriteLine("- Создать новый класс, реализующий интерфейс ICommand. Invoker и Receivers остаются без изменений.");

            Console.WriteLine("\n3) В чём отличие 'Команды' от прямого вызова методов?");
            Console.WriteLine("- Прямой вызов выполняет действие немедленно и не инкапсулирует его в объект. 'Команда' — объект, который можно хранить, логировать, отменять, отправлять по сети, комбинировать и т.д.");

            Console.WriteLine("\n--- Template Method: вопросы ---");
            Console.WriteLine("1) Преимущества шаблонного метода?");
            Console.WriteLine("- Определяет алгоритм в одном месте, подклассы переопределяют шаги. Уменьшаются дублирование и жесткая связность.");

            Console.WriteLine("\n2) Как добавить новый напиток без изменения базового класса?");
            Console.WriteLine("- Создать подкласс Beverage и реализовать абстрактные методы (и при необходимости переопределить hook).");

            Console.WriteLine("\n3) Hook vs abstract method?");
            Console.WriteLine("- Hook — необязательная точка расширения с дефолтной реализацией; abstract — обязательный шаг, который подкласс должен реализовать.");

            Console.WriteLine("\n--- Mediator: вопросы ---");
            Console.WriteLine("1) Преимущества посредника?");
            Console.WriteLine("- Развязывает участников (User не знает про других напрямую), централизует логику взаимодействия и упрощает изменение правил доставки сообщений.");

            Console.WriteLine("\n2) Как добавить новых типов участников?");
            Console.WriteLine("- Создать подкласс User или реализовать новый интерфейс; медиатор остаётся прежним (если он работает с абстракцией User).");

            Console.WriteLine("\n3) Как поддержать груповые или личные сообщения?");
            Console.WriteLine("- Mediator должен иметь методы SendMessage(from, message, toGroupId) или toUser; логика маршрутизации реализуется в Mediator (пример показан: public/private).");
        }
    }
}
