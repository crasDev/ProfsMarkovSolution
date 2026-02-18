using ProfsMarkovHub.Data;
using ProfsMarkovHub.Models;

namespace ProfsMarkovHub.Services;

public interface IPublishService
{
    void ApplyStatusTransition(Article article, ArticleStatus newStatus, DateTime? scheduledAt);
    int AutoPublishDue();
}

public class PublishService : IPublishService
{
    private readonly ApplicationDbContext _context;
    public PublishService(ApplicationDbContext context) { _context = context; }

    public void ApplyStatusTransition(Article article, ArticleStatus newStatus, DateTime? scheduledAt)
    {
        switch (newStatus)
        {
            case ArticleStatus.Draft:
                article.Status = ArticleStatus.Draft;
                article.PublishedAt = null;
                article.ScheduledAt = null;
                break;
            case ArticleStatus.Published:
                article.Status = ArticleStatus.Published;
                article.PublishedAt = DateTime.UtcNow;
                article.ScheduledAt = null;
                break;
            case ArticleStatus.Scheduled:
                article.Status = ArticleStatus.Scheduled;
                article.PublishedAt = null;
                article.ScheduledAt = scheduledAt;
                break;
        }
    }

    public int AutoPublishDue()
    {
        var now = DateTime.UtcNow;
        var due = _context.Articles.Where(a => a.Status == ArticleStatus.Scheduled && a.ScheduledAt != null && a.ScheduledAt <= now && !a.IsDeleted).ToList();
        foreach (var a in due)
        {
            a.Status = ArticleStatus.Published;
            a.PublishedAt = now;
            a.ScheduledAt = null;
        }
        _context.SaveChanges();
        return due.Count;
    }
}
