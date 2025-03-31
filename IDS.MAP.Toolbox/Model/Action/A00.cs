using IDS.MAP.Toolbox.Model.Action.Abstract;

namespace IDS.MAP.Toolbox.Model.Action
{
    public class A00 : AbstractAction
    {
      public override bool AutoRun { get; } = true;
      public override void Execute(ref MapConfiguration config)
      {
        
      }
    }
}
