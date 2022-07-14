using CsOpenGenericsInRuntime.Services;

namespace CsOpenGenericsInRuntime.Demos
{
    public abstract class DemoBase
    {
        public abstract Task RunAsync();
        public IServiceRequest GetFirstRequest()
        {
            var req = new FirstServiceRequest
            {
                AnotherProperty = "given1"
            };

            return req;
        }

        public IServiceRequest GetSecondRequest()
        {
            var req = new SecondServiceRequest();

            return req;
        }
    }
}
