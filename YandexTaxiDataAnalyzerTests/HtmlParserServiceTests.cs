using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YandexTaxiDataAnalyzer.Core.Services;

namespace YandexTaxiDataAnalyzerTests
{
    [TestClass]
    public class HtmlParserServiceTests
    {
        private readonly HtmlParserService _htmlParserService;

        public HtmlParserServiceTests()
        {
            _htmlParserService = new HtmlParserService();
        }

        [DataTestMethod]
        [DataRow("��������� ������� � 134 ���.", 134)]
        [DataRow("�212��138", 212)]
        [DataRow("8 ���", 8)]
        [DataRow("����� ������, 19/6", 19)]
        [DataRow("�� �������� ����� 20 ������� 2015 �. � 01:58", 20)]
        [DataRow("000000001", 1)]
        public void TestGetFirstNumber(string sourceString, int referenceNumber)
        {
            var number = _htmlParserService.GetFirstNumber(sourceString);
            Assert.AreEqual(referenceNumber, number);
        }

        [DataTestMethod]
        [DataRow(" ����-����� Hyundai Solaris ", "����-�����", "Hyundai Solaris")]
        [DataRow(" ����� ��� 31105 ������ ", "�����", "��� 31105 ������")]
        [DataRow("����� LADA (���) Granta", "�����", "LADA (���) Granta")]
        [DataRow(" ����-����� LADA (���) Granta ", "����-�����", "LADA (���) Granta")]
        [DataRow(" ������ Subaru Legacy ", "������", "Subaru Legacy")]
        public void TestGetColorAndModel(string description, string referenceColor, string referenceModel)
        {
            (var color, var model) = _htmlParserService.GetColorAndModel(description);
            Assert.AreEqual(referenceColor, color);
            Assert.AreEqual(referenceModel, model);
        }

        [DataTestMethod]
        [DataRow(TestDataConstants.HtmlMessage, "������", "������� ϸ�� ��������", "�����", "Toyota Corolla", "�228��38", "������", 8, 71, new[]{ "��������� �����, 19�", "����� ������, 11/5" }, 1499618400)]
        public void TestParseHtmlMessage(string htmlMessage, 
            string referenceCompany, 
            string referenceDriver, 
            string referenceCarColor, 
            string referenceCarModel, 
            string referenceCarNumber,
            string referenceTariff,
            int referenceTimeInMinutes,
            int referenceCost,
            string[] referenceWaypoints,
            int referenceOrderDateTimeUnix)
        {
            var htmlMessages = new List<string>(){ htmlMessage };
            var result = _htmlParserService.ParseHtmlMessages(htmlMessages);
            var firstResult = result.FirstOrDefault();

            Assert.IsNotNull(firstResult);
            Assert.IsNotNull(firstResult.ContractorInformation);
            Assert.IsNotNull(firstResult.OrderInformation);
            Assert.IsNotNull(firstResult.RouteInformation);

            Assert.AreEqual(referenceCompany, firstResult.ContractorInformation.Company);
            Assert.AreEqual(referenceDriver, firstResult.ContractorInformation.Driver);
            Assert.AreEqual(referenceCarColor, firstResult.ContractorInformation.CarColor);
            Assert.AreEqual(referenceCarModel, firstResult.ContractorInformation.CarModel);
            Assert.AreEqual(referenceCarNumber, firstResult.ContractorInformation.CarNumber);

            Assert.AreEqual(referenceTariff, firstResult.OrderInformation.Tariff);
            Assert.AreEqual(TimeSpan.FromMinutes(referenceTimeInMinutes), firstResult.OrderInformation.Duration);
            Assert.AreEqual(referenceCost, firstResult.OrderInformation.Cost);
            
            CollectionAssert.AreEqual(referenceWaypoints, firstResult.RouteInformation.Waypoints.ToArray());
            Assert.AreEqual(DateTimeOffset.FromUnixTimeSeconds(referenceOrderDateTimeUnix).DateTime, firstResult.RouteInformation.OrderDateTime);
        }
    }
}
