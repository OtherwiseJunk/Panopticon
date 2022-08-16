namespace Panopticon.Shared.Models
{
   public class Feedback
    {
        public int Id { get; set; }
        public ulong ReportingUser { get; set; }
        public string Message { get; set; }

        public Feedback() {
            Message = "";
        }
        public Feedback(ulong user, string msg)
        {
            ReportingUser = user;
            Message = msg;
        }

    }
}
