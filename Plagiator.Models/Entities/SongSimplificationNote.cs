namespace Plagiator.Models.Entities
{
    /// <summary>
    /// Used for a join table that associates notes to song simplifications
    /// </summary>
    public class SongSimplificationNote
    {
        public long Id { get; set; }
        public long SongSimplificationId { get; set; }
        public long NoteId { get; set; }
    }
}
