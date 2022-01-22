namespace Plml.Checkers
{
    internal interface IChecker<TModel>
    {
        bool CheckForChanges(TModel model);
    }
}