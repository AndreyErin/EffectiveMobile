using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;


namespace EffectiveMobile
{
    public class Program
    {
        private static string _cityDistrict = "";
        private static DateTime _firstDeliveryDateTime;
        private static string _deliveryLog = "";
        private static string _deliveryOrder = "";
        private static Logger? _logger;
        static void Main(string[] args)
        {
            //инициализация параметров
            bool haveParam = InitParams(args);

            //получение заказов из файла
            string _pathDataFile = Directory.GetCurrentDirectory() + @"\SourceData.txt";
            var sourceDataList = GetSourceData(_pathDataFile);

            if (sourceDataList.Count > 0)
            {                
                //проверка заказов на корректность
                var validDataList = GetValidOrderList(sourceDataList);

                //показываем список всех доступных заказов пользователю
                ShowAllOrders(validDataList);

                //если район, дату и время не удалось получить
                //через командную строку или файл конфигурации
                if (!haveParam)
                {
                    //Запрашиваем данные у пользователя
                    GetParamFromUser();
                }

                //сортировка заказов согласно условиям
                Console.WriteLine("Список заказов будет свормирован согласно следующим условиям:");
                Console.WriteLine($"Район: {_cityDistrict}");
                Console.WriteLine($"Дата: {_firstDeliveryDateTime.Date.ToShortDateString()}");
                Console.WriteLine($"Время c {_firstDeliveryDateTime.TimeOfDay.ToString()} по {_firstDeliveryDateTime.AddMinutes(30).TimeOfDay.ToString()}");
                var resultOrderList = Sort.GetSortOrderList(validDataList, _cityDistrict, _firstDeliveryDateTime, _logger);
                //сохранение в файл
                SaveToFile(resultOrderList);               
            }

            Console.WriteLine("Нажмите любую кнопку для выхода.");
            Console.ReadLine();
        }

        private static bool InitParams(string[] args) 
        {
            //Выставляем значения по умолчанию
            _deliveryLog = Directory.GetCurrentDirectory() + @"\log.txt";
            _deliveryOrder = Directory.GetCurrentDirectory() + @"\DeliveryOrder.txt";

            bool isHaveDistrictAndDate = false;
            switch (args.Length)
            {
                case 0:
                    _logger = new(_deliveryLog);
                    _logger.Log(DateTime.Now.ToString() + $" Запуск приложения. Полученных параметров: 0");
                    break;
                case 1:
                    _deliveryLog = args[0];
                    _logger = new(_deliveryLog);
                    _logger.Log(DateTime.Now.ToString() + $" Запуск приложения. Полученных параметров: 1 (Лог-файл: {_deliveryLog})");
                    break;
                case 3:
                    _cityDistrict = args[0];
                    _deliveryLog = args[2];

                    _logger = new(_deliveryLog);
                    _logger?.Log(DateTime.Now.ToString() + $" Запуск приложения. Полученных параметров: 3 ");
                    _logger?.Log("Попытка получить Район, дату и время через параметры при запуске");
                    try
                    {
                        _firstDeliveryDateTime = DateTime.Parse(args[1]);
                        isHaveDistrictAndDate = true;
                        _logger?.Log($"(Район: {_cityDistrict}, Время первой доставки: {_firstDeliveryDateTime}, Лог-файл: {_deliveryLog})");

                    }
                    catch (Exception)
                    {
                        isHaveDistrictAndDate = false;
                        _logger?.Log("Недопустимая дата и время. Данные будут взяты из файла конфигурации");
                    }
                    break;
            }

            //пробуем получить данные из файла кофигурации
            if (!isHaveDistrictAndDate)
            {
                using IHost host = Host.CreateApplicationBuilder(args).Build();
                IConfiguration config = host.Services.GetRequiredService<IConfiguration>();

                _logger?.Log("Попытка получить Район, дату и время из файла конфигурации");


                isHaveDistrictAndDate = true;
                try
                {
                    _cityDistrict = config.GetValue<string>("CityDistrict");
                    _firstDeliveryDateTime = DateTime.Parse(config.GetValue<string>("FirstDeliveryDateTime"));
                    _logger?.Log($"(Район: {_cityDistrict}, Время первой доставки: {_firstDeliveryDateTime}, Лог-файл: {_deliveryLog})");
                }
                catch (Exception ex)
                {
                    isHaveDistrictAndDate = false;
                    _logger?.Log($"Не удалось получить данные из файла конфигурации. Данные будут запрошены и упользователя. Ошибка: {ex.Message}");
                }
            }


            return isHaveDistrictAndDate;             
        }

        private static void SaveToFile(List<Order> orders) 
        {
            if (orders.Count > 0) 
            {
                using StreamWriter streamWriter = new StreamWriter(_deliveryOrder, false);
                foreach (Order item in orders)
                {
                    streamWriter.WriteLine(item.ToString());
                }
                _logger?.Log($"Итоговый список заказов сохранен в файл: {_deliveryOrder}");
                Console.WriteLine($"Список заказов сформирован. Данные сохранены в файл: {_deliveryOrder}");
            }
            else
            {
                _logger?.Log($"Нет данных для сохранения в файл.");
                Console.WriteLine("Нет заказов удовлетворяющих заданным параметрам.");
            }
            
        }

        public static List<string> GetSourceData(string pathSourceFile) 
        {
            List<string> result = new();

            try
            {
                using StreamReader streamReader = new StreamReader(pathSourceFile);
                string? line;

                while ((line = streamReader.ReadLine()) != null)
                {
                    result.Add(line);
                }
                _logger?.Log($"Чтение из файла ({pathSourceFile}). Получено строк: {result.Count}");

                if (result.Count == 0)
                {
                    _logger?.Log($"Файл данныx({pathSourceFile}) не содержит информацию о заказах.");
                    Console.WriteLine($"Файл данныx({pathSourceFile}) не содержит информацию о заказах.");
                }
            }
            catch (FileNotFoundException ex)
            {
                _logger?.Log($"Файл с данными о заказах({pathSourceFile}) не найден. Ошибка: {ex.Message}.");
                Console.WriteLine($"Ошибка. Файл с данными о заказах({pathSourceFile}) не найден.");

            }
            catch (Exception ex)
            {

                _logger?.Log($"При поппытке получить данные из файла({pathSourceFile}) произошла ошибка: {ex.Message}.");
                Console.WriteLine($"Ошибка. Не удалось получить данные о заказах из файла({pathSourceFile})");
            }

            return result;
        }

        public static List<Order> GetValidOrderList(List<string> sourceDataList)
        {
            List<string> noValidDate = new();
            List<Order> orders = new();
            foreach (var item in sourceDataList)
            {
                Order? order = JsonSerializer.Deserialize<Order>(item);
                if (order != null && order?.GetDateTime() != null)
                {
                    orders.Add(order);
                }
                else
                {
                    noValidDate.Add(item);
                }
            }

            _logger?.Log($"Количество заказов прошедших валидацию: {orders.Count}");

            if (noValidDate.Count > 0) 
            {
                _logger?.Log($"Количество заказов не прошедших валидацию: {noValidDate.Count}");
                foreach (var item in noValidDate)
                {
                    _logger?.Log("Заказ " + item.ToString() + " содержит ошибки.");
                }
            }

            return orders;
        }

        private static void ShowAllOrders(List<Order> orders)
        {
            Console.WriteLine("Распределение заказов по районам:");
            var sortDistrict = orders
                .GroupBy(x => x.District)
                .OrderByDescending(x=>x.Count());

            foreach (var group in sortDistrict) 
            {
                Console.WriteLine("Район " + group.Key + ". Заказов: " +  group.Count().ToString());

                foreach (var order in group.OrderBy(x=>x.GetDateTime()))
                {
                    Console.WriteLine($"   Заказ №{order.Id}, Вес: {order.Mass} кг, Дата: {order.GetDateTime()?.ToShortDateString()}, время: {order.GetDateTime()?.ToShortTimeString()}");
                }
            }

            Console.WriteLine("");
            _logger?.Log("Данные о заказах прошедших валидацию отсортированы и выведены в консоль.");
        }

        private static void GetParamFromUser()
        {
            //получаем данные от пользователя
            Console.WriteLine("Введие название района");
            string? district = Console.ReadLine();
            while (district == null)
            {
                Console.WriteLine("Введие название района");
                district = Console.ReadLine();
            }
            _cityDistrict = district;

            Console.WriteLine("Введите дату и время в формате: гггг-мм-дд чч:мм:сс");
            string? dateTime = Console.ReadLine();

            while (dateTime == null || !DateTime.TryParse(dateTime, out DateTime result))
            {
                Console.WriteLine("Введите дату и время в формате: гггг-мм-дд чч:мм:сс");
                dateTime = Console.ReadLine();
            }

            _firstDeliveryDateTime = DateTime.Parse(dateTime);

            _logger?.Log("Данные полученные от пользователя:");
            _logger?.Log($"(Район: {_cityDistrict}, Время первой доставки: {_firstDeliveryDateTime}, Лог-файл: {_deliveryLog})");

        }
    }
}
