using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Voidwell.DaybreakGames.Census.Test
{
    [TestClass]
    public class CensusTest
    {
        [TestMethod]
        public void Census_CreatesBaseUri()
        {
            var service = "character";
            var ns = "ps2";
            var key = "testkey";

            var expectedUri = new Uri($"http://{Constants.CensusEndpoint}/s:{key}/get/{ns}/{service}/");

            var query = GetCensusClient().CreateQuery(service);

            var censusUri = query.GetUri();

            Assert.AreEqual(expectedUri, censusUri);
        }

        [TestMethod]
        public void Census_Conditional_Equals()
        {
            var service = "character";
            var ns = "ps2";
            var key = "testkey";

            var expectedUri = new Uri($"http://{Constants.CensusEndpoint}/s:{key}/get/{ns}/{service}/?field=12345");

            var query = GetCensusClient().CreateQuery(service);

            query.Where("field").Equals("12345");

            var censusUri = query.GetUri();

            Assert.AreEqual(expectedUri, censusUri);
        }

        [TestMethod]
        public void Census_Conditional_LessThan()
        {
            var service = "character";
            var ns = "ps2";
            var key = "testkey";

            var expectedUri = new Uri($"http://{Constants.CensusEndpoint}/s:{key}/get/{ns}/{service}/?field=<12345");

            var query = GetCensusClient().CreateQuery(service);

            query.Where("field").IsLessThan("12345");

            var censusUri = query.GetUri();

            Assert.AreEqual(expectedUri, censusUri);
        }

        [TestMethod]
        public void Census_Conditional_LessThanOrEquals()
        {
            var service = "character";
            var ns = "ps2";
            var key = "testkey";

            var expectedUri = new Uri($"http://{Constants.CensusEndpoint}/s:{key}/get/{ns}/{service}/?field=[12345");

            var query = GetCensusClient().CreateQuery(service);

            query.Where("field").IsLessThanOrEquals("12345");

            var censusUri = query.GetUri();

            Assert.AreEqual(expectedUri, censusUri);
        }

        [TestMethod]
        public void Census_Conditional_GreaterThan()
        {
            var service = "character";
            var ns = "ps2";
            var key = "testkey";

            var expectedUri = new Uri($"http://{Constants.CensusEndpoint}/s:{key}/get/{ns}/{service}/?field=>12345");

            var query = GetCensusClient().CreateQuery(service);

            query.Where("field").IsGreaterThan("12345");

            var censusUri = query.GetUri();

            Assert.AreEqual(expectedUri, censusUri);
        }

        [TestMethod]
        public void Census_Conditional_GreaterThanOrEquals()
        {
            var service = "character";
            var ns = "ps2";
            var key = "testkey";

            var expectedUri = new Uri($"http://{Constants.CensusEndpoint}/s:{key}/get/{ns}/{service}/?field=]12345");

            var query = GetCensusClient().CreateQuery(service);

            query.Where("field").IsGreaterThanOrEquals("12345");

            var censusUri = query.GetUri();

            Assert.AreEqual(expectedUri, censusUri);
        }

        [TestMethod]
        public void Census_Conditional_StartsWith()
        {
            var service = "character";
            var ns = "ps2";
            var key = "testkey";

            var expectedUri = new Uri($"http://{Constants.CensusEndpoint}/s:{key}/get/{ns}/{service}/?field=^12345");

            var query = GetCensusClient().CreateQuery(service);

            query.Where("field").StartsWith("12345");

            var censusUri = query.GetUri();

            Assert.AreEqual(expectedUri, censusUri);
        }

        [TestMethod]
        public void Census_Conditional_Contains()
        {
            var service = "character";
            var ns = "ps2";
            var key = "testkey";

            var expectedUri = new Uri($"http://{Constants.CensusEndpoint}/s:{key}/get/{ns}/{service}/?field=*12345");

            var query = GetCensusClient().CreateQuery(service);

            query.Where("field").Contains("12345");

            var censusUri = query.GetUri();

            Assert.AreEqual(expectedUri, censusUri);
        }

        [TestMethod]
        public void Census_Conditional_NotEquals()
        {
            var service = "character";
            var ns = "ps2";
            var key = "testkey";

            var expectedUri = new Uri($"http://{Constants.CensusEndpoint}/s:{key}/get/{ns}/{service}/?field=!12345");

            var query = GetCensusClient().CreateQuery(service);

            query.Where("field").NotEquals("12345");

            var censusUri = query.GetUri();

            Assert.AreEqual(expectedUri, censusUri);
        }

        [TestMethod]
        public void Census_Conditional_MultipleConditions()
        {
            var service = "character";
            var ns = "ps2";
            var key = "testkey";

            var expectedUri = new Uri($"http://{Constants.CensusEndpoint}/s:{key}/get/{ns}/{service}/?field1=12&field2=34&field3=56");

            var query = GetCensusClient().CreateQuery(service);

            query.Where("field1").Equals("12");
            query.Where("field2").Equals("34");
            query.Where("field3").Equals("56");

            var censusUri = query.GetUri();

            Assert.AreEqual(expectedUri, censusUri);
        }

        [TestMethod]
        public void Census_AddResolve()
        {
            var service = "character";
            var ns = "ps2";
            var key = "testkey";

            var expectedUri = new Uri($"http://{Constants.CensusEndpoint}/s:{key}/get/{ns}/{service}/?c:resolve=items");

            var query = GetCensusClient().CreateQuery(service);

            query.AddResolve("items");

            var censusUri = query.GetUri();

            Assert.AreEqual(expectedUri, censusUri);
        }

        [TestMethod]
        public void Census_AddResolveMany()
        {
            var service = "character";
            var ns = "ps2";
            var key = "testkey";

            var expectedUri = new Uri($"http://{Constants.CensusEndpoint}/s:{key}/get/{ns}/{service}/?c:resolve=items,stuff");

            var query = GetCensusClient().CreateQuery(service);

            query.AddResolve(new[] { "items", "stuff" });

            var censusUri = query.GetUri();

            Assert.AreEqual(expectedUri, censusUri);
        }

        [TestMethod]
        public void Census_AddLanguage()
        {
            var service = "character";
            var ns = "ps2";
            var key = "testkey";

            var expectedUri = new Uri($"http://{Constants.CensusEndpoint}/s:{key}/get/{ns}/{service}/?c:lang=en");

            var query = GetCensusClient().CreateQuery(service);

            query.SetLanguage("en");

            var censusUri = query.GetUri();

            Assert.AreEqual(expectedUri, censusUri);
        }

        [TestMethod]
        public void Census_AddJoin()
        {
            var service = "character";
            var ns = "ps2";
            var key = "testkey";

            var expectedUri = new Uri($"http://{Constants.CensusEndpoint}/s:{key}/get/{ns}/{service}/?c:join=joinedservice^terms:field1=12'field2=34^on:ontestfield^to:totestfield^inject_at:testinject");

            var query = GetCensusClient().CreateQuery(service);

            var joinedService = query.JoinService("joinedservice");
            joinedService.ToField("totestfield");
            joinedService.OnField("ontestfield");
            joinedService.WithInjectAt("testinject");
            joinedService.Where("field1").Equals("12");
            joinedService.Where("field2").Equals("34");

            var censusUri = query.GetUri();

            Assert.AreEqual(expectedUri, censusUri);
        }

        [TestMethod]
        public void Census_AddJoinWithSubJoin()
        {
            var service = "character";
            var ns = "ps2";
            var key = "testkey";

            var expectedUri = new Uri($"http://{Constants.CensusEndpoint}/s:{key}/get/{ns}/{service}/?c:join=joinedservice^on:testfield(subjoined^list:true)");

            var query = GetCensusClient().CreateQuery(service);

            var joinedService = query.JoinService("joinedservice");
            joinedService.OnField("testfield");

            var subJoinedService = joinedService.JoinService("subjoined");
            subJoinedService.IsList(true);

            var censusUri = query.GetUri();

            Assert.AreEqual(expectedUri, censusUri);
        }

        [TestMethod]
        public void Census_AddTree()
        {
            var service = "character";
            var ns = "ps2";
            var key = "testkey";

            var expectedUri = new Uri($"http://{Constants.CensusEndpoint}/s:{key}/get/{ns}/{service}/?c:tree=treefield^prefix:someprefix^start:treestart");

            var query = GetCensusClient().CreateQuery(service);

            var treeField = query.TreeField("treefield");
            treeField.StartField("treestart");
            treeField.GroupPrefix("someprefix");

            var censusUri = query.GetUri();

            Assert.AreEqual(expectedUri, censusUri);
        }

        [TestMethod]
        public void Census_AddTreeWithSubTree()
        {
            var service = "character";
            var ns = "ps2";
            var key = "testkey";

            var expectedUri = new Uri($"http://{Constants.CensusEndpoint}/s:{key}/get/{ns}/{service}/?c:tree=treefield^start:treestart(subtreefield^start:subtreestart)");

            var query = GetCensusClient().CreateQuery(service);

            var treeField = query.TreeField("treefield");
            treeField.StartField("treestart");

            var subTreeField = treeField.TreeField("subtreefield");
            subTreeField.StartField("subtreestart");

            var censusUri = query.GetUri();

            Assert.AreEqual(expectedUri, censusUri);
        }

        [TestMethod]
        public void Census_ShowFields()
        {
            var service = "character";
            var ns = "ps2";
            var key = "testkey";

            var expectedUri = new Uri($"http://{Constants.CensusEndpoint}/s:{key}/get/{ns}/{service}/?c:show=field1,field2,field3");

            var query = GetCensusClient().CreateQuery(service);

            query.ShowFields(new[] { "field1", "field2", "field3" });

            var censusUri = query.GetUri();

            Assert.AreEqual(expectedUri, censusUri);
        }

        [TestMethod]
        public void Census_HideFields()
        {
            var service = "character";
            var ns = "ps2";
            var key = "testkey";

            var expectedUri = new Uri($"http://{Constants.CensusEndpoint}/s:{key}/get/{ns}/{service}/?c:hide=field1,field2,field3");

            var query = GetCensusClient().CreateQuery(service);

            query.HideFields(new[] { "field1", "field2", "field3" });

            var censusUri = query.GetUri();

            Assert.AreEqual(expectedUri, censusUri);
        }

        private CensusClient GetCensusClient()
        {
            var options = new CensusOptions
            {
                CensusServiceKey = "testkey",
                CensusServiceNamespace = "ps2"
            };

            return new CensusClient(Options.Create(options), null);
        }
    }
}
