using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JohnsonNet.Base.Data
{
    public abstract class ResultSet
    {
    }
    public class ResultSet<T, T1> : ResultSet
    {
        public ResultSet<T, T1> AddRange(ResultSet<T, T1> item)
        {
            Result1.AddRange(item.Result1);
            Result2.AddRange(item.Result2);

            return this;
        }
        public List<T> Result1 { get; set; }
        public List<T1> Result2 { get; set; }
    }
    public class ResultSet<T, T1, T2> : ResultSet
    {
        public ResultSet<T, T1, T2> AddRange(ResultSet<T, T1, T2> item)
        {
            this.Result1.AddRange(item.Result1);
            this.Result2.AddRange(item.Result2);
            this.Result3.AddRange(item.Result3);

            return this;
        }
        public List<T> Result1 { get; set; }
        public List<T1> Result2 { get; set; }
        public List<T2> Result3 { get; set; }
    }
    public class ResultSet<T, T1, T2, T3> : ResultSet
    {
        public ResultSet<T, T1, T2, T3> AddRange(ResultSet<T, T1, T2, T3> item)
        {
            Result1.AddRange(item.Result1);
            Result2.AddRange(item.Result2);
            Result3.AddRange(item.Result3);
            Result4.AddRange(item.Result4);

            return this;
        }
        public List<T> Result1 { get; set; }
        public List<T1> Result2 { get; set; }
        public List<T2> Result3 { get; set; }
        public List<T3> Result4 { get; set; }
    }
    public class ResultSet<T, T1, T2, T3, T4> : ResultSet
    {
        public ResultSet<T, T1, T2, T3, T4> AddRange(ResultSet<T, T1, T2, T3, T4> item)
        {
            Result1.AddRange(item.Result1);
            Result2.AddRange(item.Result2);
            Result3.AddRange(item.Result3);
            Result4.AddRange(item.Result4);
            Result5.AddRange(item.Result5);

            return this;
        }
        public List<T> Result1 { get; set; }
        public List<T1> Result2 { get; set; }
        public List<T2> Result3 { get; set; }
        public List<T3> Result4 { get; set; }
        public List<T4> Result5 { get; set; }
    }
    public class ResultSet<T, T1, T2, T3, T4, T5> : ResultSet
    {
        public ResultSet<T, T1, T2, T3, T4, T5> AddRange(ResultSet<T, T1, T2, T3, T4, T5> item)
        {
            Result1.AddRange(item.Result1);
            Result2.AddRange(item.Result2);
            Result3.AddRange(item.Result3);
            Result4.AddRange(item.Result4);
            Result5.AddRange(item.Result5);
            Result6.AddRange(item.Result6);

            return this;
        }
        public List<T> Result1 { get; set; }
        public List<T1> Result2 { get; set; }
        public List<T2> Result3 { get; set; }
        public List<T3> Result4 { get; set; }
        public List<T4> Result5 { get; set; }
        public List<T5> Result6 { get; set; }
    }
    public class ResultSet<T, T1, T2, T3, T4, T5, T6> : ResultSet
    {
        public ResultSet<T, T1, T2, T3, T4, T5, T6> AddRange(ResultSet<T, T1, T2, T3, T4, T5, T6> item)
        {
            Result1.AddRange(item.Result1);
            Result2.AddRange(item.Result2);
            Result3.AddRange(item.Result3);
            Result4.AddRange(item.Result4);
            Result5.AddRange(item.Result5);
            Result6.AddRange(item.Result6);
            Result7.AddRange(item.Result7);

            return this;
        }
        public List<T> Result1 { get; set; }
        public List<T1> Result2 { get; set; }
        public List<T2> Result3 { get; set; }
        public List<T3> Result4 { get; set; }
        public List<T4> Result5 { get; set; }
        public List<T5> Result6 { get; set; }
        public List<T6> Result7 { get; set; }
    }
    public class ResultSet<T, T1, T2, T3, T4, T5, T6, T7> : ResultSet
    {
        public ResultSet<T, T1, T2, T3, T4, T5, T6, T7> AddRange(ResultSet<T, T1, T2, T3, T4, T5, T6, T7> item)
        {
            Result1.AddRange(item.Result1);
            Result2.AddRange(item.Result2);
            Result3.AddRange(item.Result3);
            Result4.AddRange(item.Result4);
            Result5.AddRange(item.Result5);
            Result6.AddRange(item.Result6);
            Result7.AddRange(item.Result7);
            Result8.AddRange(item.Result8);

            return this;
        }
        public List<T> Result1 { get; set; }
        public List<T1> Result2 { get; set; }
        public List<T2> Result3 { get; set; }
        public List<T3> Result4 { get; set; }
        public List<T4> Result5 { get; set; }
        public List<T5> Result6 { get; set; }
        public List<T6> Result7 { get; set; }
        public List<T7> Result8 { get; set; }
    }
    public class ResultSet<T, T1, T2, T3, T4, T5, T6, T7, T8> : ResultSet
    {
        public ResultSet<T, T1, T2, T3, T4, T5, T6, T7, T8> AddRange(ResultSet<T, T1, T2, T3, T4, T5, T6, T7, T8> item)
        {
            Result1.AddRange(item.Result1);
            Result2.AddRange(item.Result2);
            Result3.AddRange(item.Result3);
            Result4.AddRange(item.Result4);
            Result5.AddRange(item.Result5);
            Result6.AddRange(item.Result6);
            Result7.AddRange(item.Result7);
            Result8.AddRange(item.Result8);
            Result9.AddRange(item.Result9);

            return this;
        }
        public List<T> Result1 { get; set; }
        public List<T1> Result2 { get; set; }
        public List<T2> Result3 { get; set; }
        public List<T3> Result4 { get; set; }
        public List<T4> Result5 { get; set; }
        public List<T5> Result6 { get; set; }
        public List<T6> Result7 { get; set; }
        public List<T7> Result8 { get; set; }
        public List<T8> Result9 { get; set; }
    }
    public class ResultSet<T, T1, T2, T3, T4, T5, T6, T7, T8, T9> : ResultSet
    {
        public ResultSet<T, T1, T2, T3, T4, T5, T6, T7, T8, T9> AddRange(ResultSet<T, T1, T2, T3, T4, T5, T6, T7, T8, T9> item)
        {
            Result1.AddRange(item.Result1);
            Result2.AddRange(item.Result2);
            Result3.AddRange(item.Result3);
            Result4.AddRange(item.Result4);
            Result5.AddRange(item.Result5);
            Result6.AddRange(item.Result6);
            Result7.AddRange(item.Result7);
            Result8.AddRange(item.Result8);
            Result9.AddRange(item.Result9);
            Result10.AddRange(item.Result10);

            return this;
        }
        public List<T> Result1 { get; set; }
        public List<T1> Result2 { get; set; }
        public List<T2> Result3 { get; set; }
        public List<T3> Result4 { get; set; }
        public List<T4> Result5 { get; set; }
        public List<T5> Result6 { get; set; }
        public List<T6> Result7 { get; set; }
        public List<T7> Result8 { get; set; }
        public List<T8> Result9 { get; set; }
        public List<T9> Result10 { get; set; }
    }
}
