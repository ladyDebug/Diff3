using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoMerger
{
    public class Merger
    {
        private readonly ILongestCommonSubsequence _longestCommonSubsequenceAlg;
        private readonly List<Chunk> _chunks;
        private List<ResultString> _mergedFile;

        public Merger(ILongestCommonSubsequence longestCommonSubsequenceAlg)
        {
            _longestCommonSubsequenceAlg = longestCommonSubsequenceAlg;
            _chunks = new List<Chunk>();
            _mergedFile = new List<ResultString>();

        }

        public List<ResultString> MergeThreeWay(string[] baseFile, string[] fileA, string[] fileB)
        {
            DefineHunkTwoWay(baseFile, fileA, FileType.Left);
            DefineHunkTwoWay(baseFile, fileB, FileType.Right);
            _chunks.Sort();
            CombineResult(baseFile, fileA, fileB);
            return _mergedFile;
        }

        private void DefineHunkTwoWay(string[] file1, string[] file2, FileType fileType)
        {
            var lcs = _longestCommonSubsequenceAlg.GetLongestCommonSubsequence(file1, file2);
            int baseRightPosition = file1.Length;
            int fileRoghtPosition = file2.Length;
            foreach (var item in lcs)
            {
                int diffBaseLength = baseRightPosition - 1 - item.IndexFile1;
                int diffFileLength = fileRoghtPosition - 1 - item.IndexFile2;

                if (diffBaseLength > 0 || diffFileLength > 0)
                {
                    _chunks.Add(new Chunk
                    {
                        PositionFile1 = new ItemChunk { StartPosition = item.IndexFile1 + 1, Length = diffBaseLength },
                        PositionFile2 = new ItemChunk { StartPosition = item.IndexFile2 + 1, Length = diffFileLength },
                        FileType = fileType
                    });
                }
                baseRightPosition = item.IndexFile1;
                fileRoghtPosition = item.IndexFile2;
            }

        }

        private void CombineResult(string[] baseFile, string[] fileA, string[] fileB)
        {
            int leftIndex = 0;

            if (_chunks.Count == 0)
            {
                ResultString resultString = new ResultString { Line = new List<string>() };
                resultString.Line = baseFile.ToList();
                _mergedFile.Add(resultString);
            }

            for (int i = 0; i < _chunks.Count; i++)
            {
                int chunkLeftPos = _chunks[i].PositionFile1.StartPosition;
                var res = SetResultBlock(leftIndex, chunkLeftPos, baseFile);
                _mergedFile.Add(res);

                int chunkRigthPos = _chunks[i].PositionFile1.StartPosition + _chunks[i].PositionFile1.Length;

                int chunkGroupIndexStart = i;

                while (i < _chunks.Count - 1)
                {
                    int secondChunkLeftPos = _chunks[i + 1].PositionFile1.StartPosition;
                    if (chunkRigthPos < secondChunkLeftPos)
                    {
                        break;

                    }
                    chunkRigthPos = Math.Max(chunkRigthPos, _chunks[i + 1].PositionFile1.Length + secondChunkLeftPos);
                    i++;
                }

                int chunkGroupIndexEnd = i;
                res = SetResultBlock(chunkGroupIndexStart, chunkGroupIndexEnd, baseFile, fileA, fileB);
                _mergedFile.Add(res);

                leftIndex = chunkRigthPos;
            }
            if (leftIndex < baseFile.Length)
            {
                var res = SetResultBlock(leftIndex, baseFile.Length, baseFile);
                _mergedFile.Add(res);
            }
        }

        private ResultString SetResultBlock(int leftPos, int rightPos, string[] file, bool isConflict = false)
        {
            ResultString resultString = new ResultString { IsConflicted = isConflict };
            for (int i = leftPos; i < rightPos; i++)
            {
                resultString.Line.Add(file[i]);

            }
            return resultString;
        }


        private ResultString SetResultBlock(int firstChunkIndex, int endChunkIndex, string[] baseFile, string[] fileA,
           string[] fileB)
        {
            int oldRight = -1;
            int oldLeft = _chunks[firstChunkIndex].PositionFile1.StartPosition;

            int fileALeft = fileA.Length;
            int fileARight = -1;
            int chunkALeft = baseFile.Length;
            int chunkARight = -1;

            int fileBLeft = fileB.Length;
            int fileBRight = -1;
            int chunkBLeft = baseFile.Length;
            int chunkBRight = -1;


            bool isAConflict = false;
            bool isBConflict = false;


            for (int i = firstChunkIndex; i <= endChunkIndex; i++)
            {
                var currectChunk = _chunks[i];
                oldRight = Math.Max(oldRight, currectChunk.PositionFile1.StartPosition +
                          currectChunk.PositionFile1.Length);
                int fileLeft = currectChunk.PositionFile2.StartPosition;
                int fileRight = currectChunk.PositionFile2.StartPosition +
                                currectChunk.PositionFile2.Length;

                if (_chunks[i].FileType == FileType.Left)
                {
                    isAConflict = true;
                    fileALeft = Math.Min(fileALeft, fileLeft);
                    fileARight = Math.Max(fileARight, fileRight);
                    chunkALeft = Math.Min(currectChunk.PositionFile1.StartPosition, chunkALeft);
                    chunkARight = Math.Max(currectChunk.PositionFile1.StartPosition + currectChunk.PositionFile1.Length, chunkARight);
                }
                else
                {
                    isBConflict = true;
                    fileBLeft = Math.Min(fileBLeft, fileLeft);
                    fileBRight = Math.Max(fileBRight, fileRight);
                    chunkBLeft = Math.Min(currectChunk.PositionFile1.StartPosition, chunkBLeft);
                    chunkBRight = Math.Max(currectChunk.PositionFile1.StartPosition + currectChunk.PositionFile1.Length, chunkBRight);
                }

            }

            fileALeft = fileALeft + oldLeft - chunkALeft;
            fileARight = fileARight + oldRight - chunkARight;
            fileBLeft = fileBLeft + oldLeft - chunkBLeft;
            fileBRight = fileBRight + oldRight - chunkBRight;


            bool isThreeWayConflict = isAConflict && isBConflict;
            ResultString resultString = new ResultString { IsConflicted = isThreeWayConflict };

            if (!isThreeWayConflict)
            {
                if (isAConflict)
                {
                    resultString = SetResultBlock(fileALeft, fileARight, fileA);
                }
                else if (isBConflict)
                {
                    resultString = SetResultBlock(fileBLeft, fileBRight, fileB);
                }
                return resultString;
            }


            for (int i = oldLeft; i < oldRight; i++)
            {
                resultString.BaseLine.Add(baseFile[i]);

            }
            for (int i = fileALeft; i < fileARight; i++)
            {
                resultString.LeftLine.Add(fileA[i]);

            }
            for (int i = fileBLeft; i < fileBRight; i++)
            {
                resultString.RightLine.Add(fileB[i]);

            }
            return resultString;
        }


    }
}
