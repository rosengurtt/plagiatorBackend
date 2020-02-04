namespace Plagiator.Music.Models
{
    public class PitchBendItem
    {
        public long Id { get; set; }
        public long TicksSiceBeginningOfSong { get; set; }
        public int Pitch { get; set; }

        public int SongId { get; set; }
        public Song Song { get; set; }

        public PitchBendItem Clone()
        {
            return new PitchBendItem
            {
                TicksSiceBeginningOfSong = this.TicksSiceBeginningOfSong,
                Pitch = this.Pitch
            };
        }
    }
}
