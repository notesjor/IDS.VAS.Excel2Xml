using IDS.MAP.Toolbox.Model.TestStep.Abstract;
using IDS.MAP.Toolbox.Model.TestStep;

namespace IDS.MAP.Toolbox.Model
{
  public class TestController
  {
    public AbstractTestStep[] Steps { get; } = { new S1(), new S2(), new S3(), new S4(), new S5() };
    public int Execute(ref MapConfiguration config)
    {
      for (var i = 0; i < Steps.Length; i++)
      {
        Steps[i].Execute(ref config);
        if (!Steps[i].Valid)
          return i;
      }
      return Steps.Length;
    }
  }
}
