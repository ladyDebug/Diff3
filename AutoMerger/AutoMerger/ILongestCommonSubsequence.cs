using System.Collections.Generic;

namespace AutoMerger
{
    public interface ILongestCommonSubsequence
    {
        List<ItemPosition> GetLongestCommonSubsequence(string[] file1, string[] file2);
    }
}
