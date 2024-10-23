using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;


namespace EffectiveMobile
{
    internal class Program
    {
        private static string _cityDistrict = "";
        private static DateTime _firstDeliveryDateTime;
        private static string _deliveryLog = "";
        private static string _deliveryOrder = "";
        private static Logger? _logger;
        static void Main(string[] args)
        {
            //начальная инициализация параметров
            InitParams(args);
            //получение заказов из файла
            string _pathDataFile = Directory.GetCurrentDirectory() + @"\SourceData.txt";
            var sourceDataList = GetSourceData(_pathDataFile);
            if (sourceDataList.Count > 0) 
            { 
                //проверка заказов на корректность
                var validDataList = GetValidOrderList(sourceDataList);
                //сортировка заказов согласно условиям
                var resultOrderList = Sort.GetSortOrderList(validDataList, _cityDistrict, _firstDeliveryDateTime, _logger);
                //сохранение в файл
                SaveToFile(resultOrderList);               
            }

            Console.WriteLine("Нажмите любую кнопку для выхода.");
            Console.ReadLine();
        }

        private static void InitParams(string[] args) 
        {
            using IHost host = Host.CreateApplicationBuilder(args).Build();
            IConfiguration config = host.Services.GetRequiredService<IConfiguration>();

            //Выставляем значения по умолчанию
            _deliveryLog = Directory.GetCurrentDirectory() + @"\log.txt";
            _deliveryOrder = Directory.GetCurrentDirectory() + @"\DeliveryOrder.txt";
            _cityDistrict = config.GetValue<string>("CityDistrict") ?? "Северный";
            _firstDeliveryDateTime = DateTime.Parse(config.GetValue<string>("FirstDeliveryDateTime") ?? "2024-11-01 08:00:00");

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
                    _logger.Log(DateTime.Now.ToString() + $" Запуск приложения. Полученных параметров: 3 ");

                    try
                    {
                        _firstDeliveryDateTime = DateTime.Parse(args[1]);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Недопустимая дата и время. Дата и время будут взяты из файла конфигурации");
                        _logger.Log("Недопустимая дата и время. Дата и время будут взяты из файла конфигурации");
                    }

                    _logger.Log($"(Район: {_cityDistrict}, Время первой доставки: {_firstDeliveryDateTime}, Лог-файл: {_deliveryLog})");
                    break;
            }

            Console.WriteLine("Запущена сортировка заказов по следующим данным:");
            Console.WriteLine($"Район: {_cityDistrict}");
            Console.WriteLine($"Дата: {_firstDeliveryDateTime.Date.ToShortDateString()}");
            Console.WriteLine($"Время первого заказа: {_firstDeliveryDateTime.TimeOfDay.ToString()}");
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
                Console.WriteLine($"Отчет сформирован. Данные сохранены в файл: {_deliveryOrder}");
            }
            else
            {
                _logger?.Log($"Нет данных для сохранения в файл.");
                Console.WriteLine("Нет заказов удовлетворяющих заданным параметрам.");
            }
            
        }

        private static List<string> GetSourceData(string pathSourceFile) 
        {
            List<string> result = new();

            if(!File.Exists(pathSourceFile))
            {
                _logger?.Log($"Файл с данными о заказах({pathSourceFile}) не найден.");
                Console.WriteLine($"Файл с данными о заказах({pathSourceFile}) не найден.");
                return result;
            }

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

            return result;
        }

        public static List<Order> GetValidOrderList(List<string> sourceDataList)
        {
            List<string> noValidDate = new();
            List<Order> orders = new();
            foreach (var item in sourceDataList)
            {
                Order? order = JsonSerializer.Deserialize<Order>(item);
                if (order != null && order?.GetDateTimeFormat() != null)
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
    }
}
