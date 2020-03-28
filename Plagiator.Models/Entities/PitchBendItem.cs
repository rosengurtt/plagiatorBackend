
namespace Plagiator.Models.Entities
{
    public class PitchBendItem
    {
        public long Id { get; set; }
        public long TicksSinceBeginningOfSong { get; set; }
        public ushort Pitch { get; set; }

        public long NoteId { get; set; }
        public Note Note { get; set; }

        public PitchBendItem Clone()
        {
            return new PitchBendItem
            {
                TicksSinceBeginningOfSong = this.TicksSinceBeginningOfSong,
                Pitch = this.Pitch
            };
        }
    }
}
