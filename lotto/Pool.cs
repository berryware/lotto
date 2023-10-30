using System.Collections;
using System.IO.Pipes;

namespace lotto;

public class Pool : IEnumerator<List<ushort>>
{
  private static Random _random = new Random();
  private List<ushort> _available;
  public ushort MaxPick { get; }
  public ushort NumPicks { get; }
  
  
  public Pool(ushort numPicks, ushort maxPick)
  {
    NumPicks = numPicks;
    MaxPick = maxPick;
    _available = LoadAvailable(maxPick);
    Current = new List<ushort>();
  }

  private List<ushort> LoadAvailable(ushort maxPick, List<ushort>? exclude = null)
  {
    var available = new List<ushort>();
    ushort pick = 0;
    while (pick < maxPick)
    {
      ++pick;
      if (exclude==null || !exclude.Contains(pick))
        available.Add(pick);
    }

    return available;
  }

  public ushort MinBoards()
  {
    double boards = Math.Ceiling((double)MaxPick / NumPicks);
    return (ushort)boards;
  }

  private void GetNextBoard()
  {
    Current.Clear();
    if (_available.Count < NumPicks)
    {
      Current.AddRange(_available);
      _available = LoadAvailable(MaxPick, Current);
    }

    while (Current.Count < NumPicks)
    {
      int pick = _random.Next(0, _available.Count - 1);
      Current.Add(_available[pick]);
      _available.RemoveAt(pick);
      if (!_available.Any())
      {
        _available = LoadAvailable(MaxPick, Current);
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