using System.Threading.Tasks;

namespace PatientManagement.Framework.Commands
{
    public class Dispatcher
    {
        private readonly CommandHandlerMap _map;

        public Dispatcher(CommandHandlerMap map) => _map = map;

        public Task Dispatch(object command)
        {
            var handler = _map.Get(command);

            return handler(command);
        }
    }
}
