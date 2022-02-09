namespace Plml.Dmx
{
    public class DmxRangeAttribute : RangeBoundsAttribute
    {
        public DmxRangeAttribute() : base(0x00, 0xff)
        {
        }
    }
}