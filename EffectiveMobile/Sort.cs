
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
                .Where(y=>y.GetDateTime() >= firstDeliveryDateTime && y.GetDateTime() <= lastDeliveryDateTime)
                .OrderBy(z=>z.GetDateTime())
                .ToList();

            logger?.Log($"Заказов соответвующих условиям: {orders.Count}");
            Console.WriteLine($"Найдено заказов: {orders.Count}");

            return orders;
        }
    }
}
