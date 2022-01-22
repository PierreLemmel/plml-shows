namespace Plml.Jace.Compilation
{
    public class ConstantInfo
    {
        public ConstantInfo(string constantName, float value, bool isOverWritable)
        {
            this.ConstantName = constantName;
            this.Value = value;
            this.IsOverWritable = isOverWritable;
        }

        public string ConstantName { get; private set; }

        public float Value { get; private set; }

        public bool IsOverWritable { get; set; }
    }
}
