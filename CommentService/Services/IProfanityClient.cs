namespace CommentService.Services
{
    public interface IProfanityClient
    {
        Task<bool> ContainsProfanity(string text, CancellationToken ct = default);
    }
}
