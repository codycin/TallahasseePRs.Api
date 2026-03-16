namespace TallahasseePRs.Api.DTOs.Feed
{
    //generic for pagination of other things in the future 
    //For now just posts
    public sealed class FeedPage<T>
    {
        //Returns list of whatver response T it gets
        public required IReadOnlyList<T> Items { get; init; }

        public string? NextCursor { get; init; }    
    }
}
