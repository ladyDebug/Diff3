using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoMerger
{
    public class HuntSzymanskiLcs : ILongestCommonSubsequence
    {
        private string[] _file1;
        private string[] _file2;

        private List<List<int>> _matchList;
        private List<int> _threshArray;
        private int _maxValue;
        private int _minValue;
        private ItemPosition[] _linksCorrect;
        public List<ItemPosition> GetLongestCommonSubsequence(string[] file1, string[] file2)
        {
            _file1 = file1;
            _file2 = file2;
            _minValue = Math.Min(_file1.Length, _file2.Length);
            _maxValue = Math.Max(_file1.Length, _file2.Length);
            _matchList = new List<List<int>>();
            _threshArray = new List<int>();
            _linksCorrect = new ItemPosition[_minValue + 1];
            _linksCorrect[0] = new ItemPosition { IndexFile1 = -1, IndexFile2 = -1 };
            SetMatchList();
            InitializeThresh();
            ComputeThresh();

            List<ItemPosition> result = _linksCorrect.ToList();
            result.RemoveAll(item => item == null);
            List<ItemPosition> l = new List<ItemPosition>();

            var lastItem = result.LastOrDefault();

            while (lastItem != null)
            {

                l.Add(lastItem);

                lastItem = lastItem.LinkPrevious;
            }

            return l;
        }

        private void InitializeThresh()
        {
            _threshArray.Add(0);
            for (int i = 1; i < _minValue + 2; i++)
            {
                _threshArray.Add(_maxValue + 1);
            }
        }

        private void ComputeThresh()
        {
            for (int i = 0; i < _matchList.Count; i++)
            {
                foreach (var item in _matchList[i])
                {
                    for (int k = 0; k < _threshArray.Count - 2; k++)
                    {
                        if (_threshArray[k] < item + 1 && _threshArray[k + 1] >= item + 1)
                        {
                            if (_threshArray[k + 1] > item + 1)
                            {
                                _threshArray[k + 1] = item + 1;
                                _linksCorrect[k + 1] = new ItemPosition { IndexFile1 = i, IndexFile2 = item, LinkPrevious = _linksCorrect[k] };
                            }
                            break;
                        }
                    }
                }
            }
        }

        private void SetMatchList()
        {
            foreach (string item in _file1)
            {
                List<int> matchedFile2 = new List<int>();
                for (int j = 0; j < _file2.Length; j++)
                {
                    if (_file2[j].Trim() == item.Trim())
                    {
                        matchedFile2.Add(j);
                    }

                }
                matchedFile2.Reverse();
                _matchList.Add(matchedFile2);
            }

        }

    }
}
