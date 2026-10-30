using System;
using System.Collections.Generic;
using System.IO;
using System.Collections;
class Entity
{
    private int id;

    public int Id
    {
        get { return id; }
        set { id = value; }
    }
}
static class SafeExecutor
{
    public static void Run(Action action)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            File.AppendAllText("log.txt", $"{DateTime.Now}: {ex.GetType().Name}: {ex.Message}\n");

            Console.WriteLine("Сталася помилка. Деталі записано у log.txt");
        }
    }
}


class InvalidVehicleDataException : Exception
{
    public InvalidVehicleDataException(string msg) : base(msg) { }
}

class VehicleNotFoundException : Exception
{
    public VehicleNotFoundException(string msg) : base(msg) { }
}

class OwnerNotFoundException : Exception
{
    public OwnerNotFoundException(string msg) : base(msg) { }
}

class Result<T>
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public T Value { get; set; }

    public Result(bool isSuccess, string message, T value)
    {
        IsSuccess = isSuccess;
        Message = message;
        Value = value;
    }
}

class ReportService
{
    public void ExportToCsv(List<Vehicle> vehicles)
    {
        string path = "report.csv";

        try
        {
            File.WriteAllText(path, "Id,Brand,Model,Year,Owner\n");

            int withOwner = 0;
            int withoutOwner = 0;

            foreach (Vehicle vehicle in vehicles)
            {
                string owner = vehicle.Owner == null ? "none" : vehicle.Owner.Fullname;

                if (vehicle.Owner == null)
                    withoutOwner++;
                else
                    withOwner++;

                File.AppendAllText(path,
                    $"{vehicle.Id},{vehicle.Brand},{vehicle.Model},{vehicle.Year},{owner}\n");
            }

            File.AppendAllText(path, "\n");
            File.AppendAllText(path, $"Всього транспорту,{vehicles.Count}\n");
            File.AppendAllText(path, $"З власником,{withOwner}\n");
            File.AppendAllText(path, $"Без власника,{withoutOwner}\n");

            Console.WriteLine("Звіт створено.");
        }
        catch (IOException)
        {
            Console.WriteLine("Помилка роботи з файлом.");
        }
        finally
        {
            Console.WriteLine("Спробу експорту завершено.");
        }
    }

    public void ReadFromCsv()
    {
        string path = "report.csv";

        try
        {
            string text = File.ReadAllText(path);
            Console.WriteLine(text);
        }
        catch (IOException)
        {
            Console.WriteLine("Помилка читання файлу.");
        }
        finally
        {
            Console.WriteLine("Спробу читання завершено.");
        }
    }
}

static class Validator
{
    public static bool IsValidString(string value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }

    public static bool IsValidYear(int year)
    {
        return year >= 1886 && year <= DateTime.Now.Year;
    }

    public static bool IsValidDoors(int doors)
    {
        return doors >= 2 && doors <= 5;
    }

    public static bool IsValidEngineVolume(int volume)
    {
        return volume > 0;
    }
}

static class IdGenerator
{
    private static int nextId = 1;

    public static int NextId()
    {
        return nextId++;
    }
}

class Vehicle : Entity
{
    private string brand;
    private string model;
    private int year;
    private Owner owner;

    public string Brand
    {
        get { return brand; }
        set
        {
            if (!Validator.IsValidString(value))
                throw new InvalidVehicleDataException("Марка не може бути порожньою.");

            brand = value;
        }
    }

    public string Model
    {
        get { return model; }
        set
        {
            if (!Validator.IsValidString(value))
                throw new InvalidVehicleDataException("Модель не може бути порожньою.");

            model = value;
        }
    }

    public int Year
    {
        get { return year; }
        set
        {
            if (!Validator.IsValidYear(value))
                throw new InvalidVehicleDataException("Некоректний рік випуску!");

            year = value;
        }
    }

    public Owner Owner
    {
        get { return owner; }
        set { owner = value; }
    }

    public Vehicle(string brand, string model, int year)
    {
        if (!Validator.IsValidString(brand))
            throw new InvalidVehicleDataException("Марка не може бути порожньою.");

        if (!Validator.IsValidString(model))
            throw new InvalidVehicleDataException("Модель не може бути порожньою.");

        if (!Validator.IsValidYear(year))
            throw new InvalidVehicleDataException("Некоректний рік випуску!");

        Id = IdGenerator.NextId();
        this.brand = brand;
        this.model = model;
        this.year = year;
        owner = null;
    }

    public virtual string GetInfo()
    {
        string ownerInfo = owner == null ? "none" : owner.Fullname;
        return $"Id: {Id}, Vehicle: {Brand} {Model}, Year: {Year}, Owner: {ownerInfo}";
    }
}

class Car : Vehicle
{
    private int doorscount;
    private string fueltype;

    public int Doorscount
    {
        get { return doorscount; }
        set
        {
            if (!Validator.IsValidDoors(value))
                throw new InvalidVehicleDataException("Кількість дверей повинна бути від 2 до 5.");

            doorscount = value;
        }
    }

    public string Fueltype
    {
        get { return fueltype; }
        set
        {
            if (!Validator.IsValidString(value))
                throw new InvalidVehicleDataException("Тип палива не може бути порожнім.");

            fueltype = value;
        }
    }

    public Car(string brand, string model, int year, int doorscount, string fueltype)
        : base(brand, model, year)
    {
        Doorscount = doorscount;
        Fueltype = fueltype;
    }

    public override string GetInfo()
    {
        string ownerInfo = Owner == null ? "none" : Owner.Fullname;
        return $"Id: {Id}, Car: {Brand} {Model}, Year: {Year}, Doors: {Doorscount}, Fueltype: {Fueltype}, Owner: {ownerInfo}";
    }
}

class Motorcycle : Vehicle
{
    private int enginevolume;
    private string motorcycletype;

    public int Enginevolume
    {
        get { return enginevolume; }
        set
        {
            if (!Validator.IsValidEngineVolume(value))
                throw new InvalidVehicleDataException("Об'єм двигуна повинен бути більше 0.");

            enginevolume = value;
        }
    }

    public string Motorcycletype
    {
        get { return motorcycletype; }
        set
        {
            if (!Validator.IsValidString(value))
                throw new InvalidVehicleDataException("Тип мотоцикла не може бути порожнім.");

            motorcycletype = value;
        }
    }

    public Motorcycle(string brand, string model, int year, int enginevolume, string motorcycletype)
        : base(brand, model, year)
    {
        Enginevolume = enginevolume;
        Motorcycletype = motorcycletype;
    }

    public override string GetInfo()
    {
        string ownerInfo = Owner == null ? "none" : Owner.Fullname;
        return $"Id: {Id}, Motorcycle: {Brand} {Model}, Year: {Year}, Enginevolume: {Enginevolume}, Motorcycletype: {Motorcycletype}, Owner: {ownerInfo}";
    }
}

class Owner : Entity
{
    private string fullname;
    private string phone;

    public string Fullname
    {
        get { return fullname; }
        set { fullname = value; }
    }

    public string Phone
    {
        get { return phone; }
        set { phone = value; }
    }

    public Owner(string fullname, string phone)
    {
        if (!Validator.IsValidString(fullname))
            throw new ArgumentException("ПІБ не може бути порожнім.");

        if (!Validator.IsValidString(phone))
            throw new ArgumentException("Телефон не може бути порожнім.");

        Id = IdGenerator.NextId();
        this.fullname = fullname;
        this.phone = phone;
    }

    public string GetInfo()
    {
        return $"Id: {Id}, Name: {Fullname}, Phone: {Phone}";
    }
}

class Repository<T> : IEnumerable<T> where T : Entity
{
    private List<T> entities = new List<T>();

    public bool Add(T entity)
    {
        if (entity == null)
            return false;

        foreach (T item in entities)
        {
            if (item.Id == entity.Id)
                return false;
        }

        entities.Add(entity);
        return true;
    }

    public T Find(int id)
    {
        foreach (T item in entities)
        {
            if (item.Id == id)
                return item;
        }

        return null;
    }

    public bool Remove(int id)
    {
        T item = Find(id);

        if (item == null)
            return false;

        entities.Remove(item);
        return true;
    }

    public List<T> GetAll()
    {
        return entities;
    }
    
    public IEnumerator<T> GetEnumerator()
    {
        foreach (T item in entities)
        {
            yield return item;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
interface INotificationService
{
    void ShowMessage(string message);
}

interface IEventLogger
{
    void WriteLog(string message);
}

class NotificationService : INotificationService
{
    public void ShowMessage(string message)
    {
        Console.WriteLine("[Подія] " + message);
    }
}

class EventLogService : IEventLogger
{
    public void WriteLog(string message)
    {
        File.AppendAllText("log.txt",
            $"{DateTime.Now}: {message}{Environment.NewLine}");
    }
}

class VehicleService
{
    private Repository<Vehicle> vehicles = new Repository<Vehicle>();
    private Repository<Owner> owners = new Repository<Owner>();
    public event Action<Vehicle, Owner> OwnerAssigned;
    
    
    public event Action<string> VehicleAdded;
    public event Action<string> VehicleUpdated;
    public event Action<string> VehicleRemoved;
    public event Action<string> OperationFailed;

   
    public bool AddVehicle(Vehicle vehicle)
    {
        if (vehicle == null)
        {
            OperationFailed?.Invoke("Спроба додати порожній транспорт");
            return false;
        }

        bool result = vehicles.Add(vehicle);

        if (result)
        {
            VehicleAdded?.Invoke(
                $"Додано транспорт: {vehicle.Brand} {vehicle.Model}, ID: {vehicle.Id}");
        }
        else
        {
            OperationFailed?.Invoke("Не вдалося додати транспорт");
        }

        return result;
    }
    
    public List<Vehicle> FilterVehicles(Predicate<Vehicle> condition)
    {
        List<Vehicle> result = new List<Vehicle>();

        foreach (Vehicle vehicle in vehicles.GetAll())
        {
            if (condition(vehicle))
            {
                result.Add(vehicle);
            }
        }

        return result;
    }

    public List<Vehicle> SortVehicles(Func<Vehicle, int> selector)
    {
        List<Vehicle> result = new List<Vehicle>(vehicles.GetAll());

        result.Sort((a, b) => selector(a).CompareTo(selector(b)));

        return result;
    }

    public void ProcessVehicles(Action<Vehicle> action)
    {
        foreach (Vehicle vehicle in vehicles)
        {
            action(vehicle);
        }
    }
    
    public Result<Owner> TryFindOwner(int id)
    {
        Owner owner = owners.Find(id);

        if (owner == null)
            return new Result<Owner>(false, "Owner not found", null);

        return new Result<Owner>(true, "Owner found", owner);
    }

    public Result<Vehicle> TryFindVehicle(int id)
    {
        Vehicle vehicle = vehicles.Find(id);

        if (vehicle == null)
            return new Result<Vehicle>(false, "Vehicle not found", null);

        return new Result<Vehicle>(true, "Vehicle found", vehicle);
    }

    public Vehicle FindVehicle(int id)
    {
        Vehicle vehicle = vehicles.Find(id);

        if (vehicle == null)
            throw new VehicleNotFoundException("Vehicle not found");

        return vehicle;
    }

    public List<Vehicle> FindVehicleByBrand(string brand)
    {
        List<Vehicle> result = new List<Vehicle>();

        if (!Validator.IsValidString(brand))
            return result;

        foreach (Vehicle vehicle in vehicles.GetAll())
        {
            if (vehicle.Brand.Equals(brand.Trim(), StringComparison.OrdinalIgnoreCase))
                result.Add(vehicle);
        }

        return result;
    }

    
    public bool UpdateVehicle(int id, string brand, string model, int year)
    {
        Vehicle vehicle = vehicles.Find(id);

        if (vehicle == null)
        {
            OperationFailed?.Invoke($"Транспорт з ID {id} не знайдено");
            return false;
        }

        if (!Validator.IsValidYear(year))
        {
            OperationFailed?.Invoke("Некоректний рік випуску");
            throw new InvalidVehicleDataException("Некоректний рік випуску!");
        }

        if (!Validator.IsValidString(brand))
        {
            OperationFailed?.Invoke("Марка не може бути порожньою");
            throw new InvalidVehicleDataException("Марка не може бути порожньою.");
        }

        if (!Validator.IsValidString(model))
        {
            OperationFailed?.Invoke("Модель не може бути порожньою");
            throw new InvalidVehicleDataException("Модель не може бути порожньою.");
        }

        vehicle.Brand = brand;
        vehicle.Model = model;
        vehicle.Year = year;

        VehicleUpdated?.Invoke(
            $"Оновлено транспорт: {vehicle.Brand} {vehicle.Model}, ID: {id}");

        return true;
    }


    
    public bool RemoveVehicle(int id)
    {
        Vehicle vehicle = vehicles.Find(id);

        if (vehicle == null)
        {
            OperationFailed?.Invoke($"Транспорт з ID {id} не знайдено");
            return false;
        }

        if (vehicle.Owner != null)
        {
            OperationFailed?.Invoke(
                $"Не можна видалити транспорт ID {id}, оскільки він має власника");
            return false;
        }

        bool result = vehicles.Remove(id);

        if (result)
        {
            VehicleRemoved?.Invoke(
                $"Видалено транспорт: {vehicle.Brand} {vehicle.Model}, ID: {id}");
        }
        else
        {
            OperationFailed?.Invoke($"Не вдалося видалити транспорт ID {id}");
        }

        return result;
    }


    public bool AddOwner(Owner owner)
    {
        return owners.Add(owner);
    }

    public Owner FindOwner(int id)
    {
        Owner owner = owners.Find(id);

        if (owner == null)
            throw new OwnerNotFoundException("Owner not found");

        return owner;
    }

    public bool UpdateOwner(int id, string fullname, string phone)
    {
        Owner owner = owners.Find(id);

        if (owner == null)
            return false;

        if (!Validator.IsValidString(fullname) || !Validator.IsValidString(phone))
            return false;

        owner.Fullname = fullname;
        owner.Phone = phone;

        return true;
    }

    public bool RemoveOwner(int id)
    {
        Owner owner = owners.Find(id);

        if (owner == null)
            return false;

        foreach (Vehicle vehicle in vehicles.GetAll())
        {
            if (vehicle.Owner == owner)
                return false;
        }

        return owners.Remove(id);
    }

    
    public bool AssignOwner(int vehicleId, int ownerId)
    {
        Vehicle vehicle = vehicles.Find(vehicleId);
        Owner owner = owners.Find(ownerId);

        if (vehicle == null || owner == null || vehicle.Owner != null)
            return false;

        vehicle.Owner = owner;

        OwnerAssigned?.Invoke(vehicle, owner);

        return true;
    }

    public bool RemoveOwnerFromVehicle(int vehicleId)
    {
        Vehicle vehicle = vehicles.Find(vehicleId);

        if (vehicle == null || vehicle.Owner == null)
            return false;

        vehicle.Owner = null;
        return true;
    }

    public List<Vehicle> GetVehicles()
    {
        return vehicles.GetAll();
    }

    public List<Owner> GetOwners()
    {
        return owners.GetAll();
    }
}

class PR1
{
    static void Main()
    {
        VehicleService service = new VehicleService();
        ReportService report = new ReportService();
        
        
        INotificationService notification = new NotificationService();
        IEventLogger eventLogger = new EventLogService();

        service.VehicleAdded += notification.ShowMessage;
        service.VehicleAdded += eventLogger.WriteLog;

        service.VehicleUpdated += notification.ShowMessage;
        service.VehicleUpdated += eventLogger.WriteLog;

        service.VehicleRemoved += notification.ShowMessage;
        service.VehicleRemoved += eventLogger.WriteLog;

        service.OperationFailed += notification.ShowMessage;
        service.OperationFailed += eventLogger.WriteLog;

        
        service.OwnerAssigned += (vehicle, owner) =>
        {
            Console.WriteLine(
                $"Подія: {owner.Fullname} став власником {vehicle.Brand} {vehicle.Model}"
            );
        };

        service.OwnerAssigned += (vehicle, owner) =>
        {
            File.AppendAllText("log.txt",
                $"{DateTime.Now}: {owner.Fullname} став власником {vehicle.Brand} {vehicle.Model}\n"
            );
        };

        bool work = true;

        while (work)
        {
            Console.WriteLine();
            Console.WriteLine("===== МЕНЮ =====");
            Console.WriteLine("1. Додати транспорт");
            Console.WriteLine("2. Показати весь транспорт");
            Console.WriteLine("3. Знайти транспорт за ID");
            Console.WriteLine("4. Знайти транспорт за маркою");
            Console.WriteLine("5. Оновити транспорт");
            Console.WriteLine("6. Видалити транспорт");
            Console.WriteLine("7. Додати власника");
            Console.WriteLine("8. Показати всіх власників");
            Console.WriteLine("9. Знайти власника");
            Console.WriteLine("10. Оновити власника");
            Console.WriteLine("11. Видалити власника");
            Console.WriteLine("12. Призначити власника транспорту");
            Console.WriteLine("13. Відкріпити власника");
            Console.WriteLine("14. Створити звіт");
            Console.WriteLine("15. Прочитати звіт");
            Console.WriteLine("16. Демонстрація винятку");
            Console.WriteLine("17. Демонстрація делегатів");
            Console.WriteLine("18. Демонстрація подій");
            Console.WriteLine("19. Демонстрація інтерфейсів та ітерації");
            Console.WriteLine("0. Вихід");
            Console.Write("Виберіть дію: ");

            if (!int.TryParse(Console.ReadLine(), out int choice))
            {
                Console.WriteLine("Введіть число!");
                continue;
            }
            
            SafeExecutor.Run(() =>
                {
                    switch (choice)
                    {
                        case 1:
                        {
                            Console.WriteLine("1. Автомобіль");
                            Console.WriteLine("2. Мотоцикл");
                            Console.Write("Виберіть тип: ");

                            if (!int.TryParse(Console.ReadLine(), out int type))
                            {
                                Console.WriteLine("Введіть коректний тип.");
                                break;
                            }

                            Console.Write("Введіть марку: ");
                        string brand = Console.ReadLine();

                        if (!Validator.IsValidString(brand))
                        {
                            Console.WriteLine("Марка не може бути порожньою.");
                            break;
                        }

                        Console.Write("Введіть модель: ");
                        string model = Console.ReadLine();

                        if (!Validator.IsValidString(model))
                        {
                            Console.WriteLine("Модель не може бути порожньою.");
                            break;
                        }

                        Console.Write("Введіть рік: ");

                        if (!int.TryParse(Console.ReadLine(), out int year))
                        {
                            Console.WriteLine("Введіть коректний рік.");
                            break;
                        }

                        if (!Validator.IsValidYear(year))
                        {
                            Console.WriteLine("Рік повинен бути від 1886 до поточного року.");
                            break;
                        }

                        if (type == 1)
                        {
                            Console.Write("Введіть кількість дверей: ");

                            if (!int.TryParse(Console.ReadLine(), out int doorscount))
                            {
                                Console.WriteLine("Введіть коректну кількість дверей.");
                                break;
                            }

                            if (!Validator.IsValidDoors(doorscount))
                            {
                                Console.WriteLine("Кількість дверей повинна бути від 2 до 5.");
                                break;
                            }

                            Console.Write("Введіть тип палива: ");
                            string fueltype = Console.ReadLine();

                            if (!Validator.IsValidString(fueltype))
                            {
                                Console.WriteLine("Тип палива не може бути порожнім.");
                                break;
                            }

                            Car car = new Car(brand, model, year, doorscount, fueltype);

                            if (service.AddVehicle(car))
                                Console.WriteLine("Автомобіль додано.");
                            else
                                Console.WriteLine("Не вдалося додати автомобіль.");
                        }
                        else if (type == 2)
                        {
                            Console.Write("Введіть об'єм двигуна: ");

                            if (!int.TryParse(Console.ReadLine(), out int enginevolume))
                            {
                                Console.WriteLine("Введіть коректний об'єм двигуна.");
                                break;
                            }

                            if (!Validator.IsValidEngineVolume(enginevolume))
                            {
                                Console.WriteLine("Об'єм двигуна повинен бути більше 0.");
                                break;
                            }

                            Console.Write("Введіть тип мотоцикла: ");
                            string motorcycletype = Console.ReadLine();

                            if (!Validator.IsValidString(motorcycletype))
                            {
                                Console.WriteLine("Тип мотоцикла не може бути порожнім.");
                                break;
                            }

                            Motorcycle motorcycle = new Motorcycle(
                                brand, model, year, enginevolume, motorcycletype);

                            if (service.AddVehicle(motorcycle))
                                Console.WriteLine("Мотоцикл додано.");
                            else
                                Console.WriteLine("Не вдалося додати мотоцикл.");
                        }
                        else
                        {
                            Console.WriteLine("Невірний тип транспорту.");
                        }

                        break;
                    }

                    case 2:
                    {
                        List<Vehicle> vehicles = service.GetVehicles();

                        if (vehicles.Count == 0)
                            Console.WriteLine("Транспорт відсутній.");
                        else
                            foreach (Vehicle vehicle in vehicles)
                                Console.WriteLine(vehicle.GetInfo());

                        break;
                    }

                    case 3:
                    {
                        Console.Write("Введіть ID транспорту: ");

                        if (!int.TryParse(Console.ReadLine(), out int searchId))
                        {
                            Console.WriteLine("Введіть коректний ID!");
                            break;
                        }

                        Result<Vehicle> result = service.TryFindVehicle(searchId);

                        if (result.IsSuccess)
                            Console.WriteLine(result.Value.GetInfo());
                        else
                            Console.WriteLine(result.Message);

                        break;
                    }

                    case 4:
                    {
                        Console.Write("Введіть марку: ");
                        string searchBrand = Console.ReadLine();

                        if (!Validator.IsValidString(searchBrand))
                        {
                            Console.WriteLine("Марка для пошуку не може бути порожньою.");
                            break;
                        }

                        List<Vehicle> vehiclesByBrand =
                            service.FindVehicleByBrand(searchBrand);

                        if (vehiclesByBrand.Count == 0)
                            Console.WriteLine("Транспорт не знайдено.");
                        else
                            foreach (Vehicle vehicle in vehiclesByBrand)
                                Console.WriteLine(vehicle.GetInfo());

                        break;
                    }

                    case 5:
                    {
                        Console.Write("Введіть ID транспорту: ");

                        if (!int.TryParse(Console.ReadLine(), out int updateId))
                        {
                            Console.WriteLine("Введіть коректний ID!");
                            break;
                        }

                        Console.Write("Введіть нову марку: ");
                        string newBrand = Console.ReadLine();

                        if (!Validator.IsValidString(newBrand))
                        {
                            Console.WriteLine("Марка не може бути порожньою.");
                            break;
                        }

                        Console.Write("Введіть нову модель: ");
                        string newModel = Console.ReadLine();

                        if (!Validator.IsValidString(newModel))
                        {
                            Console.WriteLine("Модель не може бути порожньою.");
                            break;
                        }

                        Console.Write("Введіть новий рік: ");

                        if (!int.TryParse(Console.ReadLine(), out int newYear))
                        {
                            Console.WriteLine("Введіть коректний рік.");
                            break;
                        }

                        if (!Validator.IsValidYear(newYear))
                        {
                            Console.WriteLine("Рік повинен бути від 1886 до поточного року.");
                            break;
                        }

                        if (service.UpdateVehicle(updateId, newBrand, newModel, newYear))
                            Console.WriteLine("Транспорт оновлено.");
                        else
                            Console.WriteLine("Транспорт не знайдено.");

                        break;
                    }

                    case 6:
                    {
                        Console.Write("Введіть ID транспорту: ");

                        if (!int.TryParse(Console.ReadLine(), out int deleteId))
                        {
                            Console.WriteLine("Введіть коректний ID!");
                            break;
                        }

                        if (service.RemoveVehicle(deleteId))
                            Console.WriteLine("Транспорт видалено.");
                        else
                            Console.WriteLine("Не можна видалити транспорт.");

                        break;
                    }

                    case 7:
                    {
                        Console.Write("Введіть ПІБ: ");
                        string fullname = Console.ReadLine();

                        if (!Validator.IsValidString(fullname))
                        {
                            Console.WriteLine("ПІБ не може бути порожнім.");
                            break;
                        }

                        Console.Write("Введіть телефон: ");
                        string phone = Console.ReadLine();

                        if (!Validator.IsValidString(phone))
                        {
                            Console.WriteLine("Телефон не може бути порожнім.");
                            break;
                        }

                        Owner owner = new Owner(fullname, phone);

                        if (service.AddOwner(owner))
                            Console.WriteLine("Власника додано.");
                        else
                            Console.WriteLine("Не вдалося додати власника.");

                        break;
                    }

                    case 8:
                    {
                        List<Owner> owners = service.GetOwners();

                        if (owners.Count == 0)
                            Console.WriteLine("Власників немає.");
                        else
                            foreach (Owner currentOwner in owners)
                                Console.WriteLine(currentOwner.GetInfo());

                        break;
                    }

                    case 9:
                    {
                        Console.Write("Введіть ID власника: ");

                        if (!int.TryParse(Console.ReadLine(), out int findOwnerId))
                        {
                            Console.WriteLine("Введіть коректний ID!");
                            break;
                        }

                        Result<Owner> result = service.TryFindOwner(findOwnerId);

                        if (result.IsSuccess)
                            Console.WriteLine(result.Value.GetInfo());
                        else
                            Console.WriteLine(result.Message);

                        break;
                    }

                    case 10:
                    {
                        Console.Write("Введіть ID власника: ");

                        if (!int.TryParse(Console.ReadLine(), out int updateOwnerId))
                        {
                            Console.WriteLine("Введіть коректний ID!");
                            break;
                        }

                        Console.Write("Введіть нове ПІБ: ");
                        string newFullname = Console.ReadLine();

                        if (!Validator.IsValidString(newFullname))
                        {
                            Console.WriteLine("ПІБ не може бути порожнім.");
                            break;
                        }

                        Console.Write("Введіть новий телефон: ");
                        string newPhone = Console.ReadLine();

                        if (!Validator.IsValidString(newPhone))
                        {
                            Console.WriteLine("Телефон не може бути порожнім.");
                            break;
                        }

                        if (service.UpdateOwner(updateOwnerId, newFullname, newPhone))
                            Console.WriteLine("Власника оновлено.");
                        else
                            Console.WriteLine("Власника не знайдено.");

                        break;
                    }

                    case 11:
                    {
                        Console.Write("Введіть ID власника: ");

                        if (!int.TryParse(Console.ReadLine(), out int deleteOwnerId))
                        {
                            Console.WriteLine("Введіть коректний ID!");
                            break;
                        }

                        if (service.RemoveOwner(deleteOwnerId))
                            Console.WriteLine("Власника видалено.");
                        else
                            Console.WriteLine("Не можна видалити власника.");

                        break;
                    }

                    case 12:
                    {
                        Console.Write("Введіть ID транспорту: ");

                        if (!int.TryParse(Console.ReadLine(), out int vehicleId))
                        {
                            Console.WriteLine("Введіть коректний ID!");
                            break;
                        }

                        Console.Write("Введіть ID власника: ");

                        if (!int.TryParse(Console.ReadLine(), out int ownerIdForVehicle))
                        {
                            Console.WriteLine("Введіть коректний ID!");
                            break;
                        }

                        if (service.AssignOwner(vehicleId, ownerIdForVehicle))
                            Console.WriteLine("Власника призначено.");
                        else
                            Console.WriteLine("Не вдалося призначити власника.");

                        break;
                    }

                    case 13:
                    {
                        Console.Write("Введіть ID транспорту: ");

                        if (!int.TryParse(Console.ReadLine(), out int vehicleIdForRemove))
                        {
                            Console.WriteLine("Введіть коректний ID!");
                            break;
                        }

                        if (service.RemoveOwnerFromVehicle(vehicleIdForRemove))
                            Console.WriteLine("Власника відкріплено.");
                        else
                            Console.WriteLine("Не вдалося відкріпити власника.");

                        break;
                    }

                    case 14:
                        report.ExportToCsv(service.GetVehicles());
                        break;

                    case 15:
                        report.ReadFromCsv();
                        break;

                    case 16:
                    {
                        Console.WriteLine("1. Пошук транспорту через виняток");
                        Console.WriteLine("2. Пошук власника через виняток");
                        Console.Write("Виберіть дію: ");

                        if (!int.TryParse(Console.ReadLine(), out int exceptionChoice))
                        {
                            Console.WriteLine("Введіть коректне число.");
                            break;
                        }

                        if (exceptionChoice == 1)
                        {
                            Console.Write("Введіть ID транспорту: ");

                            if (!int.TryParse(Console.ReadLine(), out int vehicleId))
                            {
                                Console.WriteLine("Введіть коректний ID.");
                                break;
                            }

                            Vehicle vehicle = service.FindVehicle(vehicleId);
                            Console.WriteLine(vehicle.GetInfo());
                        }
                        else if (exceptionChoice == 2)
                        {
                            Console.Write("Введіть ID власника: ");

                            if (!int.TryParse(Console.ReadLine(), out int ownerId))
                            {
                                Console.WriteLine("Введіть коректний ID.");
                                break;
                            }

                            Owner owner = service.FindOwner(ownerId);
                            Console.WriteLine(owner.GetInfo());
                        }
                        else
                        {
                            Console.WriteLine("Невірний пункт меню.");
                        }

                        break;
                    }
                    
                        case 17:
                        {
                            Console.WriteLine("=== Predicate ===");

                            List<Vehicle> filtered = service.FilterVehicles(
                                v => v.Year >= 2020
                            );

                            foreach (Vehicle vehicle in filtered)
                            {
                                Console.WriteLine(vehicle.GetInfo());
                            }

                            Console.WriteLine("=== Func ===");

                            List<Vehicle> sorted = service.SortVehicles(
                                v => v.Year
                            );

                            foreach (Vehicle vehicle in sorted)
                            {
                                Console.WriteLine(vehicle.GetInfo());
                            }

                            Console.WriteLine("=== Action ===");

                            service.ProcessVehicles(
                                v => Console.WriteLine(v.GetInfo())
                            );

                            break;
                        }
                        
                        case 18:
                        {
                            Console.WriteLine("=== Демонстрація подій ===");

                            Car car = new Car("BMW", "X5", 2022, 5, "Diesel");

                            service.AddVehicle(car);

                            service.UpdateVehicle(car.Id, "BMW", "X6", 2023);

                            service.RemoveVehicle(9999);

                            service.RemoveVehicle(car.Id);

                            Console.WriteLine("Демонстрацію завершено.");
                            Console.WriteLine("Перевірте файл log.txt");

                            break;
                        }
                        case 19:
                        {
                            Console.WriteLine("=== Демонстрація інтерфейсів ===");

                            notification.ShowMessage("Початок демонстрації");
                            eventLogger.WriteLog("Початок демонстрації інтерфейсів");

                            Console.WriteLine("=== Демонстрація IEnumerable та yield return ===");

                            Repository<Vehicle> collection = new Repository<Vehicle>();

                            collection.Add(new Car("BMW", "X5", 2022, 5, "Diesel"));
                            collection.Add(new Car("Audi", "A6", 2021, 4, "Petrol"));
                            collection.Add(new Motorcycle("Honda", "CBR", 2023, 600, "Sport"));

                            foreach (Vehicle vehicle in collection)
                            {
                                Console.WriteLine(vehicle.GetInfo());
                            }

                            notification.ShowMessage("Перебір колекції завершено");
                            eventLogger.WriteLog("Перебір колекції завершено");

                            break;
                        }

                    case 0:
                        work = false;
                        Console.WriteLine("Програму завершено.");
                        break;

                    default:
                        Console.WriteLine("Невірний пункт меню.");
                        break;
                    }
                });
        }
    }
}
              