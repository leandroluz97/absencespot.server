namespace Absencespot.Utils
{
    public class RepositoryOptions
    {
        public bool Tracking { get; set; }

        public static RepositoryOptions AsNoTracking()
        {
            return new RepositoryOptions(){ Tracking = false};
        }
        public static RepositoryOptions AsTracking()
        {
            return new RepositoryOptions() { Tracking = true };
        }

    }
}