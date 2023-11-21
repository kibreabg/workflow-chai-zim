using Chai.WorkflowManagment.CoreDomain.Requests;

namespace Chai.WorkflowManagment.Modules.Request.Views
{
    public interface IStationaryRequestView
    {
        StationaryRequest StationaryRequest { get; set; }
        string RequestNo { get; }
        string RequestDate { get; }
        int StationaryRequestId { get; }
    }
}




