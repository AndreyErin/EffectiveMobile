
namespace EffectiveMobile
{
    public static class Sort
    {
        public static List<Order> GetSortOrderList(List<Order> dataList, string cityDistrict, 
            DateTime firstDeliveryDateTime,  ILogger? logger) 
        {
            DateTime lastDeliveryDateTime = firstDeliveryDateTime.AddMinutes(30);
            List<Order> orders = dataList
                .Where(x=>x.District == cityDistrict)
                .Where(y=>y.GetDateTimeFormat() >= firstDeliveryDateTime && y.GetDateTimeFormat() <= lastDeliveryDateTime)
                .OrderBy(z=>z.GetDateTimeFormat())
                .ToList();

            logger?.Log($"Заказов соответвующих условиям: {orders.Count}");
            Console.WriteLine($"Найдено заказов: {orders.Count}");

            return orders;
        }
    }
}
