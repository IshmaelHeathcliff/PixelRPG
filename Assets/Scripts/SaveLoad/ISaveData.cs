using System;

namespace SaveLoad
{
    /// <summary>
    /// Classes that implement this interface should have an serialized instance of DataSettings to register through.
    /// </summary>
    public interface ISaveData
    {
        string DataTag { get; set; }

        Data SaveData();

        void LoadData(Data data);
    }

    public class Data
    {
    }


    public class Data<T> : Data
    {
        public T Value;

        public Data(T value)
        {
            this.Value = value;
        }
    }


    public class Data<T0, T1> : Data
    {
        public T0 Value0;
        public T1 Value1;

        public Data(T0 value0, T1 value1)
        {
            this.Value0 = value0;
            this.Value1 = value1;
        }
    }


    public class Data<T0, T1, T2> : Data
    {
        public T0 Value0;
        public T1 Value1;
        public T2 Value2;

        public Data(T0 value0, T1 value1, T2 value2)
        {
            this.Value0 = value0;
            this.Value1 = value1;
            this.Value2 = value2;
        }
    }


    public class Data<T0, T1, T2, T3> : Data
    {
        public T0 Value0;
        public T1 Value1;
        public T2 Value2;
        public T3 Value3;

        public Data(T0 value0, T1 value1, T2 value2, T3 value3)
        {
            this.Value0 = value0;
            this.Value1 = value1;
            this.Value2 = value2;
            this.Value3 = value3;
        }
    }


    public class Data<T0, T1, T2, T3, T4> : Data
    {
        public T0 Value0;
        public T1 Value1;
        public T2 Value2;
        public T3 Value3;
        public T4 Value4;

        public Data(T0 value0, T1 value1, T2 value2, T3 value3, T4 value4)
        {
            this.Value0 = value0;
            this.Value1 = value1;
            this.Value2 = value2;
            this.Value3 = value3;
            this.Value4 = value4;
        }
    }
}