using Api.Models.Enums;
using Api.Models.Enums.DTO;
using Api.Services;

namespace ApiTest
{
    [TestClass]
    public class TestDTOService
    {
        [TestMethod]
        public void GetCategory_ShouldReturnMetal_ForAluminiumFoil()
        {
            var service = new DTOService();

            var result = service.GetCategory(LitterType.AluminiumFoil);

            Assert.AreEqual(LitterCategory.Metal, result);
        }

        [TestMethod]
        public void GetCategory_ShouldReturnNull_WhenInputIsNull()
        {
            var service = new DTOService();

            var result = service.GetCategory((LitterType?)null);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetCategory_ShouldReturnGlass_ForBottle()
        {
            var service = new DTOService();

            var result = service.GetCategory(LitterType.Bottle);

            Assert.AreEqual(LitterCategory.Glass, result);
        }

        [TestMethod]
        public void GetCategory_ShouldReturnPaper_ForCarton()
        {
            var service = new DTOService();

            var result = service.GetCategory(LitterType.Carton);

            Assert.AreEqual(LitterCategory.Paper, result);
        }

        [TestMethod]
        public void GetCategory_ShouldReturnOrganic_ForCigarette()
        {
            var service = new DTOService();

            var result = service.GetCategory(LitterType.Cigarette);

            Assert.AreEqual(LitterCategory.Organic, result);
        }
        [TestMethod]
        public void GetCategory_ShouldReturnPlastic_ForCup()
        {
            var service = new DTOService();

            var result = service.GetCategory(LitterType.Cup);

            Assert.AreEqual(LitterCategory.Plastic, result);
        }
        [TestMethod]
        public void GetCategory_ShouldReturnPlastic_ForPlasticBagDescription()
        {
            var service = new DTOService();

            var result = service.GetCategory("plastic bag");

            Assert.AreEqual(LitterCategory.Plastic, result);
        }

        [TestMethod]
        public void GetWeatherCategory_ShouldReturnSnowy_ForBlizzard()
        {
            var service = new DTOService();

            var result = service.GetWeatherCategory(WeatherCondition.Blizzard);

            Assert.AreEqual(WeatherCategory.Snowy, result);
        }

        [TestMethod]
        public void GetWeatherCategoryIndex_ShouldReturn1_ForSnowy()
        {
            var service = new DTOService();

            var result = service.GetWeatherCategoryIndex(WeatherCategory.Snowy);

            Assert.AreEqual(1, result);
        }


    }
}
