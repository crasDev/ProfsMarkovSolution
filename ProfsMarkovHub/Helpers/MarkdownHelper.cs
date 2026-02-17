namespace ProfsMarkovHub.Helpers {
    using Markdig;

    public static class MarkdownHelper
    {
        public static string ToHtml(string markdown)
        {
            if (string.IsNullOrWhiteSpace(markdown)) return string.Empty;

            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            return Markdown.ToHtml(markdown, pipeline);
        }
    }
}