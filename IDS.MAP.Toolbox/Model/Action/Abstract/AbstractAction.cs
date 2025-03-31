namespace IDS.MAP.Toolbox.Model.Action.Abstract
{
  public abstract class AbstractAction
  {
    public abstract bool AutoRun { get; }
    public abstract void Execute(ref MapConfiguration config);
  }
}
