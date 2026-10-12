using project.Models;


namespace project.Data;

public static class DbInitializer
{
    public static void Initialize(AppDbContext context)
    {
        if (context.Users.Any())
            return;

        var users = new List<User>
        {
            new User
            {
                Login = "manager",
                PasswordHash = PasswordHelper.HashPassword("Manager123"),
                Role = "Менеджер",
                Email = "manager@store.ru",
                DisplayName = "Иванов Иван"
            },
            new User
            {
                Login = "client",
                PasswordHash = PasswordHelper.HashPassword("Client123"),
                Role = "Клиент",
                Email = "client@mail.ru",
                DisplayName = "Петров Пётр"
            }
        };

        context.Users.AddRange(users);

        var products = new List<Product>
        {
            new Product
            {
                Name = "Холодильник Samsung RB37A5200SA",
                Category = "Холодильники",
                Manufacturer = "Samsung",
                Price = 54990m,
                Quantity = 12,
                Description = "Двухкамерный холодильник с нижней морозильной камерой, объём 367 л",
                Specifications = "Класс энергопотребления: A+, Размеры: 595×675×2010 мм, No Frost"
            },
            new Product
            {
                Name = "Холодильник LG GA-B509CQTL",
                Category = "Холодильники",
                Manufacturer = "LG",
                Price = 47990m,
                Quantity = 8,
                Description = "Холодильник с инверторным компрессором и функцией DoorCooling+",
                Specifications = "Объём: 419 л, Класс: A+, Размеры: 595×682×2030 мм"
            },
            new Product
            {
                Name = "Стиральная машина Bosch WGA254A0ME",
                Category = "Стиральные машины",
                Manufacturer = "Bosch",
                Price = 42990m,
                Quantity = 15,
                Description = "Фронтальная стиральная машина, загрузка 10 кг, 1400 об/мин",
                Specifications = "Класс: A, Размеры: 600×590×850 мм, 15 программ стирки"
            },
            new Product
            {
                Name = "Стиральная машина LG F2V5GS0W",
                Category = "Стиральные машины",
                Manufacturer = "LG",
                Price = 36990m,
                Quantity = 0,
                Description = "Узкая стиральная машина с технологией AI DD и паром",
                Specifications = "Загрузка: 8.5 кг, 1200 об/мин, Глубина: 47 см"
            },
            new Product
            {
                Name = "Пылесос Dyson V15 Detect",
                Category = "Пылесосы",
                Manufacturer = "Dyson",
                Price = 59990m,
                Quantity = 6,
                Description = "Беспроводной пылесос с лазерным обнаружением пыли",
                Specifications = "Мощность: 230 Вт, Время работы: до 60 мин, Ёмкость: 0.76 л"
            },
            new Product
            {
                Name = "Робот-пылесос Xiaomi Robot Vacuum X10+",
                Category = "Пылесосы",
                Manufacturer = "Xiaomi",
                Price = 34990m,
                Quantity = 20,
                Description = "Робот-пылесос с функцией влажной уборки и самоочисткой",
                Specifications = "Мощность всасывания: 4000 Па, Ёмкость: 400 мл, LiDAR навигация"
            },
            new Product
            {
                Name = "Микроволновая печь Samsung ME83KRQS-3",
                Category = "Микроволновые печи",
                Manufacturer = "Samsung",
                Price = 8490m,
                Quantity = 25,
                Description = "Микроволновая печь с биокерамическим покрытием, 23 л",
                Specifications = "Мощность: 800 Вт, Объём: 23 л, 6 уровней мощности"
            },
            new Product
            {
                Name = "Электрическая плита Electrolux EKC954907W",
                Category = "Электрические плиты",
                Manufacturer = "Electrolux",
                Price = 32990m,
                Quantity = 7,
                Description = "Электрическая плита со стеклокерамической поверхностью",
                Specifications = "4 конфорки, духовка 58 л, гриль, конвекция"
            },
            new Product
            {
                Name = "Телевизор Samsung QE55Q80C",
                Category = "Телевизоры",
                Manufacturer = "Samsung",
                Price = 89990m,
                Quantity = 10,
                Description = "QLED телевизор 55 дюймов с разрешением 4K UHD",
                Specifications = "Разрешение: 3840×2160, HDR10+, 120 Гц, Smart TV Tizen"
            },
            new Product
            {
                Name = "Телевизор LG OLED55C3RLA",
                Category = "Телевизоры",
                Manufacturer = "LG",
                Price = 109990m,
                Quantity = 4,
                Description = "OLED телевизор 55 дюймов с процессором α9 Gen6",
                Specifications = "4K, 120 Гц, Dolby Vision, Dolby Atmos, webOS 23"
            },
            new Product
            {
                Name = "Кофемашина De'Longhi Magnifica S",
                Category = "Кофемашины",
                Manufacturer = "De'Longhi",
                Price = 34990m,
                Quantity = 9,
                Description = "Автоматическая кофемашина с капучинатором",
                Specifications = "Давление: 15 бар, Ёмкость: 1.8 л, Встроенная кофемолка"
            },
            new Product
            {
                Name = "Кондиционер Haier AS25S2SJ2FA-3",
                Category = "Кондиционеры",
                Manufacturer = "Haier",
                Price = 39990m,
                Quantity = 11,
                Description = "Инверторная сплит-система с Wi-Fi управлением",
                Specifications = "Мощность: 2.6 кВт, Площадь: до 25 м², Класс: A++"
            },
            new Product
            {
                Name = "Утюг Philips GC4938/20",
                Category = "Мелкая бытовая техника",
                Manufacturer = "Philips",
                Price = 6990m,
                Quantity = 30,
                Description = "Паровой утюг с подошвой SteamGlide Elite",
                Specifications = "Мощность: 3000 Вт, Паровой удар: 240 г/мин"
            },
            new Product
            {
                Name = "Блендер Bosch MSM66155",
                Category = "Мелкая бытовая техника",
                Manufacturer = "Bosch",
                Price = 4990m,
                Quantity = 18,
                Description = "Погружной блендер с набором насадок",
                Specifications = "Мощность: 600 Вт, 12 скоростей, нож из нержавеющей стали"
            },
            new Product
            {
                Name = "Микроволновая печь LG MS2595DIS",
                Category = "Микроволновые печи",
                Manufacturer = "LG",
                Price = 12490m,
                Quantity = 0,
                Description = "Микроволновая печь с инверторным нагревом NeoChef",
                Specifications = "Мощность: 1000 Вт, Объём: 25 л, Smart Inverter"
            }
        };

        context.Products.AddRange(products);
        context.SaveChanges();
    }
}