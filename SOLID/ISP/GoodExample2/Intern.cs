namespace SOLID.ISP.Good2
{
    // ✅ Intern only implements what they need
    public class Intern : IWorker
    {
        public void Work() { }
        public void TakeBreak() { }
        public void AttendMeeting() { }
        public void SubmitTimesheet() { }
    }
}
