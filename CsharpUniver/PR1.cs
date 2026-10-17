using System;
using System.Collections.Generic;
using System.IO;

class Entity
{
    private int id;

    public int Id
    {
        get { return id; }
        set { id = value; }
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
        }
        catch (IOException)
        {
            Console.WriteLine("Помилка роботи з файлом.");
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
            Console.WriteLine("Помилка читання файлу. ");
        }
    }
}

static class Validator
{
    public static bool IsValidString(string value)
    {
        if (value ==  null)
        {
            return false;
        }

        if (value == "")
        {
            return false;
        }
        return true;
    }

    public static bool IsValidYear(int year)
    {
        if (1990 <= year && year <= 2026)
        {
            return true;
        }
        return false;
    }

    public static bool IsValidDoors(int doors)
    {
        if (2 <= doors && doors <= 5)
        {
            return true;
        }
        return false;
    }

    public static bool IsValidEngineVolume(int volume)
    {
        if (volume > 0)
        {
            return true;
        }
        return  false;
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
        set { brand = value; }
    }

    public string Model
    {
        get { return model; }
        set { model = value; }
    }

    public int Year
    {
        get { return year; }
        set { year = value; }
    }

    public Owner Owner
    {
        get { return owner; }
        set { owner = value; }
    }

    public Vehicle(string brand, string model, int year)
    {
        Id = IdGenerator.NextId();
        this.brand = brand;
        this.model = model;
        this.year = year;
        owner = null;
    }

    public virtual string GetInfo()
    {
        if (owner == null)
        {
            return $"Id: {Id}, Vehicle: {Brand} {Model}, Year: {Year}, Owner: none";
        }

        return $"Id: {Id}, Vehicle: {Brand} {Model}, Year: {Year}, Owner: {Owner.Fullname}";
    }
}

class Car : Vehicle
{
    private int doorscount;
    private string fueltype;

    public int Doorscount
    {
        get { return doorscount; }
        set { doorscount = value; }
    }

    public string Fueltype
    {
        get { return fueltype; }
        set { fueltype = value; }
    }

    public Car(string brand, string model, int year, int doorscount, string fueltype)
        : base(brand, model, year)
    {
        this.doorscount = doorscount;
        this.fueltype = fueltype;
    }

    public override string GetInfo()
    {
        if (Owner == null)
        {
            return $"Id: {Id}, Car: {Brand} {Model}, Year: {Year}, Doors: {Doorscount}, Fueltype: {Fueltype}, Owner: none";
        }

        return $"Id: {Id}, Car: {Brand} {Model}, Year: {Year}, Doors: {Doorscount}, Fueltype: {Fueltype}, Owner: {Owner.Fullname}";
    }
}

class Motorcycle : Vehicle
{
    private int enginevolume;
    private string motorcycletype;

    public int Enginevolume
    {
        get { return enginevolume; }
        set { enginevolume = value; }
    }

    public string Motorcycletype
    {
        get { return motorcycletype; }
        set { motorcycletype = value; }
    }

    public Motorcycle(string brand, string model, int year, int enginevolume, string motorcycletype)
        : base(brand, model, year)
    {
        this.enginevolume = enginevolume;
        this.motorcycletype = motorcycletype;
    }

    public override string GetInfo()
    {
        if (Owner == null)
        {
            return $"Id: {Id}, Motorcycle: {Brand} {Model}, Year: {Year}, Enginevolume: {Enginevolume}, Motorcycletype: {Motorcycletype}, Owner: none";
        }

        return $"Id: {Id}, Motorcycle: {Brand} {Model}, Year: {Year}, Enginevolume: {Enginevolume}, Motorcycletype: {Motorcycletype}, Owner: {Owner.Fullname}";
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
        Id = IdGenerator.NextId();
        this.fullname = fullname;
        this.phone = phone;
    }

    public string GetInfo()
    {
        return $"Id: {Id}, Name: {Fullname}, Phone: {Phone}";
    }
}

class Repository<T> where T : Entity
{
    private List<T> entities = new List<T>();

    public bool Add(T entity)
    {
        foreach (T item in entities)
        {
            if (item.Id == entity.Id)
            {
                return false;
            }
        }
        entities.Add(entity);
        return true;
    }

    public T Find(int id)
    {
        foreach (T item in entities)
        {
            if (item.Id == id)
            {
                return item;
            }
        }
        return null;
    }

    public bool Remove(int id)
    {
        T item = Find(id);
            if (item != null)
            {
                entities.Remove(item);
                return true;
            }
        return false;
    }
    public List<T> GetAll()
    { 
        return entities;
    }

}

class VehicleService
{
    private Repository<Vehicle> vehicles = new Repository<Vehicle>();
    private Repository<Owner> owners = new Repository<Owner>();

    public bool AddVehicle(Vehicle vehicle)
    {
        if (vehicle == null)
        {
            return false;
        }
        return vehicles.Add(vehicle);
    }

    public Vehicle FindVehicle(int id)
    {
        return vehicles.Find(id);
    }

    public List<Vehicle> FindVehicleByBrand(string brand)
    {
        List<Vehicle> result = new List<Vehicle>();

        foreach (Vehicle vehicle in vehicles.GetAll())
        {
            if (vehicle.Brand == brand)
            {
                result.Add(vehicle);
            }
        }

        return result;
    }

    public bool UpdateVehicle(int id, string brand, string model, int year)
    {
        Vehicle vehicle = vehicles.Find(id);

        if (vehicle == null)
        {
            return false;
        }

        vehicle.Brand = brand;
        vehicle.Model = model;
        vehicle.Year = year;

        return true;
    }

    public bool RemoveVehicle(int id)
    {
        Vehicle vehicle = vehicles.Find(id);

        if (vehicle == null)
        {
            return false;
        }

        if (vehicle.Owner != null)
        {
            return false;
        }

        vehicles.Remove(id);
        return  true;
    }

    public bool AddOwner(Owner owner)
    {
            if (owner == null)
            {
                return false;
            }
            return owners.Add(owner);
    }

    public Owner FindOwner(int id)
    {
        return owners.Find(id);
    }

    public bool UpdateOwner(int id, string fullname, string phone)
    {
        Owner owner = owners.Find(id);

        if (owner == null)
        {
            return false;
        }

        owner.Fullname = fullname;
        owner.Phone = phone;

        return true;
    }

    public bool RemoveOwner(int id)
    {
        Owner owner = owners.Find(id);

        if (owner == null)
        {
            return false;
        }

        foreach (Vehicle vehicle in vehicles.GetAll())
        {
            if (vehicle.Owner == owner)
            {
                return false;
            }
        }
        owners.Remove(id);
        return true;
    }

    public bool AssignOwner(int vehicleId, int ownerId)
    {
        Vehicle vehicle = vehicles.Find(vehicleId);
        Owner owner = owners.Find(ownerId);

        if (vehicle == null)
        {
            return false;
        }

        if (owner == null)
        {
            return false;
        }

        if (vehicle.Owner != null)
        {
            return false;
        }

        vehicle.Owner = owner;

        return true;
    }

    public bool RemoveOwnerFromVehicle(int vehicleId)
    {
        Vehicle vehicle = vehicles.Find(vehicleId);

        if (vehicle == null)
        {
            return false;
        }

        if (vehicle.Owner == null)
        {
            return false;
        }

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
            Console.WriteLine("0. Вихід");
            Console.Write("Виберіть дію: ");

            int choice = int.Parse(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    Console.WriteLine();
                    Console.WriteLine("1. Автомобіль");
                    Console.WriteLine("2. Мотоцикл");
                    Console.Write("Виберіть тип: ");

                    int type = int.Parse(Console.ReadLine());

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
                    int year = int.Parse(Console.ReadLine());
                    if (!Validator.IsValidYear(year))
                    {
                        Console.WriteLine("Рік повинен бути від 1990 до 2026.");
                        break;
                    }

                    if (type == 1)
                    {
                        Console.Write("Введіть кількість дверей: ");
                        int doorscount = int.Parse(Console.ReadLine());
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
                        {
                            Console.WriteLine("Автомобіль додано.");
                        }
                    }
                    else if (type == 2)
                    {
                        Console.Write("Введіть об'єм двигуна: ");
                        int enginevolume = int.Parse(Console.ReadLine());
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

                        Motorcycle motorcycle = new Motorcycle(brand, model, year, enginevolume, motorcycletype);

                        if (service.AddVehicle(motorcycle))
                        {
                            Console.WriteLine("Мотоцикл додано.");
                        }
                        else
                        {
                            Console.WriteLine("Транспорт з таким ID вже існує.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Невірний тип транспорту.");
                    }

                    break;

                case 2:
                    List<Vehicle> vehicles = service.GetVehicles();

                    if (vehicles.Count == 0)
                    {
                        Console.WriteLine("Транспорт відсутній.");
                    }
                    else
                    {
                        foreach (Vehicle vehicle in vehicles)
                        {
                            Console.WriteLine(vehicle.GetInfo());
                        }
                    }

                    break;

                case 3:
                    Console.Write("Введіть ID транспорту: ");
                    int searchId = int.Parse(Console.ReadLine());

                    Vehicle vehicleById = service.FindVehicle(searchId);

                    if (vehicleById == null)
                    {
                        Console.WriteLine("Транспорт не знайдено.");
                    }
                    else
                    {
                        Console.WriteLine(vehicleById.GetInfo());
                    }

                    break;

                case 4:
                    Console.Write("Введіть марку: ");
                    string searchBrand = Console.ReadLine();

                    List<Vehicle> result = service.FindVehicleByBrand(searchBrand);

                    if (result.Count == 0)
                    {
                        Console.WriteLine("Транспорт не знайдено.");
                    }
                    else
                    {
                        foreach (Vehicle vehicle in result)
                        {
                            Console.WriteLine(vehicle.GetInfo());
                        }
                    }

                    break;

                case 5:
                    Console.Write("Введіть ID транспорту: ");
                    int updateId = int.Parse(Console.ReadLine());

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
                    int newYear = int.Parse(Console.ReadLine());
                    if (!Validator.IsValidYear(newYear))
                    {
                        Console.WriteLine("Рік повинен бути від 1990 до 2026.");
                        break;
                    }

                    if (service.UpdateVehicle(updateId, newBrand, newModel, newYear))
                    {
                        Console.WriteLine("Транспорт оновлено.");
                    }
                    else
                    {
                        Console.WriteLine("Транспорт не знайдено.");
                    }

                    break;

                case 6:
                    Console.Write("Введіть ID транспорту: ");
                    int deleteId = int.Parse(Console.ReadLine());

                    if (service.RemoveVehicle(deleteId))
                    {
                        Console.WriteLine("Транспорт видалено.");
                    }
                    else
                    {
                        Console.WriteLine("Не можна видалити транспорт.");
                    }

                    break;

                case 7:

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

                    Owner owner = new Owner( fullname, phone);

                    if (service.AddOwner(owner))
                    {
                        Console.WriteLine("Власника додано.");
                    }
                    else
                    {
                        Console.WriteLine("Власник з таким ID вже існує.");
                    }

                    break;

                case 8:
                    List<Owner> owners = service.GetOwners();

                    if (owners.Count == 0)
                    {
                        Console.WriteLine("Власників немає.");
                    }
                    else
                    {
                        foreach (Owner currentOwner in owners)
                        {
                            Console.WriteLine(currentOwner.GetInfo());
                        }
                    }

                    break;

                case 9:
                    Console.Write("Введіть ID власника: ");
                    int findOwnerId = int.Parse(Console.ReadLine());

                    Owner foundOwner = service.FindOwner(findOwnerId);

                    if (foundOwner == null)
                    {
                        Console.WriteLine("Власника не знайдено.");
                    }
                    else
                    {
                        Console.WriteLine(foundOwner.GetInfo());
                    }

                    break;

                case 10:
                    Console.Write("Введіть ID власника: ");
                    int updateOwnerId = int.Parse(Console.ReadLine());

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
                    {
                        Console.WriteLine("Власника оновлено.");
                    }
                    else
                    {
                        Console.WriteLine("Власника не знайдено.");
                    }

                    break;

                case 11:
                    Console.Write("Введіть ID власника: ");
                    int deleteOwnerId = int.Parse(Console.ReadLine());

                    if (service.RemoveOwner(deleteOwnerId))
                    {
                        Console.WriteLine("Власника видалено.");
                    }
                    else
                    {
                        Console.WriteLine("Не можна видалити власника.");
                    }
                    break;

                case 12:
                    Console.Write("Введіть ID транспорту: ");
                    int vehicleId = int.Parse(Console.ReadLine());

                    Console.Write("Введіть ID власника: ");
                    int ownerIdForVehicle = int.Parse(Console.ReadLine());

                    if (service.AssignOwner(vehicleId, ownerIdForVehicle))
                    {
                        Console.WriteLine("Власника призначено.");
                    }
                    else
                    {
                        Console.WriteLine("Не вдалося призначити власника.");
                    }
                    break;

                case 13:
                    Console.Write("Введіть ID транспорту: ");
                    int vehicleIdForRemove = int.Parse(Console.ReadLine());

                    if (service.RemoveOwnerFromVehicle(vehicleIdForRemove))
                    {
                        Console.WriteLine("Власника відкріплено.");
                    }
                    else
                    {
                        Console.WriteLine("Не вдалося відкріпити власника.");
                    }
                    break;
                
                case 14:
                    report.ExportToCsv(service.GetVehicles());
                    Console.WriteLine("Звіт створено. ");
                    break;
                case 15:
                    report.ReadFromCsv();
                    break;
                case 0:
                    work = false;
                    Console.WriteLine("Програму завершено.");
                    break;

                default:
                    Console.WriteLine("Невірний пункт меню.");
                    break;
            }
        }
    }
}