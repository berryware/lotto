using System.Collections;
using System.IO.Pipes;

namespace lotto;

public class Pool : IEnumerator<List<ushort>>
{
  private static Random _random = new Random();
  private List<ushort> _available;
  public ushort Selections { get; }
  public ushort MaxNumber { get; }
  
  
  public Pool(ushort selections, ushort maxNumber)
  {
    Selections = selections;
    MaxNumber = maxNumber;
    _available = LoadAvailable(maxNumber);
    Current = new List<ushort>();
  }

  private List<ushort> LoadAvailable(ushort maxNumber, List<ushort>? exclude = null)
  {
    var available = new List<ushort>();
    ushort pick = 0;
    while (pick < maxNumber)
    {
      ++pick;
      if (exclude==null || !exclude.Contains(pick))
        available.Add(pick);
    }

    return available;
  }

  public ushort MinBoards()
  {
    double boards = Math.Ceiling((double)MaxNumber / Selections);
    return (ushort)boards;
  }

  private void GetNextBoard()
  {
    Current.Clear();
    if (_available.Count < Selections)
    {
      Current.AddRange(_available);
      _available = LoadAvailable(MaxNumber, Current);
    }

    while (Current.Count < Selections)
    {
      int pick = _random.Next(0, _available.Count - 1);
      Current.Add(_available[pick]);
      _available.RemoveAt(pick);
      if (!_available.Any())
      {
        _available = LoadAvailable(MaxNumber, Current);
      }
    }
    
    Current.Sort();
  }

  public bool MoveNext()
  {
    GetNextBoard();
    return true;
  }

  public void Reset()
  {
  }

  public List<ushort> Current { get; }

  object IEnumerator.Current => Current;

  public void Dispose()
  {
  }
}