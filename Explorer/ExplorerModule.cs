using Nancy;
using Nancy.ModelBinding;
using PatientManagement.AdmissionDischargeTransfer.Commands;
using ProjectionManager;

namespace Explorer
{
    public class ExplorerModule : NancyModule
    {
        public ExplorerModule()
        {
            var repository = new Repository(new ConnectionFactory("PatientManagement"));
            var dispatcher = DispatcherFactory.Get();

            Get["/Wards"] = _ => View["WardView", new WardView { Wards = repository.GetWardView() }];

            Get["/Demographics"] = _ => View["Demographics"];

            Get["/WardData"] = _ => Negotiate.WithModel(repository.WardDemographicView());

            Get["/Patient/{patientId}"] = args => View["PatientView", repository.GetPatientView(args.PatientId)];

            Post["/Transfer"] = args =>
            {
                var model = this.Bind<TransferPatient>();
                dispatcher.Dispatch(model);
                return HttpStatusCode.OK;
            };

            Get["/Patient/Events/{patientId}"] = args =>
            {
                var events = repository.GetEventStream((string) args.PatientId);
                return Response.AsJson(events);
            };
        }
    }
}