using System.Collections;
using System.IO.Pipes;

namespace lotto;

public class Pool : IEnumerator<List<ushort>>
{
  private static Random _random = new Random();
  private List<ushort> _available;
  public ushort MaxPick { get; }
  public ushort NumPicks { get; }
  
  
  public Pool(ushort maxPick, ushort numPicks)
  {
    MaxPick = maxPick;
    NumPicks = numPicks;
    _available = LoadAvailable(numPicks);
    Current = new List<ushort>();
  }

  private List<ushort> LoadAvailable(ushort balls, List<ushort>? exclude = null)
  {
    var available = new List<ushort>();
    ushort ball = 0;
    while (ball < balls)
    {
      ++ball;
      if (exclude==null || !exclude.Contains(ball))
        available.Add(ball);
    }

    return available;
  }

  public ushort MinBoards()
  {
    double boards = NumPicks;
    boards = Math.Ceiling(boards / MaxPick);
    return (ushort)boards;
  }

  private void GetNextBoard()
  {
    Current.Clear();
    if (_available.Count < MaxPick)
    {
      Current.AddRange(_available);
      _available = LoadAvailable(NumPicks, Current);
    }

    while (Current.Count < MaxPick)
    {
      int pick = _random.Next(0, _available.Count - 1);
      Current.Add(_available[pick]);
      _available.RemoveAt(pick);
      if (!_available.Any())
      {
        _available = LoadAvailable(NumPicks, Current);
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