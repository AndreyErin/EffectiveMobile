
using EffectiveMobile;

namespace EffectiveMobileTest
{
    public class OrderTest
    {
        [Fact]
        public void GetDateTimeResult()
        {
            //Arrang
            Order order1 = new()
            {
                Id = 1, Mass = 1, District = "Северный", DateTimeString = "2024-11-01 08:10:00"
            };
            Order order2 = new()
            {
                Id = 2, Mass = 2, District = "Северный", DateTimeString = "2024-11-01ffd08:10:00fdsf"
            };

            //Act
            DateTime? result1 = order1.GetDateTime();
            DateTime? result2 = order2.GetDateTime();

            //Assert
            Assert.NotNull(result1);
            Assert.Null(result2);

            Assert.Equal(8, result1?.Hour);
        }
    }
}
