using System;

namespace Plml.Checkers
{
    internal class ValueArrayChecker<TStruct> :
        IChecker<TStruct[]>
        where TStruct : struct
    {
        private bool initialized = false;
        private TStruct[] lastReference;
        private TStruct[] lastValues;

        public bool CheckForChanges(TStruct[] source)
        {
            if (initialized)
            {
                if (lastReference != source)
                {
                    lastReference = source;
                    lastValues = source.ShallowCopy();

                    return true;
                }
                else
                {
                    for (int i = 0; i < source.Length; i++)
                    {
                        if (!source[i].Equals(lastValues[i]))
                        {
                            source.CopyTo(lastValues);
                            return true;
                        }
                    }

                    return false;
                }
            }
            else
            {
                initialized = true;

                lastReference = source;
                lastValues = source.ShallowCopy();

                return true;
            }
        }
    }
}