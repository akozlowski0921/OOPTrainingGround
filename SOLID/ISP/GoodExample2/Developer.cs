namespace SOLID.ISP.Good2
{
    // ✅ Developer implements all relevant interfaces
    public class Developer : IWorker, IVacationRequester, ICodeReviewer, IDeployer
    {
        public void Work() { }
        public void TakeBreak() { }
        public void AttendMeeting() { }
        public void SubmitTimesheet() { }
        public void RequestVacation() { }
        public void ReviewCode() { }
        public void DeployToProduction() { }
    }
}
