using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MockSite.Core.DTOs;
using MockSite.Core.Entities;
using MockSite.Core.Interfaces;
using MockSite.DomainService;
using MockSite.Message;
using NSubstitute;
using NUnit.Framework;

namespace MockSite.Test
{
    public class CurrencyTest
    {
        private CurrencyServiceImpl _currencyServiceImpl;
        private ICurrencyService _fakeCurrencyService;

        [OneTimeSetUp]
        public void SetUp()
        {
            _fakeCurrencyService = Substitute.For<ICurrencyService>();
            _currencyServiceImpl = new CurrencyServiceImpl(_fakeCurrencyService);
        }

        [Test]
        [TestCase("TWN")]
        [TestCase("CNY")]
        public async Task Test_Get_Currency(string code)
        {
            // Arrangement
            var expect = GetExpectCurrency(code);

            _fakeCurrencyService.GetByCode(code).Returns(GetExpectCurrency(code));

            // Action
            var actual = await _currencyServiceImpl.Get(new QueryCurrencyMessage {CurrencyCode = code}, null);

            // Assert
            Assert.AreEqual(expect.CurrencyCode, actual.CurrencyCode);
            Assert.AreEqual(expect.CurrencyRate, actual.CurrencyRate);
        }

        [Test]
        public async Task Test_Update_Currency()
        {
            // Arrangement
            var currencyDto = new CurrencyDto("CNY", "1.0");

            // Action
            await _currencyServiceImpl.Modify(
                new Currency {CurrencyCode = currencyDto.CurrencyCode, CurrencyRate = currencyDto.CurrencyRate}, null);

            // Assert
            await _fakeCurrencyService.Received()
                .Modify(Arg.Is<CurrencyDto>(dto => dto.CurrencyRate == currencyDto.CurrencyRate));
        }

        [Test]
        public async Task Test_Create_Currency()
        {
            // Arrangement
            var currencyDto = new CurrencyDto("CNY", "1.0");

            // Action
            await _currencyServiceImpl.Modify(
                new Currency {CurrencyCode = currencyDto.CurrencyCode, CurrencyRate = currencyDto.CurrencyRate}, null);

            // Assert
            await _fakeCurrencyService.Received()
                .Modify(Arg.Is<CurrencyDto>(dto => dto.CurrencyRate == currencyDto.CurrencyRate));
        }

        [Test]
        [TestCase("TWN")]
        [TestCase("CNY")]
        public async Task Test_Delete_Currency(string code)
        {
            // Arrangement
            var target = code;

            // Action
            await _currencyServiceImpl.Delete(new QueryCurrencyMessage {CurrencyCode = target}, null);

            // Assert
            await _fakeCurrencyService.Received().Delete(code);
        }

        private static CurrencyEntity GetExpectCurrency(string currencyCode)
        {
            var currencyEntities = new List<CurrencyEntity>
            {
                new CurrencyEntity {CurrencyCode = "TWN", CurrencyRate = "1"},
                new CurrencyEntity {CurrencyCode = "CNY", CurrencyRate = "4"}
            };

            return currencyEntities.FirstOrDefault(c => c.CurrencyCode == currencyCode);
        }
    }
}