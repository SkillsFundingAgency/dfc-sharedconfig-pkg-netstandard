namespace Dfc.SharedConfig.IntegrationTests
{
    internal static class Constants
    {
        public static string IntegrationTestServiceName { get; private set; } = "LocalIntegrationTest";

        public static string SingleStringKeyName { get; private set; } = "IndexName";

        public static string SingleStringValue { get; private set; } = "\"dummyTestValue\"";

        public static string SimpleObjectKeyName { get; private set; } = "TestObject";

        public static string SimpleObjectValue { get; private set; } = "{\"dataField1\": \"datavalue1\",\"dataField2\": \"datavalue2\",\"dataField3\": \"datavalue3\"}";
    }
}