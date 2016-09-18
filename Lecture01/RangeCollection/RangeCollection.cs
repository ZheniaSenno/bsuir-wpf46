using System;
using System.Collections;
using System.Collections.Generic;

namespace RangeCollection
{
    public class RangeCollection : IEnumerable<int>
    {
        private Tuple<int, int>[] _values;
        private int _count;
        private long _gen;

        public RangeCollection(params Tuple<int, int>[] values)
        {
            _gen = _count = 0;
            _values = new Tuple<int, int>[100];
            if (values.Length == 0) return;
            Array.Sort(values, (x, y) => x.Item1.CompareTo(y.Item1));
            int start = values[0].Item1, end = values[0].Item2;
            for (int i = 1; i < values.Length; ++i)
            {
                if (values[i].Item1 > end + 1)
                {
                    _values[_count] = Tuple.Create(start, end);
                    if (++_count == _values.Length) Array.Resize(ref _values, _count << 1);
                    start = values[i].Item1;
                    end = values[i].Item2;
                }
                else
                {
                    end = Math.Max(end, values[i].Item2);
                }
            }
            _values[_count] = Tuple.Create(start, end);
            if (++_count == _values.Length) Array.Resize(ref _values, _count << 1);
        }

        private int FindFirstGreaterOrEqual(int value)
        {
            int start = 0, end = _count;
            while (start < end)
            {
                int mid = (start + end) >> 1;
                if (_values[mid].Item1 < value)
                    start = mid + 1;
                else
                    end = mid;
            }
            return start;
        }

        public void Add(int start, int end)
        {
            int i = FindFirstGreaterOrEqual(start);
            if (i > 0 && start <= _values[i - 1].Item2 + 1)
            {
                --i;
                _values[i] = Tuple.Create(_values[i].Item1, Math.Max(end, _values[i].Item2));
            }
            else if (i < _count && end >= _values[i].Item1 - 1)
            {
                _values[i] = Tuple.Create(start, Math.Max(end, _values[i].Item2));
            }
            else
            {
                for (int j = _count - 1; j >= i; --j) _values[j + 1] = _values[j];
                _values[i] = Tuple.Create(start, end);
                if (++_count == _values.Length) Array.Resize(ref _values, _count << 1);
                return;
            }

            int k = FindFirstGreaterOrEqual(end);
            if (k == _count || end < _values[k].Item1 - 1) --k;
            _values[i] = Tuple.Create(_values[i].Item1, Math.Max(_values[i].Item2, _values[k].Item2));
            ++i;
            for (int m = k + 1; m < _count; ++i, ++m) _values[i] = _values[m];
            _count = i;
        }

        public void Add(string str)
        {
            string[] args = str.Split(' ');
            Add(int.Parse(args[0]), int.Parse(args[1]));
        }

        public void Remove(int start, int end)
        {
            int i = FindFirstGreaterOrEqual(start) - 1, j = FindFirstGreaterOrEqual(end + 1);
            if (j > 0 && _values[j - 1].Item2 > end) --j;
            if (i == j)
            {
                if (i >= 0 && i < _count)
                {
                    for (int k = _count; k > i; --k) _values[k] = _values[k - 1];
                    if (++_count == _values.Length) Array.Resize(ref _values, _count << 1);
                    _values[i] = Tuple.Create(_values[i].Item1, start - 1);
                    _values[i + 1] = Tuple.Create(end + 1, _values[i + 1].Item2);
                }
            }
            else
            {
                if (i >= 0 && i < _count) _values[i] = Tuple.Create(_values[i].Item1, Math.Min(_values[i].Item2, end - 1));
                if (j >= 0 && j < _count) _values[j] = Tuple.Create(Math.Max(_values[j].Item1, start + 1), _values[j].Item2);
                int n = i + 1;
                for (int m = j; m < _count; ++n, ++m) _values[n] = _values[m];
                _count = n;
            }
        }

        public IEnumerator<int> GetEnumerator() => new RangeEnumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private class RangeEnumerator : IEnumerator<int>
        {
            private readonly long _gen;
            private readonly RangeCollection _this;
            private int _range = -1, _i;

            public RangeEnumerator(RangeCollection col)
            {
                _this = col;
                _gen = col._gen;
            }

            public void Reset() => _range = -1;

            public void Dispose() { }

            public int Current => _this._values[_range].Item1 + _i;

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                if (_gen != _this._gen)
                    throw new InvalidOperationException("Collection was modified; enumeration may not continue.");
                if (_range == -1 || _i == _this._values[_range].Item2 - _this._values[_range].Item1)
                {
                    _i = 0;
                    return (++_range < _this._count);
                }
                else
                {
                    ++_i;
                    return true;
                }
            }
        }

        public IEnumerable<int> EnumerateAll()
        {
            for (int i = 0; i < _count; ++i)
                for (int j = _values[i].Item1; j <= _values[i].Item2; ++j) yield return j;
        }
    }
}
