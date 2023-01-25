namespace tDarkBot.Services
{
    public class TimeService
    {
        public TimeService()
        {
            _creationDate = DateTime.Now;
        }

        private DateTime _creationDate;
        public DateTime CreationDate
        {
            get => _creationDate;
        }

        private DateTime _readyDate;
        public DateTime ReadyDate
        {
            get => _readyDate;
            set => _readyDate = value;
        }
    }
}