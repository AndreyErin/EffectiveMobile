
namespace EffectiveMobile
{
    public class Order
    {
        public int Id { get; set; }
        public float Mass { get; set; }
        public string District { get; set; }
        public string DateTimeString { get; set; }

        public DateTime? GetDateTime()
        {
            bool isParse = DateTime.TryParse(DateTimeString, out DateTime result);

            if (isParse)
            {
                return result;
            }

            return null;
        }

        public override string ToString()
        {
            return $"Заказ №{Id}, Вес: {Mass} кг, Район: {District}, Дата и время: {DateTimeString}";
        }
    }
}
