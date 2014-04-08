namespace WordNetSharp.UnitTests
{
    public class TestHelper
    {
        static TestHelper()
        {
            WordNetEngine = new WordNetEngine(@"WordNet", true);
        }
        
        public static WordNetEngine WordNetEngine { get; private set; } 

        
    }
}