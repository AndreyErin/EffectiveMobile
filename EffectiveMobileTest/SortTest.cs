using EffectiveMobile;

namespace EffectiveMobileTest
{   
    public class SortTest
    {
        [Fact]
        public void GetSortOrderListReturn()
        {
            //Arrange
            List<Order> orders = GetOrders();
            string cityDistrict = "Северный";
            DateTime firstDeliveryDateTime = new(2024, 11, 1, 8, 0, 0);

            //Act
            var result = Sort.GetSortOrderList(orders, cityDistrict, firstDeliveryDateTime, null);

            //Assert
            Assert.Equal(4, result.Count);
            Assert.NotNull(result.FirstOrDefault(x=>x.Id == 1044));
            Assert.Null(result.FirstOrDefault(x => x.Id == 1038));
        }

        private List<Order> GetOrders()
        {
            return new() 
            {
                new(){Id = 1037, Mass = 1f,    District = "Северный",  DateTimeString = "2024-11-01 08:00:00yt"},
                new(){Id = 1038, Mass = 0.1f,  District = "Южный",    DateTimeString = "2024-11-01 08:10:00"},
                new(){Id = 1039, Mass = 0.25f, District = "Северный",  DateTimeString = "2024-11-01 08:10:00"},
                new(){Id = 1040, Mass = 0.3f,  District = "Западный",  DateTimeString = "2024-11-01 08:30:00"},
                new(){Id = 1041, Mass = 0.5f,  District = "Восточный", DateTimeString = "2024-11-01fdgd08:25:00"},
                new(){Id = 1042, Mass = 0.32f, District = "Северный",  DateTimeString = "2024-11-01 08:20:00"},
                new(){Id = 1043, Mass = 0.7f,  District = "Северный",  DateTimeString = "2024-11-01 08:30:00"},
                new(){Id = 1044, Mass = 0.15f, District = "Северный",  DateTimeString = "2024-11-01 08:25:00"}
            };
        }
    }
}
