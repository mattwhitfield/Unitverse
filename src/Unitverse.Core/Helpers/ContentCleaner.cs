namespace Unitverse.Core.Helpers
{
    using System.Text;

    public static class ContentCleaner
    {
        public static string Clean(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return content;
            }

            var builder = new StringBuilder();
            var lineBuilder = new StringBuilder();

            foreach (char c in content)
            {
                if (c == '\r' || c == '\n')
                {
                    if (lineBuilder.Length > 0)
                    {
                        var line = lineBuilder.ToString();
                        lineBuilder.Length = 0;
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            builder.Append(line);
                        }
                    }

                    builder.Append(c);
                }
                else
                {
                    lineBuilder.Append(c);
                }
            }

            if (lineBuilder.Length > 0)
            {
                var line = lineBuilder.ToString();
                if (!string.IsNullOrWhiteSpace(line))
                {
                    builder.Append(line);
                }
            }

            return builder.ToString();
        }
    }
}
