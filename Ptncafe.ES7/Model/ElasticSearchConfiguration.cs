namespace Ptncafe.ES7.Model
{
    public class ElasticSearchConfiguration
    {
        /// <summary>
        /// url của ES http://localhost:9200/
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Index mặc định
        /// </summary>
        public string DefaultIndex { get; set; }
    }
}