namespace SOLID.ISP.Bad2
{
    // ❌ BAD: Fat interface - not all workers need all methods
    public interface IWorker
    {
        void Work();
        void TakeBreak();
        void AttendMeeting();
        void SubmitTimesheet();
        void RequestVacation();
        void ReviewCode();
        void DeployToProduction();
    }

    public class Developer : IWorker
    {
        public void Work() { }
        public void TakeBreak() { }
        public void AttendMeeting() { }
        public void SubmitTimesheet() { }
        public void RequestVacation() { }
        public void ReviewCode() { }
        public void DeployToProduction() { }
    }

    public class Intern : IWorker
    {
        public void Work() { }
        public void TakeBreak() { }
        public void AttendMeeting() { }
        public void SubmitTimesheet() { }
        
        // Problem: Interns don't do these
        public void RequestVacation() { throw new System.NotImplementedException(); }
        public void ReviewCode() { throw new System.NotImplementedException(); }
        public void DeployToProduction() { throw new System.NotImplementedException(); }
    }
}
