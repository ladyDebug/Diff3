using System;

namespace AutoMerger
{
    public enum FileType
    {
        Left,
        Right
    }
    public class Chunk: IComparable<Chunk>
    {
        public ItemChunk PositionFile1 { get; set; }
        public ItemChunk PositionFile2 { get; set; }
        public FileType FileType { get; set; }
        public int CompareTo(Chunk other)
        {
            if (PositionFile1.StartPosition != other.PositionFile1.StartPosition)
                return PositionFile1.StartPosition.CompareTo(other.PositionFile1.StartPosition);
            return PositionFile2.StartPosition.CompareTo(other.PositionFile2.StartPosition);
        }
    }

    public class ItemChunk
    {
        public int StartPosition { get; set; }
        public int Length { get; set; }
    }
}
