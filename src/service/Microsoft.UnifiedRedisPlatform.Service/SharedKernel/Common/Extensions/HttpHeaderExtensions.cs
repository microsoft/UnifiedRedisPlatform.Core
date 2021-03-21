namespace System.Net.Http.Headers
{
    public static class HttpHeaderExtensions
    {
        public static void AddOrUpdate(this HttpHeaders headers, string name, string value)
        {
            if (headers.Contains(name))
                headers.Remove(name);
            headers.Add(name, value);
        }
    }
}
