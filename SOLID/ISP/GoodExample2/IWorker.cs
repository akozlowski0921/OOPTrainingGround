namespace SOLID.ISP.Good2
{
    // ✅ Basic worker interface
    public interface IWorker
    {
        void Work();
        void TakeBreak();
        void AttendMeeting();
        void SubmitTimesheet();
    }
}
