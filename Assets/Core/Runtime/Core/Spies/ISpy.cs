namespace Plml
{
    public interface ISpy : ISpyConfig
    {
        void DetectChanges();
    }
}