﻿using EffectiveMobile;

namespace EffectiveMobileTest
{
    public class ProgramTest
    {
        [Fact]
        public void GetValidOrderList()
        {
            //Arrange
            List<string> stringList = new() 
            {
                "{ \"Id\": 1037, \"Mass\": 1,    \"District\": \"Северный\", \"DateTimeString\": \"2024-11-01 08:00:00yt\"}",
                "{ \"Id\": 1038, \"Mass\": 0.1,  \"District\": \"Южный\",    \"DateTimeString\": \"2024-11-01 08:10:00\"}",
                "{ \"Id\": 1039, \"Mass\": 0.25, \"District\": \"Северный\", \"DateTimeString\": \"2024-11-01 08:10:00\"}",
                "{ \"Id\": 1040, \"Mass\": 0.3,  \"District\": \"Западный\", \"DateTimeString\": \"2024-11-01 08:30:00\"}",
                "{ \"Id\": 1041, \"Mass\": 0.5,  \"District\": \"Восточный\",\"DateTimeString\": \"2024-11-01fdgd08:25:00\"}",
                "{ \"Id\": 1042, \"Mass\": 0.32, \"District\": \"Северный\", \"DateTimeString\": \"2024-11-01 08:20:00\"}",
                "{ \"Id\": 1043, \"Mass\": 0.7,  \"District\": \"Северный\", \"DateTimeString\": \"2024-11-01 08:30:00\"}",
                "{ \"Id\": 1044, \"Mass\": 0.15, \"District\": \"Северный\", \"DateTimeString\": \"2024-11-01 08:25:00\"}"
            };

            //Act
            List<Order> orders = Program.GetValidOrderList(stringList);

            //Assert
            Assert.Equal(6, orders.Count);
        }

        [Fact]
        public void GetSourceDataTest()
        {
            //Arrange
            string pathDataFile = "SourceData.txt";

            //Act
            List<string> stringList = Program.GetSourceData(pathDataFile);

            //Assert
            Assert.Equal(8, stringList.Count);
        }

    }
}
