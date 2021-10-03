using System;
using System.Threading.Tasks;

namespace gapi.Queries
{
    /// <summary>
    /// Query class initialization
    /// </summary>
    public class Query
    {
        //sample query
        /*
            {
                demodata
            }
        */
        public Query()
        {
        }

        public async ValueTask<string> GetDemodata()
        {
            return "demo data " + new Random().Next().ToString();
        }
    }
}
