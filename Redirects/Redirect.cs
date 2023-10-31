namespace CleanTemplate.Redirects
{
    public class Redirect
    {
        public string RedirectUrl { get; set; } = "";
        public string TargetUrl { get; set; } = "";
        public int RedirectType { get; set; } = 302;
        public bool UseRelative { get; set; } = false;

        public bool IsPermanant => RedirectType == 301 || RedirectType == 308;
        public bool MatchesRelative(string url) => url.StartsWith(RedirectUrl, StringComparison.OrdinalIgnoreCase);
        public string Location(string path)
        {
            if (UseRelative && MatchesRelative(path))
                return TargetUrl + path.Remove(0, RedirectUrl.Length);

            return TargetUrl;
        }
    }
}